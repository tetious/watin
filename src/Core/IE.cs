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
using System.Diagnostics;
using System.Threading;
using mshtml;
using SHDocVw;
using WatiN.Core.Constraints;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Logging;
using WatiN.Core.Native;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
	/// <summary>
	/// This is the main class to access a webpage in Internet Explorer to 
	/// get to all the elements and (i)frames on the page.
	/// 
	/// </summary>
	/// <example>
	/// The following example creates a new Internet Explorer instances and navigates to
	/// the WatiN Project website on SourceForge.
	/// <code>
	/// using WatiN.Core;
	/// 
	/// namespace NewIEExample
	/// {
	///    public class WatiNWebsite
	///    {
	///      public WatiNWebsite()
	///      {
	///        IE ie = new IE("http://watin.sourceforge.net");
	///        ie.Link(Find.ByText("RSS Feeds")).Click;
	///        ie.Close;
	///      }
	///    }
	///  }
	/// </code>
	/// </example>
	public class IE : Browser
	{
		private bool autoClose = true;
		private bool isDisposed;
	    private IEBrowser _ieBrowser;

	    /// <summary>
		/// Attach to an existing Internet Explorer. 
		/// The first instance that matches the given <paramref name="findBy"/> will be returned.
		/// The attached Internet Explorer will be closed after destroying the IE instance.
		/// </summary>
		/// <param name="findBy">The <see cref="Constraint"/> of the IE window to find. 
		/// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
		/// <returns>An <see cref="IE"/> instance.</returns>
		/// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
		/// IENotFoundException will be thrown if an Internet Explorer window with the given <paramref name="findBy"/> isn't found within 30 seconds.
		/// To change this default, set <see cref="P:WatiN.Core.Settings.AttachToIETimeOut"/>
		/// </exception>
		/// <example>
		/// The following example searches for an Internet Exlorer instance with "Example"
		/// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
		/// When found, ieExample will hold a pointer to this Internet Explorer instance and
		/// can be used to test the Example page.
		/// <code>
		/// IE ieExample = IE.AttachToIE(Find.ByTitle("Example"));
		/// </code>
		/// A partial match should also work
		/// <code>
		/// IE ieExample = IE.AttachToIE(Find.ByTitle("Exa"));
		/// </code>
		/// </example>
		public static IE AttachToIENoWait(Constraint findBy)
		{
	        return Browser.AttachToNoWait<IE>(findBy);
		}
		
		/// <summary>
		/// Attach to an existing Internet Explorer but don't wait until the whole page is loaded. 
		/// The first instance that matches the given <paramref name="findBy"/> will be returned.
		/// The attached Internet Explorer will be closed after destroying the IE instance.
		/// </summary>
		/// <param name="findBy">The <see cref="Constraint"/> of the IE window to find. 
		/// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
		/// <returns>An <see cref="IE"/> instance.</returns>
		/// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
		/// IENotFoundException will be thrown if an Internet Explorer window with the given <paramref name="findBy"/> isn't found within 30 seconds.
		/// To change this default, set <see cref="P:WatiN.Core.Settings.AttachToIETimeOut"/>
		/// </exception>
		/// <example>
		/// The following example searches for an Internet Exlorer instance with "Example"
		/// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
		/// When found, ieExample will hold a pointer to this Internet Explorer instance and
		/// can be used to test the Example page.
		/// <code>
		/// IE ieExample = IE.AttachToIE(Find.ByTitle("Example"));
		/// </code>
		/// A partial match should also work
		/// <code>
		/// IE ieExample = IE.AttachToIE(Find.ByTitle("Exa"));
		/// </code>
		/// </example>
		public static IE AttachToIE(Constraint findBy)
		{
		    return AttachTo<IE>(findBy);
		}

		/// <summary>
		/// Attach to an existing Internet Explorer. 
		/// The first instance that matches the given <paramref name="findBy"/> will be returned.
		/// The attached Internet Explorer will be closed after destroying the IE instance.
		/// </summary>
		/// <param name="findBy">The <see cref="Constraint"/> of the IE window to find. 
		/// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
		/// <param name="timeout">The number of seconds to wait before timing out</param>
		/// <returns>An <see cref="IE"/> instance.</returns>
		/// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
		/// IENotFoundException will be thrown if an Internet Explorer window with the given 
		/// <paramref name="findBy"/> isn't found within <paramref name="timeout"/> seconds.
		/// </exception>
		/// <example>
		/// The following example searches for an Internet Exlorer instance with "Example"
		/// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
		/// It will try to find an Internet Exlorer instance for 60 seconds maxs.
		/// When found, ieExample will hold a pointer to this Internet Explorer instance and
		/// can be used to test the Example page.
		/// <code>
		/// IE ieExample = IE.AttachToIE(Find.ByTitle("Example"), 60);
		/// </code>
		/// A partial match should also work
		/// <code>
		/// IE ieExample = IE.AttachToIE(Find.ByTitle("Exa"), 60);
		/// </code>
		/// </example>
		public static IE AttachToIE(Constraint findBy, int timeout)
		{
		    return AttachTo<IE>(findBy, timeout);
		}

	    /// <summary>
		/// Attach to an existing Internet Explorer but don't wait until the whole page is loaded. 
		/// The first instance that matches the given <paramref name="findBy"/> will be returned.
		/// The attached Internet Explorer will be closed after destroying the IE instance.
		/// </summary>
		/// <param name="findBy">The <see cref="Constraint"/> of the IE window to find. 
		/// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
		/// <param name="timeout">The number of seconds to wait before timing out</param>
		/// <returns>An <see cref="IE"/> instance.</returns>
		/// <exception cref="WatiN.Core.Exceptions.IENotFoundException" >
		/// IENotFoundException will be thrown if an Internet Explorer window with the given 
		/// <paramref name="findBy"/> isn't found within <paramref name="timeout"/> seconds.
		/// </exception>
		/// <example>
		/// The following example searches for an Internet Exlorer instance with "Example"
		/// in it's titlebar (showing up as "Example - Microsoft Internet Explorer").
		/// It will try to find an Internet Exlorer instance for 60 seconds maxs.
		/// When found, ieExample will hold a pointer to this Internet Explorer instance and
		/// can be used to test the Example page.
		/// <code>
		/// IE ieExample = IE.AttachToIENoWait(Find.ByTitle("Example"), 60);
		/// </code>
		/// A partial match should also work
		/// <code>
		/// IE ieExample = IE.AttachToIENoWait(Find.ByTitle("Exa"), 60);
		/// </code>
		/// </example>
		public static IE AttachToIENoWait(Constraint findBy, int timeout)
		{
			return AttachToNoWait<IE>(findBy, timeout);
		}

		/// <summary>
		/// Does the specified Internet Explorer exist.
		/// </summary>
		/// <param name="findBy">The <see cref="Constraint"/> of the IE window to find. 
		/// <c>Find.ByUrl()</c>, <c>Find.ByUri()</c>, <c>Find.ByTitle()</c> and <c>Find.By("hwnd", windowHandle)</c> are supported.</param>
		/// <returns><c>true</c> if an Internet Explorer instance matches the given <paramref name="findBy"/> <see cref="Constraint"/>. Otherwise it returns <c>false</c>. </returns>
		public static bool Exists(Constraint findBy)
		{
		    return Exists<IE>(findBy);
		}

		/// <summary>
		/// Creates a collection of new IE instances associated with open Internet Explorer windows.
		/// </summary>
		/// <returns>An IE instance which is complete loaded.</returns>
		/// <example>
		/// This code snippet illustrates the use of this method to found out the number of open
		/// Internet Explorer windows.
		/// <code>int IECount = IE.InternetExplorers.Length;</code>
		/// </example>
		public static IECollection InternetExplorers()
		{
			return new IECollection();
		}

		/// <summary>
		/// Creates a collection of new IE instances associated with open Internet Explorer windows. Use this
		/// method if you don't want WatiN to wait until a document is fully loaded before returning it.
		/// This might be handy in situations where you encounter Internet Explorer instances which are always
		/// busy loading and that way blocks itteration through the collection.
		/// </summary>
		/// <returns>An IE instance which might not have been complete loaded yet.</returns>
		/// <example>
		/// This code snippet illustrates the use of this method to itterate through all internet explorer instances.
		/// <code>
		/// foreach (IE ie in IE.InternetExplorersNoWait)
		/// {
		/// // do something but be aware that the page might not be completely loaded yet.
		/// }
		/// </code>
		/// </example>
		public static IECollection InternetExplorersNoWait()
		{
			return new IECollection(false);
		}

		/// <summary>
		/// Opens a new Internet Explorer with a blank page. 
		/// <note>
		/// When the <see cref="WatiN.Core.IE" />
		/// instance is destroyed the created Internet Explorer window will also be closed.
		/// </note>
		/// </summary>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge.
		/// <code>
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public WatiNWebsite()
		///      {
		///        IE ie = new IE();
		///        ie.GoTo("http://watin.sourceforge.net");
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE()
		{
			CreateNewIEAndGoToUri(new Uri("about:blank"), null, false);
		}

		/// <summary>
		/// Opens a new Internet Explorer with a blank page. 
		/// <note>
		/// When the <see cref="WatiN.Core.IE" />
		/// instance is destroyed the created Internet Explorer window will also be closed.
		/// </note>
		/// </summary>
		/// <param name="createInNewProcess">if set to <c>true</c> the IE instance is created in a new process.</param>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge.
		/// <code>
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public WatiNWebsite()
		///      {
		///        IE ie = new IE();
		///        ie.GoTo("http://watin.sourceforge.net");
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE(bool createInNewProcess)
		{
			CreateNewIEAndGoToUri(new Uri("about:blank"), null, createInNewProcess);
		}

		/// <summary>
		/// Opens a new Internet Explorer and navigates to the given <paramref name="url"/>.
		/// <note>
		/// When the <see cref="WatiN.Core.IE" />
		/// instance is destroyed the created Internet Explorer window will also be closed.
		/// </note>
		/// </summary>
		/// <param name="url">The URL te open</param>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge.
		/// <code>
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public OpenWatiNWebsite()
		///      {
		///        IE ie = new IE("http://watin.sourceforge.net");
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE(string url)
		{
			CreateNewIEAndGoToUri(UtilityClass.CreateUri(url), null, false);
		}

		/// <summary>
		/// Opens a new Internet Explorer and navigates to the given <paramref name="url"/>.
		/// <note>
		/// When the <see cref="WatiN.Core.IE" />
		/// instance is destroyed the created Internet Explorer window will also be closed.
		/// </note>
		/// </summary>
		/// <param name="url">The URL te open</param>
		/// <param name="createInNewProcess">if set to <c>true</c> the IE instance is created in a new process.</param>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge.
		/// <code>
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public OpenWatiNWebsite()
		///      {
		///        IE ie = new IE("http://watin.sourceforge.net");
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE(string url, bool createInNewProcess)
		{
			CreateNewIEAndGoToUri(UtilityClass.CreateUri(url), null, createInNewProcess);
		}

		/// <summary>
		/// Opens a new Internet Explorer and navigates to the given <paramref name="uri"/>.
		/// <note>
		/// When the <see cref="WatiN.Core.IE" />
		/// instance is destroyed the created Internet Explorer window will also be closed.
		/// </note>
		/// </summary>
		/// <param name="uri">The Uri te open</param>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge.
		/// <code>
		/// using System;
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public OpenWatiNWebsite()
		///      {
		///        IE ie = new IE(new Uri("http://watin.sourceforge.net"));
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE(Uri uri)
		{
			CreateNewIEAndGoToUri(uri, null, false);
		}

		/// <summary>
		/// Opens a new Internet Explorer and navigates to the given <paramref name="uri"/>.
		/// <note>
		/// When the <see cref="WatiN.Core.IE" />
		/// instance is destroyed the created Internet Explorer window will also be closed.
		/// </note>
		/// </summary>
		/// <param name="uri">The Uri te open</param>
		/// <param name="createInNewProcess">if set to <c>true</c> the IE instance is created in a new process.</param>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge.
		/// <code>
		/// using System;
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public OpenWatiNWebsite()
		///      {
		///        IE ie = new IE(new Uri("http://watin.sourceforge.net"));
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE(Uri uri, bool createInNewProcess)
		{
			CreateNewIEAndGoToUri(uri, null, createInNewProcess);
		}

		/// <summary>
		/// Opens a new Internet Explorer and navigates to the given <paramref name="url"/>.
		/// </summary>
		/// <param name="url">The Url te open</param>
		/// <param name="logonDialogHandler">A <see cref="LogonDialogHandler"/> class instanciated with the logon credentials.</param>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge leaving the created Internet Explorer open.
		/// <code>
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public WatiNWebsite()
		///      {
		///        LogonDialogHandler logon = new LogonDialogHandler("username", "password");
		///        IE ie = new IE("http://watin.sourceforge.net", logon);
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE(string url, LogonDialogHandler logonDialogHandler)
		{
			CreateNewIEAndGoToUri(UtilityClass.CreateUri(url), logonDialogHandler, false);
		}

		/// <summary>
		/// Opens a new Internet Explorer and navigates to the given <paramref name="url"/>.
		/// </summary>
		/// <param name="url">The Url te open</param>
		/// <param name="logonDialogHandler">A <see cref="LogonDialogHandler"/> class instanciated with the logon credentials.</param>
		/// <param name="createInNewProcess">if set to <c>true</c> the IE instance is created in a new process.</param>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge leaving the created Internet Explorer open.
		/// <code>
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public WatiNWebsite()
		///      {
		///        LogonDialogHandler logon = new LogonDialogHandler("username", "password");
		///        IE ie = new IE("http://watin.sourceforge.net", logon);
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE(string url, LogonDialogHandler logonDialogHandler, bool createInNewProcess)
		{
			CreateNewIEAndGoToUri(UtilityClass.CreateUri(url), logonDialogHandler, createInNewProcess);
		}

		/// <summary>
		/// Opens a new Internet Explorer and navigates to the given <paramref name="uri"/>.
		/// </summary>
		/// <param name="uri">The Uri te open</param>
		/// <param name="logonDialogHandler">A <see cref="LogonDialogHandler"/> class instanciated with the logon credentials.</param>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge leaving the created Internet Explorer open.
		/// <code>
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public WatiNWebsite()
		///      {
		///        LogonDialogHandler logon = new LogonDialogHandler("username", "password");
		///        IE ie = new IE(new Uri("http://watin.sourceforge.net"), logon);
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE(Uri uri, LogonDialogHandler logonDialogHandler)
		{
			CreateNewIEAndGoToUri(uri, logonDialogHandler, false);
		}

		/// <summary>
		/// Opens a new Internet Explorer and navigates to the given <paramref name="uri"/>.
		/// </summary>
		/// <param name="uri">The Uri te open</param>
		/// <param name="logonDialogHandler">A <see cref="LogonDialogHandler"/> class instanciated with the logon credentials.</param>
		/// <param name="createInNewProcess">if set to <c>true</c> the IE instance is created in a new process.</param>
		/// <remarks>
		/// You could also use one of the overloaded constructors.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge leaving the created Internet Explorer open.
		/// <code>
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public WatiNWebsite()
		///      {
		///        LogonDialogHandler logon = new LogonDialogHandler("username", "password");
		///        IE ie = new IE(new Uri("http://watin.sourceforge.net"), logon);
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public IE(Uri uri, LogonDialogHandler logonDialogHandler, bool createInNewProcess)
		{
			CreateNewIEAndGoToUri(uri, logonDialogHandler, createInNewProcess);
		}

        /// <summary>
        /// (Re)Use existing <see cref="IEBrowser"/> object. 
        /// </summary>
        /// <param name="ieBrowser">An object implementing <see cref="IEBrowser"/>.</param>
        public IE(IEBrowser ieBrowser)
        {
            _ieBrowser = ieBrowser;
        }

		/// <summary>
		/// Use existing InternetExplorer object. The param is of type
		/// object because otherwise all projects using WatiN should also
		/// reference the Interop.SHDocVw assembly.
		/// </summary>
		/// <param name="iwebBrowser2">An object implementing IWebBrowser2 (like Interop.SHDocVw.InternetExplorer object)</param>
		public IE(object iwebBrowser2)
            : this(iwebBrowser2, true)
		{
		}

	    protected internal IE(object iwebBrowser2, bool finishInitialization)
        {
            CheckThreadApartmentStateIsSTA();

            var internetExplorer = iwebBrowser2 as IWebBrowser2;

            if (internetExplorer == null)
            {
                throw new ArgumentException("iwebBrowser2 needs to implement shdocvw.IWebBrowser2");
            }

            CreateIEBrowser(internetExplorer);

            if (finishInitialization)
                FinishInitialization(null);
        }

	    private void CreateIEBrowser(IWebBrowser2 IWebBrowser2Instance)
	    {
	        _ieBrowser = new IEBrowser(IWebBrowser2Instance);
	    }

	    private void CreateNewIEAndGoToUri(Uri uri, IDialogHandler logonDialogHandler, bool createInNewProcess)
		{
			CheckThreadApartmentStateIsSTA();

			UtilityClass.MoveMousePoinerToTopLeft(Settings.AutoMoveMousePointerToTopLeft);

			if (createInNewProcess)
			{
				Logger.LogAction("Creating IE instance in a new process");

                _ieBrowser = CreateIEPartiallyInitializedInNewProcess();
                FinishInitialization(uri);
			}
			else
			{
				Logger.LogAction("Creating IE instance");

                CreateIEBrowser(new InternetExplorerClass());
                FinishInitialization(uri);
			}

			if (logonDialogHandler != null)
			{
				// remove other logon dialog handlers since only one handler
				// can effectively handle the logon dialog.
				DialogWatcher.RemoveAll(new LogonDialogHandler("a", "b"));

				// Add the (new) logonHandler
				DialogWatcher.Add(logonDialogHandler);
			}

			WaitForComplete();
		}

		private static IEBrowser CreateIEPartiallyInitializedInNewProcess()
		{
			var m_Proc = CreateIExploreInNewProcess();
		    var helper = new AttachToIeHelper();

		    var action = new TryFuncUntilTimeOut(Settings.AttachToIETimeOut) { SleepTime = 500 };
            var ie = action.Try(() =>
            {
                m_Proc.Refresh();
                var mainWindowHandle = m_Proc.MainWindowHandle;

                return mainWindowHandle != IntPtr.Zero
                    ? helper.FindIEPartiallyInitialized(new AttributeConstraint("hwnd", mainWindowHandle.ToString()))
                    : null;
            });

            if (ie != null) return ie._ieBrowser;

			throw new IENotFoundException("Timeout while waiting to attach to newly created instance of IE.", Settings.AttachToIETimeOut);
		}

	    private static Process CreateIExploreInNewProcess()
	    {
	        var m_Proc = Process.Start("IExplore.exe", "about:blank");
	        if (m_Proc == null)
	        {
	            throw new WatiNException("Could not start IExplore.exe process");
	        }

	        return m_Proc;
	    }

	    private static void CheckThreadApartmentStateIsSTA()
		{
            var isSTA = (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
			if (!isSTA)
			{
				throw new ThreadStateException("The CurrentThread needs to have it's ApartmentState set to ApartmentState.STA to be able to automate Internet Explorer.");
			}
		}

	    internal void FinishInitialization(Uri uri)
        {
            // Due to UAC in Vista the navigate has to be done
            // before showing the new Internet Explorer instance
            if (uri != null)
            {
                GoTo(uri);
            }
            _ieBrowser.Visible = Settings.MakeNewIeInstanceVisible;

            StartDialogWatcher();
        }



		/// <summary>
        /// Use this method to gain access to the IWebBrowser2 interface of Internet Explorer.
		/// Do this by referencing the Interop.SHDocVw assembly (supplied in the WatiN distribution)
        /// and cast the return value of this method to type SHDocVw.IWebBrowser2.
		/// </summary>
		public object InternetExplorer
		{
			get { return _ieBrowser.AsIWebBrowser2; }
		}

	    /// <summary>
		/// Closes the referenced Internet Explorer. Almost
		/// all other functionality in this class and the element classes will give
		/// exceptions when used after closing the browser.
		/// </summary>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge. 
		/// <code>
		/// using WatiN.Core;
		/// using System.Diagnostics;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public WatiNWebsite()
		///      {
		///        IE ie = new IE("http://watin.sourceforge.net");
		///        Debug.WriteLine(ie.Html);
		///        ie.Close;
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public override void Close()
		{
	        if (isDisposed) return;
	        
            if (IsInternetExplorerStillAvailable())
	        {
	            Logger.LogAction("Closing browser '" + Title + "'");
	        }

	        DisposeAndCloseIE(true);
		}

		/// <summary>
		/// Closes then reopens Internet Explorer and navigates to the given <paramref name="uri"/>.
		/// </summary>
		/// <param name="uri">The Uri to open</param>
		/// <param name="logonDialogHandler">A <see cref="LogonDialogHandler"/> class instanciated with the logon credentials.</param>
		/// <param name="createInNewProcess">if set to <c>true</c> the IE instance is created in a new process.</param>
		/// <remarks>
		/// You could also use one of the overloaded methods.
		/// </remarks>
		/// <example>
		/// The following example creates a new Internet Explorer instances and navigates to
		/// the WatiN Project website on SourceForge leaving the created Internet Explorer open.
		/// <code>
		/// using WatiN.Core;
		/// 
		/// namespace NewIEExample
		/// {
		///    public class WatiNWebsite
		///    {
		///      public WatiNWebsite()
		///      {
		///        LogonDialogHandler logon = new LogonDialogHandler("username", "password");
		///        IE ie = new IE(new Uri("http://watin.sourceforge.net"), logon);
		///        ie.Reopen();
		///      }
		///    }
		///  }
		/// </code>
		/// </example>
		public void Reopen(Uri uri, LogonDialogHandler logonDialogHandler, bool createInNewProcess)
		{
			Close();
			Recycle();
			CreateNewIEAndGoToUri(uri, logonDialogHandler, createInNewProcess);
		}

		protected override void Recycle()
		{
			base.Recycle();
			isDisposed = false;
		}

		private void DisposeAndCloseIE(bool closeIE)
		{
		    if (isDisposed) return;
		    
            if (closeIE && IsInternetExplorerStillAvailable())
		    {
		        // Close all open HTMLDialogs
		        HtmlDialogs.CloseAll();
		    }

		    base.Dispose(true);

		    if (closeIE && IsInternetExplorerStillAvailable())
		    {
		        // Ask IE to close
		        _ieBrowser.Quit();
		    }

		    _ieBrowser = null;

		    if (closeIE)
		    {
		        // Wait for IE to close to prevent RPC errors when creating
		        // a new WatiN.Core.IE instance.
		        Thread.Sleep(1000);
		    }

		    isDisposed = true;
		}

		/// <summary>
		/// Closes <i>all</i> running instances of Internet Explorer by killing the
		/// process these instances run in. 
		/// </summary>
		public virtual void ForceClose()
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("Internet Explorer", "The Internet Explorer instance is already disposed. ForceClose can't be performed.");
			}

			Logger.LogAction("Force closing all IE instances");

			var iePid = ProcessID;

			DisposeAndCloseIE(true);

			try
			{
				// Force Internet Explorer instances to close
				Process.GetProcessById(iePid).Kill();
			}
			catch (ArgumentException)
			{
				// ignore: IE is no longer running
			}
		}

		/// <summary>
		/// Clears all browser cookies.
		/// </summary>
		/// <remarks>
		/// Internet Explorer maintains an internal cookie cache that does not immediately
		/// expire when cookies are cleared.  This is the case even when the cookies are
		/// cleared using the Internet Options dialog.  If cookies have been used by
		/// the current browser session it may be necessary to <see cref="Browser.Reopen()" /> the
		/// browser to ensure the internal cookie cache is flushed.  Therefore it is
		/// recommended to clear cookies at the beginning of the test before navigating
		/// to any pages (other than "about:blank") to avoid having to reopen the browser.
		/// </remarks>
		/// <example>
		/// <code>
		/// // Clear cookies first.
		/// IE ie = new IE();
		/// ie.ClearCookies();
		/// 
		/// // Then go to the site and sign in.
		/// ie.GoTo("http://www.example.com/");
		/// ie.Link(Find.ByText("Sign In")).Click();
		/// </code>
		/// </example>
        /// <seealso cref="Browser.Reopen()"/>
		public void ClearCookies()
		{
			Logger.LogAction("Clearing cookies for all sites.");

			WinInet.ClearCookies(null);
		}

		/// <summary>
		/// Clears the browser cookies associated with a particular site and to
		/// any of the site's subdomains.
		/// </summary>
		/// <remarks>
		/// Internet Explorer maintains an internal cookie cache that does not immediately
		/// expire when cookies are cleared.  This is the case even when the cookies are
		/// cleared using the Internet Options dialog.  If cookies have been used by
        /// the current browser session it may be necessary to <see cref="Browser.Reopen()" /> the
		/// browser to ensure the internal cookie cache is flushed.  Therefore it is
		/// recommended to clear cookies at the beginning of the test before navigating
		/// to any pages (other than "about:blank") to avoid having to reopen the browser.
		/// </remarks>
		/// <param name="url">The site url associated with the cookie.</param>
		/// <example>
		/// <code>
		/// // Clear cookies first.
		/// IE ie = new IE();
		/// ie.ClearCookies("http://www.example.com/");
		/// 
		/// // Then go to the site and sign in.
		/// ie.GoTo("http://www.example.com/");
		/// ie.Link(Find.ByText("Sign In")).Click();
		/// </code>
		/// </example>
        /// <seealso cref="Browser.Reopen()"/>
		public void ClearCookies(string url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			Logger.LogAction(String.Format("Clearing cookies for site '{0}'.", url));

			WinInet.ClearCookies(url);
		}

		/// <summary>
		/// Clears the browser cache but leaves cookies alone.
		/// </summary>
		/// <example>
		/// <code>
		/// // Clear the cache and cookies.
		/// IE ie = new IE();
		/// ie.ClearCache();
		/// ie.ClearCookies();
		/// 
		/// // Then go to the site and sign in.
		/// ie.GoTo("http://www.example.com/");
		/// ie.Link(Find.ByText("Sign In")).Click();
		/// </code>
		/// </example>
        /// <seealso cref="Browser.Reopen()"/>
		public void ClearCache()
		{
			Logger.LogAction("Clearing browser cache.");

			WinInet.ClearCache();
		}

		/// <summary>
		/// Gets the value of a cookie.
		/// </summary>
		/// <remarks>
		/// This method cannot retrieve the value of cookies protected by the <c>httponly</c> security option.
		/// </remarks>
		/// <param name="url">The site url associated with the cookie.</param>
		/// <param name="cookieName">The cookie name.</param>
		/// <returns>The cookie data of the form:
		/// &lt;name&gt;=&lt;value&gt;[; &lt;name&gt;=&lt;value&gt;]...
		/// [; expires=&lt;date:DAY, DD-MMM-YYYY HH:MM:SS GMT&gt;][; domain=&lt;domain_name&gt;]
		/// [; path=&lt;some_path&gt;][; secure][; httponly].  Returns null if there are no associated cookies.</returns>
		/// <seealso cref="ClearCookies()"/>
		/// <seealso cref="SetCookie(string,string)"/>
		public string GetCookie(string url, string cookieName)
		{
			return WinInet.GetCookie(url, cookieName);
		}

        public System.Net.CookieContainer GetCookieContainerForUrl(Uri url)
        {
            return WinInet.GetCookieContainerForUrl(url);
        }

        public System.Net.CookieCollection GetCookiesForUrl(Uri url)
        {
            return WinInet.GetCookiesForUrl(url);
        }

		/// <summary>
		/// Sets the value of a cookie.
		/// </summary>
		/// <remarks>
		/// If no expiration date is specified, the cookie expires when the session ends.
		/// </remarks>
		/// <param name="url">The site url associated with the cookie.</param>
		/// <param name="cookieData">The cookie data of the form:
		/// &lt;name&gt;=&lt;value&gt;[; &lt;name&gt;=&lt;value&gt;]...
		/// [; expires=&lt;date:DAY, DD-MMM-YYYY HH:MM:SS GMT&gt;][; domain=&lt;domain_name&gt;]
		/// [; path=&lt;some_path&gt;][; secure][; httponly].</param>
		/// <seealso cref="ClearCookies()"/>
		/// <seealso cref="GetCookie(string,string)"/>
		public void SetCookie(string url, string cookieData)
		{
			WinInet.SetCookie(url, cookieData);
		}

		private bool IsInternetExplorerStillAvailable()
		{
		    // Call a property of the
            // ie instance to see of it isn't disposed by 
            // another IE instance.
		    return UtilityClass.TryFuncIgnoreException(() => hWnd != IntPtr.Zero);
		}


		/// <summary>
		/// Waits till the webpage, it's frames and all it's elements are loaded. This
		/// function is called by WatiN after each action (like clicking a link) so you
		/// should have to use this function on rare occasions.
		/// </summary>
        /// <param name="waitForCompleteTimeOut">The number of seconds to wait before timing out</param>
        public override void WaitForComplete(int waitForCompleteTimeOut)
		{
			WaitForComplete(new IEWaitForComplete(this, waitForCompleteTimeOut));
		}

		#region IDisposable Members

		/// <summary>
		/// This method must be called by its inheritor to dispose references
		/// to internal resources.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			DisposeAndCloseIE(AutoClose);
		}

		#endregion

		/// <summary>
		/// Gets or sets a value indicating whether to auto close IE after destroying
		/// a reference to the corresponding IE instance.
		/// </summary>
		/// <value><c>true</c> when to auto close IE (this is the default); otherwise, <c>false</c>.</value>
		public bool AutoClose
		{
			get { return autoClose; }
			set { autoClose = value; }
		}

		/// <summary>
		/// Returns a collection of open HTML dialogs (modal as well as modeless).
		/// </summary>
		/// <value>The HTML dialogs.</value>
		public HtmlDialogCollection HtmlDialogs
		{
			get { return GetHtmlDialogs(true); }
		}

		/// <summary>
		/// Returns a collection of open HTML dialogs (modal as well as modeless).
		/// When itterating through this collection WaitForComplete will not be
		/// called on a HTML dialog before returning it from the collection.
		/// </summary>
		/// <value>The HTML dialogs.</value>
		public HtmlDialogCollection HtmlDialogsNoWait
		{
			get { return GetHtmlDialogs(false); }
		}

		private HtmlDialogCollection GetHtmlDialogs(bool waitForComplete)
		{
		    return new HtmlDialogCollection(hWnd, waitForComplete);
		}

        public override INativeBrowser NativeBrowser
        {
           get { return _ieBrowser; }
        }

		/// <summary>
		/// Find a HtmlDialog by an attribute. Currently 
		/// Find.ByUrl and Find.ByTitle are supported.
		/// </summary>
		/// <param name="findBy">The url of the html page shown in the dialog</param>
		public HtmlDialog HtmlDialog(Constraint findBy)
		{
			return FindHtmlDialog(findBy, Settings.AttachToIETimeOut);
		}

		/// <summary>
		/// Find a HtmlDialog by an attribute within the given <paramref name="timeout" /> period.
		/// Currently Find.ByUrl and Find.ByTitle are supported.
		/// </summary>
		/// <param name="findBy">The url of the html page shown in the dialog</param>
		/// <param name="timeout">Number of seconds before the search times out.</param>
		public HtmlDialog HtmlDialog(Constraint findBy, int timeout)
		{
			return FindHtmlDialog(findBy, timeout);
		}

	    public bool Visible
	    {
            get { return _ieBrowser.Visible; }
            set { _ieBrowser.Visible = value; }
	    }

		private HtmlDialog FindHtmlDialog(Constraint findBy, int timeout)
		{
			Logger.LogAction("Busy finding HTMLDialog matching criteria: " + findBy);

		    var action = new TryFuncUntilTimeOut(timeout){SleepTime = 500};
            var result = action.Try(() => HtmlDialogs.First(findBy));
            
            if (result == null)
            {
                throw new HtmlDialogNotFoundException(findBy.ToString(), timeout);
            }
            
            return result;
		}
    }
}