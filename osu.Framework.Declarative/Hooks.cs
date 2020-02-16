using osu.Framework.Allocation;
using static ofreact.Hooks;

namespace osu.Framework.Declarative
{
    public static class Hooks
    {
        public static T UseDependency<T>() where T : class => UseContext<IDrawableRenderContext>()?.DependencyContainer.Get<T>();
    }
}