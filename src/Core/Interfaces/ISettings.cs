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

namespace WatiN.Core.Interfaces
{
    public interface ISettings
    {
        /// <summary>
        /// Resets this instance to the initial defaults.
        /// </summary>
        void Reset();

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        ISettings Clone();

        /// <summary>
        /// Get or set the default time out used when calling IE ie = IE.AttachToIE(findBy).
        /// The default value is 30 seconds. Setting the time out to a negative value will
        /// throw a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        int AttachToBrowserTimeOut { get; set; }

        /// <summary>
        /// Get or set the default time out used when calling Element.WaitUntilExists().
        /// The default value is 30 seconds. Setting the time out to a negative value will
        /// throw a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        int WaitUntilExistsTimeOut { get; set; }

        /// <summary>
        /// Get or set the default time out used when calling ie.WaitForComplete().
        /// The default value is 30 seconds. Setting the time out to a negative value will
        /// throw a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        int WaitForCompleteTimeOut { get; set; }

        /// <summary>
        /// Get or set the default sleep time used when WatiN is waiting for something in a (retry) loop.
        /// The default value is 100 milliseconds. Setting the time out to a negative value will
        /// throw a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        int SleepTime { get; set; }

        /// <summary>
        /// Turn highlighting of elements by WatiN on (<c>true</c>) or off (<c>false</c>).
        /// Highlighting of an element is done when WatiN fires an event on an
        /// element or executes a methode (like TypeText).
        /// </summary>
        bool HighLightElement { get; set; }

        /// <summary>
        /// Set or get the color to highlight elements. Will be used if
        /// HighLightElement is set to <c>true</c>.
        /// Visit http://msdn.microsoft.com/workshop/author/dhtml/reference/colors/colors_name.asp
        /// for a full list of supported RGB colors and their names.
        /// </summary>
        string HighLightColor { get; set; }

        /// <summary>
        /// Turn auto closing of dialogs on (<c>true</c>) or off (<c>false</c>).
        /// You need to set this value before creating or attaching to any 
        /// Internet Explorer to have effect.
        /// </summary>
        bool AutoCloseDialogs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to auto start the dialog watcher at all.
        /// This value is evaluated everytime a new IE instance is created
        /// </summary>
        /// <value>
        /// 	<c>true</c> if dialog watcher should be started when a new IE instance is created; otherwise, <c>false</c>.
        /// </value>
        bool AutoStartDialogWatcher { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to move the cursor to the top left
        /// of the screen everytime a new IE instance is created.
        /// </summary>
        /// <value>
        /// 	<c>true</c> when mouse should be moved to top left; otherwise, <c>false</c>.
        /// </value>
        bool AutoMoveMousePointerToTopLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to make a new <see cref="IE"/> instance visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if you want to make a new <see cref="IE"/> instance visible; otherwise, <c>false</c>.
        /// </value>
        bool MakeNewIeInstanceVisible { get; set; }

        IFindByDefaultFactory FindByDefaultFactory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to make a new <see cref="IE"/> instance without session cookie merging.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if you want to make a new <see cref="IE"/> instance with cookie merging; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>read the section on NoMerge at http://blogs.msdn.com/ie/archive/2008/07/28/ie8-and-reliability.aspx</remarks>
        bool MakeNewIe8InstanceNoMerge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether existing firefox instances will be closed before creating a new instance.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if existing firefox instances need to be closed otherwise, <c>false</c>.
        /// </value>
        bool CloseExistingFireFoxInstances { get; set; }
    }
}