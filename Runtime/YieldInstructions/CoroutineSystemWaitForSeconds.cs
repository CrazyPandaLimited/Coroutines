namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class CoroutineSystemWaitForSeconds
	{
		#region Properties
		/// <summary>
		/// Returns time to await in seconds
		/// </summary>
		public double Seconds { get; }
		#endregion

		#region Constructors
		public CoroutineSystemWaitForSeconds( double seconds ) => Seconds = seconds;
		#endregion
	}
}