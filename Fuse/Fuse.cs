/*

	Copyright (c)  Goran Sterjov

    This file is part of the Fuse Project.

    Fuse is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    Fuse is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Fuse; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/


using System;
using Gtk;
using Fuse.Interfaces;

namespace Fuse
{
	
	/// <summary>
	/// The main class for the IFuse interface.
	/// </summary>
	public class FuseApp : IFuse
	{
		
		Config config;
		
		// global user interface widgets
		StatusBar status;
		Controls controls;
		MainMenu menu;
		MainWindow window;
		Communicator communicator;
		
		
		string chosen_engine;
		string data_dir;
		string config_dir;
		
		
		public event EventHandler Quiting;
		
		
		
		// loads the application
		public FuseApp (string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += unhandled_exception;
			
			
			// read the command line arguments
			CommandLineParser parser = new CommandLineParser (args);
			data_dir = parser.DataDir;
			
			if (string.IsNullOrEmpty (data_dir))
			{
				Console.WriteLine ("No data directory specified. Using the default directory");
				data_dir = Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
				data_dir = System.IO.Path.Combine (data_dir, "Fuse");
			}
			config_dir = System.IO.Path.Combine (data_dir, "config");
			config = new Config (config_dir);
			
			
			// load the application interface
			controls = new Controls (this);
			menu = new MainMenu (this);
			window = new MainWindow (this);
			status = new StatusBar (this, window.BottomBox);
			communicator = new Communicator ();
			
			
			loadSettings ();
			menu.LoadEngine ();
			
			if (controls.Engine == null)
				ThrowError ("Could not load a media engine.\nGo to  Options -> Media Engines  to select a working engine");
			
			
			menu.LoadPluginList ();
			
			//switch to the last opened tab
			string opened_tab = config.Window.Get ("Opened Tab", "");
			window.SwitchToTab (opened_tab);
			
			this.Quiting += on_quit;
			window.ShowAll ();
		}
		
		
		
		/// <summary>
		/// Creates a new page in the main Notebook and
		/// adds the supplied widget to it.
		/// </summary>
		public void AddWidget (Widget widget, string title)
		{
			window.AddWidget (widget, title);
		}
		
		
		/// <summary>
		/// Removes the page in the main Notebook.
		/// </summary>
		public void RemoveWidget (Widget widget)
		{
			window.RemoveWidget (widget);
		}
		
		
		
		
		/// <summary>
		/// Adds the message into the status bar.
		/// </summary>
		public void StatusPush (string message)
		{
			status.Icon = StatusIcon.None;
			status.Push (message);
		}
		
		
		
		/// <summary>
		/// Removes the message from the status bar.
		/// </summary>
		public void StatusPop ()
		{
			status.Pop ();
		}
		
		
		/// <summary>
		/// Throw a simple error dialog with the specified message.
		/// </summary>
		public void ThrowError (string message)
		{
			MessageDialog dialog = new MessageDialog (window,
				                                              DialogFlags.DestroyWithParent,
				                                              MessageType.Error,
				                                              ButtonsType.Close,
				                                              Utils.ParseMarkup (message));
			dialog.Run ();
			dialog.Destroy ();
		}
		
		
		/// <summary>
		/// Throw a warning and notify the user in the status bar.
		/// </summary>
		public void ThrowWarning (string brief_warning, string detailed_warning)
		{
			status.ErrorLog.AppendValues (brief_warning, detailed_warning);
			status.Push ("An error has occured:  " + brief_warning);
			status.Icon = StatusIcon.Warning;
		}
		
		/// <summary>
		/// Throw a critical exception and quit afterwards.
		/// </summary>
		public void ThrowException (string exception)
		{
			Console.WriteLine (exception);
			
			if (window != null)
			{
				ExceptionWindow exception_window = new ExceptionWindow (window, exception);
				exception_window.Run ();
			}
		}
		
		
		
		/// <summary>
		/// Raise the Quiting event to allow the application and the plugins
		/// to do last minute changes.
		/// </summary>
		public void Quit ()
		{
			Quiting (this, new EventArgs ());
		}
		
		
		
		/// <summary>
		/// The main controls for the media engine.
		/// </summary>
		public Controls Controls
		{
			get{ return controls; }
		}
		
		
		/// <summary>
		/// The main Fuse menu bar.
		/// </summary>
		public MainMenu Menu
		{
			get{ return menu; }
		}
		
		
		/// <summary>
		/// The main Fuse Gtk.Window.
		/// </summary>
		public MainWindow Window
		{
			get{ return window; }
		}
		
		
		/// <summary>
		/// The main Fuse status bar.
		/// </summary>
		public StatusBar Status
		{
			get{ return status; }
		}
		
		
		/// <summary>
		/// The main configuration.
		/// </summary>
		public Config Config
		{
			get{ return config; }
		}
		
		
		
		/// <summary>
		/// The main Fuse Gtk.Window, for plugins.
		/// </summary>
		public Window MainWindow
		{
			get{ return window; }
		}
		
		/// <summary>
		/// The main controls for the media engine, for plugins.
		/// </summary>
		public IMediaControls MediaControls
		{
			get{ return controls; }
		}
		
		
		/// <summary>
		/// The communicator for plugins to send commands to one another.
		/// </summary>
		public Communicator PluginCommunicator
		{
			get{ return communicator; }
		}
		
		
		
		/// <summary>
		/// The directory where user data should be placed.
		/// </summary>
		public string DataDir
		{
			get{ return data_dir; }
		}
		
		
		
		/// <summary>
		/// The directory where configuration data should be placed.
		/// </summary>
		public string ConfigDir
		{
			get{ return config_dir; }
		}
		
		
		/// <summary>
		/// The path to the selected media engine.
		/// </summary>
		public string ChosenEngine
		{
			get{ return chosen_engine; }
			set{ chosen_engine = value; }
		}
		
		
		
		// main entrance
		public static void Main (string[] args)
		{
			Application.Init ();
			new FuseApp (args);
			Application.Run ();
		}
		
		
		
		
		// loads the main applications settings
		void loadSettings ()
		{
			int width = config.Window.GetInt ("Width", 500);
			int height = config.Window.GetInt ("Height", 400);
			bool shuffle = config.MediaControls.GetBoolean ("Shuffle", false);
			bool crossfade = config.MediaControls.GetBoolean ("Crossfade", false);
			
			
			window.Resize (width, height);
			window.WindowPosition = Gtk.WindowPosition.Center;
			menu.Shuffle = shuffle;
			menu.Crossfade = crossfade;
		}
		
		
		// saves the main applications settings
		void saveSettings ()
		{
			int width, height;
			window.GetSize (out width, out height);
			
			config.Window.Set ("Width", width);
			config.Window.Set ("Height", height);
			config.MediaControls.Set ("Shuffle", controls.Shuffle);
			config.MediaControls.Set ("Crossfade", controls.Crossfade);
			config.MediaControls.Set ("Engine Path", chosen_engine);
			
			
			if (window.TabCount == 0)
				config.Window.Set ("Opened Tab", "None");
			else
				config.Window.Set ("Opened Tab", window.CurrentTab);
			
			config.Save ();
		}
		
		
		
		
		// when the application is quiting
		void on_quit (object o, EventArgs args)
		{
			saveSettings ();
			menu.SavePluginList ();
			
			controls.DisposeEngine ();
			Application.Quit ();
		}
		
		
		
		// a critical error has occured which was not handled
		void unhandled_exception (object o, UnhandledExceptionEventArgs args) {
			Exception exception = (args.ExceptionObject as Exception);
			string text = exception.Message + "\n\n" + exception.ToString ();
			ThrowException (text);
		}
		
		
	}
}


