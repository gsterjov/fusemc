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


namespace Fuse.Plugin.Theatre
{
		
	/// <summary>
	/// The main theatre widget.
	/// </summary>
	public class Core : IPlugin
	{
		
		string app_dir;
		bool plugin_initiated;
		IFuse fuse;		
		Config config;
		
		
		// global widgets
		VBox main_widget = new VBox (false, 0);
		
		
		TopBar topbar;
		Theatre theatre;
		DataManager db;
		
		
		
		/// <summary>
		/// Initiates the Theatre plugin.
		/// </summary>
		public void Initiate (IFuse fuse)
		{
			this.fuse = fuse;
			
			if (!plugin_initiated)
				createTheatre ();
			
			fuse.AddWidget (main_widget, "Theatre");
			fuse.Quiting += on_quit;
		}
		
		
		/// <summary>
		/// Disables the Theatre plugin.
		/// </summary>
		public void Deinitiate (IFuse fuse)
		{
			saveSettings ();
			fuse.RemoveWidget (main_widget);
			fuse.Quiting -= on_quit;
		}
		
		
		/// <summary>
		/// Initiates the Theatre plugin's options.
		/// </summary>
		public void InitiateOptions (IPluginOptions plugin_window)
		{
		}
		
		
		
		/// <summary>
		/// The path to the plugins configuration.
		/// </summary>
		public string AppDir
		{
			get{ return app_dir; }
		}
		
		
		
		/// <summary>
		/// The main Fuse window.
		/// </summary>
		public IFuse Fuse
		{
			get{ return fuse; }
		}
		
		
		
		/// <summary>
		/// The main Theatre widget.
		/// </summary>
		public Theatre Theatre
		{
			get{ return theatre; }
		}
		
		
		/// <summary>
		/// Provides database functionality for the theatre.
		/// </summary>
		public DataManager DataManager
		{
			get{ return db; }
		}
		
		
		
		
		// create the theatre
		void createTheatre ()
		{
			Global.Core = this;
			
			app_dir = System.IO.Path.Combine (fuse.ConfigDir, "Theatre Plugin");
			config = new Config (app_dir);
			
			topbar = new TopBar ();
			theatre = new Theatre ();
			db = new DataManager ();
			
			main_widget.PackStart (topbar, false, false, 3);
			main_widget.PackStart (theatre.MainSplitter, true, true, 3);
			main_widget.BorderWidth = 5;
			
			plugin_initiated = true;
			
			
			theatre.MainSplitter.Position = config.Window.GetInt ("Theatre Splitter", 100);
			theatre.Visualisation = config.Theatre.GetString ("Visualisation", "");
			theatre.AspectRatio = config.Theatre.GetFloat ("Aspect Ratio", 0);
			
			theatre.LoadData ();
		}
		
		
		
		
		// saves the theatre plugin settings
		void saveSettings ()
		{
			config.Window.Set ("Theatre Splitter", theatre.MainSplitter.Position);
			config.Theatre.Set ("Visualisation", theatre.Visualisation);
			config.Theatre.Set ("Aspect Ratio", theatre.AspectRatio);
			config.Save ();
		}
		
		
		
		// the application is quiting
		void on_quit (object o, EventArgs args)
		{
			saveSettings ();
		}
		
		
		
		// plugin description properties
		public string Name { get{ return "Theatre Plugin"; } }
		public string Description { get{ return "A basic video player"; } }
		public string Author { get{ return "Goran Sterjov"; } }
		public string Version { get{ return "0.2"; } }
		public string Website { get{ return "http://fusemc.sourceforge.net"; } }
		
	}
}
