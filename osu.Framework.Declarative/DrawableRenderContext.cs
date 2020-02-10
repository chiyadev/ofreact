using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Declarative
{
    public class DrawableRenderContext : IDisposable
    {
        public Container<Drawable> Container { get; }

        readonly HashSet<Drawable> _drawables;

        public DrawableRenderContext(Container<Drawable> container)
        {
            Container = container;

            _drawables = new HashSet<Drawable>(container.Count);

            foreach (var drawable in container)
                _drawables.Add(drawable);
        }

        float _depth;

        public void Render<T>(T drawable, bool explicitDepth) where T : Drawable
        {
            // already contained
            if (_drawables.Remove(drawable))
            {
                if (!explicitDepth)
                    Container.ChangeChildDepth(drawable, _depth--);
            }

            // new child
            else
            {
                if (!explicitDepth)
                    drawable.Depth = _depth--;

                Container.Add(drawable);
            }
        }

        // remove remaining not rendered drawables
        public void Dispose() => Container.RemoveRange(_drawables);
    }
}