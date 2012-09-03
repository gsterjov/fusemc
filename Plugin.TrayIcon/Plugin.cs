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


namespace Fuse.Plugin.TrayIcon
{
		
	/// <summary>
	/// The main tray icon plugin.
	/// </summary>
	public class Plugin : IPlugin
	{
		
		string app_dir;
		bool plugin_initiated;
		IFuse fuse;
		
		// global widgets
		TrayIcon tray;
		Broadcaster broadcaster;
		
		
		
		/// <summary>
		/// Initiates the tray icon plugin.
		/// </summary>
		public void Initiate (IFuse fuse)
		{
			this.fuse = fuse;
			
			if (!plugin_initiated)
				createTrayIcon ();
			
			tray.ShowTray ();
			
			broadcaster = fuse.PluginCommunicator.RegisterPlugin ("Fuse.Plugin.TrayIcon", Version, broadcastHandler);
			fuse.Quiting += on_quit;
		}
		
		
		/// <summary>
		/// Disables the tray icon plugin.
		/// </summary>
		public void Deinitiate (IFuse fuse)
		{
			saveSettings ();
			tray.HideTray ();
			
			fuse.PluginCommunicator.UnregisterPlugin (broadcaster);
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
		/// Send messages to other plugins with this broadcaster.
		/// </summary>
		public Broadcaster Broadcaster
		{
			get{ return broadcaster; }
		}
		
		
		
		// create the theatre
		void createTrayIcon ()
		{
			app_dir = System.IO.Path.Combine (fuse.ConfigDir, "TrayIcon Plugin");
			
			tray = new TrayIcon (this);
			
			plugin_initiated = true;
		}
		
		
		
		void broadcastHandler (BroadcastEventArgs args)
		{
			switch (args.Command)
			{
				case "PopupWidget":
					Widget widget = (args.Object as Widget);
					if (widget != null)
						tray.PopupWidget (widget);
					break;
					
				case "StopTimer":
					if (tray.Popup != null)
						tray.Popup.StopTimer ();
					break;
					
				case "StartTimer":
					if (tray.Popup != null)
						tray.Popup.StartTimer ();
					break;
			}
		}
		
		
		
		// saves the theatre plugin settings
		void saveSettings ()
		{
		}
		
		
		
		// the application is quiting
		void on_quit (object o, EventArgs args)
		{
			saveSettings ();
		}
		
		
		
		// plugin description properties
		public string Name { get{ return "Tray Icon Plugin"; } }
		public string Description { get{ return "A tray icon and notifier"; } }
		public string Author { get{ return "Goran Sterjov"; } }
		public string Version { get{ return "0.2"; } }
		public string Website { get{ return "http://fusemc.sourceforge.net"; } }
		
	}
}
