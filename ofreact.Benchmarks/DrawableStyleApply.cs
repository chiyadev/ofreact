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


|     Method |     Mean |    Error |   StdDev |
|----------- |---------:|---------:|---------:|
| Coalescing | 69.07 ns | 0.439 ns | 0.410 ns |
|   Checking | 47.38 ns | 0.330 ns | 0.293 ns |

// * Hints *
Outliers
  DrawableStyleApply.Checking: Default -> 1 outlier  was  removed (59.72 ns)
         */

        [Benchmark]
        public void Coalescing() => new NullCoalescing().Apply(new Drawable());

        [Benchmark]
        public void Checking() => new NullChecking().Apply(new Drawable());

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
    }
}