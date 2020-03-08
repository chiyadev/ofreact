using System.Linq.Expressions;
using System.Reflection;
using ofreact;
using osu.Framework.Graphics.Sprites;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that is a <see cref="FontUsage"/>.
    /// </summary>
    public class FontUsagePropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
        {
            if (prop.Type != typeof(FontUsage))
                return null;

            switch (node)
            {
                case YamlScalarNode scalar:
                    var parts = scalar.Value.Split(',');

                    switch (parts.Length)
                    {
                        case 1:
                            return new Provider(FontUsage.Default.With(family: parts[0]));

                        case 2:
                            return new Provider(FontUsage.Default.With(family: parts[0], size: node.ToSingle(parts[1])));

                        default: throw new YamlComponentException("Must be a scalar that indicates the font family name and an optional font size.", node);
                    }

                case YamlMappingNode mapping:
                    var usage = FontUsage.Default;

                    foreach (var (keyNode, valueNode) in mapping)
                    {
                        var key = keyNode.ToScalar().Value;

                        switch (key)
                        {
                            case "family":
                                usage = usage.With(family: valueNode.ToScalar().Value);
                                break;

                            case "weight":
                                usage = usage.With(weight: valueNode.ToScalar().Value);
                                break;

                            case "italics":
                                usage = usage.With(italics: valueNode.ToBoolean());
                                break;

                            case "size":
                                usage = usage.With(size: valueNode.ToSingle());
                                break;

                            case "fixed":
                                usage = usage.With(fixedWidth: valueNode.ToBoolean());
                                break;

                            default:
                                throw new YamlComponentException($"Invalid font property '{key}'.", keyNode);
                        }
                    }

                    return new Provider(usage);

                default:
                    throw new YamlComponentException("Must be a scalar or sequence.", node);
            }
        }

        sealed class Provider : IPropProvider
        {
            readonly FontUsage _font;

            public Provider(FontUsage font)
            {
                _font = font;
            }

            static readonly ConstructorInfo _ctor = typeof(FontUsage).GetConstructors()[0];

            public Expression GetValue(ComponentBuilderContext context)
                => Expression.New(_ctor,
                    Expression.Constant(_font.Family, typeof(string)),
                    Expression.Constant(_font.Size, typeof(float)),
                    Expression.Constant(_font.Weight, typeof(string)),
                    Expression.Constant(_font.Italics, typeof(bool)),
                    Expression.Constant(_font.FixedWidth, typeof(bool)));
        }
    }
}