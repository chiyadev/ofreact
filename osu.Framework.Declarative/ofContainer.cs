using System.Collections;
using System.Collections.Generic;
using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osuTK;

namespace osu.Framework.Declarative
{
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

    public class ofContainer : ofDrawable<Container<Drawable>>, IEnumerable<ofElement>
    {
        [Prop] public readonly List<ofElement> Children;

        public ofContainer(object key = default, RefDelegate<Container<Drawable>> @ref = default, DrawableStyleDelegate<Container<Drawable>> style = default, IEnumerable<ofElement> children = default) : base(key, @ref, style)
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

            var node = UseChild();

            var context = new DrawableRenderContext(Drawable);

            return node.Current.RenderElement(new ofContext<DrawableRenderContext>(children: Children, value: context));
        }

        public IEnumerator<ofElement> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}