using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class EnumeratorCoroutineProcessor : ICoroutineProcessorPausable
	{
		public Exception Exception { get; set; }

		private ITimeProvider _timeProvider;
		private ICoroutineProcessor _innerCoroutineProcessor;
		private CoroutineState _state;
		private IEnumerator _enumerator;

		/// <summary>
		/// Returns current coroutine execution process state
		/// </summary>
		public CoroutineState State
		{
			get { return _state; }
			private set
			{
				if( _state == value )
				{
					return;
				}

				_state = value;
				if( OnStateChange != null )
				{
					OnStateChange( _state );
				}

				if( IsCompleted && OnComplete != null )
				{
					OnComplete( this );
				}
			}
		}

		public bool IsCompleted { get { return _state == CoroutineState.Completed || _state == CoroutineState.Stopped; } }

		public event Action< ICoroutineProcessor > OnComplete;

		/// <summary>
		/// Event, which is gonna invoke after every coroutine execution process state changes
		/// </summary>
		public event Action< CoroutineState > OnStateChange;

		public EnumeratorCoroutineProcessor( ITimeProvider timeProvider, IEnumerator enumerator)
		{
			_timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
			_enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            _state = CoroutineState.NotStarted;
		}

		/// <summary>
		/// Suspends coroutine execution process
		/// </summary>
		/// <exception cref="CoroutineWrongStateException"></exception>
		public void Pause()
		{
			if( IsCompleted )
			{
				throw new CoroutineWrongStateException( State, CoroutineState.Paused );
			}

			State = CoroutineState.Paused;
		}

		/// <summary>
		/// Continues coroutine execution process
		/// </summary>
		/// <exception cref="CoroutineWrongStateException"></exception>
		public void Resume()
		{
			if( IsCompleted )
			{
				throw new CoroutineWrongStateException( State, CoroutineState.InProgress );
			}

			State = CoroutineState.InProgress;
		}

		public void Stop()
		{
			State = CoroutineState.Stopped;
		}

		/// <summary>
		/// Updates coroutine execution process
		/// </summary>
		/// <exception cref="UnityYieldInstructionNotSupportedException"></exception>
		/// <exception cref="UsingCoroutineProcessorInYieldingException"></exception>
		public virtual void Update()
		{
			int loopCount = 0;
			while( true )
			{
				if( IsCompleted || _state == CoroutineState.Paused )
				{
					break;
				}

				if( _state == CoroutineState.NotStarted )
				{
					_state = CoroutineState.InProgress;
				}

				if( _innerCoroutineProcessor != null )
				{
					_innerCoroutineProcessor.Update();

					if( _innerCoroutineProcessor.IsCompleted )
					{
						_innerCoroutineProcessor = null;
						loopCount++;
						continue;
					}
					else
					{
						break;
					}
				}

				if( _enumerator.MoveNext() )
				{					
                    _innerCoroutineProcessor = CoroutineProcessorFactory( _enumerator.Current, loopCount);                    

					if( _innerCoroutineProcessor != null )
					{
						_innerCoroutineProcessor.Update();

						if( _innerCoroutineProcessor.IsCompleted )
						{
							_innerCoroutineProcessor = null;
							loopCount++;
							continue;
						}
					}
				}
				else
				{
					State = CoroutineState.Completed;
				}

				break;
			}
		}

        protected virtual ICoroutineProcessor CoroutineProcessorFactory( object current, int loopCount )
        {
            ICoroutineProcessor res = null;
            if( current is CoroutineSystemWaitForSeconds )
            {
                float a = loopCount > 0 ? _timeProvider.deltaTime : 0f;
                res = new WaitForSecondsCoroutineProcessor( _timeProvider, (( CoroutineSystemWaitForSeconds )current).Seconds + a );
            }
            else if( current is CustomYieldInstruction )
            {
                res = new UnityCustomYieldCoroutineProcessor( ( CustomYieldInstruction )current );
            }
            else if( current is IEnumerator )
            {
                res = new EnumeratorCoroutineProcessor( _timeProvider, ( IEnumerator )current );
            }
            else if( current is AsyncOperation )
            {
                res = new AsyncOperationProcessor( ( AsyncOperation )current );
            }
            else if( current is WaitForSeconds )
            {
                throw new UnityYieldInstructionNotSupportedException( ( YieldInstruction )current, typeof( CoroutineSystemWaitForSeconds ) );
            }
            else if( current is YieldInstruction )
            {
                throw new UnityYieldInstructionNotSupportedException( ( YieldInstruction )current );
            }
            else if( current is ICoroutineProcessor )
            {
                throw new UsingCoroutineProcessorInYieldingException();
            }

            return res;
        }
    }
}