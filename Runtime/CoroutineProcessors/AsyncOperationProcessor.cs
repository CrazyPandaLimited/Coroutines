using System;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class AsyncOperationProcessor : ICoroutineProcessor
	{
		private readonly AsyncOperation _asyncOperation;
		private bool _isComplete;

		public bool IsCompleted
		{
			get
			{
				if( _isComplete != _asyncOperation.isDone )
				{
					_isComplete = _asyncOperation.isDone;
					if( _isComplete && OnComplete != null )
					{
						OnComplete( this );
					}
				}
				return _isComplete;
			}
		}
		public Exception Exception { get; set; }

		public event Action< ICoroutineProcessor > OnComplete;

		public AsyncOperationProcessor( AsyncOperation asyncOperation )
		{
			_asyncOperation = asyncOperation ?? throw new ArgumentNullException(nameof(asyncOperation));
			_isComplete = _asyncOperation.isDone;
			if( _isComplete && OnComplete != null )
			{
				OnComplete( this );
			}
		}

		public void Update()
		{
		}

		public void Stop()
		{
		}
	}
}