#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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

#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
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
using System.Collections;
using System.Diagnostics;
using System.Threading;
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;

namespace WatiN.Core.DialogHandlers
{
	/// <summary>
	/// This class handles alert/popup dialogs. Every second it checks if a dialog
	/// is shown. If so, it stores it's message in the alertQueue and closses the dialog
	/// by clicking the close button in the title bar.  
	/// </summary>
	public class DialogWatcher : IDisposable
	{
		private int ieProcessId;
		private bool keepRunning = true;
		private ArrayList handlers = new ArrayList();
		private Thread watcherThread;
		private bool closeUnhandledDialogs = Settings.AutoCloseDialogs;
		private int referenceCount = 0;

		private static ArrayList dialogWatchers = new ArrayList();
		private Exception lastException;

		/// <summary>
		/// Gets the dialog watcher for the specified process. It creates
		/// a new instance if no dialog watcher for the specified process 
		/// exists.
		/// </summary>
		/// <param name="ieProcessId">The ie process id.</param>
		/// <returns></returns>
		public static DialogWatcher GetDialogWatcherForProcess(int ieProcessId)
		{
			CleanupDialogWatcherCache();

			DialogWatcher dialogWatcher = GetDialogWatcherFromCache(ieProcessId);

			// If no dialogwatcher exists for the ieprocessid then 
			// create a new one, store it and return it.
			if (dialogWatcher == null)
			{
				dialogWatcher = new DialogWatcher(ieProcessId);

				dialogWatchers.Add(dialogWatcher);
			}

			return dialogWatcher;
		}

		public static DialogWatcher GetDialogWatcherFromCache(int ieProcessId)
		{
			// Loop through already created dialogwatchers and
			// return a dialogWatcher if one exists for the given processid
			foreach (DialogWatcher dialogWatcher in dialogWatchers)
			{
				if (dialogWatcher.ProcessId == ieProcessId)
				{
					return dialogWatcher;
				}
			}

			return null;
		}

