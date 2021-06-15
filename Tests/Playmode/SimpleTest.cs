using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    [NUnit.Framework.Category("IntegrationTests")]
    [NUnit.Framework.Category("LocalTests")]
	public class SimpleTest
	{
		private bool _flag;

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

		private IEnumerator SimpleCoroutine()
		{
			_flag = true;
			yield break;
		}
	}

}