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

# if INCLUDE_CHROME

namespace WatiN.Core.Native.Chrome
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    
    using Logging;

    using UtilityClasses;
    
    /// <summary>
    /// Handles telnet communication to the Chrome browser shell.
    /// </summary>
    public class ChromeClientPort : ClientPortBase
    {
        /// <summary>
        /// List of known errors  that the Google remote port might return.
        /// </summary>
        private static readonly List<string> knownResponseErrors = new List<string> { "unknown command", "TypeError", "SyntaxError", "\"Uncaught SyntaxError", "Uncaught ReferenceError", "true\r\r\n\"Not allowed to load local resource: file:///" };

        /// <summary>
        /// The port used to connect to chrome.
        /// </summary>
        private const int ChromePort = 9999;

        /// <summary>
        /// <c>true</c> if the <see cref="Dispose()"/> method has been called to release resources.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Underlying socket used to create a <see cref="NetworkStream"/>.
        /// </summary>
        private Socket telnetSocket;

        /// <summary>
        /// Finalizes an instance of the <see cref="ChromeClientPort"/> class. 
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="FireFox"/> is reclaimed by garbage collection.
        /// </summary>
        ~ChromeClientPort()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ChromeClientPort"/> is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected { get; private set; }

        /// <summary>
        /// Gets the name of the javascript variable that references the DOM:document object.
        /// </summary>
        public override string DocumentVariableName
        {
            get
            {
                return "document";
            }
        }

        /// <summary>
        /// Gets the type of java script engine.
        /// </summary>
        /// <value>The type of java script engine.</value>
        public override JavaScriptEngineType JavaScriptEngine
        {
            get
            {
                return JavaScriptEngineType.WebKit;
            }
        }

        /// <summary>
        /// Gets the name of the browser variable.
        /// </summary>
        /// <value>The name of the browser variable.</value>
        public override string BrowserVariableName
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the last command send to the chrome client.
        /// </summary>
        /// <value>The last command send.</value>
        private string LastCommandSend
        {
            get;
            set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Connects to the Chrome browser and navigates to the specified URL.
        /// </summary>
        /// <param name="url">The URL to connect to.</param>
        public override void Connect(string url)
        {
            this.ValidateCanConnect();
            this.disposed = false;

            Logger.LogDebug(string.Format("Attempting to connect to chrome on localhost port {0}.", ChromePort));

            this.LastResponse = string.Empty;
            this.Response = new StringBuilder();

            this.Process = Core.Chrome.CreateProcess(string.Format("--disable-hang-monitor --remote-shell-port={0} \"{1}\"", ChromePort, url), true);            

            this.telnetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { Blocking = true };

            try
            {
                this.telnetSocket.Connect(IPAddress.Parse("127.0.0.1"), ChromePort);
            }
            catch (SocketException sockException)
            {
                Logger.LogDebug(string.Format("Failed connecting to jssh server.\nError code:{0}\nError message:{1}", sockException.ErrorCode, sockException.Message));
                throw new ChromeException("Unable to connect to jssh server, please make sure you have correctly installed the jssh.xpi plugin", sockException);
            }

            this.Connected = true;
            this.WaitForConnectionEstablished();
            Logger.LogDebug(string.Format("Successfully connected to Chrome on port.{0}", ChromePort));
            
            //DefineDefaultJSVariables();
        }

        public override void ConnectToExisting()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes the document.
        /// </summary>
        public override void InitializeDocument()
        {            
        }

        /// <summary>
        /// Writes the specified data to the remote server.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="resultExpected"><c>true</c> if a result is expected.</param>
        /// <param name="checkForErrors"><c>true</c> if error checking should be applied.</param>
        /// <param name="args">Arguments to format with the data.</param>
        protected override void SendAndRead(string data, bool resultExpected, bool checkForErrors, params object[] args)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            var command = UtilityClass.StringFormat(data, args);

            if (!command.StartsWith("debug()") && !command.StartsWith("exit"))
            {
                command = "print " + command;
            }            

            this.SendCommand(command);
            this.ReadResponse(resultExpected, checkForErrors);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (this.telnetSocket != null && this.telnetSocket.Connected && !this.Process.HasExited)
                    {
                        try
                        {
                            Logger.LogDebug("Closing connection to chrome");
                            this.SendCommand("print window.close()");
                            this.telnetSocket.Close();
                            this.Process.WaitForExit(5000);
                        }
                        catch (IOException ex)
                        {
                            Logger.LogDebug("Error communicating with chrome to innitiate shut down, message: {0}", ex.Message);
                        }
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                if (this.Process != null)
                {
                    if (!this.Process.HasExited)
                    {
                        Logger.LogDebug("Closing Chrome");
                        this.Process.Kill();
                    }
                }
            }

            this.disposed = true;
            this.Connected = false;
        }

        /// <summary>
        /// Writes the specified data to the remote server.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="resultExpected"><c>true</c> if a result is expected.</param>
        /// <param name="checkForErrors"><c>true</c> if error checking should be applied.</param>
        /// <param name="args">Arguments to format with the data.</param>
        private void SendRawAndRead(string data, bool resultExpected, bool checkForErrors, params object[] args)
        {
            var command = UtilityClass.StringFormat(data, args);
            this.SendCommand(command);
            this.ReadResponse(resultExpected, checkForErrors);
        }

        /// <summary>
        /// Checks the response for an error.
        /// </summary>
        /// <param name="response">The response.</param>
        private static void CheckForError(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return;
            }

            foreach (string knownResponseError in knownResponseErrors)
            {
                if (response.StartsWith(knownResponseError, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ChromeException(string.Format("Error sending last message to chrome remote server: {0}", response));    
                }
            }            
        }

        /// <summary>
        /// Cleans the telnet response.
        /// </summary>
        /// <param name="data">The data to clean.</param>
        /// <returns>Cleaned response data</returns>
        private string CleanTelnetResponse(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            return data
                    .Replace(string.Format("{0}\r", this.LastCommandSend), string.Empty)
                    .Replace(string.Format("{0}v8(running)> ", Environment.NewLine), string.Empty)
                    .Replace(string.Format("{0}v8(paused)> ", Environment.NewLine), string.Empty)
                    .Trim(); 
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="data">The data to send.</param>
        private void SendCommand(string data)
        {
            if (!this.Connected)
            {
                throw new ChromeException("You must connect before writing to the server.");
            }

            this.LastCommandSend = data;
            var bytes = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            Logger.LogDebug("sending: {0}", data);
            using (var networkStream = new NetworkStream(this.telnetSocket))
            {
                networkStream.Write(bytes, 0, bytes.Length);
                networkStream.Flush();
            }
        }

        /// <summary>
        /// Reads the response from the Chrome telnet server.
        /// </summary>
        /// <param name="resultExpected">if set to <c>true</c> a result is expected.</param>
        /// <param name="checkForErrors">if set to <c>true</c> check for errors.</param>
        private void ReadResponse(bool resultExpected, bool checkForErrors)
        {
            var stream = new NetworkStream(this.telnetSocket);

            this.LastResponse = string.Empty;
            this.LastResponseRaw = string.Empty;

            var bufferSize = 4096;
            var buffer = new byte[bufferSize];

            // HACK Thread.Sleep beginning of ReadResponse. 
            // Problem is that if you start to read straight away we don't get all the data
            System.Threading.Thread.Sleep(100);            

            string readData;
            do
            {
                var read = stream.Read(buffer, 0, bufferSize);
                readData = Encoding.UTF8.GetString(buffer, 0, read);

                Logger.LogDebug("chrome says: '{0}'", readData.Replace("\n", "[newline]"));
                this.LastResponseRaw += readData;
                this.LastResponse += this.CleanTelnetResponse(readData);
            }
            while (!readData.EndsWith("> ") && !readData.Trim().EndsWith("lost connection to tab") || stream.DataAvailable || (resultExpected && string.IsNullOrEmpty(this.LastResponse)));

            // Convert \n to newline
            if (this.LastResponse != null)
            {
                this.LastResponse = this.LastResponse.Replace("\n", Environment.NewLine);
            }

            this.Response.Append(this.LastResponse);

            if (checkForErrors)
            {
                CheckForError(this.LastResponse);
            }
        }

        /// <summary>
        /// Validates the we can connect to chrome.
        /// </summary>
        private void ValidateCanConnect()
        {
            if (this.Connected)
            {
                throw new ChromeException("Already connected to chrome session.");
            }

            if (Core.Chrome.CurrentProcessCount > 0 && !Settings.CloseExistingFireFoxInstances)
            {
                throw new ChromeException("Existing instances of FireFox detected.");
            }

            if (Core.Chrome.CurrentProcessCount > 0)
            {
                Core.Chrome.CurrentProcess.Kill();
            }
        }

        /// <summary>
        /// Waits for connection established.
        /// </summary>
        private void WaitForConnectionEstablished()
        {
            this.SendCommand("\n");

            var rawResponse = string.Empty;
            var responseToWaitFor = "Chrome>";

            while (!rawResponse.Trim().EndsWith(responseToWaitFor))
            {
                this.ReadResponse(false, true);
                rawResponse += this.LastResponseRaw;
            }

            this.SendRawAndRead("debug()", true, true);
            Debug.WriteLine(this.LastResponse);
        }
    }
}
#endif