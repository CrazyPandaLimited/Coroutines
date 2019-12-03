using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public partial class CoroutineManager : ICoroutineManager
	{
		#region Private Fields
		private LinkedList< Entry > _coroutines;
		private ITimeProvider _timeProvider;
		#endregion

		#region Properties
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
		#endregion

		#region Events
		/// <summary>
		/// Invokes event on any errors in coroutines execution process
		/// </summary>
		public event Action< object, Exception > OnError;
		#endregion

		#region Constructors
		public CoroutineManager()
		{
#if UNITY_EDITOR
			_instance = this;
#endif
			_coroutines = new LinkedList< Entry >();
		}
		#endregion

		#region Public Members
		/// <summary>
		///  starts coroutine in near future, by adding it to execution queue
		/// </summary>
		/// <param name="target">object, where coroutine was initiated</param>
		/// <param name="enumerator">coroutine to start</param>
		/// <param name="handlerError">event, which is gonna call on any exception in coroutine execution process</param>
		/// <param name="forcePutFirst">sets coroutine immediately as first priority for execution</param>
		/// <returns>returns created CoroutineProcessor</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public virtual ICoroutineProcessorPausable StartCoroutine( object target, IEnumerator enumerator, Action< object, Exception > handlerError = null, bool forcePutFirst = false )
		{
			CheckCommonValuesForNullState(target);
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
		/// <exception cref="ArgumentException"></exception>
		public virtual ICoroutineProcessorPausable StartCoroutineBefore( object target, IEnumerator enumerator, ICoroutineProcessor before, Action< object, Exception > handlerError = null )
		{
			CheckCommonValuesForNullState(target);

			var extendedCoroutine = CreateEnumeratorCoroutine( enumerator );

			LinkedListNode< Entry > beforeNode = null;
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
				throw new ArgumentNullException(nameof(beforeNode));
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
		public virtual void StartProcessorImmediate( object target, ICoroutineProcessor processor, Action< object, Exception > handlerError = null )
		{
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
				throw new ArgumentNullException(nameof(target));
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
		#endregion

		#region Protected Members
		/// <summary>
		/// Stops all coroutines and clears all data
		/// </summary>
		public void Dispose()
		{
			TimeProvider = null;
			OnError = null;
			StopAllCoroutines();
		}
		#endregion

		#region Private Members
		private EnumeratorCoroutineProcessor CreateEnumeratorCoroutine( IEnumerator enumerator )
		{
			if( enumerator == null )
			{
				throw new ArgumentNullException(nameof(enumerator));
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

		private LinkedListNode< Entry > RemoveEntry( LinkedListNode< Entry > currentNode )
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

		private void CheckCommonValuesForNullState(object target)
		{
			if( _timeProvider == null )
			{
				throw new ArgumentNullException(nameof(_timeProvider));
			}

			if( target == null )
			{
				throw new ArgumentNullException(nameof(target));
			}
		}
		
		#endregion

#if UNITY_EDITOR

		// нужен инстанс, чтобы отобразить редактор
		public static CoroutineManager Instance { get { return _instance; } }

		private static CoroutineManager _instance;
		/// <summary>
		/// Returns all coroutines, stores at this manager
		/// </summary>
		public LinkedList< Entry > Coroutines { get { return _coroutines; } }
#endif
	}
}
