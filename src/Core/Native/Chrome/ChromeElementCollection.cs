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
    using System.Collections.Generic;

    using Mozilla;

    /// <summary>
    /// Chrome implementation of the <see cref="INativeElementCollection"/>.
    /// </summary>
    internal class ChromeElementCollection : JSElementCollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeElementCollection"/> class.
        /// </summary>
        /// <param name="clientPort">
        /// The client port.
        /// </param>
        /// <param name="containerReference">
        /// The container reference.
        /// </param>
        public ChromeElementCollection(ClientPortBase clientPort, string containerReference) : base(clientPort, containerReference)
        {
        }
    }
}