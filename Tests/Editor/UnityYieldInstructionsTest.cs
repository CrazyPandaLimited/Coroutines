#if CRAZYPANDA_UNITYCORE_TESTS && CRAZYPANDA_UNITYCORE_COROUTINE
using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class UnityYieldInstructionsTest
    {
        #region Public Members
        [ Test ]
        public void Test()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager( timeProvider );
            coroutineMgr.StartCoroutine( this, UnityWaitForSeconds() );
            Assert.Throws< UnityYieldInstructionNotSupportedException >( () => timeProvider.OnUpdate += Raise.Event< Action >() );
            Assert.Throws< UnityYieldInstructionNotSupportedException >( () => timeProvider.OnUpdate += Raise.Event< Action >() );
            Assert.Throws< UnityYieldInstructionNotSupportedException >( () => timeProvider.OnUpdate += Raise.Event< Action >() );
            Assert.DoesNotThrow( () => timeProvider.OnUpdate += Raise.Event< Action >() );
        }
        #endregion

        #region Private Members
        private IEnumerator UnityWaitForSeconds()
        {
            yield return new WaitForSeconds( 1 );
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
        }
        #endregion
    }
}
#endif