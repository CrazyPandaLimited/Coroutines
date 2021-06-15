using System;
using System.Collections;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class DuplicateEnumeratorException : Exception
    {
        /// <summary>
        /// Returns current coroutine
        /// </summary>
        public IEnumerator Enumerator { get; }

        public DuplicateEnumeratorException( IEnumerator enumerator ) : base( "Can't add same enumerator twice" )
        {
            Enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        public DuplicateEnumeratorException( IEnumerator enumerator, Exception innerException ) : base( "Can't add same enumerator twice", innerException )
        {
            Enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }
    }
}