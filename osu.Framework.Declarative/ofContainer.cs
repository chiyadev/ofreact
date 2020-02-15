using System.Collections;
using System.Collections.Generic;
using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Represents a <see cref="Container{Drawable}"/> in the osu!framework scene graph.
    /// </summary>
    public class ofContainer : ofDrawable<Container<Drawable>>, IEnumerable<ofElement>
    {
        [Prop] public readonly List<ofElement> Children;

        /// <summary>
        /// Creates a new <see cref="ofContainer"/>.
        /// </summary>
        public ofContainer(object key = default, RefDelegate<Container<Drawable>> @ref = default, DrawableStyleDelegate<Container<Drawable>> style = default, IEnumerable<ofElement> children = default) : base(key, @ref, style)
        {
            Children = children == null
                ? new List<ofElement>()
                : new List<ofElement>(children);
        }

        /// <summary>
        /// Adds an element as a child of this container.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Add(ofElement element) => Children.Add(element);

        protected override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var node    = UseChild();
            var current = node.Current ??= Node.CreateChild();

            return current.RenderElement(new ofContext<ContainerDrawableRenderContext>(children: Children, value: new ContainerDrawableRenderContext(Drawable)));
        }

        public IEnumerator<ofElement> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}