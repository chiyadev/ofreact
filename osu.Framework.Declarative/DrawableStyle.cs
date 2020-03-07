using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ofreact;
using osu.Framework.Declarative.Yaml;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osuTK;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Encapsulates a method that applies styling to a <see cref="Drawable"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="drawable">Drawable to apply styling to.</param>
    /// <typeparam name="T">Type of the drawable.</typeparam>
    [DrawableStyleDelegatePropResolver]
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

    public class DrawableStyleDelegatePropResolverAttribute : Attribute, IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, PropTypeInfo prop, ElementRenderInfo element, YamlNode node)
        {
            if (prop.Type == null)
                return null;

            if (!(node is YamlMappingNode mapping))
                throw new YamlComponentException("Must be a mapping.", node);

            var drawableType = prop.Type.GetGenericArguments()[0];
            var drawable     = Expression.Parameter(drawableType, "drawable");

            var body = new List<Expression>();

            foreach (var (key, value) in mapping)
            {
                try
                {
                    if (!(key is YamlScalarNode keyScalar))
                        throw new YamlComponentException("Must be a scalar.", key);

                    var member = drawableType.GetProperty(keyScalar.Value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase) as MemberInfo
                              ?? drawableType.GetField(keyScalar.Value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                    if (member == null || member is PropertyInfo p && !p.CanWrite) // ensure writable if property
                        throw new YamlComponentException($"Cannot resolve property or field '{keyScalar.Value}' in element {drawableType}.", keyScalar);

                    var provider = ((IYamlComponentBuilder) context.Builder).PropResolver.Resolve(context, member, element, value);

                    if (provider == null)
                        throw new YamlComponentException($"Cannot resolve property or field '{keyScalar.Value}' in element {drawableType}.", keyScalar);

                    body.Add(Expression.Assign(Expression.MakeMemberAccess(drawable, member), provider.GetValue(context)));
                }
                catch (Exception e)
                {
                    context.OnException(e);
                }
            }

            return new Provider(Expression.Lambda(prop.Type, Expression.Block(body), drawable));
        }

        sealed class Provider : IPropProvider
        {
            readonly Expression _expr;

            public Provider(Expression expr)
            {
                _expr = expr;
            }

            public Expression GetValue(ComponentBuilderContext context) => _expr;
        }
    }
}