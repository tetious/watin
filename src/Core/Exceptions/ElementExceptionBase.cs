#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

//Copyright 2006-2009 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

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
