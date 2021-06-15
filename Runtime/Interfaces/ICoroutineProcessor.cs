using System;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public interface ICoroutineProcessor
	{
		/// <summary>
		/// Returns completed coroutine execution process, or not
		/// </summary>
		bool IsCompleted { get; }
		/// <summary>
		/// Provides exception, happened during coroutine execution process
		/// </summary>
		Exception Exception { get; set; }

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
	}

	public interface ICoroutineProcessorPausable : ICoroutineProcessor
	{
		/// <summary>
		/// Suspends coroutine execution process
		/// </summary>
		void Pause();
		/// <summary>
		/// Continues coroutine execution process
		/// </summary>
		void Resume();

        CoroutineState State { get; }
    }
}
