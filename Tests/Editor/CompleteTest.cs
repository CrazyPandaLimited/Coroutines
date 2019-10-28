using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;


namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class CompleteTest
	{
		#region Constants
		private const uint STEPS_COUNT = 10;
		#endregion

		#region Public Members
		[ Test ]
		public void Test()
		{
			var isCompleted = false;
			var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager();
			coroutineMgr.TimeProvider = timeProvider;
			var coroutineProcessor = coroutineMgr.StartCoroutine( this, CompletableCoroutine() );
			coroutineProcessor.OnComplete += processor => isCompleted = true;

			for( var i = 0; i <= STEPS_COUNT; i++ )
			{
				timeProvider.OnUpdate += Raise.Event< Action >();
			}

			Assert.IsTrue( isCompleted );
		}
		#endregion

		#region Private Members
		private IEnumerator CompletableCoroutine()
		{
			for( var i = 0; i < STEPS_COUNT; i++ )
			{
				yield return null;
			}
		}
		#endregion
	}
}