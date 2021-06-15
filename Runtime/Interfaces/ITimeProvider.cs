using System;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public interface ITimeProvider
    {
        /// <summary>
        /// Returns current time
        /// </summary>
        float deltaTime { get; }

        /// <summary>
        /// Event, which invokes on every tick updated
        /// </summary>
        event Action OnUpdate;
    }
}
