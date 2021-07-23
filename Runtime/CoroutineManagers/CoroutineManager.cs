using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class CoroutineManager : ICoroutineManager
    {
        protected LinkedList<Entry> _coroutines;
        protected ITimeProvider _timeProvider;

        /// <summary>
        /// Sets or returns custom time provider
        /// </summary>
        public ITimeProvider TimeProvider
        {
            get { return _timeProvider; }
            set
            {
                if( _timeProvider != null )
                {
                    _timeProvider.OnUpdate -= HandleFrameUpdate;
                }

                _timeProvider = value;

                if( _timeProvider != null )
                {
                    _timeProvider.OnUpdate += HandleFrameUpdate;
                }
            }
        }

        /// <summary>
        /// Invokes event on any errors in coroutines execution process
        /// </summary>
        public event Action<object, Exception> OnError;

        public CoroutineManager()
        {
#if UNITY_EDITOR
            _instance = this;
#endif
            _coroutines = new LinkedList<Entry>();
        }

        /// <summary>
        ///  starts coroutine in near future, by adding it to execution queue
        /// </summary>
        /// <param name="target">object, where coroutine was initiated</param>
        /// <param name="enumerator">coroutine to start</param>
        /// <param name="handlerError">event, which is gonna call on any exception in coroutine execution process</param>
        /// <param name="forcePutFirst">sets coroutine immediately as first priority for execution</param>
        /// <returns>returns created CoroutineProcessor</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual ICoroutineProcessorPausable StartCoroutine( object target,
            IEnumerator enumerator,
            Action<object, Exception> handlerError = null,
            bool forcePutFirst = false)
        {
            CheckCommonValuesForNullState( target );
            var extendedCoroutine = CreateEnumeratorCoroutine( enumerator );
            var entry = new Entry( target, enumerator, extendedCoroutine, handlerError );
            if( forcePutFirst )
            {
                _coroutines.AddFirst( entry );
            }
            else
            {
                _coroutines.AddLast( entry );
            }
            return extendedCoroutine;
        }

        /// <summary>
        ///  Starts coroutine before any another coroutine
        /// </summary>
        /// <param name="target">object, for tracking coroutine</param>
        /// <param name="enumerator">coroutine to start</param>
        /// <param name="before">starts enumerator before it</param>
        /// <param name="handlerError">event, which is gonna call on any exception in coroutine execution process</param>
        /// <returns>returns created CoroutineProcessor</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual ICoroutineProcessorPausable StartCoroutineBefore( object target, IEnumerator enumerator, ICoroutineProcessor before, Action<object, Exception> handlerError = null )
        {
            CheckCommonValuesForNullState( target );

            var extendedCoroutine = CreateEnumeratorCoroutine( enumerator );

            LinkedListNode<Entry> beforeNode = null;
            foreach( var entry in _coroutines )
            {
                if( entry.CoroutineProcessor == before )
                {
                    beforeNode = _coroutines.Find( entry );
                    break;
                }
            }

            if( beforeNode == null )
            {
                throw new ArgumentNullException( nameof( beforeNode ) );
            }

            _coroutines.AddBefore( beforeNode, new Entry( target, enumerator, extendedCoroutine, handlerError ) );
            return extendedCoroutine;
        }

        /// <summary>
        /// Creates CoroutineProcessor from coroutine
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns>returns created coroutine</returns>
        public virtual ICoroutineProcessorPausable CreateProcessor( IEnumerator enumerator )
        {
            return CreateEnumeratorCoroutine( enumerator );
        }

        /// <summary>
        /// Executes CoroutineProcessor  immediately
        /// </summary>
        /// <param name="target">object, for tracking coroutine</param>
        /// <param name="processor">CoroutineProcessor to execute</param>
        /// <param name="handlerError">>event, which is gonna call on any exception in coroutine execution process</param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void StartProcessorImmediate( object target, ICoroutineProcessor processor, Action<object, Exception> handlerError = null )
        {
            if( target == null )
            {
                throw new ArgumentNullException( nameof( target ) );
            }

            if( processor == null )
            {
                throw new ArgumentNullException( nameof( processor ) );
            }

            try
            {
                while( !processor.IsCompleted )
                {
                    processor.Update();
                }
            }
            catch( Exception exception )
            {
                if( handlerError != null )
                {
                    handlerError( target, exception );
                    return;
                }

                throw;
            }
        }

        /// <summary>
        /// Stops all coroutines, tracks by this object, immediately
        /// </summary>
        /// <param name="target">object, which tracks coroutines</param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void StopAllCoroutinesForTarget( object target )
        {
            if( target == null )
            {
                throw new ArgumentNullException( nameof( target ) );
            }

            foreach( var coroutine in _coroutines.Where( c => c.Target == target ) )
            {
                coroutine.CoroutineProcessor.Stop();
            }
        }

        /// <summary>
        /// Stops all coroutines
        /// </summary>
        public virtual void StopAllCoroutines()
        {
            foreach( var coroutine in _coroutines )
            {
                coroutine.CoroutineProcessor.Stop();
            }
        }

        /// <summary>
        /// Stops all coroutines and clears all data
        /// </summary>
        public virtual void Dispose()
        {
            TimeProvider = null;
            OnError = null;
            StopAllCoroutines();
        }

        protected virtual ICoroutineProcessorPausable CreateEnumeratorCoroutine( IEnumerator enumerator )
        {
            if( enumerator == null )
            {
                throw new ArgumentNullException( nameof( enumerator ) );
            }

            if( _coroutines.Any( v => v.Enumerator == enumerator ) )
            {
                throw new DuplicateEnumeratorException( enumerator );
            }

            return new EnumeratorCoroutineProcessor( _timeProvider, enumerator );
        }

        private void HandleFrameUpdate()
        {
            var currentNode = _coroutines.First;
            while( currentNode != null )
            {
                var entry = currentNode.Value;

                // Check if entry complete before update, cos it could be stopped manually from outer code
                if( entry.CoroutineProcessor.IsCompleted )
                {
                    currentNode = RemoveEntry( currentNode );
                    continue;
                }

                if( !entry.IsAlive ) // GameObject destroyed OR someone forgot to unsubscribe
                {
                    entry.CoroutineProcessor.Stop();
                }
                else
                {
                    UpdateEntry( entry );
                }

                // Check if entry complete after update, cos it could be the last one and we need to get finish event in this frame, not next
                currentNode = entry.CoroutineProcessor.IsCompleted ? RemoveEntry( currentNode ) : currentNode.Next;
            }
        }

        private LinkedListNode<Entry> RemoveEntry( LinkedListNode<Entry> currentNode )
        {
            var nodeToRemove = currentNode;
            currentNode = nodeToRemove.Next;
            _coroutines.Remove( nodeToRemove );
            return currentNode;
        }

        protected virtual void UpdateEntry( Entry entry )
        {
            try
            {
                entry.CoroutineProcessor.Update();
            }
            catch( Exception ex )
            {
                entry.CoroutineProcessor.Stop();
                entry.CoroutineProcessor.Exception = ex;

                if( entry.HandlerError != null )
                {
                    entry.HandlerError.Invoke( entry.Target, ex );
                }
                else if( OnError != null )
                {
                    OnError.Invoke( entry.Target, ex );
                }
                else
                {
                    throw;
                }
            }
        }

        private void CheckCommonValuesForNullState( object target )
        {
            if( _timeProvider == null )
            {
                throw new ArgumentNullException( nameof( _timeProvider ) );
            }

            if( target == null )
            {
                throw new ArgumentNullException( nameof( target ) );
            }
        }
        
