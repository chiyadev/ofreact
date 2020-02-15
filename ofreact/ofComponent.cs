namespace ofreact
{
    /// <summary>
    /// Represents the base class for an ofreact component.
    /// </summary>
    public abstract class ofComponent : ofElement
    {
        /// <summary>
        /// Creates a new <see cref="ofComponent"/>.
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

            var nodeRef = UseChild();
            var node    = nodeRef.Current;

            var element = Render();

            if (element == null)
            {
                if (node != null)
                {
                    node.Dispose();
                    nodeRef.Current = null;
                }

                return false;
            }

            if (node == null || !node.CanRenderElement(element))
            {
                node?.Dispose();
                node = nodeRef.Current = Node.CreateChild();
            }

            return node.RenderElement(element);
        }
    }
}