#if CRAZYPANDA_UNITYCORE_TESTS && CRAZYPANDA_UNITYCORE_COROUTINE
using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class ExceptionTests
    {
        #region Public Members
        [ Test ]
        public void UnityYieldInstuctionNotSupported()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
            
            Assert.Throws< UnityYieldInstructionNotSupportedException >( () => coroutineMgr.StartCoroutine( this, UnityWaitForSeconds() ) );
            Assert.Throws< UnityYieldInstructionNotSupportedException >( () => timeProvider.OnUpdate += Raise.Event< Action >() );
            Assert.Throws< UnityYieldInstructionNotSupportedException >( () => timeProvider.OnUpdate += Raise.Event< Action >() );
            Assert.DoesNotThrow( () => timeProvider.OnUpdate += Raise.Event< Action >() );
        }

	    [ Test ]
	    public void ProcessorInYielding()
	    {
		    var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager();
		    coroutineMgr.TimeProvider = timeProvider;
		    Assert.Throws< UsingCoroutineProcessorInYieldingException >( () => coroutineMgr.StartCoroutine( this, ProcessorCoroutine() ) );
	    }
        #endregion

        #region Private Members
        private IEnumerator UnityWaitForSeconds()
        {
            yield return new WaitForSeconds( 1 );
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
        }

	    private IEnumerator ProcessorCoroutine()
	    {
		    yield return Substitute.For<ICoroutineProcessor>(  );
	    }
        #endregion
    }
}

#endif
