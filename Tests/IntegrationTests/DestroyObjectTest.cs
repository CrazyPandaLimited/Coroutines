using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace CrazyPanda.UnityCore.CoroutineSystem.Tests
{
	public class DestroyObjectTest : MonoBehaviour
	{
		#region Private Fields
		private GameObject _child;
		private bool _flag;
		#endregion

		#region Public Members
		public void Start()
		{
			var coroutineMgr = new CoroutineManager( null );
			_child = new GameObject( "Child" );
			_child.transform.SetParent( transform );

			// суть такая - мы запускаем две короутины.
			// В короутине DestroyObject дестроится объект, 
			// на который зарегистрирована короутина UsingObject
			// После дестроя объекта - короутина UsingObject должна остановиться самостоятельно
			coroutineMgr.StartCoroutine( this, DestroyObject() );
			coroutineMgr.StartCoroutine( _child, UsingObject() );
		}

		public IEnumerator UsingObject()
		{
			yield return new CoroutineSystemWaitForSeconds( 0.5f );
			yield return _child.name;
			_flag = true;
		}

		public IEnumerator DestroyObject()
		{
			yield return new CoroutineSystemWaitForSeconds( 0.25f );
			Destroy( _child );
			yield return new CoroutineSystemWaitForSeconds( 0.5f );
			Assert.IsFalse( _flag );
			IntegrationTest.Pass();
		}
		#endregion
	}
}
