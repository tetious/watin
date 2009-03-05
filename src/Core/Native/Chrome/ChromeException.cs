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

namespace WatiN.Core.Native.Chrome
{
    using System;
    using System.Runtime.Serialization;

    using WatiN.Core.Exceptions;

    /// <summary>
    /// Exceptions thrown by the Chrome web browser.
    /// </summary>
    [Serializable]
    public class ChromeException : WatiNException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeException"/> class.
        /// </summary>
        public ChromeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ChromeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerexception">The innerexception.</param>
        public ChromeException(string message, Exception innerexception) : base(message, innerexception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        public ChromeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}