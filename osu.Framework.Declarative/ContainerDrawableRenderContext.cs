using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Implements <see cref="IDrawableRenderContext"/> by adding rendered <see cref="Drawable"/>s to a <see cref="Container{Drawable}"/>.
    /// </summary>
    public sealed class ContainerDrawableRenderContext : IDrawableRenderContext
    {
        readonly Container<Drawable> _container;
        readonly HashSet<Drawable> _remaining;

        /// <summary>
        /// Creates a new <see cref="ContainerDrawableRenderContext"/>.
        /// </summary>
        /// <param name="container">The <see cref="Container{Drawable}"/> to add rendered <see cref="Drawable"/>s to.</param>
        public ContainerDrawableRenderContext(Container<Drawable> container)
        {
            _container = container;
            _remaining = new HashSet<Drawable>(container.AliveChildren.Count);

            foreach (var drawable in container.AliveChildren)
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
    }
}