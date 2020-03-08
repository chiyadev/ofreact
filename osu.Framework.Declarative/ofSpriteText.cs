using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Declarative
{
    public class SpriteTextStyle : DrawableStyle<SpriteText>
    {
        /// <inheritdoc cref="SpriteText.Text"/>
        public LocalisedString? Text;

        /// <inheritdoc cref="SpriteText.Font"/>
        public FontUsage? Font;

        /// <inheritdoc cref="SpriteText.Shadow"/>
        public bool? Shadow;

        /// <inheritdoc cref="SpriteText.ShadowColour"/>
        public Color4? ShadowColour;

        /// <inheritdoc cref="SpriteText.ShadowOffset"/>
        public Vector2? ShadowOffset;

        /// <inheritdoc cref="SpriteText.Spacing"/>
        public Vector2? Spacing;

        /// <inheritdoc cref="SpriteText.Padding"/>
        public MarginPadding? Padding;

        /// <inheritdoc cref="SpriteText.AllowMultiline"/>
        public bool? AllowMultiline;

        /// <inheritdoc cref="SpriteText.UseFullGlyphHeight"/>
        public bool? UseFullGlyphHeight;

        /// <inheritdoc cref="SpriteText.Truncate"/>
        public bool? Truncate;

        /// <inheritdoc cref="SpriteText.EllipsisString"/>
        public string EllipsisString;

        protected override void Apply(SpriteText drawable)
        {
            base.Apply(drawable);

            if (Text != null)
                drawable.Text = Text.Value;

            if (Font != null)
                drawable.Font = Font.Value;

            if (Shadow != null)
                drawable.Shadow = Shadow.Value;

            if (ShadowColour != null)
                drawable.ShadowColour = ShadowColour.Value;

            if (ShadowOffset != null)
                drawable.ShadowOffset = ShadowOffset.Value;

            if (Spacing != null)
                drawable.Spacing = Spacing.Value;

            if (Padding != null)
                drawable.Padding = Padding.Value;

            if (AllowMultiline != null)
                drawable.AllowMultiline = AllowMultiline.Value;

            if (UseFullGlyphHeight != null)
                drawable.UseFullGlyphHeight = UseFullGlyphHeight.Value;

            if (Truncate != null)
                drawable.Truncate = Truncate.Value;

            if (EllipsisString != null)
                drawable.EllipsisString = EllipsisString;
        }
    }

    public class ofSpriteText : ofDrawableBase<SpriteText>
    {
        public ofSpriteText(ElementKey key = default, RefDelegate<SpriteText> @ref = default, DrawableStyleDelegate<SpriteText> style = default) : base(key, @ref, style) { }

        protected override SpriteText CreateDrawable() => new SpriteText();
    }
}