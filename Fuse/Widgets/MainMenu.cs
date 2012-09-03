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
using System.IO;
using System.Collections.Generic;
using Gtk;

namespace Fuse
{
	
	/// <summary>
	/// The main window.
	/// </summary>
	public class MainMenu : HBox
	{
		private FuseApp fuse;
		
		// global user interface widgets
		private MenuBar bar = new MenuBar ();
		private Menu file_menu = new Menu ();
		private Menu options_menu = new Menu ();
		private Menu help_menu = new Menu ();
		private CheckMenuItem shuffle_item;
		private CheckMenuItem crossfade_item;
		
		
		private List <Plugin> plugin_list = new List <Plugin> ();
		
		
		
		// creates the main menu bar
		public MainMenu (FuseApp fuse) : base (false, 0)
		{
			this.fuse = fuse;
			
			// the actual top menu
			MenuItem file_item = new MenuItem ("File");
			MenuItem options_item = new MenuItem ("Options");
			MenuItem help_item = new MenuItem ("Help");
			
			bar.Append (file_item);
			bar.Append (options_item);
			bar.Append (help_item);
			this.PackStart (bar, true, true, 0);
			
			
			file_item.Submenu = file_menu;
			options_item.Submenu = options_menu;
			help_item.Submenu = help_menu;
			
			// the top menu's children such as quit and about
			ImageMenuItem quit_item = new ImageMenuItem (Stock.Quit, null);
			
			ImageMenuItem plugins_item = new ImageMenuItem ("Plugins");
			ImageMenuItem engine_item = new ImageMenuItem ("Media Engines");
			shuffle_item = new CheckMenuItem ("Shuffle");
			crossfade_item = new CheckMenuItem ("Crossfade");
			
			
			ImageMenuItem about_item = new ImageMenuItem (Stock.About, null);
			
			
			// change menu item properties
			plugins_item.Image = new Image (Stock.Connect, IconSize.Menu);
			engine_item.Image = new Image (Stock.Connect, IconSize.Menu);
			
			
			
			// add menu items
			file_menu.Append (quit_item);
			
			options_menu.Append (plugins_item);
			options_menu.Append (engine_item);
			options_menu.Append (new Gtk.SeparatorMenuItem ());
			options_menu.Append (shuffle_item);
			options_menu.Append (crossfade_item);
			
			help_menu.Append (about_item);
			
			// event handling for the menu items
			plugins_item.Activated += plugins_item_activated;
			engine_item.Activated += engine_item_activated;
			shuffle_item.Toggled += shuffle_item_activated;
			crossfade_item.Toggled += crossfade_item_activated;
			
			about_item.Activated += about_item_activated;
		}
		
		
		
		
		/// <summary>
		/// Saves the activated plugins.
		/// </summary>
		public void SavePluginList ()
		{
			string path = System.IO.Path.Combine (fuse.ConfigDir, "plugin_list.txt");
			StreamWriter writer = new StreamWriter (path);
			
			foreach (Plugin plugin in plugin_list)
				if (plugin.Enabled)
					writer.WriteLine (plugin.Path);
			
			writer.Close ();
		}
		
		
		/// <summary>
		/// Loads the previously activated plugins.
		/// </summary>
		public void LoadPluginList ()
		{
			string path = System.IO.Path.Combine (fuse.ConfigDir, "plugin_list.txt");
			if (!File.Exists (path)) return;
			
			StreamReader reader = new StreamReader (path);
			while (!reader.EndOfStream)
			{
				string plugin_path = reader.ReadLine ();
				if (File.Exists (plugin_path))
					plugin_list.Add (new Plugin (plugin_path));
			}
			
			reader.Close ();
			
			
			// activate all the plugins
			foreach (Plugin plugin in plugin_list)
			{
				if (plugin.Load ())
				{
					plugin.Instance.Initiate (fuse);
					plugin.Enabled = true;
				}
			}
		}
		
		
		
		/// <summary>
		/// Loads the previously selected media engine.
		/// </summary>
		public void LoadEngine ()
		{
			string engine_path = fuse.Config.MediaControls.Get ("Engine Path", "None");
			MediaEngine engine = new MediaEngine (engine_path);
			
			if (File.Exists (engine_path) && engine.Load ())
			{
				fuse.Controls.Engine = engine;
				fuse.ChosenEngine = engine_path;
			}	
			else
			{
				fuse.Controls.Engine = null;
				engine_item_activated (null, null);
			}
		}
		
		
		
		/// <summary>
		/// Whether or not shuffle is on.
		/// </summary>
		public bool Shuffle
		{
			set{ shuffle_item.Active = value; }
		}
		
		
		/// <summary>
		/// Whether or not crossfade is on.
		/// </summary>
		public bool Crossfade
		{
			set{ crossfade_item.Active = value; }
		}
		
		
		
		
		// when the user wants to change plugins
		void plugins_item_activated (object o, EventArgs args)
		{
			PluginsWindow window = new PluginsWindow (fuse, plugin_list);
			window.Run ();
		}
		
		// when the user wants to change media engines
		void engine_item_activated (object o, EventArgs args)
		{
			EnginesWindow window = new EnginesWindow (fuse);
			window.Run ();
		}
		
		
		
		// when the user selects the shuffle option
		void shuffle_item_activated (object o, EventArgs args)
		{
			fuse.Controls.Shuffle = shuffle_item.Active;
		}
		
		// when the user selects the crossfade option
		void crossfade_item_activated (object o, EventArgs args)
		{
			fuse.Controls.Crossfade = crossfade_item.Active;
		}
		
		
		
		//create and show the about window
		void about_item_activated (object o, EventArgs args)
		{
			AboutDialog dialog = new AboutDialog ();
			dialog.Name = "Fuse Media Centre";
			dialog.Website = "http://launchpad.net/fusemc";
			dialog.Logo = new Gdk.Pixbuf (null, "fuse-logo.png");
			dialog.Icon = new Gdk.Pixbuf (null, "fuse-tray.png");
			
			dialog.Response += delegate (object sender, ResponseArgs response_args) {
				if (response_args.ResponseId == ResponseType.Cancel)
					dialog.Destroy ();
			};
			
			dialog.Run ();
			dialog.Destroy ();
		}
		
		
	}
}


