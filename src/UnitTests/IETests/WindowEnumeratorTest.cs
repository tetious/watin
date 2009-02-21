using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using mshtml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SHDocVw;
using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;
using WatiN.Core.Native;

[TestFixture]
public class EnumTester
{
    [Test]
    public void WhatShouldBeTested()
    {
        // GIVEN
        var enumerator = new WindowsEnumerator();
        var ies = new List<IE>();

        var topLevelWindows = enumerator.GetTopLevelWindows("IEFrame");
        foreach (var window in topLevelWindows)
        {
            foreach (var childWindow in enumerator.GetChildWindows((int) window.Hwnd, "TabWindowClass"))
            {
                var document2 = IEUtils.IEDOMFromhWnd(childWindow.Hwnd);
                Assert.That(document2, Is.Not.Null, "Oeps");

                var window1 = document2.parentWindow;
                                
                var browser2 = YetAnotherSolution(window1);
                ies.Add(new IE(browser2));
            }
        }

        foreach (var ie in ies)
        {
            Console.WriteLine(ie.Title);
        }

        // WHEN

        // THEN
    }

    public IWebBrowser2 YetAnotherSolution(IHTMLWindow2 site)
    {
        var SID_STopLevelBrowser = new Guid(0x4C96BE40, 0x915C, 0x11CF, 0x99, 0xD3, 0x00, 0xAA, 0x00, 0x4A, 0xE8, 0x37);
        var SID_SWebBrowserApp = new Guid(0x0002DF05, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        var guidIServiceProvider = typeof(IServiceProvider).GUID;

        var serviceProvider = site as IServiceProvider;
        if (serviceProvider == null) return null;

        object objIServiceProvider;
        serviceProvider.QueryService(ref SID_STopLevelBrowser, ref guidIServiceProvider, out objIServiceProvider);
        
        serviceProvider = objIServiceProvider as IServiceProvider;
        if (serviceProvider == null) return null;
        
        object objIWebBrowser;
        var guidIWebBrowser = typeof(IWebBrowser2).GUID;
        serviceProvider.QueryService(ref SID_SWebBrowserApp, ref guidIWebBrowser, out objIWebBrowser);
        var webBrowser = objIWebBrowser as IWebBrowser2;
        
        return webBrowser;
    }

    //    Provide the IServiceProvider interface definition as below:
    [ComVisible(true), ComImport,
    Guid("6d5140c1-7436-11ce-8034-00aa006009fa"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        uint QueryService(
        ref Guid guidService,
        ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)]out object ppvObject);
    }
}

/// <summary> 
/// Enumerate top-level and child windows 
/// </summary> 
/// <example> 
/// Dim enumerator As New WindowsEnumerator() 
/// For Each top As ApiWindow in enumerator.GetTopLevelWindows() 
/// Console.WriteLine(top.MainWindowTitle) 
/// For Each child As ApiWindow child in enumerator.GetChildWindows(top.hWnd) 
/// Console.WriteLine(" " + child.MainWindowTitle) 
/// Next child 
/// Next top 
/// </example> 
public class WindowsEnumerator
{

