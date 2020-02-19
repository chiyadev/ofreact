using System;
using System.Runtime.Serialization;
using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    /// <summary>
    /// Represents errors that occur in <see cref="YamlComponentBuilder"/>.
    /// </summary>
    [Serializable]
    public class YamlComponentException : Exception
    {
        public YamlNode Node { get; }

        public YamlComponentException(string message, YamlNode node, Exception inner = null) : base(FormatMessage(message, node), inner)
        {
            Node = node;
        }

        static string FormatMessage(string message, YamlNode node)
        {
            if (node == null)
                return message;

            var mark = node.Start;

            return $"{message} <{node.NodeType.ToString().ToLowerInvariant()}> (line {mark.Line} column {mark.Column})";
        }

        protected YamlComponentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}