using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Represents a context in which <see cref="Drawable"/>s can be rendered.
    /// </summary>
    public interface IDrawableRenderContext
    {
        /// <summary>
        /// Gets the <see cref="IReadOnlyDependencyContainer"/>.
        /// </summary>
        IReadOnlyDependencyContainer DependencyContainer { get; }

        /// <summary>
        /// Renders the given <see cref="Drawable"/>.
        /// </summary>
        /// <param name="drawable">Drawable to render.</param>
        /// <param name="explicitDepth">True if the given <paramref name="drawable"/> has an explicit depth configured.</param>
        void Render(Drawable drawable, bool explicitDepth);
    }
}