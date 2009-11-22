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
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
    public class DefaultSettings : ISettings
    {
        private struct settingsStruct
        {
            public int attachToBrowserTimeOut;
            public int waitUntilExistsTimeOut;
            public int waitForCompleteTimeOut;
            public bool highLightElement;
            public string highLightColor;
            public bool autoCloseDialogs;
            public bool autoStartDialogWatcher;
            public bool autoMoveMousePointerToTopLeft;
            public bool makeNewIEInstanceVisible;
            public int sleepTime;
            public IFindByDefaultFactory findByDefaultFactory;
            public bool makeNewIe8InstanceNoMerge;
            public bool closeExistingFireFoxInstances;
        }

        private settingsStruct settings;

        public DefaultSettings()
        {
            SetDefaults();
        }

        private DefaultSettings(settingsStruct settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Resets this instance to the initial defaults.
        /// </summary>
        public virtual void Reset()
        {
            SetDefaults();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public virtual ISettings Clone()
        {
            return new DefaultSettings(settings);
        }

        private void SetDefaults()
        {
            settings = new settingsStruct
                           {
                               attachToBrowserTimeOut = 30,
                               waitUntilExistsTimeOut = 30,
                               waitForCompleteTimeOut = 30,
                               sleepTime = 100,
                               highLightElement = true,
                               highLightColor = "yellow",
                               autoCloseDialogs = true,
                               autoStartDialogWatcher = true,
                               autoMoveMousePointerToTopLeft = true,
                               makeNewIEInstanceVisible = true,
                               findByDefaultFactory = new FindByDefaultFactory(),
                               makeNewIe8InstanceNoMerge = true,
                               closeExistingFireFoxInstances = true
                           };
        }

        /// <summary>
        /// Get or set the default time out used when calling IE ie = IE.AttachToIE(findBy).
        /// The default value is 30 seconds. Setting the time out to a negative value will
        /// throw a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        public int AttachToBrowserTimeOut
        {
            get { return settings.attachToBrowserTimeOut; }
            set
            {
                IfValueLessThenZeroThrowArgumentOutOfRangeException(value);
                settings.attachToBrowserTimeOut = value;
            }
        }

        /// <summary>
        /// Get or set the default time out used when calling Element.WaitUntilExists().
        /// The default value is 30 seconds. Setting the time out to a negative value will
        /// throw a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        public int WaitUntilExistsTimeOut
        {
            get { return settings.waitUntilExistsTimeOut; }
            set
            {
                IfValueLessThenZeroThrowArgumentOutOfRangeException(value);
                settings.waitUntilExistsTimeOut = value;
            }
        }

        /// <summary>
        /// Get or set the default time out used when calling ie.WaitForComplete().
        /// The default value is 30 seconds. Setting the time out to a negative value will
        /// throw a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        public int WaitForCompleteTimeOut
        {
            get { return settings.waitForCompleteTimeOut; }
            set
            {
                IfValueLessThenZeroThrowArgumentOutOfRangeException(value);
                settings.waitForCompleteTimeOut = value;
            }
        }

        /// <summary>
        /// Get or set the default sleep time used when WatiN is waiting for something in a (retry) loop.
        /// The default value is 100 milliseconds. Setting the time out to a negative value will
        /// throw a <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        public int SleepTime
        {
            get { return settings.sleepTime; }
            set
            {
                IfValueLessThenZeroThrowArgumentOutOfRangeException(value);
                settings.sleepTime = value;
            }
        }

        /// <summary>
        /// Turn highlighting of elements by WatiN on (<c>true</c>) or off (<c>false</c>).
        /// Highlighting of an element is done when WatiN fires an event on an
        /// element or executes a methode (like TypeText).
        /// </summary>
        public bool HighLightElement
        {
            get { return settings.highLightElement; }
            set { settings.highLightElement = value; }
        }

        /// <summary>
        /// Set or get the color to highlight elements. Will be used if
        /// HighLightElement is set to <c>true</c>.
        /// Visit http://msdn.microsoft.com/workshop/author/dhtml/reference/colors/colors_name.asp
        /// for a full list of supported RGB colors and their names.
        /// </summary>
        public string HighLightColor
        {
            get { return settings.highLightColor; }
            set { settings.highLightColor = value; }
        }

        /// <summary>
        /// Turn auto closing of dialogs on (<c>true</c>) or off (<c>false</c>).
        /// You need to set this value before creating or attaching to any 
        /// Internet Explorer to have effect.
        /// </summary>
        public bool AutoCloseDialogs
        {
            get { return settings.autoCloseDialogs; }
            set { settings.autoCloseDialogs = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to auto start the dialog watcher at all.
        /// This value is evaluated everytime a new IE instance is created
        /// </summary>
        /// <value>
        /// 	<c>true</c> if dialog watcher should be started when a new IE instance is created; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStartDialogWatcher
        {
            get { return settings.autoStartDialogWatcher; }
            set { settings.autoStartDialogWatcher = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to move the cursor to the top left
        /// of the screen everytime a new IE instance is created.
        /// </summary>
        /// <value>
        /// 	<c>true</c> when mouse should be moved to top left; otherwise, <c>false</c>.
        /// </value>
        public bool AutoMoveMousePointerToTopLeft
        {
            get { return settings.autoMoveMousePointerToTopLeft; }
            set { settings.autoMoveMousePointerToTopLeft = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to make a new <see cref="IE"/> instance visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if you want to make a new <see cref="IE"/> instance visible; otherwise, <c>false</c>.
        /// </value>
        public bool MakeNewIeInstanceVisible
        {
            get { return settings.makeNewIEInstanceVisible; }
            set { settings.makeNewIEInstanceVisible = value; }
        }

        public IFindByDefaultFactory FindByDefaultFactory
        {
            get { return settings.findByDefaultFactory; }
            set { settings.findByDefaultFactory = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to make a new <see cref="IE"/> instance without session cookie merging.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if you want to make a new <see cref="IE"/> instance with cookie merging; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>read the section on NoMerge at http://blogs.msdn.com/ie/archive/2008/07/28/ie8-and-reliability.aspx</remarks>
        public bool MakeNewIe8InstanceNoMerge
        {
            get { return settings.makeNewIe8InstanceNoMerge; }
            set { settings.makeNewIe8InstanceNoMerge = value; }
        }

        public bool CloseExistingFireFoxInstances
        {
            get { return settings.closeExistingFireFoxInstances; }
            set { settings.closeExistingFireFoxInstances = value; }
        }

        private static void IfValueLessThenZeroThrowArgumentOutOfRangeException(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", "Should be 0 or more.");
            }
        }
    }
}