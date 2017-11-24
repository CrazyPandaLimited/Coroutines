#if CRAZYPANDA_UNITYCORE_TESTS && CRAZYPANDA_UNITYCORE_COROUTINE
using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class WaitTest
    {
        #region Private Fields
        private bool _flag1;
        private bool _flag2;
        #endregion

        #region Public Members
        [ Test ]
        public void Test()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            timeProvider.deltaTime.Returns( 1.0f );
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
            coroutineMgr.StartCoroutine( this, WaitCoroutine() );
            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.IsTrue( _flag1 );
            Assert.IsFalse( _flag2 );
        }
        #endregion

        #region Private Members
        private IEnumerator WaitCoroutine()
        {
            yield return new CoroutineSystemWaitForSeconds( 1.0f );
            _flag1 = true;
            yield return new CoroutineSystemWaitForSeconds( 1.0f );
            _flag2 = true;
        }
        #endregion
    }
}

#endif
