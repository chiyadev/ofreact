using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Declarative
{
    public delegate void DrawableStyleDelegate<in T>(T drawable) where T : Drawable;

    public class DrawableStyle : DrawableStyle<Drawable> { }

    public abstract class DrawableStyle<T> where T : Drawable
    {
        /// <inheritdoc cref="Drawable.Depth"/>
        public float Depth { get; set; }

        /// <inheritdoc cref="Drawable.Position"/>
        public Vector2 Position { get; set; }

        /// <inheritdoc cref="Drawable.RelativePositionAxes"/>
        public Axes RelativePositionAxes { get; set; }

        /// <inheritdoc cref="Drawable.Size"/>
        public Vector2 Size { get; set; }

        /// <inheritdoc cref="Drawable.RelativeSizeAxes"/>
        public Axes RelativeSizeAxes { get; set; }

        /// <inheritdoc cref="Drawable.Margin"/>
        public MarginPadding Margin { get; set; }

        /// <inheritdoc cref="Drawable.BypassAutoSizeAxes"/>
        public Axes BypassAutoSizeAxes { get; set; }

        /// <inheritdoc cref="Drawable.Scale"/>
        public Vector2 Scale { get; set; }

        /// <inheritdoc cref="Drawable.FillAspectRatio"/>
        public float FillAspectRatio { get; set; }

        /// <inheritdoc cref="Drawable.FillMode"/>
        public FillMode FillMode { get; set; }

        /// <inheritdoc cref="Drawable.Shear"/>
        public Vector2 Shear { get; set; }

        /// <inheritdoc cref="Drawable.Rotation"/>
        public float Rotation { get; set; }

        /// <inheritdoc cref="Drawable.Origin"/>
        public Anchor Origin { get; set; }

        /// <inheritdoc cref="Drawable.OriginPosition"/>
        public Vector2 OriginPosition { get; set; }

        /// <inheritdoc cref="Drawable.Anchor"/>
        public Anchor Anchor { get; set; }

        /// <inheritdoc cref="Drawable.RelativeAnchorPosition"/>
        public Vector2 RelativeAnchorPosition { get; set; }

        /// <inheritdoc cref="Drawable.Colour"/>
        public ColourInfo Colour { get; set; }

        /// <inheritdoc cref="Drawable.Alpha"/>
        public float Alpha { get; set; }

        /// <inheritdoc cref="Drawable.AlwaysPresent"/>
        public bool AlwaysPresent { get; set; }

        /// <inheritdoc cref="Drawable.Blending"/>
        public BlendingParameters Blending { get; set; }

        /// <inheritdoc cref="Drawable.LifetimeStart"/>
        public double LifetimeStart { get; set; }

        /// <inheritdoc cref="Drawable.LifetimeEnd"/>
        public double LifetimeEnd { get; set; }

        protected virtual void Apply(T drawable)
        {
            if (drawable.Parent is Container<Drawable> container)
                container.ChangeChildDepth(drawable, Depth);

            else if (drawable.Depth != Depth)
                drawable.Depth = Depth;

            drawable.Position               = Position;
            drawable.RelativePositionAxes   = RelativePositionAxes;
            drawable.Size                   = Size;
            drawable.RelativeSizeAxes       = RelativeSizeAxes;
            drawable.Margin                 = Margin;
            drawable.BypassAutoSizeAxes     = BypassAutoSizeAxes;
            drawable.Scale                  = Scale;
            drawable.FillAspectRatio        = FillAspectRatio;
            drawable.FillMode               = FillMode;
            drawable.Shear                  = Shear;
            drawable.Rotation               = Rotation;
            drawable.Origin                 = Origin;
            drawable.OriginPosition         = OriginPosition;
            drawable.Anchor                 = Anchor;
            drawable.RelativeAnchorPosition = RelativeAnchorPosition;
            drawable.Colour                 = Colour;
            drawable.Alpha                  = Alpha;
            drawable.AlwaysPresent          = AlwaysPresent;
            drawable.Blending               = Blending;
            drawable.LifetimeStart          = LifetimeStart;
            drawable.LifetimeEnd            = LifetimeEnd;
        }

        public static implicit operator DrawableStyleDelegate<T>(DrawableStyle<T> style) => style.Apply;
    }

    public class ofDrawable<T> : ofElement where T : Drawable, new()
    {
        [Prop] public readonly RefDelegate<T> Ref;
        [Prop] public readonly DrawableStyleDelegate<T> Style;

        public ofDrawable(object key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default) : base(key)
        {
            Ref   = @ref;
            Style = style;
        }

        protected RefObject<T> Drawable { get; private set; }

        protected override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var context = UseContext<DrawableRenderContext>();

            if (context == null)
                return false;

            // create drawable
            var drawable = (Drawable = UseRef<T>()).Current;

            bool drawableCreated;

            if (drawable == null)
            {
                drawable        = Drawable.Current = new T();
                drawableCreated = true;
            }
            else
            {
                drawableCreated = false;
            }

            drawable.Name = Key?.ToString();

            // add styling
            Style?.Invoke(drawable);

            // render drawable
            var explicitDepth = UseRef(drawableCreated && drawable.Depth == 0).Current;

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
}