		public static void CleanupDialogWatcherCache()
		{
			ArrayList cleanedupDialogWatcherCache = new ArrayList();

			foreach (DialogWatcher dialogWatcher in dialogWatchers)
			{
				if (!dialogWatcher.IsRunning)
				{
					dialogWatcher.Dispose();
				}
				else
				{
					cleanedupDialogWatcherCache.Add(dialogWatcher);
				}
			}

			dialogWatchers = cleanedupDialogWatcherCache;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DialogWatcher"/> class.
		/// You are encouraged to use the Factory method DialogWatcher.GetDialogWatcherForProcess
		/// instead.
		/// </summary>
		/// <param name="ieProcessId">The ie process id.</param>
		public DialogWatcher(int ieProcessId)
		{
			this.ieProcessId = ieProcessId;

			handlers = new ArrayList();

			// Create thread to watch windows
			watcherThread = new Thread(new ThreadStart(Start));
			// Start the thread.
			watcherThread.Start();
		}

		/// <summary>
		/// Increases the reference count of this DialogWatcher instance with 1.
		/// </summary>
		public void IncreaseReferenceCount()
		{
			referenceCount++;
		}

		/// <summary>
		/// Decreases the reference count of this DialogWatcher instance with 1.
		/// When reference count becomes zero, the Dispose method will be 
		/// automatically called. This method will throw an <see cref="ReferenceCountException"/>
		/// if the reference count is zero.
		/// </summary>
		public void DecreaseReferenceCount()
		{
			if (ReferenceCount > 0)
			{
				referenceCount--;
			}
			else
			{
				throw new ReferenceCountException();
			}

			if (ReferenceCount == 0)
			{
				Dispose();
			}
		}

		/// <summary>
		/// Adds the specified handler.
		/// </summary>
		/// <param name="handler">The handler.</param>
		public void Add(IDialogHandler handler)
		{
			lock (this)
			{
				handlers.Add(handler);
			}
		}

		/// <summary>
		/// Removes the specified handler.
		/// </summary>
		/// <param name="handler">The handler.</param>
		public void Remove(IDialogHandler handler)
		{
			lock (this)
			{
				handlers.Remove(handler);
			}
		}

		/// <summary>
		/// Removes all instances that match <paramref name="handler"/>.
		/// This method determines equality by calling Object.Equals.
		/// </summary>
		/// <param name="handler">The object implementing IDialogHandler.</param>
		/// <example>
		/// If you want to use RemoveAll with your custom dialog handler to
		/// remove all instances of your dialog handler from a DialogWatcher instance,
		/// you should override the Equals method in your custom dialog handler class 
		/// like this:
		/// <code>
		/// public override bool Equals(object obj)
		/// {
		///   if (obj == null) return false;
		///   
		///   return (obj is YourDialogHandlerClassNameGoesHere);
		/// }                               
		/// </code>
		/// You could also inherit from <see cref="BaseDialogHandler"/> instead of implementing
		/// <see cref="IDialogHandler"/> in your custom dialog handler. <see cref="BaseDialogHandler"/> provides
		/// overrides for Equals and GetHashCode that work with RemoveAll.
		/// </example>
		public void RemoveAll(IDialogHandler handler)
		{
			while (Contains(handler))
			{
				Remove(handler);
			}
		}

		/// <summary>
		/// Removes all registered dialog handlers.
		/// </summary>
		public void Clear()
		{
			lock (this)
			{
				handlers.Clear();
			}
		}

		/// <summary>
		/// Determines whether this <see cref="DialogWatcher"/> contains the specified dialog handler.
		/// </summary>
		/// <param name="handler">The dialog handler.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified handler]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(object handler)
		{
			lock (this)
			{
				return handlers.Contains(handler);
			}
		}

		/// <summary>
		/// Gets the count of registered dialog handlers.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get
			{
				lock (this)
				{
					return handlers.Count;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether unhandled dialogs should be closed automaticaly.
		/// The initial value is set to the value of <paramref name="Settings.AutoCloseDialogs" />.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if unhandled dialogs should be closed automaticaly; otherwise, <c>false</c>.
		/// </value>
		public bool CloseUnhandledDialogs
		{
			get
			{
				lock (this)
				{
					return closeUnhandledDialogs;
				}
			}
			set
			{
				lock (this)
				{
					closeUnhandledDialogs = value;
				}
			}
		}

		/// <summary>
		/// Gets the process id this dialog watcher watches.
		/// </summary>
		/// <value>The process id.</value>
		public int ProcessId
		{
			get { return ieProcessId; }
		}

		/// <summary>
		/// Called by the constructor to start watching popups
		/// on a separate thread.
		/// </summary>
		private void Start()
		{
			while (keepRunning)
			{
				Process process = getProcess(ProcessId);

				if (process != null)
				{
					foreach (ProcessThread t in process.Threads)
					{
                        if (!keepRunning) return;

						int threadId = t.Id;

						NativeMethods.EnumThreadProc callbackProc = new NativeMethods.EnumThreadProc(myEnumThreadWindowsProc);
						NativeMethods.EnumThreadWindows(threadId, callbackProc, IntPtr.Zero);
					}

					// Keep DialogWatcher responsive during 1 second sleep period
					int count = 0;
					while (keepRunning && count < 5)
					{
						Thread.Sleep(200);
						count++;
					}
				}
				else
				{
					keepRunning = false;
				}
			}
		}

		public bool IsRunning
		{
			get { return watcherThread.IsAlive; }
		}

		/// <summary>
		/// Gets a value indicating whether the process this dialog watcher
		/// watches (still) exists.
		/// </summary>
		/// <value><c>true</c> if process exists; otherwise, <c>false</c>.</value>
		public bool ProcessExists
		{
			get { return (getProcess(ProcessId) != null); }
		}

		public int ReferenceCount
		{
			get { return referenceCount; }
		}

		private Process getProcess(int processId)
		{
			Process process;
			try
			{
				process = Process.GetProcessById(processId);
			}
			catch (ArgumentException)
			{
				// Thrown when the ieProcessId is not running (anymore)
				process = null;
			}
			return process;
		}

		private bool myEnumThreadWindowsProc(IntPtr hwnd, IntPtr lParam)
		{
			// Create a window wrapper class
			HandleWindow(new Window(hwnd));

			// Always return true so all windows in all threads will
			// be enumerated.
			return true;
		}

		/// <summary>
		/// Get the last stored exception thrown by a dialog handler while 
		/// calling the <see cref="IDialogHandler.HandleDialog"/> method of the
		/// dialog handler.
		/// </summary>
		/// <value>The last exception.</value>
		public Exception LastException
		{
			get { return lastException; }
		}

		/// <summary>
		/// If the window is a dialog and visible, it will be passed to
		/// the registered dialog handlers. I none if these can handle
		/// it, it will be closed if <see cref="CloseUnhandledDialogs"/>
		/// is <c>true</c>.
		/// </summary>
		/// <param name="window">The window.</param>
		public void HandleWindow(Window window)
		{
			if (window.IsDialog())
			{
				WaitUntilVisibleOrTimeOut(window);

				// Lock the thread and see if a handler will handle
				// this dialog window
				lock (this)
				{
					foreach (IDialogHandler dialogHandler in handlers)
					{
						try
						{
							if (dialogHandler.HandleDialog(window)) return;
						}
						catch (Exception e)
						{
							lastException = e;

							Logger.LogAction("Exception was thrown while DialogWatcher called HandleDialog:");
							Logger.LogAction(e.ToString());
						}
					}

					// If no handler handled the dialog, see if the dialog
					// should be closed automatically.
					if (CloseUnhandledDialogs)
					{
						Logger.LogAction("Auto closing dialog with title '{0}'.", window.Title);
						window.ForceClose();
					}
				}
			}
		}

		private static void WaitUntilVisibleOrTimeOut(Window window)
		{
			// Wait untill window is visible so all properties
			// of the window class (like Style and StyleInHex)
			// will return valid values.
			SimpleTimer timer = new SimpleTimer(Settings.WaitForCompleteTimeOut);

			do
			{
				if (window.Visible) return;

				Thread.Sleep(50);
			} while (!timer.Elapsed);

			Logger.LogAction("Dialog with title '{0}' not visible after {1} seconds.", window.Title, Settings.WaitForCompleteTimeOut);
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			lock (this)
			{
				keepRunning = false;
				if (IsRunning)
				{
					watcherThread.Join();
				}
				Clear();
			}
		}

		#endregion
	}
}