using System.Collections.Generic;
using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Applies transformations on the nearest descendant <see cref="ofDrawable{T}"/>.
    /// </summary>
    public class ofTransform : ofContext<IDrawableRenderContext>
    {
        /// <summary>
        /// Creates a new <see cref="ofTransform"/>.
        /// </summary>
        public ofTransform(ElementKey key = default, IEnumerable<ofElement> children = default, RefDelegate<ofTransform> @ref = default) : base(key, children, new Context())
        {
            @ref?.Invoke(this);
        }

        protected override bool RenderSubtree()
        {
            ((Context) Value).Parent = Node.FindNearestContext<IDrawableRenderContext>().Value;

            return base.RenderSubtree();
        }

        sealed class Context : IDrawableRenderContext
        {
            public IDrawableRenderContext Parent { get; set; }

            IReadOnlyDependencyContainer IDrawableRenderContext.DependencyContainer => Parent.DependencyContainer;

            public void Render(Drawable drawable, bool explicitDepth)
                => throw new System.NotImplementedException();
        }
    }
}