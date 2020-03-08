using System.Globalization;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    static class YamlExtensions
    {
        public static YamlScalarNode ToScalar(this YamlNode node)
        {
            switch (node)
            {
                case YamlScalarNode scalar:
                    return scalar;

                default:
                    throw new YamlComponentException("Must be a scalar.", node);
            }
        }

        public static YamlSequenceNode ToSequence(this YamlNode node)
        {
            switch (node)
            {
                case YamlSequenceNode sequence:
                    return sequence;

                case YamlScalarNode scalar when string.IsNullOrEmpty(scalar.Value):
                    return new YamlSequenceNode();

                default:
                    throw new YamlComponentException("Must be a sequence.", node);
            }
        }

        public static YamlMappingNode ToMapping(this YamlNode node)
        {
            switch (node)
            {
                case YamlMappingNode mapping:
                    return mapping;

                case YamlScalarNode scalar when string.IsNullOrEmpty(scalar.Value):
                    return new YamlMappingNode();

                default:
                    throw new YamlComponentException("Must be a mapping.", node);
            }
        }

        public static float ToSingle(this YamlNode node)
        {
            var s = node.ToScalar().Value;

            if (float.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var v))
                return v;

            throw new YamlComponentException($"Cannot convert '{s}' to number.", node);
        }

        public static float ToSingle(this YamlNode node, string value)
        {
            if (float.TryParse(value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var v))
                return v;

            throw new YamlComponentException($"Cannot convert '{value}' to number.", node);
        }

        public static bool ToBoolean(this YamlNode node)
        {
            var s = node.ToScalar().Value;

            if (bool.TryParse(s, out var v))
                return v;

            throw new YamlComponentException($"Cannot convert '{s}' to boolean.", node);
        }
    }
}