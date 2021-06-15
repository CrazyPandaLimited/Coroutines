using System;


namespace CrazyPanda.UnityCore.CoroutineSystem
{
    class WaitForSecondsCoroutineProcessor : ICoroutineProcessor
    {
	    public Exception Exception { get; set; }
		private double _currentTimer;
        private ITimeProvider _timeProvider;

        public bool IsCompleted
        {
	        get
	        {
		        bool isComplete = _currentTimer <= 0;
		        if( isComplete && OnComplete != null )
		        {
			        OnComplete( this );
		        }
		        return _currentTimer <= 0;
	        }
        }

	    public event Action< ICoroutineProcessor > OnComplete;

        public WaitForSecondsCoroutineProcessor( ITimeProvider timeProvider, double timer )
        {
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            _currentTimer = timer;
        }

        public void Update()
        {
            _currentTimer -= _timeProvider.deltaTime;
        }

        public void Stop()
        {
        }
    }
}