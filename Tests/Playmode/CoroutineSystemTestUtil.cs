using NSubstitute;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public static class CoroutineSystemTestUtil
    {
        public static ITimeProvider TestTimeProvider()
        {
            var timeProvider = Substitute.For< ITimeProvider >();
            timeProvider.deltaTime.Returns( 1f / 60f );
            return timeProvider;
        }
    }
}
