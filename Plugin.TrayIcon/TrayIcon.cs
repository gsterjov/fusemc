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
	/// Shows the tray icon.
	/// </summary>
	public class TrayIcon : EventBox
	{
		Timer timer = new Timer ();
		int timeout;
		int x, y;
		
		
		Egg.TrayIcon tray;
		Plugin plugin;
		TrayPopup popup;
		
		
		// create the tray icon
		public TrayIcon (Plugin plugin) : base ()
		{
			this.plugin = plugin;
			
			int width, height;
			IconFactory factory = new IconFactory ();
			factory.LookupIconSize (IconSize.Menu, out width, out height);
			
			Gdk.Pixbuf icon = new Gdk.Pixbuf (null, "fuse-tray.png", width, height);
			this.Add (new Image (icon));
			this.ButtonPressEvent += tray_click;
			this.EnterNotifyEvent += mouse_enter;
			this.LeaveNotifyEvent += mouse_leave;
			
			timer.Elapsed += timer_count;
			
			tray = new Egg.TrayIcon ("Fuse");
		}
		
		
		/// <summary>
		/// Hide the tray icon.
		/// </summary>
		public void HideTray ()
		{
			tray.Remove (this);
			tray.ShowAll ();
		}
		
		/// <summary>
		/// Show the tray icon.
		/// </summary>
		public void ShowTray ()
		{
			tray.Add (this);
			tray.ShowAll ();
		}
		
		
		/// <summary>
		/// Shows the specified widget within the notification popup.
		/// </summary>
		public void PopupWidget (Widget widget)
		{
			if (popup != null)
				popup.Destroy ();
			
			popup = new TrayPopup (this, widget);
			popup.Show ();
		}
		
		
		/// <summary>
		/// Shows the last widget within the notification popup.
		/// </summary>
		public void PopupWidget ()
		{
			if (popup != null)
				popup.Show ();
		}
		
		
		/// <summary>
		/// The popup widget.
		/// </summary>
		public TrayPopup Popup
		{
			get{ return popup; }
		}
		
		
		
		// make the main window appear and disappear
		void toggleUI ()
		{
			if (plugin.Fuse.MainWindow.Visible)
				plugin.Fuse.MainWindow.GetPosition (out x, out y);
			else
				plugin.Fuse.MainWindow.Move (x, y);
			
			plugin.Fuse.MainWindow.Visible = !plugin.Fuse.MainWindow.Visible;
			plugin.Fuse.MainWindow.SkipPagerHint = !plugin.Fuse.MainWindow.Visible;
		}
		
		
		
		// the context menu upon right clicking on the tray icon
		void showMenu (ButtonPressEventArgs args)
		{
			Menu menu = new Menu ();
			ImageMenuItem play = new ImageMenuItem (Stock.MediaPlay, null);
			ImageMenuItem pause = new ImageMenuItem (Stock.MediaPause, null);
			ImageMenuItem next = new ImageMenuItem (Stock.MediaNext, null);
			ImageMenuItem prev = new ImageMenuItem (Stock.MediaPrevious, null);
			ImageMenuItem quit = new ImageMenuItem (Stock.Quit, null);
			
			menu.Add (next);
			menu.Add (prev);
			
			if (plugin.Fuse.MediaControls.MediaEngine.CurrentStatus == MediaStatus.Playing)
				menu.Add (pause);
			else
				menu.Add (play);
			
			menu.Add (new SeparatorMenuItem ());
			menu.Add (quit);
			
			next.Activated += menu_next;
			prev.Activated += menu_prev;
			play.Activated += menu_play;
			pause.Activated += menu_pause;
			quit.Activated += menu_quit;
			
			menu.ShowAll ();
			menu.Popup (null, null, new MenuPositionFunc (getPos), args.Event.Button, args.Event.Time);
		}
		
		
		// function to set the position for the new context menu
		void getPos (Menu menu, out int x, out int y, out bool push_in)
		{
			push_in = true;
			int width, height;
			this.GdkWindow.GetSize (out width, out height);
			this.GdkWindow.GetOrigin (out x, out y);
			this.Toplevel.GdkWindow.GetSize (out width, out height);
			y += height + 2;
		}
		
		
		
		// the user clicked on the tray icon
		void tray_click (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 1)
				toggleUI ();
			else if (args.Event.Button == 3)
				showMenu (args);
		}
		
		
		// the user is hovering over the tray icon
		void mouse_enter (object o, EnterNotifyEventArgs args)
		{
			timeout = 0;
			timer.Start (500);
		}
		
		// the user left the tray icon
		void mouse_leave (object o, LeaveNotifyEventArgs args)
		{
			timer.Stop ();
		}
		
		
		
		// popups the notification window if the mouse is left hovered
		void timer_count ()
		{
			if (timeout < 500)
				timeout += 500;
			else
			{
				PopupWidget ();
				timer.Stop ();
			}
		}
		
		
		// play the next media file
		void menu_next (object o, EventArgs args)
		{
			plugin.Fuse.MediaControls.Next ();
		}
		
		// play the previous media file
		void menu_prev (object o, EventArgs args)
		{
			plugin.Fuse.MediaControls.Previous ();
		}
		
		// play the loaded media file
		void menu_play (object o, EventArgs args)
		{
			plugin.Fuse.MediaControls.Play ();
		}
		
		// pause the loaded media file
		void menu_pause (object o, EventArgs args)
		{
			plugin.Fuse.MediaControls.Pause ();
		}
		
		// quit the application
		void menu_quit (object o, EventArgs args)
		{
			plugin.Fuse.Quit ();
		}
		
		
	}
}
