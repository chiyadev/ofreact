using ofreact;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

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
        public ofBox(ElementKey key = default, RefDelegate<Box> @ref = default, DrawableStyleDelegate<Box> style = default, DrawableEventDelegate @event = default) : base(key, @ref, style, @event) { }

        protected override Box CreateDrawable() => new InternalBox();

        sealed class InternalBox : Box, ISupportEventDelegation
        {
            public DrawableEventDelegate EventDelegate { get; set; }
            public override bool HandlePositionalInput => base.HandlePositionalInput || EventDelegate != null;
            public override bool HandleNonPositionalInput => base.HandleNonPositionalInput || EventDelegate != null;

            protected override bool Handle(UIEvent e) => base.Handle(e) || EventDelegate(e);
        }
    }
}