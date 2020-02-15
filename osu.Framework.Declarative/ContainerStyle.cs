using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osuTK;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Contains styling information for <see cref="ofContainer"/>.
    /// </summary>
    public class ContainerStyle : DrawableStyle<Container<Drawable>>
    {
        /// <inheritdoc cref="CompositeDrawable.Masking"/>
        public bool Masking { get; set; }

        /// <inheritdoc cref="CompositeDrawable.MaskingSmoothness"/>
        public float MaskingSmoothness { get; set; }

        /// <inheritdoc cref="CompositeDrawable.CornerRadius"/>
        public float CornerRadius { get; set; }

        /// <inheritdoc cref="CompositeDrawable.CornerExponent"/>
        public float CornerExponent { get; set; }

        /// <inheritdoc cref="CompositeDrawable.BorderThickness"/>
        public float BorderThickness { get; set; }

        /// <inheritdoc cref="CompositeDrawable.BorderColour"/>
        public SRGBColour BorderColour { get; set; }

        /// <inheritdoc cref="CompositeDrawable.EdgeEffect"/>
        public EdgeEffectParameters EdgeEffect { get; set; }

        /// <inheritdoc cref="CompositeDrawable.Padding"/>
        public MarginPadding Padding { get; set; }

        /// <inheritdoc cref="CompositeDrawable.ForceLocalVertexBatch"/>
        public bool ForceLocalVertexBatch { get; set; }

        /// <inheritdoc cref="CompositeDrawable.RelativeChildSize"/>
        public Vector2 RelativeChildSize { get; set; }

        /// <inheritdoc cref="CompositeDrawable.RelativeChildOffset"/>
        public Vector2 RelativeChildOffset { get; set; }

        /// <inheritdoc cref="CompositeDrawable.AutoSizeAxes"/>
        public Axes AutoSizeAxes { get; set; }

        /// <inheritdoc cref="CompositeDrawable.AutoSizeDuration"/>
        public float AutoSizeDuration { get; set; }

        /// <inheritdoc cref="CompositeDrawable.AutoSizeEasing"/>
        public Easing AutoSizeEasing { get; set; }

        protected override void Apply(Container<Drawable> drawable)
        {
            base.Apply(drawable);

            drawable.Masking               = Masking;
            drawable.MaskingSmoothness     = MaskingSmoothness;
            drawable.CornerRadius          = CornerRadius;
            drawable.CornerExponent        = CornerExponent;
            drawable.BorderThickness       = BorderThickness;
            drawable.BorderColour          = BorderColour;
            drawable.EdgeEffect            = EdgeEffect;
            drawable.Padding               = Padding;
            drawable.ForceLocalVertexBatch = ForceLocalVertexBatch;
            drawable.RelativeChildSize     = RelativeChildSize;
            drawable.RelativeChildOffset   = RelativeChildOffset;
            drawable.AutoSizeAxes          = AutoSizeAxes;
            drawable.AutoSizeDuration      = AutoSizeDuration;
            drawable.AutoSizeEasing        = AutoSizeEasing;
        }
    }
}