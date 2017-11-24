﻿#if CRAZYPANDA_UNITYCORE_TESTS && CRAZYPANDA_UNITYCORE_COROUTINE
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
            Assert.AreEqual( CoroutineState.NotStarted, state );
            coroutine.OnStateChange += newState => state = newState;

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.AreEqual( 1, _count );
            coroutine.Pause();
            Assert.AreEqual( CoroutineState.Paused, state );
            Assert.AreEqual( CoroutineState.Paused, coroutine.State );

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.AreEqual( 1, _count );
            coroutine.Resume();
            Assert.AreEqual( CoroutineState.InProgress, state );
            Assert.AreEqual( CoroutineState.InProgress, coroutine.State );

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.AreEqual( 11, _count );
            coroutine.Stop();
            Assert.AreEqual( CoroutineState.Completed, state );
            Assert.AreEqual( CoroutineState.Completed, coroutine.State );

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.AreEqual( 11, _count );
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
