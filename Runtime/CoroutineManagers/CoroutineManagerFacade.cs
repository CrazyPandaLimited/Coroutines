using System;
using System.Collections;
using CrazyPanda.UnityCore.Flogs;
using CrazyPanda.UnityCore.Network.HttpSystem;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    /// <summary>
    /// Facade for coroutineManager
    /// Provides coroutineManager methods calling without exceptions
    /// Only constructors can throw exceptions
    /// </summary>
    public sealed class CoroutineManagerFacade : ICoroutineManager
    {
        #region Consts
        private const string ProjectName = "coroutines";
        #endregion

        #region Private Fields
        private readonly ICoroutineManager _coroutineManager;
        #endregion

        #region Constructors
        public CoroutineManagerFacade( ITimeProvider timeProvider = null ) : this( new FlogsManagerV2( new FlogsConfig( FlogsConsts.FLOGS_PRODUCTION_ENDPOINT, ProjectName ), new UnityHttpConnection( new HttpSettings() ) ), timeProvider )
        {
        }

        public CoroutineManagerFacade( ICoroutineManager coroutineManager ) => _coroutineManager = coroutineManager ?? throw new ArgumentNullException( nameof(coroutineManager) );

        public CoroutineManagerFacade( IFlogsManager flogsManager, ITimeProvider timeProvider = null ) =>
            _coroutineManager = new CoroutineManagerExceptionsLogProxy( flogsManager ) { TimeProvider = timeProvider };
        #endregion

        #region Public Properties
        public ITimeProvider TimeProvider 
        { 
            get => _coroutineManager.TimeProvider;
            set => _coroutineManager.TimeProvider = value;
        }
        public event Action< object, Exception > OnError 
        { 
            add => _coroutineManager.OnError += value;
            remove => _coroutineManager.OnError -= value;
        }
        #endregion

        #region Public Members
        public ICoroutineProcessorPausable StartCoroutine( object target, IEnumerator enumerator, Action< object, Exception > handlerError = null, bool forcePutFirst = false ) => 
            _coroutineManager.StartCoroutine( target, enumerator, handlerError, forcePutFirst );

        public ICoroutineProcessorPausable StartCoroutineBefore( object target, IEnumerator enumerator, ICoroutineProcessor before, Action< object, Exception > handlerError = null ) =>
            _coroutineManager.StartCoroutineBefore( target, enumerator, before, handlerError );

        public ICoroutineProcessorPausable CreateProcessor( IEnumerator enumerator ) => _coroutineManager.CreateProcessor( enumerator );

        public void StartProcessorImmediate( object target, ICoroutineProcessor processor, Action< object, Exception > handlerError = null ) => _coroutineManager.StartProcessorImmediate( target, processor, handlerError );

        public void StopAllCoroutinesForTarget( object target ) => _coroutineManager.StopAllCoroutinesForTarget( target );

        public void StopAllCoroutines() => _coroutineManager.StopAllCoroutines();
        #endregion
    }
}
