using ofreact;
using osu.Framework.Graphics;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Defines the base class for rendering a <see cref="Drawable"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Drawable"/> to render.</typeparam>
    public abstract class ofDrawableBase<T> : ofElement where T : Drawable
    {
        [Prop] public readonly RefDelegate<T> Ref;
        [Prop] public readonly DrawableStyleDelegate<T> Style;

        /// <summary>
        /// Creates a new <see cref="ofDrawableBase{T}"/>.
        /// </summary>
        protected ofDrawableBase(object key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default) : base(key)
        {
            Ref   = @ref;
            Style = style;
        }

        /// <summary>
        /// Creates a new drawable of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Newly created drawable.</returns>
        protected abstract T CreateDrawable();

        /// <summary>
        /// Gets the drawable that was rendered.
        /// </summary>
        protected RefObject<T> Drawable { get; private set; }

        protected override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var context = UseContext<IDrawableRenderContext>();

            if (context == null)
                return false;

            // create drawable
            var drawable = (Drawable = UseRef<T>()).Current;

            bool drawableCreated;

            if (drawable == null)
            {
                drawable        = Drawable.Current = CreateDrawable();
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
            var explicitDepth = UseRef(drawableCreated && drawable.Depth != 0).Current;

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

    /// <summary>
    /// Renders a <see cref="Drawable"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Drawable"/> to render.</typeparam>
    public sealed class ofDrawable<T> : ofDrawableBase<T> where T : Drawable, new()
    {
        public ofDrawable(object key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default) : base(key, @ref, style) { }

        protected override T CreateDrawable() => new T();
    }
}