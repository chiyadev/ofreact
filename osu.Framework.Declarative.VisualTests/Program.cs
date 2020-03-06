namespace osu.Framework.Declarative.VisualTests
{
    static class Program
    {
        public static void Main()
        {
            using var node = new ofGameHost("ofreact");

            node.RenderElement(new TestGame());
        }
    }
}