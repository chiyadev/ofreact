using System;
using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using static ofreact.Hooks;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Encapsulates a method that applies styling to a <see cref="Drawable"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="drawable">Drawable to apply styling to.</param>
    /// <typeparam name="T">Type of the drawable.</typeparam>
    public delegate void DrawableStyleDelegate<in T>(T drawable) where T : Drawable;

    /// <summary>
    /// Contains styling information for <see cref="ofDrawable{T}"/>.
    /// </summary>
    public sealed class DrawableStyle : DrawableStyle<Drawable> { }

    /// <summary>
    /// Base class for defining styles for <see cref="ofDrawable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the drawable to apply styling to.</typeparam>
    public abstract class DrawableStyle<T> where T : Drawable
    {
        /// <inheritdoc cref="Drawable.Depth"/>
        public float? Depth;

        /// <inheritdoc cref="Drawable.Position"/>
        public Vector2? Position;

        /// <inheritdoc cref="Drawable.RelativePositionAxes"/>
        public Axes? RelativePositionAxes;

        /// <inheritdoc cref="Drawable.Size"/>
        public Vector2? Size;

        /// <inheritdoc cref="Drawable.RelativeSizeAxes"/>
        public Axes? RelativeSizeAxes;

        /// <inheritdoc cref="Drawable.Margin"/>
        public MarginPadding? Margin;

        /// <inheritdoc cref="Drawable.BypassAutoSizeAxes"/>
        public Axes? BypassAutoSizeAxes;

        /// <inheritdoc cref="Drawable.Scale"/>
        public Vector2? Scale;

        /// <inheritdoc cref="Drawable.FillAspectRatio"/>
        public float? FillAspectRatio;

        /// <inheritdoc cref="Drawable.FillMode"/>
        public FillMode? FillMode;

        /// <inheritdoc cref="Drawable.Shear"/>
        public Vector2? Shear;

        /// <inheritdoc cref="Drawable.Rotation"/>
        public float? Rotation;

        /// <inheritdoc cref="Drawable.Origin"/>
        public Anchor? Origin;

        /// <inheritdoc cref="Drawable.OriginPosition"/>
        public Vector2? OriginPosition;

        /// <inheritdoc cref="Drawable.Anchor"/>
        public Anchor? Anchor;

        /// <inheritdoc cref="Drawable.RelativeAnchorPosition"/>
        public Vector2? RelativeAnchorPosition;

        /// <inheritdoc cref="Drawable.Colour"/>
        public ColourInfo? Colour;

        /// <inheritdoc cref="Drawable.Alpha"/>
        public float? Alpha;

        /// <inheritdoc cref="Drawable.AlwaysPresent"/>
        public bool? AlwaysPresent;

        /// <inheritdoc cref="Drawable.Blending"/>
        public BlendingParameters? Blending;

        /// <inheritdoc cref="Drawable.LifetimeStart"/>
        public double? LifetimeStart;

        /// <inheritdoc cref="Drawable.LifetimeEnd"/>
        public double? LifetimeEnd;

        /// <summary>
        /// Applies styling to the given <see cref="Drawable"/>.
        /// </summary>
        protected virtual void Apply(T drawable)
        {
            if (Depth != null && drawable.Depth != Depth.Value)
                switch (drawable.Parent)
                {
                    case null:
                        drawable.Depth = Depth.Value;
                        break;

                    case Container<Drawable> container:
                        container.ChangeChildDepth(drawable, Depth.Value);
                        break;
                }

            if (Position != null)
                drawable.Position = Position.Value;

            if (RelativePositionAxes != null)
                drawable.RelativePositionAxes = RelativePositionAxes.Value;

            if (Size != null)
                drawable.Size = Size.Value;

            if (RelativeSizeAxes != null)
                drawable.RelativeSizeAxes = RelativeSizeAxes.Value;

            if (Margin != null)
                drawable.Margin = Margin.Value;

            if (BypassAutoSizeAxes != null)
                drawable.BypassAutoSizeAxes = BypassAutoSizeAxes.Value;

            if (Scale != null)
                drawable.Scale = Scale.Value;

            if (FillAspectRatio != null)
                drawable.FillAspectRatio = FillAspectRatio.Value;

            if (FillMode != null)
                drawable.FillMode = FillMode.Value;

            if (Shear != null)
                drawable.Shear = Shear.Value;

            if (Rotation != null)
                drawable.Rotation = Rotation.Value;

            if (Origin != null)
                drawable.Origin = Origin.Value;

            if (OriginPosition != null)
                drawable.OriginPosition = OriginPosition.Value;

            if (Anchor != null)
                drawable.Anchor = Anchor.Value;

            if (RelativeAnchorPosition != null)
                drawable.RelativeAnchorPosition = RelativeAnchorPosition.Value;

            if (Colour != null)
                drawable.Colour = Colour.Value;

            if (Alpha != null)
                drawable.Alpha = Alpha.Value;

            if (AlwaysPresent != null)
                drawable.AlwaysPresent = AlwaysPresent.Value;

            if (Blending != null)
                drawable.Blending = Blending.Value;

            if (LifetimeStart != null)
                drawable.LifetimeStart = LifetimeStart.Value;

            if (LifetimeEnd != null)
                drawable.LifetimeEnd = LifetimeEnd.Value;
        }

        public static implicit operator DrawableStyleDelegate<T>(DrawableStyle<T> style) => style.Apply;
    }

    /// <summary>
    /// Encapsulates a method that handles an input event triggered on a <see cref="Drawable"/>.
    /// </summary>
    /// <remarks>
    /// Not all drawable elements (i.e. inheritors of <see cref="ofDrawableBase{T}"/>) support this delegate.
    /// Unlike styling delegates that simply manipulate public properties of a drawable,
    /// event delegates must be invoked manually by deriving the drawable being wrapped and implementing <see cref="ISupportEventDelegation"/> which is not always possible.
    /// </remarks>
    /// <param name="event">Input event information.</param>
    public delegate bool DrawableEventDelegate(UIEvent @event);

    /// <summary>
    /// Indicates that the <see cref="Drawable"/> implementing this interface supports delegation of input events.
    /// </summary>
    public interface ISupportEventDelegation : IDrawable
    {
        DrawableEventDelegate EventDelegate { get; set; }
    }

    /// <summary>
    /// Contains event handlers for <see cref="ofDrawable{T}"/>.
    /// </summary>
    public sealed class DrawableEvent : DrawableEvent<Drawable>
    {
        internal static readonly DrawableEventDelegate EmptyDelegate = _ => false;
    }

    /// <summary>
    /// Base class for defining event handlers for <see cref="ofDrawable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the drawable to handle the events of.</typeparam>
    public abstract class DrawableEvent<T> where T : Drawable
    {
        /// <inheritdoc cref="Drawable.OnMouseMove"/>
        public Action<MouseMoveEvent> OnMouseMove;

        /// <inheritdoc cref="Drawable.OnHover"/>
        public Action<HoverEvent> OnHover;

        /// <inheritdoc cref="Drawable.OnHoverLost"/>
        public Action<HoverLostEvent> OnHoverLost;

        /// <inheritdoc cref="Drawable.OnMouseDown"/>
        public Action<MouseDownEvent> OnMouseDown;

        /// <inheritdoc cref="Drawable.OnMouseUp"/>
        public Action<MouseUpEvent> OnMouseUp;

        /// <inheritdoc cref="Drawable.OnClick"/>
        public Action<ClickEvent> OnClick;

        /// <inheritdoc cref="Drawable.OnDoubleClick"/>
        public Action<DoubleClickEvent> OnDoubleClick;

        /// <inheritdoc cref="Drawable.OnDragStart"/>
        public Action<DragStartEvent> OnDragStart;

        /// <inheritdoc cref="Drawable.OnDrag"/>
        public Action<DragEvent> OnDrag;

        /// <inheritdoc cref="Drawable.OnDragEnd"/>
        public Action<DragEndEvent> OnDragEnd;

        /// <inheritdoc cref="Drawable.OnScroll"/>
        public Action<ScrollEvent> OnScroll;

        /// <inheritdoc cref="Drawable.OnFocus"/>
        public Action<FocusEvent> OnFocus;

        /// <inheritdoc cref="Drawable.OnFocusLost"/>
        public Action<FocusLostEvent> OnFocusLost;

        /// <inheritdoc cref="Drawable.OnKeyDown"/>
        public Action<KeyDownEvent> OnKeyDown;

        /// <inheritdoc cref="Drawable.OnKeyUp"/>
        public Action<KeyUpEvent> OnKeyUp;

        /// <inheritdoc cref="Drawable.OnJoystickPress"/>
        public Action<JoystickPressEvent> OnJoystickPress;

        /// <inheritdoc cref="Drawable.OnJoystickRelease"/>
        public Action<JoystickReleaseEvent> OnJoystickRelease;

        /// <summary>
        /// Handles input event of the given <see cref="Drawable"/>.
        /// </summary>
        protected virtual bool Handle(UIEvent @event)
        {
            switch (@event)
            {
                case MouseMoveEvent e when OnMouseMove != null:
                    OnMouseMove(e);
                    return true;

                case HoverEvent e when OnHover != null:
                    OnHover(e);
                    return true;

                case HoverLostEvent e when OnHoverLost != null:
                    OnHoverLost(e);
                    return true;

                case MouseDownEvent e when OnMouseDown != null:
                    OnMouseDown(e);
                    return true;

                case MouseUpEvent e when OnMouseUp != null:
                    OnMouseUp(e);
                    return true;

                case ClickEvent e when OnClick != null:
                    OnClick(e);
                    return true;

                case DoubleClickEvent e when OnDoubleClick != null:
                    OnDoubleClick(e);
                    return true;

                case DragStartEvent e when OnDragStart != null:
                    OnDragStart(e);
                    return true;

                case DragEvent e when OnDrag != null:
                    OnDrag(e);
                    return true;

                case DragEndEvent e when OnDragEnd != null:
                    OnDragEnd(e);
                    return true;

                case ScrollEvent e when OnScroll != null:
                    OnScroll(e);
                    return true;

                case FocusEvent e when OnFocus != null:
                    OnFocus(e);
                    return true;

                case FocusLostEvent e when OnFocusLost != null:
                    OnFocusLost(e);
                    return true;

                case KeyDownEvent e when OnKeyDown != null:
                    OnKeyDown(e);
                    return true;

                case KeyUpEvent e when OnKeyUp != null:
                    OnKeyUp(e);
                    return true;

                case JoystickPressEvent e when OnJoystickPress != null:
                    OnJoystickPress(e);
                    return true;

                case JoystickReleaseEvent e when OnJoystickRelease != null:
                    OnJoystickRelease(e);
                    return true;
            }

            return false;
        }

        public static implicit operator DrawableEventDelegate(DrawableEvent<T> @event) => @event.Handle;
    }

    /// <summary>
    /// Renders a <see cref="Drawable"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Drawable"/> to render.</typeparam>
    public sealed class ofDrawable<T> : ofDrawableBase<T> where T : Drawable, new()
    {
        public ofDrawable(ElementKey key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default, DrawableEventDelegate @event = default) : base(key, @ref, style, @event) { }

        protected override T CreateDrawable() => new T();
    }

    /// <summary>
    /// Defines the base class for rendering a <see cref="Drawable"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Drawable"/> to render.</typeparam>
    public abstract class ofDrawableBase<T> : ofElement where T : Drawable
    {
        [Prop] public readonly RefDelegate<T> Ref;
        [Prop] public readonly DrawableStyleDelegate<T> Style;
        [Prop] public readonly DrawableEventDelegate Event;

        /// <summary>
        /// Creates a new <see cref="ofDrawableBase{T}"/>.
        /// </summary>
        protected ofDrawableBase(ElementKey key = default, RefDelegate<T> @ref = default, DrawableStyleDelegate<T> style = default, DrawableEventDelegate @event = default) : base(key)
        {
            Ref   = @ref;
            Style = style;
            Event = @event;
        }

        /// <summary>
        /// Creates a new drawable of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Newly created drawable.</returns>
        protected abstract T CreateDrawable();

        /// <summary>
        /// Applies styles on the given drawable.
        /// </summary>
        protected virtual void ApplyStyles(T drawable) => Style?.Invoke(drawable);

        /// <summary>
        /// Applies event handlers of the given drawable, if it implements <see cref="ISupportEventDelegation"/>.
        /// </summary>
        protected virtual void ApplyEvents(T drawable)
        {
            if (drawable is ISupportEventDelegation d)
                d.EventDelegate = Event ?? DrawableEvent.EmptyDelegate; // always not null
        }

        /// <summary>
        /// Reference of the drawable that was rendered.
        /// </summary>
        protected RefObject<T> Drawable { get; private set; }

        protected override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            var context = Node.FindNearestContext<IDrawableRenderContext>().Value;

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

            drawable.Name = Key.ToString();

            // add styling and events
            ApplyStyles(drawable);
            ApplyEvents(drawable);

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
}