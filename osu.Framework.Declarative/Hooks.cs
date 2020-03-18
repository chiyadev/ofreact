using osu.Framework.Allocation;
using osu.Framework.Platform;
using osu.Framework.Threading;
using static ofreact.Hooks;

namespace osu.Framework.Declarative
{
    public static class Hooks
    {
        public static T UseDependency<T>() where T : class => UseContext<IDrawableRenderContext>()?.DependencyContainer.Get<T>();
        public static Scheduler UseScheduler() => UseDependency<GameHost>().UpdateThread.Scheduler;
    }
}