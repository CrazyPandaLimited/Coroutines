using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class StartCoroutineTests
    {
        #region Public Members
        [ Test ]
        public void NullParameters()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
            Assert.Throws< ArgumentNullException >( () => coroutineMgr.StartCoroutine( null, SimpleCoroutine() ) );
            Assert.Throws< ArgumentNullException >( () => coroutineMgr.StartCoroutine( this, null ) );
            Assert.Throws< ArgumentNullException >( () => coroutineMgr.StopAllCoroutinesForTarget( null ) );
        }

        [ Test ]
        public void SameCoroutinesRightWay()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
            coroutineMgr.StartCoroutine( this, SimpleCoroutine() );
            Assert.DoesNotThrow( () => coroutineMgr.StartCoroutine( this, SimpleCoroutine() ) );
        }

        [ Test ]
        public void SameCoroutinesWrongWay()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
            var enumerator = SimpleCoroutine();
            coroutineMgr.StartCoroutine( this, enumerator );
            Assert.Throws< DuplicateEnumeratorException >( () => coroutineMgr.StartCoroutine( this, enumerator ) );
        }

        [ Test ]
        public void SameCoroutinesReading()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
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