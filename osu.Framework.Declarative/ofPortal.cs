using System.Collections.Generic;
using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Provides a first-class way to render children into a <see cref="Container{Drawable}"/> that exists outside the scene graph of the parent component.
    /// </summary>
    /// <remarks>
    /// A typical use case for portals is when you need the children to visually "break out" of the parent container. For example, dialogs, hovercards, and tooltips.
    /// Portals can also be used to bootstrap any ofreact scene graph from a <see cref="Drawable"/>.
    /// </remarks>
    public class ofPortal : ofContext<IDrawableRenderContext>
    {
        /// <summary>
        /// Creates a new <see cref="ofPortal"/>.
        /// </summary>
        public ofPortal(Container<Drawable> container, ElementKey key = default, IEnumerable<ofElement> children = default) : base(key, children, new Context(container)) { }

        sealed class Context : IDrawableRenderContext
        {
            readonly Container<Drawable> _container;
            readonly HashSet<Drawable> _rendered;

            public IReadOnlyDependencyContainer DependencyContainer => _container.Dependencies;

            public Context(Container<Drawable> container)
            {
                _container = container;
                _rendered  = new HashSet<Drawable>(container.Count);
            }

            float _depth;

            public void Render(Drawable drawable, bool explicitDepth)
            {
                if (!_rendered.Add(drawable))
                    return;

                // already contained
                if (_container.Contains(drawable))
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