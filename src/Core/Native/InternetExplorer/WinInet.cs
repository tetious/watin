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
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using WatiN.Core.Logging;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace WatiN.Core.Native.InternetExplorer
{
    /// <summary>
    /// Provides low-level support for manipulating cookies, clearing caches and
    /// setting internet options.
    /// </summary>
    /// <remarks author="jeff.brown">
    /// This cookie clearing code is based on the sample code from the following MS KB article:
    /// http://support.microsoft.com/default.aspx?scid=kb;EN-US;326201
    /// Beware, the code presented in that article is somewhat buggy so it has been
    /// completely rewritten here.
    /// </remarks>
    internal class WinInet
    {
        private delegate void CacheGroupAction(long groupId);

        private delegate void CacheEntryAction(INTERNET_CACHE_ENTRY_INFO cacheEntry);

        #region Constants

        private const int NORMAL_CACHE_ENTRY = 0x1;
        private const int EDITED_CACHE_ENTRY = 0x8;
        private const int TRACK_OFFLINE_CACHE_ENTRY = 0x10;
        private const int TRACK_ONLINE_CACHE_ENTRY = 0x20;
        private const int STICKY_CACHE_ENTRY = 0x40;
        private const int SPARSE_CACHE_ENTRY = 0x10000;
        private const int COOKIE_CACHE_ENTRY = 0x100000;
        private const int URLHISTORY_CACHE_ENTRY = 0x200000;

        // Indicates that all of the cache groups in the user's system should be enumerated
        private const int CACHEGROUP_SEARCH_ALL = 0x0;
        // Indicates that all the cache entries that are associated with the cache group
        // should be deleted, unless the entry belongs to another cache group.
        private const int CACHEGROUP_FLAG_FLUSHURL_ONDELETE = 0x2;

        private const int ERROR_FILE_NOT_FOUND = 0x2;
        private const int ERROR_NO_MORE_ITEMS = 259;
        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        public enum InternetCookieState
        {
            COOKIE_STATE_UNKNOWN = 0x0,
            COOKIE_STATE_ACCEPT = 0x1,
            COOKIE_STATE_PROMPT = 0x2,
            COOKIE_STATE_LEASH = 0x3,
            COOKIE_STATE_DOWNGRADE = 0x4,
            COOKIE_STATE_REJECT = 0x5
        }

        #endregion

        private WinInet() {}

        private static string RetrieveIECookiesForUrl(string url)
        {
            var cookieHeader = new StringBuilder(new String(' ', 256), 256);
            int datasize = cookieHeader.Length;
            if (!InternetGetCookie(url, null, cookieHeader, ref datasize))
            {
                if (datasize < 0)
                    return String.Empty;
                cookieHeader = new StringBuilder(datasize);
                InternetGetCookie(url, null, cookieHeader, ref datasize);
            }
            return cookieHeader.ToString();
        }

        public static CookieContainer GetCookieContainerForUrl(Uri url)
        {
            var container = new CookieContainer();
            string cookieHeaders = RetrieveIECookiesForUrl(url.AbsoluteUri);
            if (cookieHeaders.Length > 0)
            {
                try { container.SetCookies(url, cookieHeaders); }
                catch (CookieException) { }
            }
            return container;
        }

        public static CookieCollection GetCookiesForUrl(Uri url)
        {
            var container = GetCookieContainerForUrl(url);
            return container.GetCookies(url);
        }

        public static string GetCookie(string url, string cookieName)
        {
            int bufferLength = 0;
            IntPtr buffer = IntPtr.Zero;

            try
            {
                for (;;)
                {
                    bool returnValue = InternetGetCookieEx(url, cookieName, buffer, ref bufferLength, 0, IntPtr.Zero);
                    int err = Marshal.GetLastWin32Error();

                    if (returnValue && buffer != IntPtr.Zero)
                        break;

                    if (err == ERROR_NO_MORE_ITEMS)
                        return null;
                    if (err != 0 && err != ERROR_INSUFFICIENT_BUFFER)
                        ThrowExceptionForLastWin32Error();

                    buffer = Marshal.ReAllocCoTaskMem(buffer, bufferLength);
                }

                return Marshal.PtrToStringUni(buffer);
            }
            finally
            {
                Marshal.FreeCoTaskMem(buffer);
            }
        }

        public static void SetCookie(string url, string cookieData)
        {
            InternetCookieState cookieState = (InternetCookieState)
                InternetSetCookieEx(url, null, cookieData, 0, IntPtr.Zero);
            if (cookieState == InternetCookieState.COOKIE_STATE_UNKNOWN)
                ThrowExceptionForLastWin32Error();

            if (cookieState != InternetCookieState.COOKIE_STATE_ACCEPT)
                throw new InvalidOperationException("The cookie could not be set.  Its acceptance state was: " + cookieState);
        }

        public static void ClearCookies(string url)
        {
            new ClearCookiesCommand(url).Run();
        }

        public static void ClearCache()
        {
            ForEachCacheGroup(new CacheGroupAction(DeleteCacheGroup));
        }

        private static void DeleteCacheGroup(long groupId)
        {
            if (!DeleteUrlCacheGroup(groupId, CACHEGROUP_FLAG_FLUSHURL_ONDELETE, IntPtr.Zero))
            {
                int err = Marshal.GetLastWin32Error();
                if (err != ERROR_FILE_NOT_FOUND)
                    ThrowExceptionForLastWin32Error();
            }
        }

        private static void ForEachCacheGroup(CacheGroupAction action)
        {
            // Groups may not always exist on the system.
            // For more information, visit the following Microsoft Web site:
            // http://msdn.microsoft.com/library/?url=/workshop/networking/wininet/overview/cache.asp			
            // By default, a URL does not belong to any group. Therefore, that cache may become
            // empty even when the CacheGroup APIs are not used because the existing URL does not belong to any group.
            long groupId = 0;
            IntPtr enumHandle = FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);
            int err = Marshal.GetLastWin32Error();

            // If there are no items in the Cache, you are finished.
            if (enumHandle == IntPtr.Zero)
            {
                if (err != ERROR_NO_MORE_ITEMS && err != ERROR_FILE_NOT_FOUND)
                    ThrowExceptionForLastWin32Error();
            }
            else
            {
                // Loop through Cache Group.
                for (;;)
                {
                    action(groupId);

                    // Get the next one.
                    bool returnValue = FindNextUrlCacheGroup(enumHandle, ref groupId, IntPtr.Zero);
                    err = Marshal.GetLastWin32Error();

                    if (!returnValue)
                    {
                        if (err != ERROR_NO_MORE_ITEMS && err != ERROR_FILE_NOT_FOUND)
                            ThrowExceptionForLastWin32Error();
                        break;
                    }
                }
            }

            // Process group 0.
            action(0);
        }

        private static void ForEachCacheEntry(long groupId, string searchPattern, int flags, CacheEntryAction action)
        {
            int cacheEntryInfoBufferSize = 0;
            IntPtr cacheEntryInfoBuffer = IntPtr.Zero;

            IntPtr enumHandle = IntPtr.Zero;
            try
            {
                for (;;)
                {
                    enumHandle = FindFirstUrlCacheEntryEx(searchPattern, 0, flags,
                        groupId, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSize, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    int err = Marshal.GetLastWin32Error();

                    if (enumHandle != IntPtr.Zero)
                        break;

                    if (err == ERROR_NO_MORE_ITEMS || err == ERROR_FILE_NOT_FOUND)
                        return;
                    if (err != ERROR_INSUFFICIENT_BUFFER)
                        ThrowExceptionForLastWin32Error();

                    cacheEntryInfoBuffer = Marshal.ReAllocCoTaskMem(cacheEntryInfoBuffer, cacheEntryInfoBufferSize);
                }

                for (;;)
                {
                    INTERNET_CACHE_ENTRY_INFO entry = (INTERNET_CACHE_ENTRY_INFO)
                        Marshal.PtrToStructure(cacheEntryInfoBuffer, typeof (INTERNET_CACHE_ENTRY_INFO));

                    action(entry);

                    // Get next entry.
                    bool returnValue = FindNextUrlCacheEntryEx(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSize,
                        IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    int err = Marshal.GetLastWin32Error();

                    if (!returnValue)
                    {
                        if (err == ERROR_NO_MORE_ITEMS || err == ERROR_FILE_NOT_FOUND)
                            return;
                        if (err != ERROR_INSUFFICIENT_BUFFER)
                            ThrowExceptionForLastWin32Error();

                        cacheEntryInfoBuffer = Marshal.ReAllocCoTaskMem(cacheEntryInfoBuffer, cacheEntryInfoBufferSize);
                    }
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(cacheEntryInfoBuffer);

                if (enumHandle != IntPtr.Zero)
                    FindCloseUrlCache(enumHandle);
            }
        }

        private static void ThrowExceptionForLastWin32Error()
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        private struct INTERNET_CACHE_ENTRY_INFO
        {
            public UInt32 dwStructSize;
            public IntPtr lpszSourceUrlName;
            public IntPtr lpszLocalFileName;
            public UInt32 CacheEntryType;
            public UInt32 dwUseCount;
            public UInt32 dwHitRate;
            public UInt32 dwSizeLow;
            public UInt32 dwSizeHigh;
            public FILETIME LastModifiedTime;
            public FILETIME ExpireTime;
            public FILETIME LastAccessTime;
            public FILETIME LastSyncTime;
            public IntPtr lpHeaderInfo;
            public UInt32 dwHeaderInfoSize;
            public IntPtr lpszFileExtension;
            public UInt32 dwExemptDelta;
        }
        #endregion

        #region PInvokes

        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindFirstUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr FindFirstUrlCacheGroup(
            int dwFlags,
            int dwFilter,
            IntPtr lpSearchCondition,
            int dwSearchCondition,
            ref long lpGroupId,
            IntPtr lpReserved);

        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindNextUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        [return : MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindNextUrlCacheGroup(
            IntPtr hFind,
            ref long lpGroupId,
            IntPtr lpReserved);

        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "DeleteUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        [return : MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteUrlCacheGroup(
            long GroupId,
            int dwFlags,
            IntPtr lpReserved);

        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Unicode,
            EntryPoint = "FindFirstUrlCacheEntryExW",
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr FindFirstUrlCacheEntryEx(
            [MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern,
            int flags, int filter, long groupId,
            IntPtr lpFirstCacheEntryInfo,
            ref int lpdwFirstCacheEntryInfoBufferSize,
            IntPtr reserved, IntPtr reserved2, IntPtr reserved3);

        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Unicode,
            EntryPoint = "FindNextUrlCacheEntryExW",
            CallingConvention = CallingConvention.StdCall)]
        [return : MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindNextUrlCacheEntryEx(
            IntPtr hFind,
            IntPtr lpNextCacheEntryInfo,
            ref int lpdwNextCacheEntryInfoBufferSize,
            IntPtr reserved, IntPtr reserved2, IntPtr reserved3);

        [DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto)]
        [return : MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindCloseUrlCache(IntPtr handle);

        [DllImport(@"wininet", SetLastError = true,
            CharSet = CharSet.Unicode,
            EntryPoint = "DeleteUrlCacheEntryW",
            CallingConvention = CallingConvention.StdCall)]
        [return : MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteUrlCacheEntry(
            IntPtr lpszUrlName);

        [DllImport("wininet.dll", SetLastError = true,
            CharSet = CharSet.Unicode,
            EntryPoint = "InternetQueryOptionW")]
        [return : MarshalAs(UnmanagedType.Bool)]
        private static extern bool InternetQueryOption(IntPtr hInternet, uint dwOption, IntPtr lpBuffer, ref int lpdwBufferLength);

        [DllImport("wininet.dll", SetLastError = true,
            CharSet = CharSet.Unicode,
            EntryPoint = "InternetSetOptionW")]
        [return : MarshalAs(UnmanagedType.Bool)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        [DllImport("wininet.dll", SetLastError = true,
            CharSet = CharSet.Unicode)]
        public static extern bool InternetGetCookie (string url, string name, StringBuilder data, ref int dataSize);

        [DllImport("wininet.dll", SetLastError = true,
            CharSet = CharSet.Unicode,
            EntryPoint = "InternetGetCookieExW")]
        [return : MarshalAs(UnmanagedType.Bool)]
        private static extern bool InternetGetCookieEx(
            [MarshalAs(UnmanagedType.LPTStr)] string pchURL,
            [MarshalAs(UnmanagedType.LPTStr)] string pchCookieName,
            IntPtr pchCookieData,
            ref int pcchCookieData,
            int dwFlags,
            IntPtr lpReserved);

        [DllImport("wininet.dll", SetLastError = true,
            CharSet = CharSet.Unicode,
            EntryPoint = "InternetSetCookieExW")]
        [return : MarshalAs(UnmanagedType.I4)]
        private static extern int InternetSetCookieEx(
            [MarshalAs(UnmanagedType.LPTStr)] string lpszURL,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszCookieName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpszCookieData,
            int dwFlags,
            IntPtr dwReserved);

        #endregion

        /// <summary>
        /// Holds state for the duration of the clear cookies operation because we
        /// don't have anonymous delegates in .Net 1.1.
        /// </summary>
        private class ClearCookiesCommand
        {
            private readonly string[] cacheEntrySuffixes;

            public ClearCookiesCommand(string url)
            {
                // The entry looks like "Cookie:user@my.domain.com/".
                // Generate a list of suffixes to delete.
                if (url == null)
                    cacheEntrySuffixes = null;
                else
                {
                    string remainder = new Uri(url).Host + "/";
                    cacheEntrySuffixes = new string[] {"@" + remainder, "." + remainder};
                }
            }

            public void Run()
            {
                ForEachCacheGroup(new CacheGroupAction(ClearCookiesInCacheGroup));
            }

            private void ClearCookiesInCacheGroup(long groupId)
            {
                ForEachCacheEntry(groupId, "cookie:", COOKIE_CACHE_ENTRY | NORMAL_CACHE_ENTRY | STICKY_CACHE_ENTRY,
                    new CacheEntryAction(DeleteUrlCacheEntryIfUrlMatches));
            }

            private void DeleteUrlCacheEntryIfUrlMatches(INTERNET_CACHE_ENTRY_INFO entry)
            {
                string cacheEntryName = Marshal.PtrToStringUni(entry.lpszSourceUrlName);

                if (cacheEntrySuffixes != null)
                {
                    foreach (string suffix in cacheEntrySuffixes)
                    {
                        if (cacheEntryName.EndsWith(suffix))
                            goto Match;
                    }
                    return;
                }

                Match:
                Logger.LogAction(String.Format("Deleting '{0}'", cacheEntryName));

                if (!DeleteUrlCacheEntry(entry.lpszSourceUrlName))
                {
                    int err = Marshal.GetLastWin32Error(); 
                    if (err != ERROR_FILE_NOT_FOUND)
                        ThrowExceptionForLastWin32Error();
                }
            }
        }
    }
}