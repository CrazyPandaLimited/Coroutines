using System.Collections;
using CrazyPanda.UnityCore.Flogs;
using CrazyPanda.UnityCore.Network.HttpSystem;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public sealed class CoroutineManagerFacadeFlogsTests
    {
        private IFlogsManager _flogsManager;
        private ICoroutineManager _coroutineManager;

        [ SetUp ]
        public void Initialize()
        {
            _flogsManager = new FlogsManagerV2( new FlogsConfig( "no", "test" ), new UnityHttpConnection( new HttpSettings() ) );
            _coroutineManager = new CoroutineManagerFacade( _flogsManager );
        }

        [ UnityTest ]
        public IEnumerator LogSendedToFlogsOnExceptionTest()
        {
            var logSended = false;
            _flogsManager.OnSendException += ( sender, exception ) => logSended = true;

            Assert.DoesNotThrow( () => _coroutineManager.StartCoroutine( null, null ) );

            while( !logSended )
            {
                yield return null;
            }

            Assert.IsTrue( logSended );
        }
    }
}
