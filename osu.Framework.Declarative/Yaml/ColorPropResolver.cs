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

        public IPropProvider Resolve(ComponentBuilderContext context, PropTypeInfo prop, ElementBuilder element, YamlNode node)
        {
            if (prop.Type == typeof(Color4))
            {
                if (!(node is YamlScalarNode scalar))
                    throw new YamlComponentException("Must be a scalar.", node);

                return new Provider(ParseColor(scalar));
            }

            if (prop.Type == typeof(SRGBColour))
            {
                if (!(node is YamlScalarNode scalar))
                    throw new YamlComponentException("Must be a scalar.", node);

                return new SrgbProvider(ParseColor(scalar));
            }

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
                        foreach (var (key, value) in mapping)
                        {
                            if (!(key is YamlScalarNode scalarKey))
                                throw new YamlComponentException("Must be a scalar.", key);

                            if (!(value is YamlSequenceNode sequenceValue))
                                throw new YamlComponentException("Must be a sequence.", value);

                            switch (scalarKey.Value)
                            {
                                case "vertical":
                                    if (sequenceValue.Children.Count != 2)
                                        throw new YamlComponentException("Must be a sequence containing two colors representing the top and bottom of gradient.", sequenceValue);

                                    return new MultiProvider(ColourInfo.GradientVertical(ParseColor(sequenceValue[0]), ParseColor(sequenceValue[1])));

                                case "horizontal":
                                    if (sequenceValue.Children.Count != 2)
                                        throw new YamlComponentException("Must be a sequence containing two colors representing the left and right of gradient.", sequenceValue);

                                    return new MultiProvider(ColourInfo.GradientHorizontal(ParseColor(sequenceValue[0]), ParseColor(sequenceValue[1])));

                                default:
                                    throw new YamlComponentException("Must specify either vertical or horizontal gradient.", scalarKey);
                            }
                        }

                        break;

                    default:
                        throw new YamlComponentException("Must be a sequence containing 4 components or a mapping that specifies a gradient.", node);
                }

            return null;
        }

        static Color4 ParseColor(YamlNode s)
        {
            if (!(s is YamlScalarNode scalar))
                throw new YamlComponentException("Must be a scalar.", s);

            if (_namedColors.TryGetValue(scalar.Value, out var color))
                return color;

            var parts = scalar.Value.Split(',');

            switch (parts.Length)
            {
                // name, alpha
                case 2:
                    if (!_namedColors.TryGetValue(parts[0], out color))
                        throw new YamlComponentException($"Cannot convert '{parts[0]}' to named color.", s);

                    color.A = ParseNumber(s, parts[1]);

                    return color;

                // r, g, b, [alpha]
                case 3:
                case 4:
                    return new Color4(
                        ParseNumber(s, parts[0]) / byte.MaxValue,
                        ParseNumber(s, parts[1]) / byte.MaxValue,
                        ParseNumber(s, parts[2]) / byte.MaxValue,
                        parts.Length == 4 ? ParseNumber(s, parts[3]) : 1); // alpha is [0, 1] so we don't divide
            }

            throw new YamlComponentException("Color requires three or four components representing R, G, B and optionally A.", s);
        }

        static float ParseNumber(YamlNode n, string s)
        {
            if (float.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var value))
                return value;

            throw new YamlComponentException($"Cannot convert '{s}' to number.", n);
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