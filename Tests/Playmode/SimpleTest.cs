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

}