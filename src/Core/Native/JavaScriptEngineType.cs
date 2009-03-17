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

namespace WatiN.Core.Native
{
    /// <summary>
    /// Defines the different types of javascript engine.
    /// </summary>
    public enum JavaScriptEngineType
    {
        /// <summary>
        /// Unkown javascript engine
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// JScript engine used by internet explorer http://en.wikipedia.org/wiki/JScript.
        /// </summary>
        JScript = 1,

        /// <summary>
        /// WebKit javascript engine used be Safari, Chrome and others http://en.wikipedia.org/wiki/WebKit
        /// </summary>
        WebKit = 2,

        /// <summary>
        /// Mozilla javascript engine.
        /// </summary>
        Mozilla = 3
    }
}