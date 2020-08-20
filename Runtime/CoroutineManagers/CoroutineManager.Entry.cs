using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public partial class CoroutineManager
	{
		public class Entry
		{
			#region Private Fields
			private WeakReference _targetReference;
			private bool _isUnityObject;
			#endregion

			#region Properties
			/// <summary>
			/// Returns target, which tracks coroutine  
			/// </summary>
			public object Target { get { return _targetReference.Target; } }

			/// <summary>
			/// Returns coroutine alive state
			/// </summary>
			public bool IsAlive
			{
				get
				{
					var alive = _targetReference.IsAlive;
					if( !alive || !_isUnityObject )
					{
						return alive;
					}
					var unityObj = _targetReference.Target as Object;
					if( unityObj == null )
					{
						return false;
					}
					return true;
				}
			}

			/// <summary>
			/// Invokes on any errors, during coroutines execution process
			/// </summary>
			public Action< object, Exception > HandlerError { get; }
			/// <summary>
			/// Returns current coroutine
			/// </summary>
			public IEnumerator Enumerator { get; }
			/// <summary>
			/// Returns current CoroutineProcessor, which controls coroutine execution process
			/// </summary>
			public EnumeratorCoroutineProcessor CoroutineProcessor { get; private set; }
			#endregion

			#region Constructors
			public Entry( object target, IEnumerator enumerator, EnumeratorCoroutineProcessor coroutineProcessor, Action< object, Exception > handlerError )
			{
				_isUnityObject = target is Object;
				_targetReference = new WeakReference( target );
				Enumerator = enumerator ?? throw new ArgumentException(nameof(enumerator));
				CoroutineProcessor = coroutineProcessor ?? throw new ArgumentException(nameof(coroutineProcessor));
				HandlerError = handlerError;
			}
			#endregion
		}
	}
}