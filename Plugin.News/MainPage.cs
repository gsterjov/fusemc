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


namespace Fuse.Plugin.News
{
		
	/// <summary>
	/// The main news widget.
	/// </summary>
	public class MainPage : IPlugin
	{
		
		string app_dir;
		bool plugin_initiated;
		IFuse fuse;
		Config config;
		
		int refresh = 20;
		Timer timer = new Timer ();
		
		
		// global widgets
		VBox main_widget = new VBox (false, 0);
		SpinButton autorefresh;
		
		TopBar top_bar;
		News news;
		DataManager db;
		
		
		
		/// <summary>
		/// Initiates the News plugin.
		/// </summary>
		public void Initiate (IFuse fuse)
		{
			this.fuse = fuse;
			
			if (!plugin_initiated)
				createNews ();
			
			fuse.AddWidget (main_widget, "News");
			fuse.Quiting += on_quit;
			
			
			timer.Elapsed += refresh_timer;
			int timeout = (refresh * 60) * 1000;
			timer.Start (timeout);
		}
		
		
		/// <summary>
		/// Disables the News plugin.
		/// </summary>
		public void Deinitiate (IFuse fuse)
		{
			saveSettings ();
			fuse.RemoveWidget (main_widget);
			fuse.Quiting -= on_quit;
			timer.Elapsed -= refresh_timer;
			timer.Stop ();
		}
		
		
		/// <summary>
		/// Initiates the News plugin's options.
		/// </summary>
		public void InitiateOptions (IPluginOptions plugin_window)
		{
			VBox backbone = new VBox (false, 5);
			HBox note_box = new HBox (false, 5);
			
			Label header = new Label ();
			Label note = new Label ();
			autorefresh = new SpinButton (10, 120, 5);
			
			autorefresh.Value = refresh;
			autorefresh.Changed += refresh_changed;
			header.Markup = "How often should news feeds <i>auto-refresh</i>:";
			note.Markup = "<b>Note:</b> <i>Requires Restart</i>";
			
			
			note_box.PackStart (autorefresh, false, false, 0);
			note_box.PackStart (note, true, true, 0);
			
			backbone.PackStart (header, false, false, 0);
			backbone.PackStart (note_box, false, false, 0);
			backbone.BorderWidth = 10;
			
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
		/// The Top Bar of the news plugin.
		/// </summary>
		public TopBar TopBar
		{
			get{ return top_bar; }
		}
		
		
		/// <summary>
		/// The main news section of the plugin.
		/// </summary>
		public News News
		{
			get{ return news; }
		}
		
		
		
		/// <summary>
		/// Provides database functionality for the news plugin.
		/// </summary>
		public DataManager DataManager
		{
			get{ return db; }
		}
		
		
		/// <summary>
		/// How often to auto-refresh.
		/// </summary>
		public int AutoRefresh
		{
			get{ return refresh; }
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
		
		
		
		// create the news widget
		void createNews ()
		{
			app_dir = System.IO.Path.Combine (fuse.ConfigDir, "News Plugin");
			config = new Config (app_dir);
			
			db = new DataManager (this);
			top_bar = new TopBar (this);
			news = new News (this);
			
			
			main_widget.PackStart (top_bar, false, false, 3);
			main_widget.PackStart (news.MainSplitter, true, true, 3);
			main_widget.BorderWidth = 5;
			
			plugin_initiated = true;
			
			
			
			news.MainSplitter.Position = config.Window.GetInt ("News Splitter", 100);
			refresh = config.Options.GetInt ("Auto-Refresh", 20);
			
			news.LoadData ();
		}
		
		
		
		
		// saves the news plugin settings
		void saveSettings ()
		{
			config.Window.Set ("News Splitter", news.MainSplitter.Position);
			config.Options.Set ("Auto-Refresh", refresh);
			config.Save ();
		}
		
		
		
		// the application is quiting
		void on_quit (object o, EventArgs args)
		{
			saveSettings ();
		}
		
		
		// when the autorefresh value is changed;
		void refresh_changed (object o, EventArgs args)
		{
			refresh = (int) autorefresh.Value;
		}
		
		
		
		// the autorefresh timer
		private void refresh_timer ()
		{
			news.AutoRefresh ();
		}
		
		
		
		
		// plugin description properties
		public string Name { get{ return "News Plugin"; } }
		public string Description { get{ return "A news aggregator"; } }
		public string Author { get{ return "Goran Sterjov"; } }
		public string Version { get{ return "0.2"; } }
		public string Website { get{ return "http://fusemc.sourceforge.net"; } }
		
	}
}
