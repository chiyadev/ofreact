using System.Collections.Generic;
using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace osu.Framework.Declarative
{
    public sealed class ScrollStyle : ScrollStyle<ScrollContainer<Drawable>> { }

    public abstract class ScrollStyle<T> : ContainerStyle<T> where T : ScrollContainer<Drawable>
    {
        /// <inheritdoc cref="ScrollContainer{T}.ScrollbarAnchor"/>
        public Anchor? ScrollbarAnchor;

        /// <inheritdoc cref="ScrollContainer{T}.ScrollbarVisible"/>
        public bool? ScrollbarVisible;

        /// <inheritdoc cref="ScrollContainer{T}.ScrollbarOverlapsContent"/>
        public bool? ScrollbarOverlapsContent;

        /// <inheritdoc cref="ScrollContainer{T}.ScrollDistance"/>
        public float? ScrollDistance;

        /// <inheritdoc cref="ScrollContainer{T}.ClampExtension"/>
        public float? ClampExtension;

        /// <inheritdoc cref="ScrollContainer{T}.DistanceDecayDrag"/>
        public double? DistanceDecayDrag;

        /// <inheritdoc cref="ScrollContainer{T}.DistanceDecayScroll"/>
        public double? DistanceDecayScroll;

        /// <inheritdoc cref="ScrollContainer{T}.DistanceDecayJump"/>
        public double? DistanceDecayJump;

        protected override void Apply(T drawable)
        {
            base.Apply(drawable);

            if (ScrollbarAnchor != null)
                drawable.ScrollbarAnchor = ScrollbarAnchor.Value;

            if (ScrollbarVisible != null)
                drawable.ScrollbarVisible = ScrollbarVisible.Value;

            if (ScrollbarOverlapsContent != null)
                drawable.ScrollbarOverlapsContent = ScrollbarOverlapsContent.Value;

            if (ScrollDistance != null)
                drawable.ScrollDistance = ScrollDistance.Value;

            if (ClampExtension != null)
                drawable.ClampExtension = ClampExtension.Value;

            if (DistanceDecayDrag != null)
                drawable.DistanceDecayDrag = DistanceDecayDrag.Value;

            if (DistanceDecayScroll != null)
                drawable.DistanceDecayScroll = DistanceDecayScroll.Value;

            if (DistanceDecayJump != null)
                drawable.DistanceDecayJump = DistanceDecayJump.Value;
        }
    }

    public sealed class ofScroll : ofScroll<BasicScrollContainer<Drawable>>
    {
        public ofScroll(ElementKey key = default, RefDelegate<BasicScrollContainer<Drawable>> @ref = default, DrawableStyleDelegate<BasicScrollContainer<Drawable>> style = default, DrawableEventDelegate @event = default, IEnumerable<ofElement> children = default) : base(key, @ref, style, @event, children) { }

        protected override BasicScrollContainer<Drawable> CreateDrawable() => new InternalScrollContainer();

        sealed class InternalScrollContainer : BasicScrollContainer, ISupportEventDelegation
        {
            public DrawableEventDelegate EventDelegate { get; set; }
            public override bool HandlePositionalInput => base.HandlePositionalInput || EventDelegate != null;
            public override bool HandleNonPositionalInput => base.HandleNonPositionalInput || EventDelegate != null;

            protected override bool Handle(UIEvent e) => base.Handle(e) || EventDelegate(e);
        }
    }

    public abstract class ofScroll<T> : ofContainer<T> where T : ScrollContainer<Drawable>
    {
        protected ofScroll(ElementKey key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default, DrawableEventDelegate @event = default, IEnumerable<ofElement> children = default) : base(key, @ref, style, @event, children) { }
    }
}