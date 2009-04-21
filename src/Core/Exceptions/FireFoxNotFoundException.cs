using System;
using System.Runtime.Serialization;

namespace WatiN.Core.Exceptions
{
    /// <summary>
    /// Thrown if the searched for Firefox can't be found.
    /// </summary>
    [Serializable]
    public class FireFoxNotFoundException : WatiNException
    {
        public FireFoxNotFoundException(string constraint, int waitTimeInSeconds) :
            base("Could not find a Firefox window matching constraint: " + constraint + ". Search expired after '" + waitTimeInSeconds.ToString() + "' seconds.") { }

        public FireFoxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
