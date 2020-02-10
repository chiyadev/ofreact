using System.Collections;
using System.Collections.Generic;

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

        protected override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var nodesRef = UseChildren();
            var nodes    = nodesRef.Current;

            var rendered = new List<ofNode>(nodes.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];

                if (child == null)
                    continue;

                ofNode node;

                // iterate nodes until matching node for child element is found
                for (var j = 0; j < nodes.Length; j++)
                {
                    node = nodes[j];

                    if (node != null && child.MatchNode(node))
                    {
                        nodes[j] = null;

                        goto render;
                    }
                }

                // if no node matched, create a new one
                node = Node.CreateChild();

                render:

                // render child element
                child.RenderSubtree(node);

                rendered.Add(node);
            }

            // update node list
            nodesRef.Current = rendered.ToArray();

            // dispose removed nodes
            foreach (var node in nodes)
                node?.Dispose();

            return true;
        }

        public IEnumerator<ofElement> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator ofFragment(ofElement[] x) => new ofFragment(children: new List<ofElement>(x));
        public static implicit operator ofFragment(List<ofElement> x) => new ofFragment(children: x);
    }
}