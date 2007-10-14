#region WatiN Copyright (C) 2006-2007 Jeroen van Menen

//Copyright 2006-2007 Jeroen van Menen
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

namespace WatiN.Core
{
  using System;

  /// <summary>
  /// This class is used to define the default settings used by WatiN. 
  /// Use <c>IE.Settings</c> to access or change these settings.
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
  ///   IE.Settings.AttachToIETimeOut = 60;
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
  /// and set the settings class to IE.Settings.
  /// <code>
  /// public void ChangeSettings()
  /// {
  ///   IE.Settings = LongTimeOut();
  ///   
  ///   // Do something here that requires more time then the defaults
  /// 
  ///   IE.Settings = ShortTimeOut();
  /// 
  ///   // Do something here if you want a short time out to get
  ///   // the exception quickly incase the item isn't found.  
  /// }
  /// 
  /// public Settings LongTimeOut()
  /// {
  ///   Settings settings = new Settings();
  /// 
  ///   settings.AttachToIETimeOut = 60;
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
  ///   settings.AttachToIETimeOut = 5;
  ///   settings.WaitUntilExistsTimeOut = 5;
  ///   settings.WaitForCompleteTimeOut = 5;
  /// 
  ///   return settings;
  /// }
  /// </code>
  /// </example>
  public class Settings
  {
    private struct settingsStruct
    {
      public int attachToIETimeOut;
      public int waitUntilExistsTimeOut;
      public int waitForCompleteTimeOut;
      public bool highLightElement;
      public string highLightColor;
      public bool autoCloseDialogs;
      public bool autoStartDialogWatcher;
      public bool autoMoveMousePointerToTopLeft;
      public bool makeNewIEInstanceVisible;
    }
    
    private settingsStruct settings;

    public Settings()
    {
      SetDefaults();
    }

    private Settings(settingsStruct settings)
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
    public virtual Settings Clone()
    {
      return new Settings(settings);
    }
    
    private void SetDefaults()
    {
      settings = new settingsStruct();
      settings.attachToIETimeOut = 30;
      settings.waitUntilExistsTimeOut = 30;
      settings.waitForCompleteTimeOut = 30;
      settings.highLightElement = true;
      settings.highLightColor = "yellow";
      settings.autoCloseDialogs = true;
      settings.autoStartDialogWatcher = true;
      settings.autoMoveMousePointerToTopLeft = true;
      settings.makeNewIEInstanceVisible = true;
    }

    /// <summary>
    /// Get or set the default time out used when calling IE ie = IE.AttachToIE(findBy).
    /// The initial value is 30 seconds. Setting the time out to a negative value will
    /// throw a <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    public int AttachToIETimeOut
    {
      get { return settings.attachToIETimeOut; }
      set
      {
        IfValueLessThenZeroThrowArgumentOutOfRangeException(value);
        settings.attachToIETimeOut = value;
      }
    }

    /// <summary>
    /// Get or set the default time out used when calling Element.WaitUntilExists().
    /// The initial value is 30 seconds. Setting the time out to a negative value will
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
    /// The initial value is 30 seconds. Setting the time out to a negative value will
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

    private static void IfValueLessThenZeroThrowArgumentOutOfRangeException(int value)
    {
      if (value < 0 )
      {
        throw new ArgumentOutOfRangeException("value", "time out should be 0 seconds or more.");
      }
    }
  }
}