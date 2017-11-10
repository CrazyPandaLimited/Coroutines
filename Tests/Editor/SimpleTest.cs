#if CRAZYPANDA_UNITYCORE_TESTS && CRAZYPANDA_UNITYCORE_COROUTINE
using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class SimpleTest
	{
		#region Private Fields
		private bool _flag;
		#endregion

		#region Public Members
		[ Test ]
		public void Test()
		{
			var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager();
			coroutineMgr.TimeProvider = timeProvider;
			coroutineMgr.StartCoroutine( this, SimpleCoroutine() );
			timeProvider.OnUpdate += Raise.Event< Action >();
			Assert.IsTrue( _flag );
		}
		#endregion

		#region Private Members
		private IEnumerator SimpleCoroutine()
		{
			_flag = true;
			yield break;
		}
		#endregion
	}

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

#endif
