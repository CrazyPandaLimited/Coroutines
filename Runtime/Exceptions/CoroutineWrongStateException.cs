﻿using System;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class CoroutineWrongStateException : Exception
    {
        public CoroutineWrongStateException( CoroutineState _from, CoroutineState _to ) : base( "Can't change coroutine state from " + _from + " to " + _to )
        {
        }

        public CoroutineWrongStateException( CoroutineState _from, CoroutineState _to, Exception innerException ) : base( "Can't change coroutine state from " + _from + " to " + _to, innerException )
        {
        }
    }
}