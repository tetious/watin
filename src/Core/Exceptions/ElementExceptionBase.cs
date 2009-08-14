using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WatiN.Core.Logging;

namespace WatiN.Core.Exceptions
{
    public class ElementExceptionBase : WatiNException
    {
        public Element Element;

        public ElementExceptionBase(){}		
        public ElementExceptionBase(string message) : base(message){}
        public ElementExceptionBase(string message, Exception innerexception) : base(message, innerexception){}
        public ElementExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
