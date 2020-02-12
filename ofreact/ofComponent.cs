namespace ofreact
{
    /// <summary>
    /// Represents the base class for an ofreact component.
    /// </summary>
    public abstract class ofComponent : ofElement
    {
        /// <summary>
        /// Creates a new <see cref="ofComponent"/>/
        /// </summary>
        protected ofComponent(object key = default) : base(key) { }

        /// <summary>
        /// Renders this component and returns the rendered element.
        /// </summary>
        protected abstract ofElement Render();

        protected internal sealed override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var node    = UseChild();
            var element = Render();

            if (element == null)
                return false;

            if (!element.Equals(node.Current.Element))
            {
                node.Current.Dispose();
                node.Current = Node.CreateChild();
            }

            return node.Current.RenderElement(element);
        }
    }
}