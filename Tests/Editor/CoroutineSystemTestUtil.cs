using CrazyPanda.UnityCore.CoroutineSystem.Interfaces;
using NSubstitute;

namespace CrazyPanda.UnityCore.CoroutineSystem.Tests
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
