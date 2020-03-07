using ofreact;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Renders a <see cref="DrawSizePreservingFillContainer"/>.
    /// </summary>
    public class ofDrawSizePreservingFillContainer : ofContainerBase<DrawSizePreservingFillContainer>
    {
        [Prop] public readonly Vector2? TargetDrawSize;
        [Prop] public readonly DrawSizePreservationStrategy? Strategy;

        /// <summary>
        /// Creates a new <see cref="ofDrawSizePreservingFillContainer"/>.
        /// </summary>
        public ofDrawSizePreservingFillContainer(ElementKey key = default,
                                                 RefDelegate<DrawSizePreservingFillContainer> @ref = default,
                                                 DrawableStyleDelegate<DrawSizePreservingFillContainer> style = default,
                                                 Vector2? targetDrawSize = default,
                                                 DrawSizePreservationStrategy? strategy = default) : base(key, @ref, style)
        {
            TargetDrawSize = targetDrawSize;
            Strategy       = strategy;
        }

        protected override DrawSizePreservingFillContainer CreateDrawable() => new DrawSizePreservingFillContainer();

        protected override void ApplyStyles(DrawSizePreservingFillContainer drawable)
        {
            base.ApplyStyles(drawable);

            if (TargetDrawSize != null)
                drawable.TargetDrawSize = TargetDrawSize.Value;

            if (Strategy != null)
                drawable.Strategy = Strategy.Value;
        }
    }
}