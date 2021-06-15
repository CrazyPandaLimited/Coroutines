namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class CoroutineSystemWaitForSeconds
	{
		/// <summary>
		/// Returns time to await in seconds
		/// </summary>
		public double Seconds { get; }

		public CoroutineSystemWaitForSeconds( double seconds ) => Seconds = seconds;
	}
}