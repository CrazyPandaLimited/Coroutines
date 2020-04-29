using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    [NUnit.Framework.Category("IntegrationTests")]
    [NUnit.Framework.Category("LocalTests")]
    public class ExceptionTests
    {
        #region Public Members
        [ Test ]
        public void UnityYieldInstuctionNotSupportedWaitForSeconds()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
            coroutineMgr.StartCoroutine( this, UnityWaitForSeconds() );
            Assert.Throws< UnityYieldInstructionNotSupportedException >( () => timeProvider.OnUpdate += Raise.Event< Action >() );
            Assert.DoesNotThrow( () => timeProvider.OnUpdate += Raise.Event< Action >() );
        }
        [ Test ]
        public void UnityYieldInstuctionNotSupportedWaitForEndFrame()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
            coroutineMgr.StartCoroutine( this, UnityWaitEndEndFrame() );
            Assert.Throws< UnityYieldInstructionNotSupportedException >( () => timeProvider.OnUpdate += Raise.Event< Action >() );
            Assert.DoesNotThrow( () => timeProvider.OnUpdate += Raise.Event< Action >() );
        }
        [ Test ]
        public void UnityYieldInstuctionNotSupportedWaitForFixedUpdate()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;
            coroutineMgr.StartCoroutine( this, UnityWaitForFixedUpdate() );
            Assert.Throws< UnityYieldInstructionNotSupportedException >( () => timeProvider.OnUpdate += Raise.Event< Action >() );
            Assert.DoesNotThrow( () => timeProvider.OnUpdate += Raise.Event< Action >() );
        }

	    [ Test ]
	    public void ProcessorInYielding()
	    {
		    var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager();
		    coroutineMgr.TimeProvider = timeProvider;
		    coroutineMgr.StartCoroutine( this, ProcessorCoroutine() );
		    Assert.Throws< UsingCoroutineProcessorInYieldingException >( () => timeProvider.OnUpdate += Raise.Event< Action >() );
	    }

	    [ Test ]
	    public void CheckCorrectProcessWhenError()
	    {
		    var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
			var coroutineMgr = new CoroutineManager();
		    coroutineMgr.TimeProvider = timeProvider;
			Assert.Throws< Exception >( () => coroutineMgr.StartCoroutine( this, CorrectProcessWhenError() ) );
	    }
        #endregion

        #region Private Members
        private IEnumerator CorrectProcessWhenError()
        {
			throw new Exception( "Test exeption" );
        }
        private IEnumerator UnityWaitForSeconds()
        {
            yield return new WaitForSeconds( 1 );
        }
        private IEnumerator UnityWaitEndEndFrame()
        {
            yield return new WaitForEndOfFrame();
        }
        private IEnumerator UnityWaitForFixedUpdate()
        {
            yield return new WaitForFixedUpdate();
        }

	    private IEnumerator ProcessorCoroutine()
	    {
		    yield return Substitute.For<ICoroutineProcessor>(  );
	    }
        #endregion
    }
}