using System;
using System.Collections.Generic;
using System.Text;

namespace ofreact.Diagnostics
{
    public sealed class RenderDiagnostics
    {
        public List<ofNode> NodesRendered { get; } = new List<ofNode>();
        public List<ofNode> NodesSkipped { get; } = new List<ofNode>();
        public List<ofNode> NodesInvalidated { get; } = new List<ofNode>();
        public List<ofNode> NodesDisposed { get; } = new List<ofNode>();

        public List<EffectInfo> EffectsInvoked { get; } = new List<EffectInfo>();

        public List<Exception> Exceptions { get; } = new List<Exception>();

        public void OnNodeRendering(ofNode node) => NodesRendered.Add(node);
        public void OnNodeRenderSkipped(ofNode node) => NodesSkipped.Add(node);
        public void OnNodeInvalidated(ofNode node) => NodesInvalidated.Add(node);
        public void OnNodeDisposed(ofNode node) => NodesDisposed.Add(node);

        public void OnEffectInvoking(EffectInfo effect) => EffectsInvoked.Add(effect);

        public void OnException(Exception exception) => Exceptions.Add(exception);

        public void Clear()
        {
            NodesRendered.Clear();
            NodesSkipped.Clear();
            NodesInvalidated.Clear();
            NodesDisposed.Clear();

            EffectsInvoked.Clear();

            Exceptions.Clear();
        }

        public void EnsureRendered(params Type[] elementTypes)
        {
            if (NodesRendered.Count != elementTypes.Length)
                bad(NodesRendered.Count);

            for (var i = 0; i < NodesRendered.Count; i++)
            {
                var node = NodesRendered[i];

                if (node.Element == null || node.Element.GetType() != elementTypes[i])
                    bad(i);
            }

            void bad(int index)
            {
                var builder = new StringBuilder($"List of rendered elements differs at index {index}.");

                builder.AppendLine().AppendLine("Expected:");

                foreach (var type in elementTypes)
                    builder.AppendLine($"  - {type}");

                builder.AppendLine().AppendLine("Actual:");

                foreach (var node in NodesRendered)
                    builder.AppendLine($"  - {node.Element?.GetType().ToString() ?? "<null>"}");

                throw new ArgumentException(builder.ToString());
            }
        }
    }
}