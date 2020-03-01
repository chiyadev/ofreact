using System.Collections;
using System.Collections.Generic;
using static ofreact.Hooks;

namespace ofreact
{
    /// <summary>
    /// Represents an ordered collection of nested <see cref="ofElement"/>s.
    /// </summary>
    public class ofFragment : ofElement, IEnumerable<ofElement>
    {
        [Prop] public readonly List<ofElement> Children;

        /// <summary>
        /// Creates a new <see cref="ofFragment"/>.
        /// </summary>
        public ofFragment(object key = default, IEnumerable<ofElement> children = default) : base(key)
        {
            Children = children == null
                ? new List<ofElement>()
                : new List<ofElement>(children);
        }

        /// <summary>
        /// Adds an element as a child of this fragment.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Add(ofElement element) => Children.Add(element);

        protected internal override unsafe bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var nodesRef = UseChildren();
            var nodes    = nodesRef.Current;

            var nodeMatched = stackalloc bool[nodes.Length];

            var rendered = false;
            var newNodes = new List<ofNode>(nodes.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];

                if (child == null)
                    continue;

                ofNode       node;
                RenderResult result;

                // iterate nodes until matching node for child element is found
                for (var j = 0; j < nodes.Length; j++)
                {
                    if (!nodeMatched[j])
                    {
                        node   = nodes[j];
                        result = node.RenderElement(child);

                        if (result != RenderResult.Mismatch)
                        {
                            nodeMatched[j] = true;

                            newNodes.Add(node);
                            rendered |= result == RenderResult.Rendered;

                            goto next;
                        }
                    }
                }

                // if no node matched, make a new one
                node   = Node.CreateChild();
                result = node.RenderElement(child);

                if (result != RenderResult.Mismatch)
                {
                    newNodes.Add(node);
                    rendered |= result == RenderResult.Rendered;
                }

                next: ;
            }

            // update node list
            nodesRef.Current = newNodes.ToArray();

            // dispose removed nodes (nodes not matched)
            for (var j = 0; j < nodes.Length; j++)
            {
                if (!nodeMatched[j])
                {
                    nodes[j].Dispose();
                    rendered = true;
                }
            }

            return rendered;
        }

        public IEnumerator<ofElement> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator ofFragment(ofElement[] x) => new ofFragment(children: new List<ofElement>(x));
        public static implicit operator ofFragment(List<ofElement> x) => new ofFragment(children: x);
    }
}