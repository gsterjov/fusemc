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


namespace Fuse.Plugin.Library
{
		
	/// <summary>
	/// The main library widget.
	/// </summary>
	public class Core : IPlugin
	{
		
		private string app_dir;
		private bool plugin_initiated;
		private bool quickload = true;
		private IFuse fuse;
		private Config config;
		
		public event EventHandler PluginLoaded;
		
		
		// global widgets
		private TopBar topbar;
		private Library library;
		
		private VBox main_widget = new VBox (false, 0);
		private CheckButton quickload_button;
		
		
		
		
		/// <summary>
		/// Initiates the Library plugin.
		/// </summary>
		public void Initiate (IFuse fuse)
		{
			this.fuse = fuse;
			
			if (!plugin_initiated)
				createLibrary ();
			
			fuse.AddWidget (main_widget, "Library");
			fuse.Quiting += on_quit;
		}
		
		
		/// <summary>
		/// Disables the Library plugin.
		/// </summary>
		public void Deinitiate (IFuse fuse)
		{
			saveSettings ();
			fuse.RemoveWidget (main_widget);
			fuse.Quiting -= on_quit;
		}
		
		
		/// <summary>
		/// Initiates the Library plugin's options.
		/// </summary>
		public void InitiateOptions (IPluginOptions plugin_window)
		{
			VBox backbone = new VBox (false, 5);
			HBox note_box = new HBox (false, 0);
			
			quickload_button = new CheckButton ("Enable quick media library loading");
			Label note_header = new Label ();
			Label note_info = new Label ();
			
			quickload_button.Active = quickload;
			note_header.Markup = "<b>Note:</b>";
			note_header.Yalign = 0;
			note_info.Markup = "<i>This significantly decreases\nthe load time of the plugin</i>";
			
			
			note_box.PackStart (note_header, false, false, 5);
			note_box.PackStart (note_info, false, false, 0);
			
			backbone.PackStart (quickload_button, false, false, 0);
			backbone.PackStart (note_box, false, false, 0);
			backbone.BorderWidth = 10;
			
			quickload_button.Toggled += quick_load_toggle;
			
			plugin_window.AddOptionsWidget (backbone);
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
		/// Contains the media library.
		/// </summary>
		public Library Library
		{
			get{ return library; }
		}
		
		/// <summary>
		/// Provides basic functionality for the library.
		/// </summary>
		public TopBar TopBar
		{
			get{ return topbar; }
		}
		
		
		/// <summary>
		/// Is quick load enabled.
		/// </summary>
		public bool QuickLoad
		{
			get{ return quickload; }
		}
		
		
		/// <summary>
		/// The plugin configuration.
		/// </summary>
		public Config Config
		{
			get{ return config; }
		}
		
		
		
		/// <summary>
		/// The broadcaster for the tray icon plugin.
		/// </summary>
		public Broadcaster TrayIcon
		{
			get{ return fuse.PluginCommunicator.GetBroadcaster ("Fuse.Plugin.TrayIcon", "0.2"); }
		}
		
		
		/// <summary>
		/// Send a command to a plugin.
		/// </summary>
		public void SendCommand (Broadcaster receiver, string command, object obj)
		{
			if (receiver != null)
				receiver.SendCommand (command, obj);
		}
		
		
		/// <summary>
		/// Popup a widget under the tray icon plugin.
		/// </summary>
		public void PopupWidget (Widget widget)
		{
			SendCommand (TrayIcon, "PopupWidget", widget);
		}
		
		
		
		
		// create the library
		void createLibrary ()
		{
			Global.Core = this;
			
			app_dir = System.IO.Path.Combine (fuse.ConfigDir, "Library Plugin");
			config = new Config (app_dir);
			
			topbar = new TopBar ();
			library = new Library ();
			
			main_widget.PackStart (topbar, false, false, 3);
			main_widget.PackStart (library.MainSplitter, true, true, 3);
			main_widget.BorderWidth = 5;
			
			plugin_initiated = true;
			
			loadSettings ();
			if (PluginLoaded != null)
				PluginLoaded (this, new EventArgs ());
			
			Loader loader = new Loader ();
			loader.Load ();
			
			bool expand_all = config.Window.GetBoolean ("Library Expanded", true);
			if (expand_all)
				library.FolderTree.ExpandAll ();
		}
		
		
		
		
		// saves the media library settings
		void saveSettings ()
		{
			bool expanded = library.FolderTree.GetRowExpanded (new TreePath ("0"));
			config.Window.Set ("Library Expanded", expanded);
			
			config.Options.Set ("Quickload", quickload);
			config.Window.Set ("Search Bar", topbar.Search.Text);
			config.Window.Set ("Splitter Position", library.MainSplitter.Position);
			config.Window.Set ("Information Bar Splitter Position", library.InfoSplitter.Position);
			
			config.TreeColumns.Set ("Artist Width", library.MediaTree.Columns[0].Width);
			config.TreeColumns.Set ("Title Width", library.MediaTree.Columns[1].Width);
			config.TreeColumns.Set ("Album Width", library.MediaTree.Columns[2].Width);
			
			foreach (TreeViewColumn column in library.MediaTree.Columns)
			{
				if (column.SortIndicator)
				{
					config.Sorting.Set ("Column", column.Title);
					config.Sorting.Set ("Direction", column.SortOrder);
					break;
				}
			}
			
			config.Save ();
		}
		
		
		// loads the media library settings
		void loadSettings ()
		{
			quickload = config.Options.GetBoolean ("Quickload", true);
			topbar.Search.Text = config.Window.Get ("Search Bar", "");
			
			library.MainSplitter.Position = config.Window.GetInt ("Splitter Position", 100);
			library.InfoSplitter.Position = config.Window.GetInt ("Information Bar Splitter Position", -1);
			
			library.MediaTree.Columns[0].FixedWidth = config.TreeColumns.GetInt ("Artist Width", 100);
			library.MediaTree.Columns[1].FixedWidth = config.TreeColumns.GetInt ("Title Width", 100);
			library.MediaTree.Columns[2].FixedWidth = config.TreeColumns.GetInt ("Album Width", 100);
			
			string sorted_column = config.Sorting.Get ("Column", "None");
			string sorted_direction = config.Sorting.Get ("Direction", "Descending");
			
			foreach (TreeViewColumn column in library.MediaTree.Columns)
			{
				if (column.Title == sorted_column)
				{
					column.SortIndicator = true;
					
					if (sorted_direction == "Descending")
						column.SortOrder = SortType.Descending;
					else if (sorted_direction == "Ascending")
						column.SortOrder = SortType.Ascending;
					
					break;
				}
			}
			
			if (!quickload)
				library.MediaTree.ThawTree ();
		}
		
		
		// when the application is quiting
		void on_quit (object o, EventArgs args)
		{
			saveSettings ();
		}
		
		
		
		// the quick load option has changed
		void quick_load_toggle (object o, EventArgs args)
		{
			quickload = quickload_button.Active;
		}
		
		
		
		// plugin description properties
		public string Name { get{ return "Media Library Plugin"; } }
		public string Description { get{ return "A full featured media library"; } }
		public string Author { get{ return "Goran Sterjov"; } }
		public string Version { get{ return "0.4"; } }
		public string Website { get{ return "http://fusemc.sourceforge.net"; } }
		
	}
}
