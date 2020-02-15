using ofreact;
using osu.Framework.Graphics;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Represents a <see cref="Drawable"/> in the osu!framework scene graph.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Drawable"/> to render.</typeparam>
    public class ofDrawable<T> : ofElement where T : Drawable, new()
    {
        [Prop] public readonly RefDelegate<T> Ref;
        [Prop] public readonly DrawableStyleDelegate<T> Style;

        /// <summary>
        /// Creates a new <see cref="ofDrawable{T}"/>.
        /// </summary>
        public ofDrawable(object key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default) : base(key)
        {
            Ref   = @ref;
            Style = style;
        }

        protected RefObject<T> Drawable { get; private set; }

        protected override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var context = UseContext<DrawableRenderContext>();

            if (context == null)
                return false;

            // create drawable
            var drawable = (Drawable = UseRef<T>()).Current;

            bool drawableCreated;

            if (drawable == null)
            {
                drawable        = Drawable.Current = new T();
                drawableCreated = true;
            }
            else
            {
                drawableCreated = false;
            }

            drawable.Name = Key?.ToString();

            // add styling
            Style?.Invoke(drawable);

            // render drawable
            var explicitDepth = UseRef(drawableCreated && drawable.Depth == 0).Current;

            context.Render(drawable, explicitDepth);

            // invoke ref callback
            if (drawableCreated)
                Ref?.Invoke(drawable);

            // schedule expiry on unmount
            UseEffect(() => () =>
            {
                drawable.Expire();

                Ref?.Invoke(null);
            }, null);

            return true;
        }
    }
}