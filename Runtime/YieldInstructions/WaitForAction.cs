using System;
using UnityEngine;

public class WaitForAction : CustomYieldInstruction
{
    private Action _eventInstance;

    /// <summary>
    /// Returns state, which provides info about invokes target event or not
    /// </summary>
    public bool Dispatched { get; private set; }

    public override bool keepWaiting { get { return !Dispatched; } }

	public Action GetHandler { get { return HandleInvokeEvent; } }

    public WaitForAction( Action action = null )
    {
        if( action != null )
        {
            _eventInstance = action;
            _eventInstance += HandleInvokeEvent;
        }
            Dispatched = false;
    }

    protected void ClearSubscription()
    {
        _eventInstance -= HandleInvokeEvent;
    }

    protected void HandleInvokeEvent( )
    {
        Dispatched = true;
        ClearSubscription();
    }
}

public class WaitForAction<T> : CustomYieldInstruction
{
    private Action<T> _eventInstance;

    public T Arg { get; private set; }

    public bool Dispatched { get; private set; }

    public override bool keepWaiting { get { return !Dispatched; } }

    public WaitForAction( Action<T> action )
    {
        _eventInstance = action ?? throw new ArgumentNullException(nameof(action));
        Dispatched = false;
        _eventInstance += HandleInvokeEvent;
    }

    protected void ClearSubscription()
    {
        _eventInstance -= HandleInvokeEvent;
    }

    private void HandleInvokeEvent(T arg )
    {
        Dispatched = true;
        ClearSubscription();
    }
}