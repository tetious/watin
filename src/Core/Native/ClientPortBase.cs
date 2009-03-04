// --------------------------------------------------------------------------------------------------------------------- 
// <copyright file="ClientPortBase.cs">
//   Copyright 2006-2009 Jeroen van Menen
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
// </copyright>
// <summary>
//   Defines Common client port behaviour.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------

namespace WatiN.Core.Native
{
    using System;
    using System.Text;

    /// <summary>
    /// Common client port behaviour.
    /// </summary>
    public abstract class ClientPortBase : IClientPort
    {
        /// <summary>
        /// Gets the last response recieved from the jssh server
        /// </summary>
        private string lastResponse;

        /// <summary>
        /// Used by CreateElementVariableName
        /// </summary>
        private static long elementCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPortBase"/> class.
        /// </summary>
        protected ClientPortBase()
        {
            this.Response = new StringBuilder();
        }

        /// <summary>
        /// Gets the name of the javascript variable that references the DOM:document object.
        /// </summary>
        public abstract string DocumentVariableName
        {
            get;
        }

        /// <summary>
        /// Gets or sets the entire response from the remote server so far.
        /// </summary>
        /// <value>The response from the remote server so far.</value>
        protected StringBuilder Response
        {
            get;

            set;
        }

        /// <summary>
        /// Gets or sets the last reponse recieved from the jssh server
        /// </summary>
        protected string LastResponse
        {
            get
            {
                return this.LastResponseIsNull ? null : this.lastResponse;
            }
            set { lastResponse = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the last response was <c>true</c>.
        /// </summary>
        protected bool LastResponseAsBool
        {
            get
            {
                bool lastBoolResponse;
                Boolean.TryParse(this.LastResponse, out lastBoolResponse);
                return lastBoolResponse;
            }
        }

        /// <summary>
        /// Gets or sets the last response without any cleaning applied to it.
        /// </summary>
        /// <value>The last response raw.</value>
        protected string LastResponseRaw
        {
            get;

            set;
        }

        /// <summary>
        /// Gets a value indicating whether last response is null.
        /// </summary>
        /// <value><c>true</c> if last response is null; otherwise, <c>false</c>.</value>
        private bool LastResponseIsNull
        {
            get
            {
                return this.lastResponse.Equals("null", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets LastResponseAsInt.
        /// </summary>
        /// <value>
        /// The last response as int.
        /// </value>
        private int LastResponseAsInt
        {
            get { return string.IsNullOrEmpty(this.LastResponse) ? 0 : int.Parse(this.lastResponse); }
        }

        /// <summary>
        /// Writes the specified data to the jssh server.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">Arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        public virtual void Write(string data, params object[] args)
        {
            var command = data.EndsWith(";") == false ? data + ";" : data;
            this.SendAndRead(command + " true;", true, true, args);
        }

        /// <summary>
        /// Writes the specified data ignoring any errors and reads the response.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">The arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        /// <returns>The response to the data written.</returns>
        public virtual string WriteAndReadIgnoreError(string data, params object[] args)
        {
            this.SendAndRead(data, false, false, args);
            return this.LastResponse;
        }

        /// <summary>
        /// Writes the specified data and reads the response.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">The arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        /// <returns>The response to the data written.</returns>
        public virtual string WriteAndRead(string data, params object[] args)
        {
            this.SendAndRead(data, false, true, args);
            return this.LastResponse;
        }

        /// <summary>
        /// Writes the specified data and read the response parsing it as a boolean.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">The arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        /// <returns>A boolean value from the response to the data written.</returns>
        public virtual bool WriteAndReadAsBool(string data, params object[] args)
        {
            this.SendAndRead(data, true, true, args);
            return this.LastResponseAsBool;
        }

        /// <summary>
        /// Writes the and read as int.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="args">The arguments to be passed to <see cref="string.Format(string,object[])"/></param>
        /// <returns>An integer value parsed from the response.</returns>
        public virtual int WriteAndReadAsInt(string data, params object[] args)
        {
            this.SendAndRead(data, true, true, args);
            return this.LastResponseAsInt;
        }

        /// <summary>
        /// Initializes the document.
        /// </summary>
        public abstract void InitializeDocument();

        /// <summary>
        /// Writes the specified data to the jssh server.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="resultExpected"><c>true</c> if a result is expected.</param>
        /// <param name="checkForErrors"><c>true</c> if error checking should be applied.</param>
        /// <param name="args">Arguments to format with the data.</param>        
        protected abstract void SendAndRead(string data, bool resultExpected, bool checkForErrors, params object[] args);

        protected void AddToLastResponse(string response)
        {
            lastResponse += response;
        }

        /// <summary>
        /// Creates a unique variable name, i.e. doc.watin23
        /// </summary>
        /// <returns>A unique variable.</returns>
        public string CreateVariableName()
        {
            if (elementCounter == long.MaxValue)
            {
                elementCounter = 0;
            }

            elementCounter++;
            return string.Format("{0}.watin{1}", this.DocumentVariableName, elementCounter);
        }
    }
}