using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers.Markdown;
using osu.Framework.Input.Events;

namespace osu.Framework.Declarative
{
    public sealed class MarkdownStyle : MarkdownStyle<MarkdownContainer> { }

    public abstract class MarkdownStyle<T> : DrawableStyle<T> where T : MarkdownContainer
    {
        /// <inheritdoc cref="MarkdownContainer.AutoSizeAxes"/>
        public Axes? AutoSizeAxes;

        /// <inheritdoc cref="MarkdownContainer.Text"/>
        public string Text;

        /// <inheritdoc cref="MarkdownContainer.LineSpacing"/>
        public float? LineSpacing;

        /// <inheritdoc cref="MarkdownContainer.DocumentMargin"/>
        public MarginPadding? DocumentMargin;

        /// <inheritdoc cref="MarkdownContainer.DocumentPadding"/>
        public MarginPadding? DocumentPadding;

        protected override void Apply(T drawable)
        {
            base.Apply(drawable);

            if (AutoSizeAxes != null)
                drawable.AutoSizeAxes = AutoSizeAxes.Value;

            if (Text != null)
                drawable.Text = Text;

            if (LineSpacing != null)
                drawable.LineSpacing = LineSpacing.Value;

            if (DocumentMargin != null)
                drawable.DocumentMargin = DocumentMargin.Value;

            if (DocumentPadding != null)
                drawable.DocumentPadding = DocumentPadding.Value;
        }
    }

    public sealed class ofMarkdown : ofMarkdown<MarkdownContainer>
    {
        public ofMarkdown(ElementKey key = default, RefDelegate<MarkdownContainer> @ref = default, DrawableStyleDelegate<MarkdownContainer> style = default, DrawableEventDelegate @event = default) : base(key, @ref, style, @event) { }

        protected override MarkdownContainer CreateDrawable() => new InternalMarkdownContainer();

        sealed class InternalMarkdownContainer : MarkdownContainer, ISupportEventDelegation
        {
            public DrawableEventDelegate EventDelegate { get; set; }
            public override bool HandlePositionalInput => base.HandlePositionalInput || EventDelegate != null;
            public override bool HandleNonPositionalInput => base.HandleNonPositionalInput || EventDelegate != null;

            protected override bool Handle(UIEvent e) => base.Handle(e) || EventDelegate(e);
        }
    }

    public abstract class ofMarkdown<T> : ofDrawableBase<T> where T : MarkdownContainer
    {
        protected ofMarkdown(ElementKey key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default, DrawableEventDelegate @event = default) : base(key, @ref, style, @event) { }
    }
}