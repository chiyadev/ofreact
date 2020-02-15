using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace ofreact.Benchmarks
{
    public class DrawableStyleApply
    {
        /*
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
AMD Ryzen 5 3600X, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT
  DefaultJob : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT


|                    Method |     Mean |    Error |   StdDev |
|-------------------------- |---------:|---------:|---------:|
|                Coalescing | 69.54 ns | 0.505 ns | 0.472 ns |
|                  Checking | 47.27 ns | 0.593 ns | 0.463 ns |
|            CheckingFields | 27.66 ns | 0.122 ns | 0.114 ns |
| CheckingFieldsStackLoaded | 56.03 ns | 0.078 ns | 0.061 ns |

// * Hints *
Outliers
  DrawableStyleApply.Checking: Default                  -> 3 outliers were removed (51.79 ns..56.76 ns)
  DrawableStyleApply.CheckingFieldsStackLoaded: Default -> 3 outliers were removed (57.84 ns..58.06 ns)
         */

        [Benchmark]
        public void Coalescing() => new NullCoalescing().Apply(new Drawable());

        [Benchmark]
        public void Checking() => new NullChecking().Apply(new Drawable());

        [Benchmark]
        public void CheckingFields() => new NullCheckingFields().Apply(new Drawable());

        [Benchmark]
        public void CheckingFieldsStackLoaded() => new NullCheckingFieldsStackLoaded().Apply(new Drawable());

        public class NullCoalescing
        {
            public Vector2? Position { get; set; }
            public Axes? RelativePositionAxes { get; set; }
            public Vector2? Size { get; set; }
            public Axes? RelativeSizeAxes { get; set; }
            public MarginPadding? Margin { get; set; }
            public Axes? BypassAutoSizeAxes { get; set; }
            public Vector2? Scale { get; set; }
            public float? FillAspectRatio { get; set; }
            public FillMode? FillMode { get; set; }
            public Vector2? Shear { get; set; }
            public float? Rotation { get; set; }
            public Anchor? Origin { get; set; }
            public Vector2? OriginPosition { get; set; }
            public Anchor? Anchor { get; set; }
            public Vector2? RelativeAnchorPosition { get; set; }
            public ColourInfo? Colour { get; set; }
            public float? Alpha { get; set; }
            public bool? AlwaysPresent { get; set; }
            public BlendingParameters? Blending { get; set; }
            public double? LifetimeStart { get; set; }
            public double? LifetimeEnd { get; set; }

            public void Apply(Drawable drawable)
            {
                drawable.Position               = Position ?? drawable.Position;
                drawable.RelativePositionAxes   = RelativePositionAxes ?? drawable.RelativePositionAxes;
                drawable.Size                   = Size ?? drawable.Size;
                drawable.RelativeSizeAxes       = RelativeSizeAxes ?? drawable.RelativeSizeAxes;
                drawable.Margin                 = Margin ?? drawable.Margin;
                drawable.BypassAutoSizeAxes     = BypassAutoSizeAxes ?? drawable.BypassAutoSizeAxes;
                drawable.Scale                  = Scale ?? drawable.Scale;
                drawable.FillAspectRatio        = FillAspectRatio ?? drawable.FillAspectRatio;
                drawable.FillMode               = FillMode ?? drawable.FillMode;
                drawable.Shear                  = Shear ?? drawable.Shear;
                drawable.Rotation               = Rotation ?? drawable.Rotation;
                drawable.Origin                 = Origin ?? drawable.Origin;
                drawable.OriginPosition         = OriginPosition ?? drawable.OriginPosition;
                drawable.Anchor                 = Anchor ?? drawable.Anchor;
                drawable.RelativeAnchorPosition = RelativeAnchorPosition ?? drawable.RelativeAnchorPosition;
                drawable.Colour                 = Colour ?? drawable.Colour;
                drawable.Alpha                  = Alpha ?? drawable.Alpha;
                drawable.AlwaysPresent          = AlwaysPresent ?? drawable.AlwaysPresent;
                drawable.Blending               = Blending ?? drawable.Blending;
                drawable.LifetimeStart          = LifetimeStart ?? drawable.LifetimeStart;
                drawable.LifetimeEnd            = LifetimeEnd ?? drawable.LifetimeEnd;
            }
        }

        public class NullChecking
        {
            public Vector2? Position { get; set; }
            public Axes? RelativePositionAxes { get; set; }
            public Vector2? Size { get; set; }
            public Axes? RelativeSizeAxes { get; set; }
            public MarginPadding? Margin { get; set; }
            public Axes? BypassAutoSizeAxes { get; set; }
            public Vector2? Scale { get; set; }
            public float? FillAspectRatio { get; set; }
            public FillMode? FillMode { get; set; }
            public Vector2? Shear { get; set; }
            public float? Rotation { get; set; }
            public Anchor? Origin { get; set; }
            public Vector2? OriginPosition { get; set; }
            public Anchor? Anchor { get; set; }
            public Vector2? RelativeAnchorPosition { get; set; }
            public ColourInfo? Colour { get; set; }
            public float? Alpha { get; set; }
            public bool? AlwaysPresent { get; set; }
            public BlendingParameters? Blending { get; set; }
            public double? LifetimeStart { get; set; }
            public double? LifetimeEnd { get; set; }

            public void Apply(Drawable drawable)
            {
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
        }

        public class NullCheckingFields
        {
            public Vector2? Position;
            public Axes? RelativePositionAxes;
            public Vector2? Size;
            public Axes? RelativeSizeAxes;
            public MarginPadding? Margin;
            public Axes? BypassAutoSizeAxes;
            public Vector2? Scale;
            public float? FillAspectRatio;
            public FillMode? FillMode;
            public Vector2? Shear;
            public float? Rotation;
            public Anchor? Origin;
            public Vector2? OriginPosition;
            public Anchor? Anchor;
            public Vector2? RelativeAnchorPosition;
            public ColourInfo? Colour;
            public float? Alpha;
            public bool? AlwaysPresent;
            public BlendingParameters? Blending;
            public double? LifetimeStart;
            public double? LifetimeEnd;

            public void Apply(Drawable drawable)
            {
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
        }

        public class NullCheckingFieldsStackLoaded
        {
            public Vector2? Position;
            public Axes? RelativePositionAxes;
            public Vector2? Size;
            public Axes? RelativeSizeAxes;
            public MarginPadding? Margin;
            public Axes? BypassAutoSizeAxes;
            public Vector2? Scale;
            public float? FillAspectRatio;
            public FillMode? FillMode;
            public Vector2? Shear;
            public float? Rotation;
            public Anchor? Origin;
            public Vector2? OriginPosition;
            public Anchor? Anchor;
            public Vector2? RelativeAnchorPosition;
            public ColourInfo? Colour;
            public float? Alpha;
            public bool? AlwaysPresent;
            public BlendingParameters? Blending;
            public double? LifetimeStart;
            public double? LifetimeEnd;

            public void Apply(Drawable drawable)
            {
                var position               = Position;
                var relativePositionAxes   = RelativePositionAxes;
                var size                   = Size;
                var relativeSizeAxes       = RelativeSizeAxes;
                var margin                 = Margin;
                var bypassAutoSizeAxes     = BypassAutoSizeAxes;
                var scale                  = Scale;
                var fillAspectRatio        = FillAspectRatio;
                var fillMode               = FillMode;
                var shear                  = Shear;
                var rotation               = Rotation;
                var origin                 = Origin;
                var originPosition         = OriginPosition;
                var anchor                 = Anchor;
                var relativeAnchorPosition = RelativeAnchorPosition;
                var colour                 = Colour;
                var alpha                  = Alpha;
                var alwaysPresent          = AlwaysPresent;
                var blending               = Blending;
                var lifetimeStart          = LifetimeStart;
                var lifetimeEnd            = LifetimeEnd;

                if (position != null)
                    drawable.Position = position.Value;
                if (relativePositionAxes != null)
                    drawable.RelativePositionAxes = relativePositionAxes.Value;
                if (size != null)
                    drawable.Size = size.Value;
                if (relativeSizeAxes != null)
                    drawable.RelativeSizeAxes = relativeSizeAxes.Value;
                if (margin != null)
                    drawable.Margin = margin.Value;
                if (bypassAutoSizeAxes != null)
                    drawable.BypassAutoSizeAxes = bypassAutoSizeAxes.Value;
                if (scale != null)
                    drawable.Scale = scale.Value;
                if (fillAspectRatio != null)
                    drawable.FillAspectRatio = fillAspectRatio.Value;
                if (fillMode != null)
                    drawable.FillMode = fillMode.Value;
                if (shear != null)
                    drawable.Shear = shear.Value;
                if (rotation != null)
                    drawable.Rotation = rotation.Value;
                if (origin != null)
                    drawable.Origin = origin.Value;
                if (originPosition != null)
                    drawable.OriginPosition = originPosition.Value;
                if (anchor != null)
                    drawable.Anchor = anchor.Value;
                if (relativeAnchorPosition != null)
                    drawable.RelativeAnchorPosition = relativeAnchorPosition.Value;
                if (colour != null)
                    drawable.Colour = colour.Value;
                if (alpha != null)
                    drawable.Alpha = alpha.Value;
                if (alwaysPresent != null)
                    drawable.AlwaysPresent = alwaysPresent.Value;
                if (blending != null)
                    drawable.Blending = blending.Value;
                if (lifetimeStart != null)
                    drawable.LifetimeStart = lifetimeStart.Value;
                if (lifetimeEnd != null)
                    drawable.LifetimeEnd = lifetimeEnd.Value;
            }
        }
    }
}