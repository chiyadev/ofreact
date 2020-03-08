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
    }
}