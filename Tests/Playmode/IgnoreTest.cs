using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class IgnoreTest
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

            coroutineMgr.StartCoroutine( this, IgnoreCoroutine() );
            for( var i = 0; i < 16; i++ )
            {
                timeProvider.OnUpdate += Raise.Event< Action >();
            }
            Assert.IsTrue( _flag );
        }
        #endregion

        #region Private Members
        private IEnumerator IgnoreCoroutine()
        {
            yield return null;
            yield return true;
            yield return false;
            yield return ( sbyte ) 10;
            yield return ( byte ) 10; // 5
            yield return 'a';
            yield return ( short ) 10;
            yield return ( ushort ) 10;
            yield return 10;
            yield return 10u; // 10
            yield return 10L;
            yield return 10LU;
            yield return 10.5f;
            yield return 10.5;
            yield return new object(); // 15
            _flag = true;
        }
        #endregion
    }
}