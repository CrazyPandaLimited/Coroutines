#if CRAZYPANDA_UNITYCORE_TESTS && CRAZYPANDA_UNITYCORE_COROUTINE
using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class CoroutineStateTests
    {
        #region Private Fields
        private int _count;
        #endregion

        #region Public Members
        [ Test ]
        public void Test()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
            var coroutine = (EnumeratorCoroutineProcessor) coroutineMgr.StartCoroutine( this, InterruptableCoroutine() );

            var state = coroutine.State;
            Assert.AreEqual( state, CoroutineState.NotStarted );
            coroutine.OnStateChange += newState => state = newState;

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.AreEqual( _count, 1 );
            coroutine.Pause();
            Assert.AreEqual( state, CoroutineState.Paused );
            Assert.AreEqual( coroutine.State, CoroutineState.Paused );

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.AreEqual( _count, 1 );
            coroutine.Resume();
            Assert.AreEqual( state, CoroutineState.InProgress );
            Assert.AreEqual( coroutine.State, CoroutineState.InProgress );

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.AreEqual( _count, 11 );
            coroutine.Stop();
            Assert.AreEqual( state, CoroutineState.Completed );
            Assert.AreEqual( coroutine.State, CoroutineState.Completed );

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.AreEqual( _count, 11 );
            Assert.IsTrue( coroutine.IsCompleted );
        }
        #endregion

        #region Private Members
        private IEnumerator InterruptableCoroutine()
        {
            _count += 1;
            yield return null;
            _count += 10;
            yield return null;
            _count += 100000;
        }
        #endregion
    }
}

#endif
