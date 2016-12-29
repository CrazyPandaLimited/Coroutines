using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem.Tests
{
	class NestedTest
	{
		#region Private Fields
		private bool[ ] _flags = new bool[ 6 ];
		#endregion

		#region Public Members
		[ Test ]
		public void Test()
		{
			var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager( timeProvider );
			var coroutine = coroutineMgr.StartCoroutine( this, ParentCoroutine() );

			Assert.AreEqual( _flags, new[ ]
			{
				false,
				false,
				false,
				false,
				false,
				false
			} );

			timeProvider.OnUpdate += Raise.Event< Action >();
			Assert.AreEqual( _flags, new[ ]
			{
				true,
				false,
				false,
				false,
				false,
				false
			} );
			Assert.IsFalse( coroutine.IsCompleted );

			timeProvider.OnUpdate += Raise.Event< Action >();
			Assert.AreEqual( _flags, new[ ]
			{
				true,
				true,
				false,
				false,
				false,
				false
			} );
			Assert.IsFalse( coroutine.IsCompleted );

			timeProvider.OnUpdate += Raise.Event< Action >();
			Assert.AreEqual( _flags, new[ ]
			{
				true,
				true,
				true,
				false,
				false,
				false
			} );
			Assert.IsFalse( coroutine.IsCompleted );

			timeProvider.OnUpdate += Raise.Event< Action >();
			Assert.AreEqual( _flags, new[ ]
			{
				true,
				true,
				true,
				true,
				true,
				true
			} );
			Assert.IsTrue( coroutine.IsCompleted );
		}
		#endregion

		#region Private Members
		private IEnumerator ParentCoroutine()
		{
			_flags[ 0 ] = true;
			yield return NestedCoroutine();
			_flags[ 5 ] = true;
		}

		private IEnumerator NestedCoroutine()
		{
			_flags[ 1 ] = true;
			yield return Nested2Coroutine();
			_flags[ 4 ] = true;
		}

		private IEnumerator Nested2Coroutine()
		{
			_flags[ 2 ] = true;
			yield return 0;
			_flags[ 3 ] = true;
		}
		#endregion
	}
}
