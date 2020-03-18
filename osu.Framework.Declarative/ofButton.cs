using ofreact;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace osu.Framework.Declarative
{
    public sealed class ButtonStyle : ButtonStyle<BasicButton> { }

    public abstract class ButtonStyle<T> : DrawableStyle<T> where T : BasicButton
    {
        /// <inheritdoc cref="BasicButton.Text"/>
        public string Text;

        /// <inheritdoc cref="BasicButton.BackgroundColour"/>
        public Color4? BackgroundColour;

        /// <inheritdoc cref="BasicButton.FlashColour"/>
        public Color4? FlashColour;

        /// <inheritdoc cref="BasicButton.HoverColour"/>
        public Color4? HoverColour;

        /// <inheritdoc cref="BasicButton.DisabledColour"/>
        public Color4? DisabledColour;

        /// <inheritdoc cref="BasicButton.HoverFadeDuration"/>
        public double? HoverFadeDuration;

        /// <inheritdoc cref="BasicButton.FlashDuration"/>
        public double? FlashDuration;

        /// <inheritdoc cref="BasicButton.DisabledFadeDuration"/>
        public double? DisabledFadeDuration;

        protected override void Apply(T drawable)
        {
            base.Apply(drawable);

            if (Text != null)
                drawable.Text = Text;

            if (BackgroundColour != null)
                drawable.BackgroundColour = BackgroundColour.Value;

            if (FlashColour != null)
                drawable.FlashColour = FlashColour.Value;

            if (HoverColour != null)
                drawable.HoverColour = HoverColour.Value;

            if (DisabledColour != null)
                drawable.DisabledColour = DisabledColour.Value;

            if (HoverFadeDuration != null)
                drawable.HoverFadeDuration = HoverFadeDuration.Value;

            if (FlashDuration != null)
                drawable.FlashDuration = FlashDuration.Value;

            if (DisabledFadeDuration != null)
                drawable.DisabledFadeDuration = DisabledFadeDuration.Value;
        }
    }

    public sealed class ofButton : ofButton<BasicButton>
    {
        public ofButton(ElementKey key = default, RefDelegate<BasicButton> @ref = default, DrawableStyleDelegate<BasicButton> style = default, DrawableEventDelegate @event = default) : base(key, @ref, style, @event) { }

        protected override BasicButton CreateDrawable() => new InternalButton();

        sealed class InternalButton : BasicButton, ISupportEventDelegation
        {
            public DrawableEventDelegate EventDelegate { get; set; }
            public override bool HandlePositionalInput => base.HandlePositionalInput || EventDelegate != null;
            public override bool HandleNonPositionalInput => base.HandleNonPositionalInput || EventDelegate != null;

            protected override bool Handle(UIEvent e) => base.Handle(e) || EventDelegate(e);
        }
    }

    public abstract class ofButton<T> : ofDrawableBase<T> where T : Button
    {
        protected ofButton(ElementKey key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default, DrawableEventDelegate @event = default) : base(key, @ref, style, @event) { }
    }
}