using System;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public interface ITimeProvider
    {
        #region Properties
        /// <summary>
        /// Returns current time
        /// </summary>
        float deltaTime { get; }
        #endregion

        #region Public Members
        /// <summary>
        /// Event, which invokes on every tick updated
        /// </summary>
        event Action OnUpdate;
        #endregion
    }
}
