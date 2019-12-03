using System;
using System.Collections;
using JetBrains.Annotations;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public interface ICoroutineManager : IDisposable
	{
		#region Properties
		/// <summary>
		/// Sets or returns custom time provider
		/// </summary>		
		ITimeProvider TimeProvider { get; set; }
		#endregion

		#region Events
		/// <summary>
		/// Invokes event on any errors in coroutines execution process
		/// </summary>
		event Action< object, Exception > OnError;
		#endregion

		#region Public Members
		/// <summary>
		///  starts coroutine in near future, by adding it to execution queue
		/// </summary>
		/// <param name="target">object, where coroutine was initiated</param>
		/// <param name="enumerator">coroutine to start</param>
		/// <param name="handlerError">event, which is gonna call on any exception in coroutine execution process</param>
		/// <param name="forcePutFirst">sets coroutine immediately as first priority for execution</param>
		[CanBeNull]
		ICoroutineProcessorPausable StartCoroutine( object target, IEnumerator enumerator, Action< object, Exception > handlerError = null, bool forcePutFirst = false );
		/// <summary>
		///  Starts coroutine before any another coroutine
		/// </summary>
		/// <param name="target">object, for tracking coroutine</param>
		/// <param name="enumerator">coroutine to start</param>
		/// <param name="before">starts enumerator before it</param>
		/// <param name="handlerError">event, which is gonna call on any exception in coroutine execution process</param>
		/// <returns>returns created CoroutineProcessor</returns>
		[CanBeNull]
		ICoroutineProcessorPausable StartCoroutineBefore( object target, IEnumerator enumerator, ICoroutineProcessor before, Action< object, Exception > handlerError = null );
		/// <summary>
		/// Creates CoroutineProcessor from coroutine
		/// </summary>
		/// <param name="enumerator"></param>
		/// <returns>returns created coroutine</returns>
		[CanBeNull]
		ICoroutineProcessorPausable CreateProcessor( IEnumerator enumerator );
		/// <summary>
		/// Executes CoroutineProcessor  immediately
		/// </summary>
		/// <param name="target">object, for tracking coroutine</param>
		/// <param name="processor">CoroutineProcessor to execute</param>
		/// <param name="handlerError">>event, which is gonna call on any exception in coroutine execution process</param>
		void StartProcessorImmediate( object target, ICoroutineProcessor processor, Action< object, Exception > handlerError = null );

		/// <summary>
		/// Stops all coroutines, tracks by this object, immediately
		/// </summary>
		/// <param name="target">object, which tracks coroutines</param>
		void StopAllCoroutinesForTarget( object target );
		/// <summary>
		/// Stops all coroutines
		/// </summary>
		void StopAllCoroutines();
		#endregion
	}
}
