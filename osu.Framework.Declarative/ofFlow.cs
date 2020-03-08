using System.Collections.Generic;
using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Declarative
{
    public abstract class FlowStyle<T> : ContainerStyle<T> where T : FlowContainer<Drawable>
    {
        /// <inheritdoc cref="FlowContainer{T}.LayoutEasing"/>
        public Easing? LayoutEasing;

        /// <inheritdoc cref="FlowContainer{T}.LayoutDuration"/>
        public float? LayoutDuration;

        /// <inheritdoc cref="FlowContainer{T}.MaximumSize"/>
        public Vector2? MaximumSize;

        protected override void Apply(T drawable)
        {
            base.Apply(drawable);

            if (LayoutEasing != null)
                drawable.LayoutEasing = LayoutEasing.Value;

            if (LayoutDuration != null)
                drawable.LayoutDuration = LayoutDuration.Value;

            if (MaximumSize != null)
                drawable.MaximumSize = MaximumSize.Value;
        }
    }

    public abstract class ofFlow<T> : ofContainer<T> where T : FlowContainer<Drawable>
    {
        protected ofFlow(ElementKey key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default, IEnumerable<ofElement> children = default) : base(key, @ref, style, children) { }
    }
}