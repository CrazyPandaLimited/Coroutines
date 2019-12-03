using System;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class MonoBehaviourTimeProvider : MonoBehaviour, ITimeProvider
	{
		#region Constants
		protected const string Name = "-MonoBehaviourTimeProvider";
		#endregion

		#region Private Fields
		private static MonoBehaviourTimeProvider _instance;
		#endregion

		#region Properties
		/// <summary>
		/// Creates or returns MonoBehaviourTimeProvider single instance
		/// </summary>
		public static MonoBehaviourTimeProvider Instance
		{
			get
			{
				if( _instance == null )
				{
					_instance = FindObjectOfType< MonoBehaviourTimeProvider >();

					if( _instance == null )
					{
						var go = new GameObject( Name );
						DontDestroyOnLoad( go );
						_instance = go.AddComponent< MonoBehaviourTimeProvider >();
					}
				}
				return _instance;
			}
		}

		/// <summary>
		/// Returns current time
		/// </summary>
		public float deltaTime
		{
			get { return Time.deltaTime; }
		}
		#endregion

		#region Events
		/// <summary>
		/// Event, which invokes on any error happened during updated time tick process
		/// </summary>
		public event Action< object, Exception > OnError;
		/// <summary>
		/// Event, which invokes on every tick updated
		/// </summary>
		public event Action OnUpdate;
		#endregion

		#region Public Members
		/// <summary>
		/// Updates time tick
		/// </summary>
		public void Update()
		{
			try
			{
				if( OnUpdate != null )
				{
					OnUpdate();
				}
			}
			catch( Exception exception )
			{
				if( OnError != null )
				{
					OnError.Invoke( this, exception );
				}
				else
				{
					Debug.LogException( exception );
				}
			}
		}
		#endregion
	}
}