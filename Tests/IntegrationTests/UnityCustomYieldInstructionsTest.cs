using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace CrazyPanda.UnityCore.CoroutineSystem.Tests
{
	public class UnityCustomYieldInstructionsTest : MonoBehaviour
	{
		#region Public Members
		public void Start()
		{
			var coroutineMgr = new CoroutineManager( null );
			coroutineMgr.StartCoroutine( this, UnityWaitForRealtime() );
		}
		#endregion

		#region Private Members
		private IEnumerator UnityWaitForRealtime()
		{
			// прогрев
			yield return new CoroutineSystemWaitForSeconds( 0.25f );


			var timeStart = DateTime.Now;
			yield return new WaitForSecondsRealtime( 0.5f );
			Assert.AreApproximatelyEqual( ( float ) ( DateTime.Now - timeStart ).TotalSeconds, 0.5f, 0.1f );

			var waitFrames = 7;
			yield return new WaitUntil( () =>
			{
				waitFrames--;
				return waitFrames == 0;
			} );
			Assert.AreEqual( waitFrames, 0 );

			IntegrationTest.Pass( gameObject );
		}
		#endregion
	}
}
