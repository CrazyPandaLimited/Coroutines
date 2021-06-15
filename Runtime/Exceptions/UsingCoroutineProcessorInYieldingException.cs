using System;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
	public class UsingCoroutineProcessorInYieldingException : Exception
	{
		private const string ErrorMessage = "You are not allow use IProcessor in yieldinig. Looks like you don't understand what you doing";

		public UsingCoroutineProcessorInYieldingException() : base( ErrorMessage )
		{
		}

		public UsingCoroutineProcessorInYieldingException( Exception innerException ) : base( ErrorMessage, innerException )
		{
		}
	}
}