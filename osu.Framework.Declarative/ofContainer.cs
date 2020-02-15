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

            var node = UseChild();

            var context = new RenderContext(Drawable);

            return node.Current.RenderElement(new ofContext<RenderContext>(children: Children, value: context));
        }

        sealed class RenderContext : IDrawableRenderContext
        {
            readonly Container<Drawable> _container;
            readonly HashSet<Drawable> _remaining;

            public RenderContext(Container<Drawable> container)
            {
                _container = container;
                _remaining = new HashSet<Drawable>(container.Count);

                foreach (var drawable in container)
                    _remaining.Add(drawable);
            }

            float _depth;

            public void Render(Drawable drawable, bool explicitDepth)
            {
                // already contained
                if (_remaining.Remove(drawable))
                {
                    if (!explicitDepth)
                        _container.ChangeChildDepth(drawable, _depth--);
                }

                // new child
                else
                {
                    if (!explicitDepth)
                        drawable.Depth = _depth--;

                    _container.Add(drawable);
                }
            }

            // remove remaining drawables that weren't rendered
            public void Dispose() => _container.RemoveRange(_remaining);
        }

        public IEnumerator<ofElement> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}