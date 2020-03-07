using System.Collections;
using System.Collections.Generic;
using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osuTK;
using static ofreact.Hooks;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Contains styling information for <see cref="ofContainer"/>.
    /// </summary>
    public sealed class ContainerStyle : ContainerStyle<Container<Drawable>> { }

    /// <summary>
    /// Base class for defining styles for <see cref="ofContainer"/>.
    /// </summary>
    /// <typeparam name="T">Type of the container to apply styling to.</typeparam>
    public abstract class ContainerStyle<T> : DrawableStyle<T> where T : Container<Drawable>
    {
        /// <inheritdoc cref="CompositeDrawable.Masking"/>
        public bool? Masking;

        /// <inheritdoc cref="CompositeDrawable.MaskingSmoothness"/>
        public float? MaskingSmoothness;

        /// <inheritdoc cref="CompositeDrawable.CornerRadius"/>
        public float? CornerRadius;

        /// <inheritdoc cref="CompositeDrawable.CornerExponent"/>
        public float? CornerExponent;

        /// <inheritdoc cref="CompositeDrawable.BorderThickness"/>
        public float? BorderThickness;

        /// <inheritdoc cref="CompositeDrawable.BorderColour"/>
        public SRGBColour? BorderColour;

        /// <inheritdoc cref="CompositeDrawable.EdgeEffect"/>
        public EdgeEffectParameters? EdgeEffect;

        /// <inheritdoc cref="CompositeDrawable.Padding"/>
        public MarginPadding? Padding;

        /// <inheritdoc cref="CompositeDrawable.ForceLocalVertexBatch"/>
        public bool? ForceLocalVertexBatch;

        /// <inheritdoc cref="CompositeDrawable.RelativeChildSize"/>
        public Vector2? RelativeChildSize;

        /// <inheritdoc cref="CompositeDrawable.RelativeChildOffset"/>
        public Vector2? RelativeChildOffset;

        /// <inheritdoc cref="CompositeDrawable.AutoSizeAxes"/>
        public Axes? AutoSizeAxes;

        /// <inheritdoc cref="CompositeDrawable.AutoSizeDuration"/>
        public float? AutoSizeDuration;

        /// <inheritdoc cref="CompositeDrawable.AutoSizeEasing"/>
        public Easing? AutoSizeEasing;

        protected override void Apply(T drawable)
        {
            base.Apply(drawable);

            if (Masking != null)
                drawable.Masking = Masking.Value;

            if (MaskingSmoothness != null)
                drawable.MaskingSmoothness = MaskingSmoothness.Value;

            if (CornerRadius != null)
                drawable.CornerRadius = CornerRadius.Value;

            if (CornerExponent != null)
                drawable.CornerExponent = CornerExponent.Value;

            if (BorderThickness != null)
                drawable.BorderThickness = BorderThickness.Value;

            if (BorderColour != null)
                drawable.BorderColour = BorderColour.Value;

            if (EdgeEffect != null)
                drawable.EdgeEffect = EdgeEffect.Value;

            if (Padding != null)
                drawable.Padding = Padding.Value;

            if (ForceLocalVertexBatch != null)
                drawable.ForceLocalVertexBatch = ForceLocalVertexBatch.Value;

            if (RelativeChildSize != null)
                drawable.RelativeChildSize = RelativeChildSize.Value;

            if (RelativeChildOffset != null)
                drawable.RelativeChildOffset = RelativeChildOffset.Value;

            if (AutoSizeAxes != null)
                drawable.AutoSizeAxes = AutoSizeAxes.Value;

            if (AutoSizeDuration != null)
                drawable.AutoSizeDuration = AutoSizeDuration.Value;

            if (AutoSizeEasing != null)
                drawable.AutoSizeEasing = AutoSizeEasing.Value;
        }
    }

    /// <summary>
    /// Defines the base class for rendering a <see cref="Container{T}"/> of type <typeparamref name="T"/> and all children inside it.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Container{T}"/> to render.</typeparam>
    public abstract class ofContainerBase<T> : ofDrawableBase<T>, IEnumerable<ofElement> where T : Container<Drawable>
    {
        [Prop] public readonly List<ofElement> Children;

        /// <summary>
        /// Creates a new <see cref="ofContainerBase{T}"/>.
        /// </summary>
        protected ofContainerBase(ElementKey key = default,
                                  RefDelegate<T> @ref = default,
                                  DrawableStyleDelegate<T> style = default,
                                  IEnumerable<ofElement> children = default) : base(key, @ref, style)
        {
            Children = children == null
                ? new List<ofElement>()
                : new List<ofElement>(children);
        }

        /// <summary>
        /// Adds an element as a child of this container.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Add(ofElement element) => Children.Add(element);

        protected override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var nodeRef = UseChild();
            var node    = nodeRef.Current ??= Node.CreateChild();

            return node.RenderElement(new ofPortal(Drawable, children: Children)) == RenderResult.Rendered;
        }

        public IEnumerator<ofElement> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Renders a <see cref="Container{Drawable}"/> and all children inside it.
    /// </summary>
    public sealed class ofContainer : ofContainerBase<Container<Drawable>>
    {
        protected override Container<Drawable> CreateDrawable() => new Container<Drawable>();
    }
}