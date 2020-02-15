using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Encapsulates a method that applies styling to a <see cref="Drawable"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="drawable">Drawable to apply styling to.</param>
    /// <typeparam name="T">Type of the drawable.</typeparam>
    public delegate void DrawableStyleDelegate<in T>(T drawable) where T : Drawable;

    /// <summary>
    /// Contains styling information for <see cref="ofDrawable{T}"/>.
    /// </summary>
    public sealed class DrawableStyle : DrawableStyle<Drawable> { }

    /// <summary>
    /// Base class for defining styles for <see cref="ofDrawable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the drawable to apply styling to.</typeparam>
    public abstract class DrawableStyle<T> where T : Drawable
    {
        /// <inheritdoc cref="Drawable.Depth"/>
        public float? Depth;

        /// <inheritdoc cref="Drawable.Position"/>
        public Vector2? Position;

        /// <inheritdoc cref="Drawable.RelativePositionAxes"/>
        public Axes? RelativePositionAxes;

        /// <inheritdoc cref="Drawable.Size"/>
        public Vector2? Size;

        /// <inheritdoc cref="Drawable.RelativeSizeAxes"/>
        public Axes? RelativeSizeAxes;

        /// <inheritdoc cref="Drawable.Margin"/>
        public MarginPadding? Margin;

        /// <inheritdoc cref="Drawable.BypassAutoSizeAxes"/>
        public Axes? BypassAutoSizeAxes;

        /// <inheritdoc cref="Drawable.Scale"/>
        public Vector2? Scale;

        /// <inheritdoc cref="Drawable.FillAspectRatio"/>
        public float? FillAspectRatio;

        /// <inheritdoc cref="Drawable.FillMode"/>
        public FillMode? FillMode;

        /// <inheritdoc cref="Drawable.Shear"/>
        public Vector2? Shear;

        /// <inheritdoc cref="Drawable.Rotation"/>
        public float? Rotation;

        /// <inheritdoc cref="Drawable.Origin"/>
        public Anchor? Origin;

        /// <inheritdoc cref="Drawable.OriginPosition"/>
        public Vector2? OriginPosition;

        /// <inheritdoc cref="Drawable.Anchor"/>
        public Anchor? Anchor;

        /// <inheritdoc cref="Drawable.RelativeAnchorPosition"/>
        public Vector2? RelativeAnchorPosition;

        /// <inheritdoc cref="Drawable.Colour"/>
        public ColourInfo? Colour;

        /// <inheritdoc cref="Drawable.Alpha"/>
        public float? Alpha;

        /// <inheritdoc cref="Drawable.AlwaysPresent"/>
        public bool? AlwaysPresent;

        /// <inheritdoc cref="Drawable.Blending"/>
        public BlendingParameters? Blending;

        /// <inheritdoc cref="Drawable.LifetimeStart"/>
        public double? LifetimeStart;

        /// <inheritdoc cref="Drawable.LifetimeEnd"/>
        public double? LifetimeEnd;

        /// <summary>
        /// Applies styling to the given <see cref="Drawable"/>.
        /// </summary>
        protected virtual void Apply(T drawable)
        {
            if (Depth != null && drawable.Depth != Depth.Value)
                switch (drawable.Parent)
                {
                    case null:
                        drawable.Depth = Depth.Value;
                        break;

                    case Container<Drawable> container:
                        container.ChangeChildDepth(drawable, Depth.Value);
                        break;
                }

            if (Position != null)
                drawable.Position = Position.Value;

            if (RelativePositionAxes != null)
                drawable.RelativePositionAxes = RelativePositionAxes.Value;

            if (Size != null)
                drawable.Size = Size.Value;

            if (RelativeSizeAxes != null)
                drawable.RelativeSizeAxes = RelativeSizeAxes.Value;

            if (Margin != null)
                drawable.Margin = Margin.Value;

            if (BypassAutoSizeAxes != null)
                drawable.BypassAutoSizeAxes = BypassAutoSizeAxes.Value;

            if (Scale != null)
                drawable.Scale = Scale.Value;

            if (FillAspectRatio != null)
                drawable.FillAspectRatio = FillAspectRatio.Value;

            if (FillMode != null)
                drawable.FillMode = FillMode.Value;

            if (Shear != null)
                drawable.Shear = Shear.Value;

            if (Rotation != null)
                drawable.Rotation = Rotation.Value;

            if (Origin != null)
                drawable.Origin = Origin.Value;

            if (OriginPosition != null)
                drawable.OriginPosition = OriginPosition.Value;

            if (Anchor != null)
                drawable.Anchor = Anchor.Value;

            if (RelativeAnchorPosition != null)
                drawable.RelativeAnchorPosition = RelativeAnchorPosition.Value;

            if (Colour != null)
                drawable.Colour = Colour.Value;

            if (Alpha != null)
                drawable.Alpha = Alpha.Value;

            if (AlwaysPresent != null)
                drawable.AlwaysPresent = AlwaysPresent.Value;

            if (Blending != null)
                drawable.Blending = Blending.Value;

            if (LifetimeStart != null)
                drawable.LifetimeStart = LifetimeStart.Value;

            if (LifetimeEnd != null)
                drawable.LifetimeEnd = LifetimeEnd.Value;
        }

        public static implicit operator DrawableStyleDelegate<T>(DrawableStyle<T> style) => style.Apply;
    }

    /// <summary>
    /// Defines the base class for rendering a <see cref="Drawable"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Drawable"/> to render.</typeparam>
    public abstract class ofDrawableBase<T> : ofElement where T : Drawable
    {
        [Prop] public readonly RefDelegate<T> Ref;
        [Prop] public readonly DrawableStyleDelegate<T> Style;

        /// <summary>
        /// Creates a new <see cref="ofDrawableBase{T}"/>.
        /// </summary>
        protected ofDrawableBase(object key = default,
                                 RefDelegate<T> @ref = default,
                                 DrawableStyleDelegate<T> style = default) : base(key)
        {
            Ref   = @ref;
            Style = style;
        }

        /// <summary>
        /// Creates a new drawable of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Newly created drawable.</returns>
        protected abstract T CreateDrawable();

        /// <summary>
        /// Applies styles on the given drawable.
        /// </summary>
        protected virtual void ApplyStyles(T drawable) => Style?.Invoke(drawable);

        /// <summary>
        /// Reference of the drawable that was rendered.
        /// </summary>
        protected RefObject<T> Drawable { get; private set; }

        protected override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var context = UseContext<IDrawableRenderContext>();

            if (context == null)
                return false;

            // create drawable
            var drawable = (Drawable = UseRef<T>()).Current;

            bool drawableCreated;

            if (drawable == null)
            {
                drawable        = Drawable.Current = CreateDrawable();
                drawableCreated = true;
            }
            else
            {
                drawableCreated = false;
            }

            drawable.Name = Key?.ToString();

            // add styling
            ApplyStyles(drawable);

            // render drawable
            var explicitDepth = UseRef(drawableCreated && drawable.Depth != 0).Current;

            context.Render(drawable, explicitDepth);

            // invoke ref callback
            if (drawableCreated)
                Ref?.Invoke(drawable);

            // schedule expiry on unmount
            UseEffect(() => () =>
            {
                drawable.Expire();

                Ref?.Invoke(null);
            }, null);

            return true;
        }
    }

    /// <summary>
    /// Renders a <see cref="Drawable"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Drawable"/> to render.</typeparam>
    public sealed class ofDrawable<T> : ofDrawableBase<T> where T : Drawable, new()
    {
        public ofDrawable(object key = default,
                          RefDelegate<T> @ref = default,
                          DrawableStyleDelegate<T> style = default) : base(key, @ref, style) { }

        protected override T CreateDrawable() => new T();
    }
}