using System;
using System.Runtime.Serialization;

namespace WatiN.Core.Exceptions
{
    public class ElementExceptionBase : WatiNException
    {
        /// <summary>
        /// Element around which the exception is being thrown.
        /// Be advised that this property may be null.
        /// </summary>
        public Element Element { get; set; }

        public ElementExceptionBase(){}		
        public ElementExceptionBase(string message) : base(message){}
        public ElementExceptionBase(string message, Exception innerexception) : base(message, innerexception){}
        public ElementExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
