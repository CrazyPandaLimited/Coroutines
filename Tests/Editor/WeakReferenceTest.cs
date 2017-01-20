#if CRAZYPANDA_UNITYCORE_TESTS && CRAZYPANDA_UNITYCORE_COROUTINE
using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class WeakReferenceTest
    {
        #region Private Fields
        private bool _flag;
        #endregion

        #region Public Members
        [ Test ]
        public void WeakReferenceBaseTest()
        {
            WeakReference reference = null;
            new Action( () =>
            {
                var obj = new object();
                reference = new WeakReference( obj, true );
            } )();

            Assert.IsNotNull( reference.Target );

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsNull( reference.Target );
        }

        [ Test ]
        public void CoroutineWeakTest()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;

            new Action( () =>
            {
                var obj = new object();
                coroutineMgr.StartCoroutine( obj, SimpleCoroutine() );
            } )();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            timeProvider.OnUpdate += Raise.Event< Action >();
            Assert.IsFalse( _flag );
        }
        #endregion

        #region Private Members
        private IEnumerator SimpleCoroutine()
        {
            _flag = true;
            yield break;
        }
        #endregion
    }
}

#endif
