using System;
using System.Collections;
using CrazyPanda.UnityCore.CoroutineSystem.Exceptions;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem.Tests
{
	public class StartCoroutineTests
	{
		#region Public Members
		[ Test ]
		public void NullParameters()
		{
			var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager( timeProvider );
			Assert.Throws< NullReferenceException >( () => coroutineMgr.StartCoroutine( null, SimpleCoroutine() ) );
			Assert.Throws< NullReferenceException >( () => coroutineMgr.StartCoroutine( this, null ) );
			Assert.Throws< NullReferenceException >( () => coroutineMgr.StopAllCoroutinesForTarget( null ) );
		}

		[ Test ]
		public void SameCoroutinesRightWay()
		{
			var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager( timeProvider );
			coroutineMgr.StartCoroutine( this, SimpleCoroutine() );
			Assert.DoesNotThrow( () => coroutineMgr.StartCoroutine( this, SimpleCoroutine() ) );
		}

		[ Test ]
		public void SameCoroutinesWrongWay()
		{
			var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager( timeProvider );
			var enumerator = SimpleCoroutine();
			coroutineMgr.StartCoroutine( this, enumerator );
			Assert.Throws< DuplicateEnumeratorException >( () => coroutineMgr.StartCoroutine( this, enumerator ) );
		}

		[ Test ]
		public void SameCoroutinesReading()
		{
			var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager( timeProvider );
			var enumerator = SimpleCoroutine();
			coroutineMgr.StartCoroutine( this, enumerator );
			timeProvider.OnUpdate += Raise.Event< Action >();
			Assert.DoesNotThrow( () => coroutineMgr.StartCoroutine( this, enumerator ) );
		}
		#endregion

		#region Private Members
		private IEnumerator SimpleCoroutine()
		{
			yield break;
		}
		#endregion
	}
}
