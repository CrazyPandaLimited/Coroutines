using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class CoroutineStateTests
    {
        private int _count;

        [ Test ]
        public void Test() => CoroutineRunTest( new CoroutineManager() );
        public void CoroutineRunTest(ICoroutineManager coroutineMgr)
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
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
            Assert.AreEqual( CoroutineState.Stopped, state );
            Assert.AreEqual( CoroutineState.Stopped, coroutine.State );

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.AreEqual( 11, _count );
            Assert.IsTrue( coroutine.IsCompleted );
        }


        private IEnumerator InterruptableCoroutine()
        {
            _count += 1;
            yield return null;
            _count += 10;
            yield return null;
            _count += 100000;
        }
    }
}