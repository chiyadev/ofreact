using System.Numerics;
using BenchmarkDotNet.Attributes;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ofreact.Benchmarks
{
    public class DrawableCreationBenchmarks
    {
        /*
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
AMD Ryzen 5 3600X, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT
  DefaultJob : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT


|                                  Method |     Mean |    Error |   StdDev |
|---------------------------------------- |---------:|---------:|---------:|
|                     CreateDrawableByVal | 41.58 ns | 0.180 ns | 0.160 ns |
|        CreateDrawableByRefConservative1 | 39.24 ns | 0.253 ns | 0.236 ns |
|        CreateDrawableByRefConservative2 | 40.23 ns | 0.183 ns | 0.171 ns |
|                     CreateDrawableByRef | 50.20 ns | 0.134 ns | 0.119 ns |
| CreateDrawableByRefConservativeDerived1 | 50.38 ns | 0.224 ns | 0.198 ns |
| CreateDrawableByRefConservativeDerived2 | 49.64 ns | 0.209 ns | 0.195 ns |

// * Hints *
Outliers
  DrawableCreationBenchmarks.CreateDrawableByVal: Default                     -> 1 outlier  was  removed (45.29 ns)
  DrawableCreationBenchmarks.CreateDrawableByRef: Default                     -> 1 outlier  was  removed (67.81 ns)
  DrawableCreationBenchmarks.CreateDrawableByRefConservativeDerived1: Default -> 1 outlier  was  removed (54.37 ns)
         */

        [Benchmark]
        public MockDrawableByVal CreateDrawableByVal() => new MockDrawableByVal();

        [Benchmark]
        public MockDrawableByRefConservative1 CreateDrawableByRefConservative1() => new MockDrawableByRefConservative1();

        [Benchmark]
        public MockDrawableByRefConservative2 CreateDrawableByRefConservative2() => new MockDrawableByRefConservative2();

        [Benchmark]
        public MockDrawableByRef CreateDrawableByRef() => new MockDrawableByRef();

        [Benchmark]
        public MockDrawableByRefConservativeDerived1 CreateDrawableByRefConservativeDerived1() => new MockDrawableByRefConservativeDerived1();

        [Benchmark]
        public MockDrawableByRefConservativeDerived2 CreateDrawableByRefConservativeDerived2() => new MockDrawableByRefConservativeDerived2();

        public struct MarginPadding
        {
            public float Top;
            public float Left;
            public float Bottom;
            public float Right;
        }

        public struct ColourInfo
        {
            public Vector4 TopLeft;
            public Vector4 BottomLeft;
            public Vector4 TopRight;
            public Vector4 BottomRight;
            public bool HasSingleColour;
        }

        public struct BlendingParameters
        {
            public int Source;
            public int Destination;
            public int SourceAlpha;
            public int DestinationAlpha;
            public int RGBEquation;
            public int AlphaEquation;
        }

        // pass all props by val
        public class MockDrawableByVal
        {
            public string Key { get; }
            public float Depth { get; }
            public Vector2 Position { get; }
            public int RelativePositionAxes { get; }
            public Vector2 Size { get; }
            public int RelativeSizeAxes { get; }
            public MarginPadding Margin { get; }
            public int BypassAutoSizeAxes { get; }
            public Vector2 Scale { get; }
            public float FillAspectRatio { get; }
            public int FillMode { get; }
            public Vector2 Shear { get; }
            public float Rotation { get; }
            public int Origin { get; }
            public Vector2 OriginPosition { get; }
            public int Anchor { get; }
            public Vector2 RelativeAnchorPosition { get; }
            public ColourInfo Colour { get; }
            public float Alpha { get; }
            public bool AlwaysPresent { get; }
            public BlendingParameters Blending { get; }
            public object Clock { get; }
            public double LifetimeStart { get; }
            public double LifetimeEnd { get; }

            public MockDrawableByVal(string key = default,
                                     float depth = default,
                                     Vector2 position = default,
                                     int relativePositionAxes = default,
                                     Vector2 size = default,
                                     int relativeSizeAxes = default,
                                     MarginPadding margin = default,
                                     int bypassAutoSizeAxes = default,
                                     Vector2 scale = default,
                                     float fillAspectRatio = default,
                                     int fillMode = default,
                                     Vector2 shear = default,
                                     float rotation = default,
                                     int origin = default,
                                     Vector2 originPosition = default,
                                     int anchor = default,
                                     Vector2 relativeAnchorPosition = default,
                                     ColourInfo colour = default,
                                     float alpha = default,
                                     bool alwaysPresent = default,
                                     BlendingParameters blending = default,
                                     object clock = default,
                                     double lifetimeStart = default,
                                     double lifetimeEnd = default)
            {
                Key                    = key;
                Depth                  = depth;
                Position               = position;
                RelativePositionAxes   = relativePositionAxes;
                Size                   = size;
                RelativeSizeAxes       = relativeSizeAxes;
                Margin                 = margin;
                BypassAutoSizeAxes     = bypassAutoSizeAxes;
                Scale                  = scale;
                FillAspectRatio        = fillAspectRatio;
                FillMode               = fillMode;
                Shear                  = shear;
                Rotation               = rotation;
                Origin                 = origin;
                OriginPosition         = originPosition;
                Anchor                 = anchor;
                RelativeAnchorPosition = relativeAnchorPosition;
                Colour                 = colour;
                Alpha                  = alpha;
                AlwaysPresent          = alwaysPresent;
                Blending               = blending;
                Clock                  = clock;
                LifetimeStart          = lifetimeStart;
                LifetimeEnd            = lifetimeEnd;
            }
        }

        // pass props larger than 8 bytes by ref
        public class MockDrawableByRefConservative1
        {
            public string Key { get; }
            public float Depth { get; }
            public Vector2 Position { get; }
            public int RelativePositionAxes { get; }
            public Vector2 Size { get; }
            public int RelativeSizeAxes { get; }
            public MarginPadding Margin { get; }
            public int BypassAutoSizeAxes { get; }
            public Vector2 Scale { get; }
            public float FillAspectRatio { get; }
            public int FillMode { get; }
            public Vector2 Shear { get; }
            public float Rotation { get; }
            public int Origin { get; }
            public Vector2 OriginPosition { get; }
            public int Anchor { get; }
            public Vector2 RelativeAnchorPosition { get; }
            public ColourInfo Colour { get; }
            public float Alpha { get; }
            public bool AlwaysPresent { get; }
            public BlendingParameters Blending { get; }
            public object Clock { get; }
            public double LifetimeStart { get; }
            public double LifetimeEnd { get; }

            public MockDrawableByRefConservative1(string key = default,
                                                  float depth = default,
                                                  Vector2 position = default,
                                                  int relativePositionAxes = default,
                                                  Vector2 size = default,
                                                  int relativeSizeAxes = default,
                                                  in MarginPadding margin = default,
                                                  int bypassAutoSizeAxes = default,
                                                  Vector2 scale = default,
                                                  float fillAspectRatio = default,
                                                  int fillMode = default,
                                                  Vector2 shear = default,
                                                  float rotation = default,
                                                  int origin = default,
                                                  Vector2 originPosition = default,
                                                  int anchor = default,
                                                  Vector2 relativeAnchorPosition = default,
                                                  in ColourInfo colour = default,
                                                  float alpha = default,
                                                  bool alwaysPresent = default,
                                                  in BlendingParameters blending = default,
                                                  object clock = default,
                                                  double lifetimeStart = default,
                                                  double lifetimeEnd = default)
            {
                Key                    = key;
                Depth                  = depth;
                Position               = position;
                RelativePositionAxes   = relativePositionAxes;
                Size                   = size;
                RelativeSizeAxes       = relativeSizeAxes;
                Margin                 = margin;
                BypassAutoSizeAxes     = bypassAutoSizeAxes;
                Scale                  = scale;
                FillAspectRatio        = fillAspectRatio;
                FillMode               = fillMode;
                Shear                  = shear;
                Rotation               = rotation;
                Origin                 = origin;
                OriginPosition         = originPosition;
                Anchor                 = anchor;
                RelativeAnchorPosition = relativeAnchorPosition;
                Colour                 = colour;
                Alpha                  = alpha;
                AlwaysPresent          = alwaysPresent;
                Blending               = blending;
                Clock                  = clock;
                LifetimeStart          = lifetimeStart;
                LifetimeEnd            = lifetimeEnd;
            }
        }

        // pass props larger than 4 bytes by ref
        public class MockDrawableByRefConservative2
        {
            public string Key { get; }
            public float Depth { get; }
            public Vector2 Position { get; }
            public int RelativePositionAxes { get; }
            public Vector2 Size { get; }
            public int RelativeSizeAxes { get; }
            public MarginPadding Margin { get; }
            public int BypassAutoSizeAxes { get; }
            public Vector2 Scale { get; }
            public float FillAspectRatio { get; }
            public int FillMode { get; }
            public Vector2 Shear { get; }
            public float Rotation { get; }
            public int Origin { get; }
            public Vector2 OriginPosition { get; }
            public int Anchor { get; }
            public Vector2 RelativeAnchorPosition { get; }
            public ColourInfo Colour { get; }
            public float Alpha { get; }
            public bool AlwaysPresent { get; }
            public BlendingParameters Blending { get; }
            public object Clock { get; }
            public double LifetimeStart { get; }
            public double LifetimeEnd { get; }

            public MockDrawableByRefConservative2(string key = default,
                                                  float depth = default,
                                                  in Vector2 position = default,
                                                  int relativePositionAxes = default,
                                                  in Vector2 size = default,
                                                  int relativeSizeAxes = default,
                                                  in MarginPadding margin = default,
                                                  int bypassAutoSizeAxes = default,
                                                  in Vector2 scale = default,
                                                  float fillAspectRatio = default,
                                                  int fillMode = default,
                                                  in Vector2 shear = default,
                                                  float rotation = default,
                                                  int origin = default,
                                                  in Vector2 originPosition = default,
                                                  int anchor = default,
                                                  in Vector2 relativeAnchorPosition = default,
                                                  in ColourInfo colour = default,
                                                  float alpha = default,
                                                  bool alwaysPresent = default,
                                                  in BlendingParameters blending = default,
                                                  object clock = default,
                                                  in double lifetimeStart = default,
                                                  in double lifetimeEnd = default)
            {
                Key                    = key;
                Depth                  = depth;
                Position               = position;
                RelativePositionAxes   = relativePositionAxes;
                Size                   = size;
                RelativeSizeAxes       = relativeSizeAxes;
                Margin                 = margin;
                BypassAutoSizeAxes     = bypassAutoSizeAxes;
                Scale                  = scale;
                FillAspectRatio        = fillAspectRatio;
                FillMode               = fillMode;
                Shear                  = shear;
                Rotation               = rotation;
                Origin                 = origin;
                OriginPosition         = originPosition;
                Anchor                 = anchor;
                RelativeAnchorPosition = relativeAnchorPosition;
                Colour                 = colour;
                Alpha                  = alpha;
                AlwaysPresent          = alwaysPresent;
                Blending               = blending;
                Clock                  = clock;
                LifetimeStart          = lifetimeStart;
                LifetimeEnd            = lifetimeEnd;
            }
        }

        public class MockDrawableByRefConservativeDerived1 : MockDrawableByRefConservative1
        {
            public int DerivedInt { get; }
            public double DerivedDouble { get; }

            public MockDrawableByRefConservativeDerived1(int derivedInt = default, in double derivedDouble = default, string key = default, float depth = default, Vector2 position = default, int relativePositionAxes = default,
                                                         Vector2 size = default, int relativeSizeAxes = default, in MarginPadding margin = default, int bypassAutoSizeAxes = default, Vector2 scale = default, float fillAspectRatio = default,
                                                         int fillMode = default, Vector2 shear = default, float rotation = default, int origin = default, Vector2 originPosition = default, int anchor = default,
                                                         Vector2 relativeAnchorPosition = default, in ColourInfo colour = default, float alpha = default, bool alwaysPresent = default, in BlendingParameters blending = default, object clock = default,
                                                         double lifetimeStart = default, double lifetimeEnd = default)
                : base(key, depth, position, relativePositionAxes, size, relativeSizeAxes, in margin, bypassAutoSizeAxes, scale, fillAspectRatio, fillMode, shear, rotation, origin, originPosition, anchor, relativeAnchorPosition, in colour, alpha, alwaysPresent, in blending, clock, lifetimeStart, lifetimeEnd)
            {
                DerivedInt    = derivedInt;
                DerivedDouble = derivedDouble;
            }
        }

        public class MockDrawableByRefConservativeDerived2 : MockDrawableByRefConservative2
        {
            public int DerivedInt { get; }
            public double DerivedDouble { get; }

            public MockDrawableByRefConservativeDerived2(int derivedInt = default, in double derivedDouble = default, string key = default, float depth = default, in Vector2 position = default, int relativePositionAxes = default,
                                                         in Vector2 size = default, int relativeSizeAxes = default, in MarginPadding margin = default, int bypassAutoSizeAxes = default, in Vector2 scale = default, float fillAspectRatio = default,
                                                         int fillMode = default, in Vector2 shear = default, float rotation = default, int origin = default, in Vector2 originPosition = default, int anchor = default,
                                                         in Vector2 relativeAnchorPosition = default, in ColourInfo colour = default, float alpha = default, bool alwaysPresent = default, in BlendingParameters blending = default, object clock = default,
                                                         in double lifetimeStart = default, in double lifetimeEnd = default)
                : base(key, depth, in position, relativePositionAxes, in size, relativeSizeAxes, in margin, bypassAutoSizeAxes, in scale, fillAspectRatio, fillMode, in shear, rotation, origin, in originPosition, anchor, in relativeAnchorPosition, in colour, alpha, alwaysPresent, in blending, clock, in lifetimeStart, in lifetimeEnd)
            {
                DerivedInt    = derivedInt;
                DerivedDouble = derivedDouble;
            }
        }

        // pass all props by ref
        public class MockDrawableByRef
        {
            public string Key { get; }
            public float Depth { get; }
            public Vector2 Position { get; }
            public int RelativePositionAxes { get; }
            public Vector2 Size { get; }
            public int RelativeSizeAxes { get; }
            public MarginPadding Margin { get; }
            public int BypassAutoSizeAxes { get; }
            public Vector2 Scale { get; }
            public float FillAspectRatio { get; }
            public int FillMode { get; }
            public Vector2 Shear { get; }
            public float Rotation { get; }
            public int Origin { get; }
            public Vector2 OriginPosition { get; }
            public int Anchor { get; }
            public Vector2 RelativeAnchorPosition { get; }
            public ColourInfo Colour { get; }
            public float Alpha { get; }
            public bool AlwaysPresent { get; }
            public BlendingParameters Blending { get; }
            public object Clock { get; }
            public double LifetimeStart { get; }
            public double LifetimeEnd { get; }

            public MockDrawableByRef(string key = default,
                                     in float depth = default,
                                     in Vector2 position = default,
                                     in int relativePositionAxes = default,
                                     in Vector2 size = default,
                                     in int relativeSizeAxes = default,
                                     in MarginPadding margin = default,
                                     in int bypassAutoSizeAxes = default,
                                     in Vector2 scale = default,
                                     in float fillAspectRatio = default,
                                     in int fillMode = default,
                                     in Vector2 shear = default,
                                     in float rotation = default,
                                     in int origin = default,
                                     in Vector2 originPosition = default,
                                     in int anchor = default,
                                     in Vector2 relativeAnchorPosition = default,
                                     in ColourInfo colour = default,
                                     in float alpha = default,
                                     in bool alwaysPresent = default,
                                     in BlendingParameters blending = default,
                                     object clock = default,
                                     in double lifetimeStart = default,
                                     in double lifetimeEnd = default)
            {
                Key                    = key;
                Depth                  = depth;
                Position               = position;
                RelativePositionAxes   = relativePositionAxes;
                Size                   = size;
                RelativeSizeAxes       = relativeSizeAxes;
                Margin                 = margin;
                BypassAutoSizeAxes     = bypassAutoSizeAxes;
                Scale                  = scale;
                FillAspectRatio        = fillAspectRatio;
                FillMode               = fillMode;
                Shear                  = shear;
                Rotation               = rotation;
                Origin                 = origin;
                OriginPosition         = originPosition;
                Anchor                 = anchor;
                RelativeAnchorPosition = relativeAnchorPosition;
                Colour                 = colour;
                Alpha                  = alpha;
                AlwaysPresent          = alwaysPresent;
                Blending               = blending;
                Clock                  = clock;
                LifetimeStart          = lifetimeStart;
                LifetimeEnd            = lifetimeEnd;
            }
        }
    }
}