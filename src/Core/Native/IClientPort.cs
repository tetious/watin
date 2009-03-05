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
    /// Common behavior for a client port used to communicate with a browsers remote automation server.
    /// </summary>
    public interface IClientPort
    {
        /// <summary>
        /// Writes the specified data to the jssh server.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">Arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        void Write(string data, params object[] args);

        /// <summary>
        /// Writes the specified data ignoring any errors and reads the response.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">The arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        /// <returns>The response to the data written.</returns>
        string WriteAndReadIgnoreError(string data, params object[] args);

        /// <summary>
        /// Writes the specified data and reads the response.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">The arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        /// <returns>The response to the data written.</returns>
        string WriteAndRead(string data, params object[] args);

        /// <summary>
        /// Writes the specified data and read the response parsing it as a boolean.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">The arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        /// <returns>A boolean value from the response to the data written.</returns>
        bool WriteAndReadAsBool(string data, params object[] args);

        /// <summary>
        /// Writes the and read as int.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">The arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        /// <returns>An integer value parsed from the response.</returns>
        int WriteAndReadAsInt(string data, params object[] args);

        /// <summary>
        /// Initializes the document.
        /// </summary>
        void InitializeDocument();
    }
}