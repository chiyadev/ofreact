using System.Collections.Generic;
using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Declarative
{
    public sealed class FillFlowStyle : FillFlowStyle<FillFlowContainer<Drawable>> { }

    public abstract class FillFlowStyle<T> : FlowStyle<T> where T : FillFlowContainer<Drawable>
    {
        /// <inheritdoc cref="FillFlowContainer{T}.Direction"/>
        public FillDirection? Direction;

        /// <inheritdoc cref="FillFlowContainer{T}.Spacing"/>
        public Vector2? Spacing;

        protected override void Apply(T drawable)
        {
            base.Apply(drawable);

            if (Direction != null)
                drawable.Direction = Direction.Value;

            if (Spacing != null)
                drawable.Spacing = Spacing.Value;
        }
    }

    public sealed class ofFillFlow : ofFillFlow<FillFlowContainer<Drawable>>
    {
        public ofFillFlow(ElementKey key = default, RefDelegate<FillFlowContainer<Drawable>> @ref = default, DrawableStyleDelegate<FillFlowContainer<Drawable>> style = default, IEnumerable<ofElement> children = default) : base(key, @ref, style, children) { }

        protected override FillFlowContainer<Drawable> CreateDrawable() => new FillFlowContainer<Drawable>();
    }

    public abstract class ofFillFlow<T> : ofFlow<T> where T : FillFlowContainer<Drawable>
    {
        protected ofFillFlow(ElementKey key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default, IEnumerable<ofElement> children = default) : base(key, @ref, style, children) { }
    }
}