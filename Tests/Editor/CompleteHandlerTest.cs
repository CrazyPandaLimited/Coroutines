using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class CompleteHandlerTest
	{
		#region Constants
		private const uint STEPS_COUNT = 10;
		private bool _isComplete = false;
		#endregion

		#region Public Members
		[ Test ]
		public void Test()
		{
			_isComplete = false;
			var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager();
			coroutineMgr.TimeProvider = timeProvider;
			coroutineMgr.StartCoroutine( this, CompletableCoroutine(), HandlerFinish, HandlerError );

			for( var i = 0; i <= STEPS_COUNT; i++ )
			{
				timeProvider.OnUpdate += Raise.Event< Action >();
			}

			Assert.IsTrue(_isComplete);
		}
		#endregion

		#region Private Members
		private void HandlerError( object obj, Exception exception )
		{
			throw new NotImplementedException();
		}

		private void HandlerFinish( object obj )
		{
			_isComplete = true;
		}

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