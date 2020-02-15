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
    public class DrawableStyle : DrawableStyle<Drawable> { }

    /// <summary>
    /// Base class for defining styles for <see cref="Drawable"/>.
    /// </summary>
    /// <typeparam name="T">Type of the drawable to apply styling to.</typeparam>
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

        /// <summary>
        /// Applies styling to the given <see cref="Drawable"/>.
        /// </summary>
        protected virtual void Apply(T drawable)
        {
            if (drawable.Depth != Depth)
                switch (drawable.Parent)
                {
                    case null:
                        drawable.Depth = Depth;
                        break;

                    case Container<Drawable> container:
                        container.ChangeChildDepth(drawable, Depth);
                        break;
                }

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
}