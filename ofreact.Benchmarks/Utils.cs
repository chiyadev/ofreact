using System.Numerics;

namespace ofreact.Benchmarks
{
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

    public enum Anchor { }

    public enum Axes { }

    public enum FillMode { }

    public class Drawable
    {
        public float Depth { get; set; }
        public Vector2 Position { get; set; }
        public Axes RelativePositionAxes { get; set; }
        public Vector2 Size { get; set; }
        public Axes RelativeSizeAxes { get; set; }
        public MarginPadding Margin { get; set; }
        public Axes BypassAutoSizeAxes { get; set; }
        public Vector2 Scale { get; set; }
        public float FillAspectRatio { get; set; }
        public FillMode FillMode { get; set; }
        public Vector2 Shear { get; set; }
        public float Rotation { get; set; }
        public Anchor Origin { get; set; }
        public Vector2 OriginPosition { get; set; }
        public Anchor Anchor { get; set; }
        public Vector2 RelativeAnchorPosition { get; set; }
        public ColourInfo Colour { get; set; }
        public float Alpha { get; set; }
        public bool AlwaysPresent { get; set; }
        public BlendingParameters Blending { get; set; }
        public object Clock { get; set; }
        public double LifetimeStart { get; set; }
        public double LifetimeEnd { get; set; }
    }
}