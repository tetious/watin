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
using WatiN.Core.Exceptions;
using WatiN.Core.Logging;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.DialogHandlers
{
	public class FileDownloadHandler : BaseDialogHandler
	{
		private Window downloadProgressDialog;
	    private readonly FileDownloadOptionEnum _optionEnum;
		private readonly string saveAsFilename = String.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDownloadHandler"/> class.
		/// Use this constructor if you want to Run, Open or Cancel the download.
		/// </summary>
		/// <param name="option">The option to choose on the File Download dialog.</param>
		public FileDownloadHandler(FileDownloadOptionEnum option)
		{
			if (option == FileDownloadOptionEnum.Save)
			{
				throw new ArgumentException("When using FileDownloadOptionEnum.Save call the constructor which accepts a filename as an argument");
			}

			_optionEnum = option;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDownloadHandler"/> class.
		/// Use this contructor if you want to download and save a file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		public FileDownloadHandler(string filename)
		{
			if (UtilityClass.IsNullOrEmpty(filename))
			{
				throw new ArgumentNullException("filename", "Not a valid value");
			}

			_optionEnum = FileDownloadOptionEnum.Save;
			saveAsFilename = filename;
		}

	    /// <summary>
	    /// Gets a value indicating whether this instance has handled a file download dialog.
	    /// </summary>
	    /// <value>
	    /// 	<c>true</c> if this instance has handled a file download dialog; otherwise, <c>false</c>.
	    /// </value>
	    public bool HasHandledFileDownloadDialog { get; private set; }

	    /// <summary>
		/// Gets the full save as filename used when the downloaded file will be saved to disk.
		/// </summary>
		/// <value>The save as filename.</value>
		public string SaveAsFilename
		{
			get { return saveAsFilename; }
		}

		/// <summary>
		/// Handles the dialogs to download (and save) a file
		/// Mainly used internally by WatiN.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns></returns>
		public override bool HandleDialog(Window window)
		{
			if (HandledFileDownloadDialog(window)) return true;

			return HandledDownloadProgressDialog(window) || HandledFileSaveDialog(window);
		}

	    public override bool CanHandleDialog(Window window, IntPtr mainWindowHwnd)
	    {
            return CanHandleDialog(window);
	    }

        public override bool CanHandleDialog(Window window)
	    {
	        return IsFileDownloadDialog(window) || IsDownloadProgressDialog(window) || IsFileSaveDialog(window);
	    }

	    private bool HandledFileSaveDialog(Window window)
	    {
	        if (IsFileSaveDialog(window))
	        {
	            Logger.LogAction("Saving Download file as {0}", saveAsFilename);

	            DownloadProgressDialog = new Window(window.ParentHwnd);

	            HandleFileSaveDialog(window);

	            return true;
	        }
	        return false;
	    }

	    private bool HandledDownloadProgressDialog(Window window)
	    {
	        if (IsDownloadProgressDialog(window))
	        {
	            DownloadProgressDialog = window;

	            var openOrRun= new WinButton(4377, new Hwnd(window.Hwnd));

	            if (openOrRun.Enabled)
	            {
	                var close = new WinButton(2, new Hwnd(window.Hwnd));
	                close.Click();

	                var actionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(5));
                    actionUntilTimeOut.Try(() => window.Exists());

                    // TODO: What to do if the window doesn't close after timeout?
	            }

	            return true;
	        }
	        return false;
	    }

	    private bool HandledFileDownloadDialog(Window window)
	    {
	        if (!HasHandledFileDownloadDialog && IsFileDownloadDialog(window))
	        {
	            window.ToFront();
	            window.SetActivate();

	            DownloadProgressDialog = new Window(window.ParentHwnd);

	            var btn = GetButtonToPress(window);
	            btn.Click();

	            HasHandledFileDownloadDialog = !Exists(window);

	            if (HasHandledFileDownloadDialog)
	            {
	                Logger.LogAction("Download started at {0}", DateTime.Now.ToLongTimeString());
	                Logger.LogAction("Clicked {0}", _optionEnum);
	            }

	            return true;
	        }
	        return false;
	    }

	    private WinButton GetButtonToPress(Window window)
		{
			WinButton btn = null;

			switch (_optionEnum)
			{
				case FileDownloadOptionEnum.Run:
					btn = new WinButton(4426, window.Hwnd);
					break;

				case FileDownloadOptionEnum.Open:
					btn = new WinButton(4426, window.Hwnd);
					break;

				case FileDownloadOptionEnum.Save:
					btn = new WinButton(4427, window.Hwnd);
					if (!btn.Exists())
					{
						btn = new WinButton(4424, window.Hwnd);
					}
					break;

				case FileDownloadOptionEnum.Cancel:
					btn = new WinButton(2, window.Hwnd);
					break;
			}

			return btn;
		}

		/// <summary>
		/// Determines whether the specified window is a file download dialog by
		/// checking the style property of the window. It should match
		/// <c>window.StyleInHex == "94C80AC4"</c>
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>
		/// 	<c>true</c> if the specified window is a file download dialog; otherwise, <c>false</c>.
		/// </returns>
		public bool IsFileDownloadDialog(Window window)
		{
			return (window.StyleInHex == "94C80AC4");
		}

		/// <summary>
		/// Determines whether the specified window is a download progress dialog by
		/// checking the style property of the window. It should match
		/// <c>(window.StyleInHex == "9CCA0BC4") || (window.StyleInHex == "94CA0BC4")</c>
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>
		/// 	<c>true</c> if the specified window is a download progress dialog; otherwise, <c>false</c>.
		/// </returns>
		public bool IsDownloadProgressDialog(Window window)
		{
			// "9CCA0BC4" is valid before downloading of the file has started
			// "94CA0BC4" is valid during and after the download
			return (window.StyleInHex == "9CCA0BC4") || (window.StyleInHex == "94CA0BC4");
		}

		/// <summary>
		/// Determines whether the specified window is a file save as dialog by
		/// checking the style property of the window. It should match
		/// <c>(window.StyleInHex == "96CC20C4") || (window.StyleInHex == "96CC02C4")</c>
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>
		/// 	<c>true</c> if the specified window is a file save as dialog; otherwise, <c>false</c>.
		/// </returns>
		public bool IsFileSaveDialog(Window window)
		{
			// "96CC20C4" is valid for Windows XP, Win 2000 and Win 2003
			// "96CC02C4" is valid for Windows Vista
			return (window.StyleInHex == "96CC20C4") || (window.StyleInHex == "96CC02C4");
		}

		/// <summary>
		/// Determines if a dialog still exists by checking the the existance of the 
		/// window.Hwnd and checking if the window is visible.
		/// </summary>
		/// <param name="dialog">The dialog.</param>
		/// <returns><c>true</c> if exists and visible, otherwise <c>false</c></returns>
		public bool Exists(Window dialog)
		{
			return dialog.Exists() && dialog.Visible;
		}

		/// <summary>
		/// Checks if window is null or <see cref="Exists"/>.
		/// </summary>
		/// <param name="dialog">The dialog.</param>
		/// <returns><c>true</c> if null or exists, otherwise <c>false</c></returns>
		public bool ExistsOrNull(Window dialog)
		{
		    return dialog == null || Exists(dialog);
		}

	    /// <summary>
		/// Wait until the save/open/run dialog opens.
		/// This exists because some web servers are slower to start a file than others.
		/// </summary>
		/// <param name="waitDurationInSeconds">duration in seconds to wait</param>
		public void WaitUntilFileDownloadDialogIsHandled(int waitDurationInSeconds)
		{
	        var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(waitDurationInSeconds));
            tryActionUntilTimeOut.Try(() => HasHandledFileDownloadDialog);

	        if (!HasHandledFileDownloadDialog)
			{
				throw new WatiNException(string.Format("Has not shown dialog after {0} seconds.", waitDurationInSeconds));
			}
		}

		/// <summary>
		/// Wait until the download progress window does not exist any more
		/// </summary>
		/// <param name="waitDurationInSeconds">duration in seconds to wait</param>
		public void WaitUntilDownloadCompleted(int waitDurationInSeconds)
		{
            var tryActionUntilTimeOut = new TryFuncUntilTimeOut(TimeSpan.FromSeconds(waitDurationInSeconds));
            tryActionUntilTimeOut.Try(() => !ExistsOrNull(DownloadProgressDialog));

			if (ExistsOrNull(DownloadProgressDialog))
			{
				throw new WatiNException(string.Format("Still downloading after {0} seconds.", waitDurationInSeconds));
			}

			Logger.LogAction("Download complete at {0}", DateTime.Now.ToLongTimeString());
		}

		private Window DownloadProgressDialog
		{
			get { return downloadProgressDialog; }
			set
			{
			    if (downloadProgressDialog != null) return;
			    
                var dialog = value;
			    if (dialog != null)
			    {
			        if (!dialog.Exists() || !IsDownloadProgressDialog(dialog))
			        {
			            dialog = null;
			        }
			    }

			    if (dialog != null)
			    {
			        downloadProgressDialog = dialog;
			    }
			}
		}

		private void HandleFileSaveDialog(Window window)
		{
            var fileNameHandle = NativeMethods.GetChildWindowHwnd(window.Hwnd, "Edit");
            var fileNameHwnd = new Hwnd(fileNameHandle);

            fileNameHwnd.SetFocus();
            fileNameHwnd.SendString(saveAsFilename);

            var openButton = new WinButton(1, window.Hwnd);
            openButton.Click();
		}
	}
}