using osu.Framework.Platform;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Encapsulates a method that is used to create a new <see cref="GameHost"/>.
    /// </summary>
    public delegate GameHost GameHostFactoryDelegate(string name);

    /// <summary>
    /// Contains variables used in <see cref="GameHost"/> initialization.
    /// </summary>
    public class GameHostOptions
    {
        /// <summary>
        /// True to enable IPC capabilities (inter-process communication).
        /// </summary>
        public bool BindIpc { get; set; }

        public bool PortableInstallation { get; set; }
        public bool UseSdl { get; set; }

        /// <summary>
        /// Creates a new <see cref="GameHost"/> suitable for the calling environment.
        /// </summary>
        public GameHost Create(string name) => Host.GetSuitableHost(name, BindIpc, PortableInstallation, UseSdl);

        public static implicit operator GameHostFactoryDelegate(GameHostOptions args) => args.Create;
    }
}