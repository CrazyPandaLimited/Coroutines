using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	class NestedTest
	{
		#region Private Fields
		private int[ ] _flags = new int[ 4 ];
		#endregion

		#region Public Members
		[ Test ]
		public void Test()
		{
			var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager();
			coroutineMgr.TimeProvider = timeProvider;
			var coroutine = coroutineMgr.StartCoroutine( this, ParentCoroutine() );

			Assert.AreEqual( new[ ]
			{
				0,
				0,
				0,
				0
			}, _flags );

			timeProvider.OnUpdate += Raise.Event< Action >();
			Assert.AreEqual( new[ ]
			{
				1,
				1,
				1,
				0
			}, _flags );
			Assert.IsFalse( coroutine.IsCompleted );

			timeProvider.OnUpdate += Raise.Event< Action >();
			Assert.AreEqual( new[ ]
			{
				1,
				3,
				3,
				1
			}, _flags );
			Assert.IsFalse( coroutine.IsCompleted );
   
			 timeProvider.OnUpdate += Raise.Event< Action >();
			 Assert.AreEqual( new[ ]
			 {
			 	2,
			 	4,
			 	4,
			 	1
			 }, _flags );
			Assert.IsTrue( coroutine.IsCompleted );
		}
		#endregion

		#region Private Members
		private IEnumerator ParentCoroutine()
		{
			_flags[ 0 ] = 1;
			yield return NestedCoroutine();
			_flags[ 0 ] = 2;
		}

		private IEnumerator NestedCoroutine()
		{
			_flags[ 1 ] = 1;
			yield return Nested2Coroutine();
			_flags[ 1 ] = 2;
			yield return NestedBreak();
			_flags[ 1 ] = 3;
			yield return Nested2Coroutine();
			_flags[ 1 ] = 4;
		}

		private IEnumerator Nested2Coroutine()
		{
			_flags[ 2 ]++;
			yield return null;
			_flags[ 2 ]++;
		}

		private IEnumerator NestedBreak()
		{
			_flags[ 3 ]++;
			yield break;
		}
		#endregion
	}
}