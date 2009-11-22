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
    /// <summary>
	/// This class is used to define the default settings used by WatiN. 
	/// Use <c>Settings.Instance</c> to access or change these settings.
	/// </summary>
	/// <example>
	/// The following example shows you how to change the default time out which is used
	/// by the AttachtToIE(findBy) method to attach to an already existing Internet Explorer window 
	/// or to an Internet Explorer window that will show up within 60 seconds after calling
	/// the AttachToIE(findBy) method.
	/// <code>
	/// public void AttachToIEExample()
	/// {
	///   // Change de default time out from 30 to 60 seconds.
	///   Settings.AttachToBrowserTimeOut = 60;
	/// 
	///   // Start Internet Explorer manually and type 
	///   // http://watin.sourceforge.net in the navigation bar.
	/// 
	///   // Now Attach to an existing Internet Explorer window
	///   IE ie = IE.AttachToIE(Find.ByTitle("WatiN");
	/// 
	///   System.Diagnostics.Debug.WriteLine(ie.Url);
	/// }
	/// </code>
	/// When you frequently want to change these settings you could also create
	/// two or more instances of the Settings class, set the desired defaults 
	/// and set the settings class to Settings.
	/// <code>
	/// public void ChangeSettings()
	/// {
	///   Settings.Instance = LongTimeOut();
	///   
	///   // Do something here that requires more time then the defaults
	/// 
	///   Settings.Instance = ShortTimeOut();
	/// 
	///   // Do something here if you want a short time out to get
	///   // the exception quickly incase the item isn't found.  
	/// }
	/// 
	/// public Settings LongTimeOut()
	/// {
	///   Settings settings = new Settings();
	/// 
	///   settings.AttachToBrowserTimeOut = 60;
	///   settings.WaitUntilExistsTimeOut = 60;
	///   settings.WaitForCompleteTimeOut = 60;
	/// 
	///   return settings;
	/// }
	/// 
	/// public Settings ShortTimeOut()
	/// {
	///   Settings settings = new Settings();
	/// 
	///   settings.AttachToBrowserTimeOut = 5;
	///   settings.WaitUntilExistsTimeOut = 5;
	///   settings.WaitForCompleteTimeOut = 5;
	/// 
	///   return settings;
	/// }
	/// </code>
	/// </example>
	public static class Settings
    {
        private static ISettings _instance = CreateDefaultSettings();

        private static DefaultSettings CreateDefaultSettings()
        {
            return new DefaultSettings();
        }

        public static ISettings Instance
        {
            get { return _instance; }
            set
			{
				_instance = value ?? CreateDefaultSettings();
			}
        }

		/// <summary>
		/// Resets this instance to the initial defaults.
		/// </summary>
		public static void Reset()
		{
			Instance.Reset();
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
        public static ISettings Clone()
		{
			return Instance.Clone();
		}

		/// <summary>
		/// Get or set the default time out used when calling IE ie = IE.AttachToIE(findBy).
		/// The default value is 30 seconds. Setting the time out to a negative value will
		/// throw a <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
        public static int AttachToBrowserTimeOut
		{
			get { return Instance.AttachToBrowserTimeOut; }
			set { Instance.AttachToBrowserTimeOut = value; }
		}

		/// <summary>
		/// Get or set the default time out used when calling Element.WaitUntilExists().
		/// The default value is 30 seconds. Setting the time out to a negative value will
		/// throw a <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
        public static int WaitUntilExistsTimeOut
		{
            get { return Instance.WaitUntilExistsTimeOut; }
            set { Instance.WaitUntilExistsTimeOut = value; }
		}

		/// <summary>
		/// Get or set the default time out used when calling ie.WaitForComplete().
		/// The default value is 30 seconds. Setting the time out to a negative value will
		/// throw a <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
        public static int WaitForCompleteTimeOut
		{
			get { return Instance.WaitForCompleteTimeOut; }
			set { Instance.WaitForCompleteTimeOut = value; }
		}

        /// <summary>
		/// Get or set the default sleep time used when WatiN is waiting for something in a (retry) loop.
		/// The default value is 100 milliseconds. Setting the time out to a negative value will
		/// throw a <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
        public static int SleepTime
		{
			get { return Instance.SleepTime; }
			set { Instance.SleepTime = value; }
		}

		/// <summary>
		/// Turn highlighting of elements by WatiN on (<c>true</c>) or off (<c>false</c>).
		/// Highlighting of an element is done when WatiN fires an event on an
		/// element or executes a methode (like TypeText).
		/// </summary>
        public static bool HighLightElement
		{
			get { return Instance.HighLightElement; }
			set { Instance.HighLightElement = value; }
		}

		/// <summary>
		/// Set or get the color to highlight elements. Will be used if
		/// HighLightElement is set to <c>true</c>.
		/// Visit http://msdn.microsoft.com/workshop/author/dhtml/reference/colors/colors_name.asp
		/// for a full list of supported RGB colors and their names.
		/// </summary>
        public static string HighLightColor
		{
			get { return Instance.HighLightColor; }
			set { Instance.HighLightColor = value; }
		}

		/// <summary>
		/// Turn auto closing of dialogs on (<c>true</c>) or off (<c>false</c>).
		/// You need to set this value before creating or attaching to any 
		/// Internet Explorer to have effect.
		/// </summary>
        public static bool AutoCloseDialogs
		{
			get { return Instance.AutoCloseDialogs; }
			set { Instance.AutoCloseDialogs = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to auto start the dialog watcher at all.
		/// This value is evaluated everytime a new IE instance is created
		/// </summary>
		/// <value>
		/// 	<c>true</c> if dialog watcher should be started when a new IE instance is created; otherwise, <c>false</c>.
		/// </value>
        public static bool AutoStartDialogWatcher
		{
			get { return Instance.AutoStartDialogWatcher; }
			set { Instance.AutoStartDialogWatcher = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to move the cursor to the top left
		/// of the screen everytime a new IE instance is created.
		/// </summary>
		/// <value>
		/// 	<c>true</c> when mouse should be moved to top left; otherwise, <c>false</c>.
		/// </value>
        public static bool AutoMoveMousePointerToTopLeft
		{
			get { return Instance.AutoMoveMousePointerToTopLeft; }
			set { Instance.AutoMoveMousePointerToTopLeft = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to make a new <see cref="IE"/> instance visible.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if you want to make a new <see cref="IE"/> instance visible; otherwise, <c>false</c>.
		/// </value>
        public static bool MakeNewIeInstanceVisible
		{
			get { return Instance.MakeNewIeInstanceVisible; }
			set { Instance.MakeNewIeInstanceVisible = value; }
		}

        /// <summary>
        /// Gets or sets a factory to find element by their default characteristics.
        /// </summary>
        /// <remarks>
        /// The default value is a <see cref="FindByDefaultFactory"/> which finds elements
        /// by id.
        /// </remarks>
        public static IFindByDefaultFactory FindByDefaultFactory
	    {
            get { return Instance.FindByDefaultFactory; }
            set 
            {
                Instance.FindByDefaultFactory = value;
                if (Instance.FindByDefaultFactory == null)
                {
                    Instance.FindByDefaultFactory = new FindByDefaultFactory();
                }
            }
	    }

        /// <summary>
        /// Gets or sets a value indicating whether to make a new <see cref="IE"/> instance without session cookie merging.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if you want to make a new <see cref="IE"/> instance with cookie merging; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>read the section on NoMerge at http://blogs.msdn.com/ie/archive/2008/07/28/ie8-and-reliability.aspx</remarks>
        public static bool MakeNewIe8InstanceNoMerge
        {
            get { return Instance.MakeNewIe8InstanceNoMerge; }
            set { Instance.MakeNewIe8InstanceNoMerge = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether existing FireFox instances will be closed before creating a new instance.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if existing FireFox instances need to be closed otherwise, <c>false</c>.
        /// </value>
        public static bool CloseExistingFireFoxInstances
        {
            get { return Instance.CloseExistingFireFoxInstances; }
            set { Instance.CloseExistingFireFoxInstances = value; }            
        }
	}
}