using ofreact;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Declarative
{
    public sealed class DrawSizePreservingFillContainerStyle : DrawableStyle<DrawSizePreservingFillContainer>
    {
        /// <inheritdoc cref="DrawSizePreservingFillContainer.TargetDrawSize"/>
        public Vector2? TargetDrawSize;

        /// <inheritdoc cref="DrawSizePreservingFillContainer.Strategy"/>
        public DrawSizePreservationStrategy? Strategy;

        protected override void Apply(DrawSizePreservingFillContainer drawable)
        {
            base.Apply(drawable);

            if (TargetDrawSize != null)
                drawable.TargetDrawSize = TargetDrawSize.Value;

            if (Strategy != null)
                drawable.Strategy = Strategy.Value;
        }
    }

    /// <summary>
    /// Renders a <see cref="DrawSizePreservingFillContainer"/>.
    /// </summary>
    public sealed class ofDrawSizePreservingFillContainer : ofContainer<DrawSizePreservingFillContainer>
    {
        /// <summary>
        /// Creates a new <see cref="ofDrawSizePreservingFillContainer"/>.
        /// </summary>
        public ofDrawSizePreservingFillContainer(ElementKey key = default, RefDelegate<DrawSizePreservingFillContainer> @ref = default, DrawableStyleDelegate<DrawSizePreservingFillContainer> style = default) : base(key, @ref, style) { }

        protected override DrawSizePreservingFillContainer CreateDrawable() => new DrawSizePreservingFillContainer();
    }
}