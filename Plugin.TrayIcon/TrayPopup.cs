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

namespace Fuse.Plugin.TrayIcon
{
	
	/// <summary>
	/// Shows the notification popup under the tray icon.
	/// </summary>
	public class TrayPopup
	{
		Timer timer = new Timer ();
		
		int timeout;
		int timeout_max = 3000;
		
		Window window;
		Widget parent;
		EventBox eb;
		
		
		
		// creates the notification window
		public TrayPopup (Widget parent, Widget widget)
		{
			this.parent = parent;
			
			window = new Window (WindowType.Popup);
			window.TypeHint = Gdk.WindowTypeHint.Notification;
			window.SkipTaskbarHint = true;
			window.SkipPagerHint = true;
			
			
			eb = new EventBox ();
			eb.Add (widget);
			
			timer.Elapsed += timer_count;
			
			eb.ExposeEvent += expose_window;
			eb.ButtonReleaseEvent += mouse_clicked;
			eb.EnterNotifyEvent += mouse_enter;
			eb.LeaveNotifyEvent += mouse_leave;
			eb.Realized += box_realized;
			
			window.Add (eb);
		}
		
		
		
		void box_realized (object o, EventArgs args)
		{
			movePopup ();
		}
		
		
		/// <summary>
		/// Show the popup window.
		/// </summary>
		public void Show ()
		{
			movePopup ();
			window.ShowAll ();
			timeout = 0;
			timer.Start (500);
		}
		
		
		/// <summary>
		/// Hides the popup window.
		/// </summary>
		public void Hide ()
		{
			timer.Stop ();
			timeout = 0;
			window.HideAll ();
		}
		
		
		
		/// <summary>
		/// Destroys the popup window.
		/// </summary>
		public void Destroy ()
		{
			Hide ();
			window.Destroy ();
			window.Dispose ();
		}
		
		
		/// <summary>
		/// Stops the timer which hides the popup.
		/// </summary>
		public void StopTimer ()
		{
			timer.Stop ();
			timeout = 0;
			timeout_max = 1000;
		}
		
		
		
		/// <summary>
		/// Starts the timer which hides the popup.
		/// </summary>
		public void StartTimer ()
		{
			timer.Start (500);
		}
		
		
		
		
		// move the popup to the right position
		void movePopup ()
		{
			int x, y, width, height, pop_width, pop_height;
			
			// get the size of various windows
			window.GetSize (out pop_width, out pop_height);
			parent.GdkWindow.GetOrigin (out x, out y);
			parent.Toplevel.GdkWindow.GetSize (out width, out height);
			width = parent.Toplevel.Screen.Width;
			
			
			// calculate position
			y += height + 4;
			x -= (pop_width / 2);
			
			int trim = x + pop_width;
			if (trim >= width)
			{
				x -= (trim - width);
				x -= 5;
			}
			else if (x <= 0)
				x = 5;
			
			
			height = parent.Toplevel.Screen.Height;
			trim = y + pop_height;
			if (trim > height)
			{
				y -= pop_height;
				y -= 8;
			}
			
			window.Move (x, y);
		}
		
		
		// close when time is up
		void timer_count ()
		{
			if (timeout < timeout_max)
				timeout += 500;
			else Hide ();
		}
		
		
		
		// user clicked on the window
		void mouse_clicked (object o, ButtonReleaseEventArgs args)
		{
			Hide ();
		}
		
		// mouse entered the window
		void mouse_enter (object o, EnterNotifyEventArgs args)
		{
			StopTimer ();
		}
		
		// mouse left the window
		void mouse_leave (object o, LeaveNotifyEventArgs args)
		{
			StartTimer ();
		}
		
		
		
		// make it.. presentable
		void expose_window (object o, ExposeEventArgs args)
		{
			int width, height;
			eb.GdkWindow.GetSize (out width, out height);
			eb.GdkWindow.DrawRectangle (eb.Style.ForegroundGC (eb.State), false, 0, 0, width-1, height-1);
		}
		
	}
}
