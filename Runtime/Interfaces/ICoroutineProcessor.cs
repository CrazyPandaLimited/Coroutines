using System;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public interface ICoroutineProcessor
	{
		#region Properties
		/// <summary>
		/// Returns completed coroutine execution process, or not
		/// </summary>
		bool IsCompleted { get; }
		/// <summary>
		/// Provides exception, happened during coroutine execution process
		/// </summary>
		Exception Exception { get; }
		#endregion

		#region Public Members
		/// <summary>
		/// Event, which is gonna invoke on coroutine execution process ending
		/// </summary>
		event Action< ICoroutineProcessor > OnComplete;
		/// <summary>
		/// Updates coroutine execution process
		/// </summary>
		void Update();
		/// <summary>
		/// Stops coroutine execution process immediately
		/// </summary>
		void Stop();
		#endregion
	}

	public interface ICoroutineProcessorPausable : ICoroutineProcessor
	{
		#region Public Members
		/// <summary>
		/// Suspends coroutine execution process
		/// </summary>
		void Pause();
		/// <summary>
		/// Continues coroutine execution process
		/// </summary>
		void Resume();
		#endregion
	}
}
