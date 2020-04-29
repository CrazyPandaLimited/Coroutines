using System;
using System.Collections;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    [NUnit.Framework.Category("IntegrationTests")]
    [NUnit.Framework.Category("LocalTests")]
    public sealed class CoroutineManagerFailedWithExceptionsTests
    {
        private ICoroutineManager _coroutineManager;

        [ SetUp ]
        public void Initialize() => _coroutineManager = new CoroutineManager { TimeProvider = CoroutineSystemTestUtil.TestTimeProvider()};

        [ Test ]
        public void CreateProcessorFailedTest() => Assert.Throws< ArgumentNullException >( () => _coroutineManager.CreateProcessor( null ) );

        [ Test ]
        public void StartCoroutineFailedTest() => Assert.Throws< ArgumentNullException >( () => _coroutineManager.StartCoroutine( null, null ) );

        [ Test ]
        public void StartCoroutineBeforeFailedTest() => Assert.Throws< ArgumentNullException >( () => _coroutineManager.StartCoroutineBefore( null, null, null ) );

        [ Test ]
        public void StartProcessorImmediateFailedTest() => Assert.Throws< ArgumentNullException >( () => _coroutineManager.StartProcessorImmediate( null, null ) );

        [ Test ]
        public void StopAllCoroutinesForTargetFailedTest() => Assert.Throws< ArgumentNullException >( () => _coroutineManager.StopAllCoroutinesForTarget( null ) );

        [ Test ]
        public void StopCoroutinesForTargetFailedAfterStartingTest()
        {
            _coroutineManager.StartCoroutine( this, SimpleEnumerator() );
            Assert.Throws< ArgumentNullException >( () => _coroutineManager.StopAllCoroutinesForTarget( null ) );
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
