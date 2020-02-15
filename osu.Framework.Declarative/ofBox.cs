using ofreact;
using osu.Framework.Graphics.Shapes;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Renders a <see cref="Box"/>.
    /// </summary>
    public class ofBox : ofDrawableBase<Box>
    {
        /// <summary>
        /// Creates a new <see cref="ofBox"/>.
        /// </summary>
        public ofBox(object key = default,
                     RefDelegate<Box> @ref = default,
                     DrawableStyleDelegate<Box> style = default) : base(key, @ref, style) { }

        protected override Box CreateDrawable() => new Box();
    }
}