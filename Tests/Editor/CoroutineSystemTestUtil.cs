#if CRAZYPANDA_UNITYCORE_TESTS && CRAZYPANDA_UNITYCORE_COROUTINE
using NSubstitute;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class CoroutineSystemTestUtil
    {
        #region Public Members
        public static ITimeProvider TestTimeProvider()
        {
            var timeProvider = Substitute.For< ITimeProvider >();
            timeProvider.deltaTime.Returns( 1f / 60f );
            return timeProvider;
        }
        #endregion
    }
}
#endif