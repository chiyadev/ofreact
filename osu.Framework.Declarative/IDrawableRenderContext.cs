using System;
using osu.Framework.Graphics;

namespace osu.Framework.Declarative
{
    public interface IDrawableRenderContext : IDisposable
    {
        void Render(Drawable drawable, bool explicitDepth);
    }
}