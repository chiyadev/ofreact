using ofreact;

namespace osu.Framework.Declarative.VisualTests
{
    static class Program
    {
        public static void Main() => new ofRootNode().RenderElement(new TestGame());
    }
}