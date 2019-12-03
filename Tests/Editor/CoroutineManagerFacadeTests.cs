using System;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public sealed class CoroutineManagerFacadeTests
    {
        private ICoroutineManager _coroutineManager;

        [ SetUp ]
        public void Initialize() => _coroutineManager = new CoroutineManagerFacade(CoroutineSystemTestUtil.TestTimeProvider());

        [ Test ]
        public void TestEnumeratorCreatedSafetyOnNullValues() => Assert.DoesNotThrow( ()=>_coroutineManager.CreateProcessor( null ) );
    }
}
