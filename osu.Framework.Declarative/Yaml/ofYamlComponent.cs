using System;
using System.IO;
using ofreact;
using YamlDotNet.RepresentationModel;
using static ofreact.Hooks;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Represents a dynamic component that is created from an YAML document.
    /// </summary>
    /// <remarks>
    /// This is a helper component that wraps <see cref="YamlComponentBuilder"/>.
    /// </remarks>
    public class ofYamlComponent : ofComponent
    {
        readonly Func<FunctionComponent> _build;

        /// <summary>
        /// Creates a new <see cref="ofYamlComponent"/> from an YAML document.
        /// </summary>
        public ofYamlComponent(YamlDocument document, object key = default) : base(key)
        {
            _build = new YamlComponentBuilder(document).BuildRenderer;
        }

        /// <summary>
        /// Creates a new <see cref="ofYamlComponent"/> from a reader.
        /// </summary>
        /// <remarks>
        /// <paramref name="reader"/> must not be disposed until this element is rendered at least once.
        /// Caller is responsible for disposing the reader afterwards.
        /// </remarks>
        public ofYamlComponent(TextReader reader, object key = default) : base(key)
        {
            _build = () =>
            {
                var stream = new YamlStream();

                stream.Load(reader);

                if (stream.Documents.Count == 0)
                    return n => null;

                return new YamlComponentBuilder(stream.Documents[0]).BuildRenderer();
            };
        }

        /// <summary>
        /// Creates a new <see cref="ofYamlComponent"/> from a document string.
        /// </summary>
        public ofYamlComponent(string document, object key = default) : this(new StringReader(document), key) { }

        protected override ofElement Render() => UseMemo(_build)(Node);
    }
}