    private delegate int EnumCallBackDelegate(int hwnd, int lParam);
    [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int EnumWindows(EnumCallBackDelegate lpEnumFunc, int lParam);
    [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int EnumChildWindows(int hWndParent, EnumCallBackDelegate lpEnumFunc, int lParam);
    [DllImport("user32", EntryPoint = "GetClassNameA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int GetClassName(int hwnd, StringBuilder lpClassName, int nMaxCount);
    [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int IsWindowVisible(int hwnd);
    [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int GetParent(int hwnd);
    [DllImport("user32", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern Int32 SendMessage(Int32 hwnd, Int32 wMsg, Int32 wParam, Int32 lParam);
    [DllImport("user32", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern Int32 SendMessage(Int32 hwnd, Int32 wMsg, Int32 wParam, StringBuilder lParam);

    // Top-level windows. 

    // Child windows. 

    // Get the window class. 

    // Test if the window is visible--only get visible ones. 

    // Test if the window's parent--only get the one's without parents. 

    // Get window text length signature. 

    // Get window text signature. 

    private List<Window> _listChildren = new List<Window>();
    private readonly List<Window> _listTopLevel = new List<Window>();

    private string _topLevelClass = "";
    private string _childClass = "";

    /// <summary> 
    /// Get all top-level window information 
    /// </summary> 
    /// <returns>List of window information objects</returns> 
    public List<Window> GetTopLevelWindows()
    {

        EnumWindows(EnumWindowProc, 0x0);

        return _listTopLevel;

    }

    public List<Window> GetTopLevelWindows(string className)
    {

        _topLevelClass = className;

        return GetTopLevelWindows();

    }

    /// <summary> 
    /// Get all child windows for the specific windows handle (hwnd). 
    /// </summary> 
    /// <returns>List of child windows for parent window</returns> 
    public List<Window> GetChildWindows(Int32 hwnd)
    {

        // Clear the window list. 
        _listChildren = new List<Window>();

        // Start the enumeration process. 
        EnumChildWindows(hwnd, EnumChildWindowProc, 0x0);

        // Return the children list when the process is completed. 
        return _listChildren;

    }

    public List<Window> GetChildWindows(Int32 hwnd, string childClass)
    {

        // Set the search 
        _childClass = childClass;

        return GetChildWindows(hwnd);

    }

    /// <summary> 
    /// Callback function that does the work of enumerating top-level windows. 
    /// </summary> 
    /// <param name="hwnd">Discovered Window handle</param> 
    /// <param name="lParam"></param>
    /// <returns>1=keep going, 0=stop</returns> 
    private Int32 EnumWindowProc(Int32 hwnd, Int32 lParam)
    {

        // Eliminate windows that are not top-level. 
        if (GetParent(hwnd) == 0 && IsWindowVisible(hwnd) == 1)
        {

            // Get the window title / class name. 
            var window = new Window((IntPtr) hwnd);

            // Match the class name if searching for a specific window class. 
            if (_topLevelClass.Length == 0 || window.ClassName.ToLower() == _topLevelClass.ToLower())
            {
                _listTopLevel.Add(window);
            }
        }


        // To continue enumeration, return True (1), and to stop enumeration 
        // return False (0). 
        // When 1 is returned, enumeration continues until there are no 
        // more windows left. 

        return 1;

    }

    /// <summary> 
    /// Callback function that does the work of enumerating child windows. 
    /// </summary> 
    /// <param name="hwnd">Discovered Window handle</param> 
    /// <param name="lParam"></param>
    /// <returns>1=keep going, 0=stop</returns> 
    private Int32 EnumChildWindowProc(Int32 hwnd, Int32 lParam)
    {

        var window = new Window((IntPtr) hwnd);

        // Attempt to match the child class, if one was specified, otherwise 
        // enumerate all the child windows. 
        if (_childClass.Length == 0 || window.ClassName.ToLower() == _childClass.ToLower())
        {
            _listChildren.Add(window);
        }

        return 1;

    }

//    /// <summary> 
//    /// Build the ApiWindow object to hold information about the Window object. 
//    /// </summary> 
//    private static Window GetWindowIdentification(int hwnd)
//    {
//
//        const Int32 WM_GETTEXT = 0xd;
//        const Int32 WM_GETTEXTLENGTH = 0xe;
//
//        var window = new Window();
//
//        var title = new StringBuilder();
//
//        // Get the size of the string required to hold the window title. 
//        var size = SendMessage(hwnd, WM_GETTEXTLENGTH, 0, 0);
//
//        // If the return is 0, there is no title. 
//        if (size > 0)
//        {
//            title = new StringBuilder(size + 1);
//
//            SendMessage(hwnd, WM_GETTEXT, title.Capacity, title);
//        }
//
//        // Get the class name for the window. 
//        var classBuilder = new StringBuilder(64);
//        GetClassName(hwnd, classBuilder, 64);
//
//        // Set the properties for the ApiWindow object. 
//        window.ClassName = classBuilder.ToString();
//        window.MainWindowTitle = title.ToString();
//        window.hWnd = hwnd;
//
//        return window;
//
//    }
}