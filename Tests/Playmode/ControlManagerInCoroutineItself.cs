using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    [NUnit.Framework.Category("IntegrationTests")]
    [NUnit.Framework.Category("LocalTests")]
    public class ControlCoroutineManagerInCoroutineItself
    {
        private bool _firstFlag;
        private int _secondFlag;
        private int _thirdFlag;

        private CoroutineManager _coroutineManager;

        private object hook = new object();

        [ Test ]
        public void Test()
        {
            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            _coroutineManager = new CoroutineManager();
            _coroutineManager.TimeProvider = timeProvider;

            _coroutineManager.StartCoroutine( this, AddEnumerator() );

            timeProvider.OnUpdate += Raise.Event< Action >();
            timeProvider.OnUpdate += Raise.Event< Action >();
            timeProvider.OnUpdate += Raise.Event< Action >();
            timeProvider.OnUpdate += Raise.Event< Action >();
            timeProvider.OnUpdate += Raise.Event< Action >();

            Assert.IsTrue( _firstFlag );
            Assert.AreEqual( _secondFlag, 1 );
            Assert.AreEqual( _thirdFlag, 1 );
        }

        private IEnumerator AddEnumerator()
        {
            _coroutineManager.StartCoroutine( hook, SecondEnumerator() );
            _coroutineManager.StartCoroutine( this, ThirdEnumerator() );
            _firstFlag = true;
            yield break;
        }

        private IEnumerator SecondEnumerator()
        {
            yield return null;
            _coroutineManager.StopAllCoroutinesForTarget( hook );
            _secondFlag = 1;
            yield return null;
            _secondFlag = 10;
        }

        private IEnumerator ThirdEnumerator()
        {
            yield return null;
            _coroutineManager.StopAllCoroutines();
            _thirdFlag = 1;
            yield return null;
            _thirdFlag = 10;
        }
    }
}