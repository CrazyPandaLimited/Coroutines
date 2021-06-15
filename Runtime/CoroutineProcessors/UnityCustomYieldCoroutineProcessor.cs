using System;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class UnityCustomYieldCoroutineProcessor : ICoroutineProcessor
    {
	    public Exception Exception { get; set; }
		private CustomYieldInstruction _customInstruction;
	    private bool _isComplete;

        public bool IsCompleted
        {
	        get
	        {
		        if( _isComplete == _customInstruction.keepWaiting )
		        {
			        _isComplete = !_customInstruction.keepWaiting;
			        if( _isComplete && OnComplete != null )
			        {
				        OnComplete( this );
			        }
		        }
		        return !_customInstruction.keepWaiting;
	        }
        }

	    public event Action< ICoroutineProcessor > OnComplete;

        public UnityCustomYieldCoroutineProcessor( CustomYieldInstruction customInstruction )
        {
            _customInstruction = customInstruction ?? throw new ArgumentNullException(nameof(customInstruction));
	        _isComplete = !_customInstruction.keepWaiting;
			if (_isComplete && OnComplete != null)
			{
				OnComplete(this);
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