#if UNITY_EDITOR

        // нужен инстанс, чтобы отобразить редактор
        public static CoroutineManager Instance { get { return _instance; } }

        private static CoroutineManager _instance;
        /// <summary>
        /// Returns all coroutines, stores at this manager
        /// </summary>
        public LinkedList<Entry> Coroutines { get { return _coroutines; } }
#endif

        public class Entry
        {
            private WeakReference _targetReference;
            private bool _isUnityObject;

            /// <summary>
            /// Returns target, which tracks coroutine  
            /// </summary>
            public object Target { get { return _targetReference.Target; } }

            /// <summary>
            /// Returns coroutine alive state
            /// </summary>
            public bool IsAlive
            {
                get
                {
                    var alive = _targetReference.IsAlive;
                    if( !alive || !_isUnityObject )
                    {
                        return alive;
                    }
                    var unityObj = _targetReference.Target as Object;
                    if( unityObj == null )
                    {
                        return false;
                    }
                    return true;
                }
            }

            /// <summary>
            /// Invokes on any errors, during coroutines execution process
            /// </summary>
            public Action<object, Exception> HandlerError { get; }
            /// <summary>
            /// Returns current coroutine
            /// </summary>
            public IEnumerator Enumerator { get; }
            /// <summary>
            /// Returns current CoroutineProcessor, which controls coroutine execution process
            /// </summary>
            public ICoroutineProcessorPausable CoroutineProcessor { get; private set; }

            public Entry( object target, IEnumerator enumerator, ICoroutineProcessorPausable coroutineProcessor, Action<object, Exception> handlerError )
            {
                _isUnityObject = target is Object;
                _targetReference = new WeakReference( target );
                Enumerator = enumerator ?? throw new ArgumentException( nameof( enumerator ) );
                CoroutineProcessor = coroutineProcessor ?? throw new ArgumentException( nameof( coroutineProcessor ) );
                HandlerError = handlerError;
            }
        }


    }
}
