﻿using System;
using UnityEngine;

public class WaitForAction : CustomYieldInstruction
{
    #region Private Fields
    private Action _eventInstance;
    #endregion

    #region Properties
    /// <summary>
    /// Returns state, which provides info about invokes target event or not
    /// </summary>
    public bool Dispatched { get; private set; }

    public override bool keepWaiting { get { return !Dispatched; } }

	public Action GetHandler { get { return HandleInvokeEvent; } }
	#endregion

    #region Constructors
    public WaitForAction( Action action = null )
    {
        if( action != null )
        {
            _eventInstance = action;
            _eventInstance += HandleInvokeEvent;
        }
            Dispatched = false;
    }
    #endregion

    #region Protected Members
    protected void ClearSubscription()
    {
        _eventInstance -= HandleInvokeEvent;
    }
    #endregion

    #region Private Members
    protected void HandleInvokeEvent( )
    {
        Dispatched = true;
        ClearSubscription();
    }
    #endregion
}

public class WaitForAction<T> : CustomYieldInstruction
{
    #region Private Fields
    private Action<T> _eventInstance;
    #endregion

    public T Arg { get; private set; }

    #region Properties
    public bool Dispatched { get; private set; }

    public override bool keepWaiting { get { return !Dispatched; } }
    #endregion

    #region Constructors
    public WaitForAction( Action<T> action )
    {
        _eventInstance = action ?? throw new ArgumentNullException(nameof(action));
        Dispatched = false;
        _eventInstance += HandleInvokeEvent;
    }
    #endregion

    #region Protected Members
    protected void ClearSubscription()
    {
        _eventInstance -= HandleInvokeEvent;
    }
    #endregion

    #region Private Members
    private void HandleInvokeEvent(T arg )
    {
        Dispatched = true;
        ClearSubscription();
    }
    #endregion
}