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
using System.Runtime.InteropServices;
using System.Text;
using mshtml;
using SHDocVw;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// Class that contains native win32 API support.
	/// </summary>
	public class NativeMethods
	{
		private static string enumChildWindowClassName;

		#region Constants

		internal const int WM_SYSCOMMAND = 0x0112;
		internal const int WM_CLOSE = 0x0010;
		internal const int SC_CLOSE = 0xF060;

		internal const int KEYEVENTF_EXTENDEDKEY = 0x1;
		internal const int KEYEVENTF_KEYUP = 0x2;
		internal const int KEYEVENTF_TAB = 0x09;

		public const Int32 SMTO_ABORTIFHUNG = 2;
		internal const int BM_CLICK = 245;
		internal const int WM_ACTIVATE = 6;
		internal const int MA_ACTIVATE = 1;

		public const int GW_CHILD = 5;
		public const int GW_HWNDNEXT = 2;

		#endregion Constants

		#region Structs

		[StructLayout(LayoutKind.Sequential)]
		internal struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public int showCmd;
			public POINT ptMinPosition;
			public POINT ptMaxPosition;
			public RECT rcNormalPosition;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct POINT
		{
			public int X;
			public int Y;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct WINDOWINFO
		{
			public uint cbSize;
			public RECT rcWindow;
			public RECT rcClient;
			public uint dwStyle;
			public uint dwExStyle;
			public uint dwWindowStatus;
			public uint cxWindowBorders;
			public uint cyWindowBorders;
			public ushort atomWindowType;
			public ushort wCreatorVersion;
		}

		#endregion Structs

		public delegate bool EnumThreadProc(IntPtr hwnd, IntPtr lParam);

		internal delegate bool EnumChildProc(IntPtr hWnd, ref IntPtr lParam);

		#region Enums

		/// <summary>
		/// Enumeration of the different ways of showing a window using 
		/// ShowWindow
		/// </summary>
		public enum WindowShowStyle : int
		{
			/// <summary>Hides the window and activates another window.</summary>
			/// <remarks>See SW_HIDE</remarks>
			Hide = 0,
			/// <summary>Activates and displays a window. If the window is minimized 
			/// or maximized, the system restores it to its original size and 
			/// position. An application should specify this flag when displaying 
			/// the window for the first time.</summary>
			/// <remarks>See SW_SHOWNORMAL</remarks>
			ShowNormal = 1,
			/// <summary>Activates the window and displays it as a minimized window.</summary>
			/// <remarks>See SW_SHOWMINIMIZED</remarks>
			ShowMinimized = 2,
			/// <summary>Activates the window and displays it as a maximized window.</summary>
			/// <remarks>See SW_SHOWMAXIMIZED</remarks>
			ShowMaximized = 3,
			/// <summary>Maximizes the specified window.</summary>
			/// <remarks>See SW_MAXIMIZE</remarks>
			Maximize = 3,
			/// <summary>Displays a window in its most recent size and position. 
			/// This value is similar to "ShowNormal", except the window is not 
			/// actived.</summary>
			/// <remarks>See SW_SHOWNOACTIVATE</remarks>
			ShowNormalNoActivate = 4,
			/// <summary>Activates the window and displays it in its current size 
			/// and position.</summary>
			/// <remarks>See SW_SHOW</remarks>
			Show = 5,
			/// <summary>Minimizes the specified window and activates the next 
			/// top-level window in the Z order.</summary>
			/// <remarks>See SW_MINIMIZE</remarks>
			Minimize = 6,
			/// <summary>Displays the window as a minimized window. This value is 
			/// similar to "ShowMinimized", except the window is not activated.</summary>
			/// <remarks>See SW_SHOWMINNOACTIVE</remarks>
			ShowMinNoActivate = 7,
			/// <summary>Displays the window in its current size and position. This 
			/// value is similar to "Show", except the window is not activated.</summary>
			/// <remarks>See SW_SHOWNA</remarks>
			ShowNoActivate = 8,
			/// <summary>Activates and displays the window. If the window is 
			/// minimized or maximized, the system restores it to its original size 
			/// and position. An application should specify this flag when restoring 
			/// a minimized window.</summary>
			/// <remarks>See SW_RESTORE</remarks>
			Restore = 9,
			/// <summary>Sets the show state based on the SW_ value specified in the 
			/// STARTUPINFO structure passed to the CreateProcess function by the 
			/// program that started the application.</summary>
			/// <remarks>See SW_SHOWDEFAULT</remarks>
			ShowDefault = 10,
			/// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
			/// that owns the window is hung. This flag should only be used when 
			/// minimizing windows from a different thread.</summary>
			/// <remarks>See SW_FORCEMINIMIZE</remarks>
			ForceMinimized = 11
		}

		[Flags()]
		internal enum tagOLECONTF
		{
			OLECONTF_EMBEDDINGS = 1,
			OLECONTF_LINKS = 2,
			OLECONTF_OTHERS = 4,
			OLECONTF_ONLYUSER = 8,
			OLECONTF_ONLYIFRUNNING = 16,
		}

		#endregion Enums

		/// <summary>
		/// Prevent creating an instance of this class (contains only static members)
		/// </summary>
		private NativeMethods() {}

		#region DllImport User32

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern bool EnumThreadWindows(int threadId, EnumThreadProc pfnEnum, IntPtr lParam);

		[DllImport("user32", EntryPoint = "GetClassNameA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		internal static extern int GetClassName(IntPtr handleToWindow, StringBuilder className, int maxClassNameLength);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

		[DllImport("user32.dll", SetLastError=true)]
		internal static extern IntPtr SetActiveWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		internal static extern IntPtr GetDlgItem(IntPtr handleToWindow, int ControlId);

		/// <summary>
		/// The GetForegroundWindow function returns a handle to the foreground window.
		/// </summary>
		[DllImport("user32.dll", SetLastError=true)]
		internal static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
		public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

		[DllImport("user32.dll")]
		internal static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

		[DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		internal static extern int GetWindowText(IntPtr handleToWindow, StringBuilder windowText, int maxTextLength);

		[DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		internal static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
		internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		internal static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool IsWindowEnabled(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		internal static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll", SetLastError=true)]
		[return : MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		internal static extern Int32 EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, ref IntPtr lParam);

		[DllImport("user32", EntryPoint = "RegisterWindowMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern Int32 RegisterWindowMessage(string lpString);

		[DllImport("user32", EntryPoint = "SendMessageTimeoutA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern Int32 SendMessageTimeout(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam, Int32 fuFlags, Int32 uTimeout, ref Int32 lpdwResult);

		[DllImport("user32.dll")]
		internal static extern bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

		[DllImport("user32.dll")]
		internal static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

		/// <summary>Shows a Window</summary>
		/// <remarks>
		/// <para>To perform certain special effects when showing or hiding a 
		/// window, use AnimateWindow.</para>
		///<para>The first time an application calls ShowWindow, it should use 
		///the WinMain function's nCmdShow parameter as its nCmdShow parameter. 
		///Subsequent calls to ShowWindow must use one of the values in the 
		///given list, instead of the one specified by the WinMain function's 
		///nCmdShow parameter.</para>
		///<para>As noted in the discussion of the nCmdShow parameter, the 
		///nCmdShow value is ignored in the first call to ShowWindow if the 
		///program that launched the application specifies startup information 
		///in the structure. In this case, ShowWindow uses the information 
		///specified in the STARTUPINFO structure to show the window. On 
		///subsequent calls, the application must call ShowWindow with nCmdShow 
		///set to SW_SHOWDEFAULT to use the startup information provided by the 
		///program that launched the application. This behavior is designed for 
		///the following situations: </para>
		///<list type="">
		///    <item>Applications create their main window by calling CreateWindow 
		///    with the WS_VISIBLE flag set. </item>
		///    <item>Applications create their main window by calling CreateWindow 
		///    with the WS_VISIBLE flag cleared, and later call ShowWindow with the 
		///    SW_SHOW flag set to make it visible.</item>
		///</list></remarks>
		/// <param name="hWnd">Handle to the window.</param>
		/// <param name="nCmdShow">Specifies how the window is to be shown. 
		/// This parameter is ignored the first time an application calls 
		/// ShowWindow, if the program that launched the application provides a 
		/// STARTUPINFO structure. Otherwise, the first time ShowWindow is called, 
		/// the value should be the value obtained by the WinMain function in its 
		/// nCmdShow parameter. In subsequent calls, this parameter can be one of 
		/// the WindowShowStyle members.</param>
		/// <returns>
		/// If the window was previously visible, the return value is nonzero. 
		/// If the window was previously hidden, the return value is zero.
		/// </returns>
		[DllImport("user32.dll")]
		internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("User32.dll")]
		internal static extern int GetWindowThreadProcessId(IntPtr window, out int processId);

		[DllImport("User32.dll")]
		internal static extern IntPtr GetParent(IntPtr hwnd);

		#endregion DllImport User32

		[DllImport("oleacc", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern Int32 ObjectFromLresult(Int32 lResult, ref Guid riid, Int32 wParam, ref IHTMLDocument2 ppvObject);

		[DllImport("kernel32")]
		internal static extern int GetCurrentThreadId();

		[DllImport("gdi32.dll")]
		internal static extern bool DeleteObject(IntPtr hObject);

		#region ComImport Interfaces

		[ComImport, Guid("00000100-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IEnumUnknown
		{
			[PreserveSig]
			int Next(
				[In, MarshalAs(UnmanagedType.U4)] int celt,
				[Out, MarshalAs(UnmanagedType.IUnknown)] out object rgelt,
				[Out, MarshalAs(UnmanagedType.U4)] out int pceltFetched
				);

			[PreserveSig]
			int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);

			void Reset();

			void Clone(out IEnumUnknown ppenum);
		}

		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000011B-0000-0000-C000-000000000046")]
		internal interface IOleContainer
		{
			[PreserveSig]
			int ParseDisplayName(
				[In, MarshalAs(UnmanagedType.Interface)] object pbc,
				[In, MarshalAs(UnmanagedType.BStr)] string pszDisplayName,
				[Out, MarshalAs(UnmanagedType.LPArray)] int[] pchEaten,
				[Out, MarshalAs(UnmanagedType.LPArray)] object[] ppmkOut
				);

			[PreserveSig]
			int EnumObjects(
				[In, MarshalAs(UnmanagedType.U4)] tagOLECONTF grfFlags,
				out IEnumUnknown ppenum
				);

			[PreserveSig]
			int LockContainer(bool fLock);
		}

		#endregion ComImport Interfaces

		#region Methods

		internal static void EnumIWebBrowser2Interfaces(IWebBrowser2Processor processor)
		{
			IOleContainer oc = processor.HTMLDocument() as IOleContainer;

			if (oc != null)
			{
				IEnumUnknown eu;
				int hr = oc.EnumObjects(tagOLECONTF.OLECONTF_EMBEDDINGS, out eu);
				Marshal.ThrowExceptionForHR(hr);

				if (eu != null)
				{
					try
					{
						object pUnk;
						int fetched;
						const int MAX_FETCH_COUNT = 1;

						// get the first embedded object
						// pUnk alloc
						hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
						Marshal.ThrowExceptionForHR(hr);

						while (hr == 0)
						{
							// Query Interface pUnk for the IWebBrowser2 interface
							IWebBrowser2 brow = pUnk as IWebBrowser2;

							try
							{
								if (brow != null)
								{
									processor.Process(brow);
									if (!processor.Continue())
									{
										break;
									}
									// free brow
									ReleaseComObjectButIgnoreNull(brow);
								}
							}
							catch
							{
								// free brow
								ReleaseComObjectButIgnoreNull(brow);
								ReleaseComObjectButIgnoreNull(pUnk);
							}

							// pUnk free
							ReleaseComObjectButIgnoreNull(pUnk);

							// get the next embedded object
							// pUnk alloc
							hr = eu.Next(MAX_FETCH_COUNT, out pUnk, out fetched);
							Marshal.ThrowExceptionForHR(hr);
						}
					}
					finally
					{
						// eu free
						ReleaseComObjectButIgnoreNull(eu);
					}
				}
			}
		}

		public static void ReleaseComObjectButIgnoreNull(object comObject)
		{
			if (comObject != null)
			{
				Marshal.ReleaseComObject(comObject);
			}
		}

		/// <summary>
		/// This method incapsulates all the details of getting
		/// the full length text in a StringBuffer and returns
		/// the StringBuffer contents as a string.
		/// </summary>
		/// <param name="hwnd">The handle to the window</param>
		/// <returns>Text of the window</returns>
		internal static string GetWindowText(IntPtr hwnd)
		{
			int length = GetWindowTextLength(hwnd) + 1;
			StringBuilder buffer = new StringBuilder(length);
			GetWindowText(hwnd, buffer, length);

			return buffer.ToString();
		}

		/// <summary>
		/// This method incapsulates all the details of getting
		/// the full length classname in a StringBuffer and returns
		/// the StringBuffer contents as a string.
		/// </summary>
		/// <param name="hwnd">The handle to the window</param>
		/// <returns>Text of the window</returns>
		public static string GetClassName(IntPtr hwnd)
		{
			const int maxCapacity = 255;

			StringBuilder className = new StringBuilder(maxCapacity);

			Int32 lRes = GetClassName(hwnd, className, maxCapacity);
			if (lRes == 0)
			{
				return String.Empty;
			}

			return className.ToString();
		}

		internal static Int64 GetWindowStyle(IntPtr hwnd)
		{
			WINDOWINFO info = new WINDOWINFO();
			info.cbSize = (uint) Marshal.SizeOf(info);
			GetWindowInfo(hwnd, ref info);

			return Convert.ToInt64(info.dwStyle);
		}

		internal static void ClickDialogButton(int buttonid, IntPtr parentHwnd)
		{
			IntPtr buttonPtr = GetDlgItem(parentHwnd, buttonid);
			SendMessage(buttonPtr, BM_CLICK, 0, 0);
		}

		public static IntPtr GetChildWindowHwnd(IntPtr parentHwnd, string className)
		{
			IntPtr hWnd = IntPtr.Zero;
			enumChildWindowClassName = className;

			// Go throught the child windows of the dialog window
			EnumChildProc childProc = new EnumChildProc(EnumChildWindows);
			EnumChildWindows(parentHwnd, childProc, ref hWnd);

			// If a logon dialog window is found hWnd will be set.
			return hWnd;
		}

		private static bool EnumChildWindows(IntPtr hWnd, ref IntPtr lParam)
		{
			if (UtilityClass.CompareClassNames(hWnd, enumChildWindowClassName))
			{
				lParam = hWnd;
				return false;
			}

			return true;
		}

		#endregion Methods
	}
}

namespace WatiN.Core.Interfaces
{
	internal interface IWebBrowser2Processor
	{
		HTMLDocument HTMLDocument();
		void Process(IWebBrowser2 webBrowser2);
		bool Continue();
	}
}