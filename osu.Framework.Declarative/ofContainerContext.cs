using System.Collections.Generic;
using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Makes an existing <see cref="Container{Drawable}"/> in the scene graph available for all descendants to render to.
    /// </summary>
    public class ofContainerContext : ofContext<IDrawableRenderContext>
    {
        /// <summary>
        /// Creates a new <see cref="ofContainerContext"/>.
        /// </summary>
        public ofContainerContext(Container<Drawable> container, object key = default, IEnumerable<ofElement> children = default) : base(key, children, new Context(container)) { }

        sealed class Context : IDrawableRenderContext
        {
            readonly Container<Drawable> _container;
            readonly HashSet<Drawable> _remaining;

            public Context(Container<Drawable> container)
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
                if (_remaining.Contains(drawable))
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
        }
    }
}