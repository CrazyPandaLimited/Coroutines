using System;
using System.Collections;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class EnumeratorCoroutineProcessor : ICoroutineProcessorPausable
	{
		public Exception Exception { get; set; }

		#region Private Fields
		private ITimeProvider _timeProvider;
		private ICoroutineProcessor _innerCoroutineProcessor;
		private CoroutineState _state;
		private IEnumerator _enumerator;
		#endregion

		#region Properties
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
		#endregion

		#region Events
		/// <summary>
		/// Event, which is gonna invoke after every coroutine execution process state changes
		/// </summary>
		public event Action< CoroutineState > OnStateChange;
		#endregion

		#region Constructors
		public EnumeratorCoroutineProcessor( ITimeProvider timeProvider, IEnumerator enumerator )
		{
			_timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
			_enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
			_state = CoroutineState.NotStarted;
		}
		#endregion

		#region Public Members
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
		public void Update()
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
					var current = _enumerator.Current;
					if( current is CoroutineSystemWaitForSeconds )
					{
						float a = loopCount > 0 ? _timeProvider.deltaTime : 0f;
						_innerCoroutineProcessor = new WaitForSecondsCoroutineProcessor( _timeProvider, ( ( CoroutineSystemWaitForSeconds ) current ).Seconds + a );
					}
					else if( current is CustomYieldInstruction )
					{
						_innerCoroutineProcessor = new UnityCustomYieldCoroutineProcessor( ( CustomYieldInstruction ) current );
					}
					else if( current is IEnumerator )
					{
						_innerCoroutineProcessor = new EnumeratorCoroutineProcessor( _timeProvider, ( IEnumerator ) current );
					}
					else if( current is AsyncOperation )
					{
						_innerCoroutineProcessor = new AsyncOperationProcessor( ( AsyncOperation ) current );
					}
					else if( current is WaitForSeconds )
					{
						throw new UnityYieldInstructionNotSupportedException( ( YieldInstruction ) current, typeof( CoroutineSystemWaitForSeconds ) );
					}
					else if( current is YieldInstruction )
					{
						throw new UnityYieldInstructionNotSupportedException( ( YieldInstruction ) current );
					}
					else if( current is ICoroutineProcessor )
					{
						throw new UsingCoroutineProcessorInYieldingException();
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
					}
				}
				else
				{
					State = CoroutineState.Completed;
				}

				break;
			}
		}
		#endregion
	}
}