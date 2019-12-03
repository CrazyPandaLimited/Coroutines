using System.Collections;
using CrazyPanda.UnityCore.Flogs;
using CrazyPanda.UnityCore.Network.HttpSystem;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public sealed class CoroutineManagerFacadeSilentFailedTests
    {
        private ICoroutineManager _coroutineManager;

        [ SetUp ]
        public void Initialize() => _coroutineManager = new CoroutineManagerFacade( new FlogsManagerV2( new FlogsConfig( "no", "" ), new UnityHttpConnection( new HttpSettings() ) ) );

        [ Test ]
        public void CreateProcessorFailedSilentOnNullValuesTest() => Assert.DoesNotThrow( () => _coroutineManager.CreateProcessor( null ) );

        [ Test ]
        public void StartCoroutineFailedSilentOnNullValuesTest() => Assert.DoesNotThrow( () => _coroutineManager.StartCoroutine( null, null ) );

        [ Test ]
        public void StartCoroutineBeforeFailedSilentOnNullValuesTest() => Assert.DoesNotThrow( () => _coroutineManager.StartCoroutineBefore( null, null, null ) );

        [ Test ]
        public void StartProcessorImmediateFailedSilentOnNullValuesTest() => Assert.DoesNotThrow( () => _coroutineManager.StartProcessorImmediate( null, null ) );

        [ Test ]
        public void StopAllCoroutinesForTargetFailedSilentOnNullValuesTest() => Assert.DoesNotThrow( () => _coroutineManager.StopAllCoroutinesForTarget( null ) );

        [ Test ]
        public void StopAllCoroutinesFailedSilentTest() => Assert.DoesNotThrow( () => _coroutineManager.StopAllCoroutines() );

        [ Test ]
        public void TestEnumeratorDisposeFailedSilentTest() => Assert.DoesNotThrow( () => _coroutineManager.Dispose() );

        [ Test ]
        public void StopCoroutinesForTargetFailedSilentAfterStartingTest()
        {
            _coroutineManager.StartCoroutine( this, SimpleEnumerator() );
            Assert.DoesNotThrow( () => _coroutineManager.StopAllCoroutinesForTarget( null ) );
        }

        private IEnumerator SimpleEnumerator()
        {
            while( true )
            {
                yield return null;
            }
        }
    }
}
