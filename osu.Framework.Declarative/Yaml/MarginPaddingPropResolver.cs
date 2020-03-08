using System.Linq.Expressions;
using System.Reflection;
using ofreact;
using osu.Framework.Graphics;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that is a <see cref="MarginPadding"/>.
    /// </summary>
    public class MarginPaddingPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
        {
            if (prop.Type != typeof(MarginPadding))
                return null;

            switch (node)
            {
                case YamlScalarNode scalar:
                    return new Provider(new MarginPadding(scalar.ToSingle()));

                case YamlSequenceNode sequence when sequence.Children.Count == 1:
                    return new Provider(new MarginPadding(sequence[0].ToSingle()));

                case YamlSequenceNode sequence when sequence.Children.Count == 4:
                    return new Provider(new MarginPadding
                    {
                        Top    = sequence[0].ToSingle(),
                        Right  = sequence[1].ToSingle(),
                        Bottom = sequence[2].ToSingle(),
                        Left   = sequence[3].ToSingle()
                    });

                case YamlMappingNode mapping:
                    var value = new MarginPadding();

                    foreach (var (keyNode, valueNode) in mapping)
                    {
                        var key = keyNode.ToScalar().Value;

                        switch (key)
                        {
                            case "top":
                                value.Top = valueNode.ToSingle();
                                break;

                            case "right":
                                value.Right = valueNode.ToSingle();
                                break;

                            case "bottom":
                                value.Bottom = valueNode.ToSingle();
                                break;

                            case "left":
                                value.Left = valueNode.ToSingle();
                                break;

                            default:
                                throw new YamlComponentException($"Invalid margin/padding property '{key}'.", keyNode);
                        }
                    }

                    return new Provider(value);

                default:
                    throw new YamlComponentException("Must be a sequence containing 4 components or a mapping that specifies top-left-bottom-right margin/padding respectively.", node);
            }
        }

        sealed class Provider : IPropProvider
        {
            readonly MarginPadding _value;

            public Provider(MarginPadding value)
            {
                _value = value;
            }

            static readonly MemberInfo _top = typeof(MarginPadding).GetField(nameof(MarginPadding.Top));
            static readonly MemberInfo _right = typeof(MarginPadding).GetField(nameof(MarginPadding.Right));
            static readonly MemberInfo _bottom = typeof(MarginPadding).GetField(nameof(MarginPadding.Bottom));
            static readonly MemberInfo _left = typeof(MarginPadding).GetField(nameof(MarginPadding.Left));

            public Expression GetValue(ComponentBuilderContext context)
                => Expression.MemberInit(
                    Expression.New(typeof(MarginPadding)),
                    Expression.Bind(_top, Expression.Constant(_value.Top)),
                    Expression.Bind(_right, Expression.Constant(_value.Right)),
                    Expression.Bind(_bottom, Expression.Constant(_value.Bottom)),
                    Expression.Bind(_left, Expression.Constant(_value.Left)));
        }
    }
}