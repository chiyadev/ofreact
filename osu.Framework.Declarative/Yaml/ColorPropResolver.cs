using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ofreact;
using osu.Framework.Graphics.Colour;
using osuTK.Graphics;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    public class ColorPropResolver : IPropResolver
    {
        static readonly Dictionary<string, Color4> _namedColors = new Dictionary<string, Color4>(StringComparer.OrdinalIgnoreCase);

        static ColorPropResolver()
        {
            foreach (var property in typeof(Color4).GetProperties(BindingFlags.Public | BindingFlags.Static)
                                                   .Where(p => p.CanRead && p.PropertyType == typeof(Color4)))
                _namedColors[property.Name] = (Color4) property.GetValue(null);
        }

        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
        {
            if (prop.Type == typeof(Color4))
                return new Provider(ParseColor(node));

            if (prop.Type == typeof(SRGBColour))
                return new SrgbProvider(ParseColor(node));

            if (prop.Type == typeof(ColourInfo))
                switch (node)
                {
                    case YamlScalarNode scalar:
                        return new MultiProvider(ParseColor(scalar));

                    case YamlSequenceNode sequence when sequence.Children.Count == 1:
                        return new MultiProvider(ParseColor(sequence[0]));

                    case YamlSequenceNode sequence when sequence.Children.Count == 4:
                        return new MultiProvider(new ColourInfo
                        {
                            TopLeft     = ParseColor(sequence[0]),
                            TopRight    = ParseColor(sequence[1]),
                            BottomRight = ParseColor(sequence[2]),
                            BottomLeft  = ParseColor(sequence[3])
                        });

                    case YamlMappingNode mapping when mapping.Children.Count == 1:
                        foreach (var (keyNode, valueNode) in mapping)
                        {
                            var value = valueNode.ToSequence().Children;

                            switch (keyNode.ToScalar().Value)
                            {
                                case "vertical":
                                    if (value.Count != 2)
                                        throw new YamlComponentException("Must be a sequence containing two colors representing the top and bottom of gradient.", valueNode);

                                    return new MultiProvider(ColourInfo.GradientVertical(ParseColor(value[0]), ParseColor(value[1])));

                                case "horizontal":
                                    if (value.Count != 2)
                                        throw new YamlComponentException("Must be a sequence containing two colors representing the left and right of gradient.", valueNode);

                                    return new MultiProvider(ColourInfo.GradientHorizontal(ParseColor(value[0]), ParseColor(value[1])));

                                default:
                                    throw new YamlComponentException("Must specify either vertical or horizontal gradient.", keyNode);
                            }
                        }

                        break;

                    default:
                        throw new YamlComponentException("Must be a sequence containing 4 components or a mapping that specifies a gradient.", node);
                }

            return null;
        }

        static Color4 ParseColor(YamlNode node)
        {
            var value = node.ToScalar().Value;

            if (ParseColorString(value, node, out var color))
                return color;

            var parts = value.Split(',');

            switch (parts.Length)
            {
                // name, alpha
                case 2:
                    if (!ParseColorString(parts[0], node, out color))
                        throw new YamlComponentException($"Cannot convert '{parts[0]}' to named color.", node);

                    color.A = node.ToSingle(parts[1]);

                    return color;

                // r, g, b, [alpha]
                case 3:
                case 4:
                    return new Color4(
                        node.ToSingle(parts[0]) / byte.MaxValue,
                        node.ToSingle(parts[1]) / byte.MaxValue,
                        node.ToSingle(parts[2]) / byte.MaxValue,
                        parts.Length == 4 ? node.ToSingle(parts[3]) : 1); // alpha is [0, 1] so we don't divide
            }

            throw new YamlComponentException("Must be a scalar containing three or four components representing R, G, B and optionally A.", node);
        }

        static bool ParseColorString(string value, YamlNode node, out Color4 color)
        {
            // named color
            if (_namedColors.TryGetValue(value, out color))
                return true;

            // hex
            if (value.Length != 0 && value[0] == '#')
            {
                value = value.Substring(1);

                switch (value.Length)
                {
                    case 3:
                        value = $"{value[0]}{value[0]}{value[1]}{value[1]}{value[2]}{value[2]}FF";
                        break;

                    case 6:
                        value = $"{value}FF";
                        break;

                    case 8:
                        break;

                    default:
                        value = null;
                        break;
                }

                if (int.TryParse(value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out var argb))
                {
                    color = new Color4(
                        (byte) ((argb & 0xff000000) >> 0x18),
                        (byte) ((argb & 0xff0000) >> 0x10),
                        (byte) ((argb & 0xff00) >> 0x08),
                        (byte) (argb & 0xff));

                    return true;
                }

                throw new YamlComponentException($"Cannot convert '{value}' to number.", node);
            }

            return false;
        }

        sealed class Provider : IPropProvider
        {
            readonly Color4 _color;

            public Provider(Color4 color)
            {
                _color = color;
            }

            static readonly ConstructorInfo _ctor = typeof(Color4).GetConstructor(new[] { typeof(float), typeof(float), typeof(float), typeof(float) });

            public Expression GetValue(ComponentBuilderContext context)
                => Expression.New(_ctor,
                    Expression.Constant(_color.R),
                    Expression.Constant(_color.G),
                    Expression.Constant(_color.B),
                    Expression.Constant(_color.A));
        }

        sealed class SrgbProvider : IPropProvider
        {
            readonly SRGBColour _color;

            public SrgbProvider(SRGBColour color)
            {
                _color = color;
            }

            public Expression GetValue(ComponentBuilderContext context)
                => Expression.Convert(new Provider(_color).GetValue(context), typeof(SRGBColour));
        }

        sealed class MultiProvider : IPropProvider
        {
            readonly ColourInfo _color;

            public MultiProvider(ColourInfo color)
            {
                _color = color;
            }

            static readonly MemberInfo _topLeft = typeof(ColourInfo).GetField(nameof(ColourInfo.TopLeft));
            static readonly MemberInfo _topRight = typeof(ColourInfo).GetField(nameof(ColourInfo.TopRight));
            static readonly MemberInfo _bottomRight = typeof(ColourInfo).GetField(nameof(ColourInfo.BottomRight));
            static readonly MemberInfo _bottomLeft = typeof(ColourInfo).GetField(nameof(ColourInfo.BottomLeft));

            public Expression GetValue(ComponentBuilderContext context)
                => Expression.MemberInit(
                    Expression.New(typeof(ColourInfo)),
                    Expression.Bind(_topLeft, new SrgbProvider(_color.TopLeft).GetValue(context)),
                    Expression.Bind(_topRight, new SrgbProvider(_color.TopRight).GetValue(context)),
                    Expression.Bind(_bottomRight, new SrgbProvider(_color.BottomRight).GetValue(context)),
                    Expression.Bind(_bottomLeft, new SrgbProvider(_color.BottomLeft).GetValue(context)));
        }
    }
}