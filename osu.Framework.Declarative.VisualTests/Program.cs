namespace osu.Framework.Declarative.VisualTests
{
    static class Program
    {
        public static void Main()
        {
            using var game = new ofGame("ofreact");

            game.RenderElement(new TestGame());
        }
    }
}