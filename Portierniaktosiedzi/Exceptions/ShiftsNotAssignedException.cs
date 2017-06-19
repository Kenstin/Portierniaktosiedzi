using System;
using System.Runtime.Serialization;

namespace Portierniaktosiedzi.Exceptions
{
    [Serializable]
    public class ShiftsNotAssignedException : Exception
    {
        public ShiftsNotAssignedException()
            : base("All shifts have to be assigned.")
        {
        }

        public ShiftsNotAssignedException(string message)
            : base(message)
        {
        }

        public ShiftsNotAssignedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ShiftsNotAssignedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
