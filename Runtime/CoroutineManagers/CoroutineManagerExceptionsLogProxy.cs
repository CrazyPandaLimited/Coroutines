using System;
using System.Collections;
using CrazyPanda.UnityCore.Flogs;
using CrazyPanda.UnityCore.Network.HttpSystem;
using JetBrains.Annotations;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public sealed class CoroutineManagerExceptionsLogProxy : CoroutineManager
    {
        #region Private Fields
        private readonly IFlogsManager _flogsManager;
        #endregion

        #region Constructors
        public CoroutineManagerExceptionsLogProxy( IFlogsManager flogsManager ) => _flogsManager = flogsManager ?? throw new ArgumentNullException( nameof(flogsManager) );
        #endregion

        #region Public Members
        public override ICoroutineProcessorPausable StartCoroutine( object target, IEnumerator enumerator, Action< object, Exception > handlerError = null, bool forcePutFirst = false ) =>
            ExecuteEventSafely( () => base.StartCoroutine( target, enumerator, handlerError, forcePutFirst ) );

        public override ICoroutineProcessorPausable StartCoroutineBefore( object target, IEnumerator enumerator, ICoroutineProcessor before, Action< object, Exception > handlerError = null ) =>
            ExecuteEventSafely( () => base.StartCoroutineBefore( target, enumerator, before, handlerError ) );

        public override ICoroutineProcessorPausable CreateProcessor( IEnumerator enumerator ) =>
            ExecuteEventSafely( () => base.CreateProcessor( enumerator ) );

        public override void StartProcessorImmediate( object target, ICoroutineProcessor processor, Action< object, Exception > handlerError = null ) => 
            ExecuteEventSafely( () => base.StartProcessorImmediate( target, processor, handlerError ) );

        public override void StopAllCoroutinesForTarget( object target ) => ExecuteEventSafely( () => base.StopAllCoroutinesForTarget( target ) );

        public override void StopAllCoroutines() => ExecuteEventSafely( () => base.StopAllCoroutines() );
        #endregion

        #region Protected Members
        protected override void UpdateEntry( Entry entry ) => ExecuteEventSafely( () => base.UpdateEntry( entry ) );
        #endregion

        #region Private Members
        private void ExecuteEventSafely( Action eventToExecute )
        {
            try
            {
                eventToExecute();
            }
            catch( Exception exception )
            {
                SendException( exception );
            }
        }

        [ CanBeNull ]
        private T ExecuteEventSafely< T >( Func< T > eventToExecute ) where T : class
        {
            try
            {
                return eventToExecute();
            }
            catch( Exception exception )
            {
                SendException( exception );
            }

            return null;
        }

        private void SendException( Exception exception )
        {
            Console.WriteLine( exception );
            _flogsManager.SendException( exception );
        }
        #endregion
    }
}
