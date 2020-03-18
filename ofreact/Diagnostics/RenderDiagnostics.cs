using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ofreact.Diagnostics
{
    public class RenderDiagnostics
    {
        public List<ofNode> NodesRendered { get; } = new List<ofNode>();
        public List<ofNode> NodesSkipped { get; } = new List<ofNode>();
        public List<ofNode> NodesInvalidated { get; } = new List<ofNode>();
        public List<ofNode> NodesDisposed { get; } = new List<ofNode>();
        public List<EffectInfo> EffectsInvoked { get; } = new List<EffectInfo>();
        public List<Exception> Exceptions { get; } = new List<Exception>();

        public virtual void OnNodeRendering(ofNode node) => NodesRendered.Add(node);
        public virtual void OnNodeRenderSkipped(ofNode node) => NodesSkipped.Add(node);
        public virtual void OnNodeInvalidated(ofNode node) => NodesInvalidated.Add(node);
        public virtual void OnNodeDisposed(ofNode node) => NodesDisposed.Add(node);
        public virtual void OnEffectInvoking(EffectInfo effect) => EffectsInvoked.Add(effect);
        public virtual void OnException(Exception exception) => Exceptions.Add(exception);

        public virtual void Clear()
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
            var renderedTypes = NodesRendered.Select(n => n.Element?.GetType()).Where(e => e != null).ToArray();

            for (var i = 0; i < renderedTypes.Length && i < elementTypes.Length; i++)
            {
                if (renderedTypes[i] != elementTypes[i])
                    bad(i);
            }

            if (renderedTypes.Length != elementTypes.Length)
                bad(renderedTypes.Length);

            void bad(int index)
            {
                var builder = new StringBuilder($"List of rendered elements differs at index {index}.");

                builder.AppendLine().AppendLine("Expected:");

                foreach (var type in elementTypes)
                    builder.AppendLine($"  - {type}");

                builder.AppendLine().AppendLine("Actual:");

                foreach (var type in renderedTypes)
                    builder.AppendLine($"  - {type}");

                throw new ArgumentException(builder.ToString());
            }
        }
    }
}