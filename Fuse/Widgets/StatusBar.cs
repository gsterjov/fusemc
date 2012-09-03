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

namespace Fuse
{
	
	public enum StatusIcon { Warning, None }
	
	
	/// <summary>
	/// The status bar which can dislpay various images.
	/// </summary>
	public class StatusBar : Statusbar
	{
		FuseApp fuse;
		VBox box;
		bool visible;
		
		Image image = new Image ();
		StatusIcon icon = StatusIcon.None;
		ListStore error_log = new ListStore (typeof (string), typeof (string));
		
		
		// create the status bar
		public StatusBar (FuseApp fuse, VBox box)
		{
			this.fuse = fuse;
			this.box = box;
			
			EventBox eb = new EventBox ();
			eb.Add (image);
			eb.ButtonReleaseEvent += notify_clicked;
			this.PackStart (eb, false, false, 2);
		}
		
		
		/// <summary>
		/// The application's error log.
		/// </summary>
		public ListStore ErrorLog
		{
			get{ return error_log; }
		}
		
		
		
		/// <summary>
		/// The current icon type being displayed.
		/// </summary>
		public StatusIcon Icon
		{
			get{ return icon; }
			set
			{
				icon = value;
				if (icon == StatusIcon.Warning)
					image.Stock = Stock.DialogWarning;
				else if (icon == StatusIcon.None)
					image.Clear ();
			}
		}
		
		
		/// <summary>
		/// Add a message onto the status bar stack.
		/// </summary>
		public void Push (string message)
		{
			this.Pop (0);
			this.Push (0, message);
			
			if (!visible)
			{
				box.PackStart (this, false, false, 0);
				box.ShowAll ();
				visible = true;
			}
		}
		
		/// <summary>
		/// Remove a message from the status bar stack.
		/// </summary>
		public void  Pop ()
		{
			this.Pop (0);
			
			if (visible)
			{
				box.Remove (this);
				box.ShowAll ();
				visible = false;
			}
		}
		
		
		// when the user clicks on the status bar image
		void notify_clicked (object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button != 1) return;
			
			if (icon == StatusIcon.Warning)
			{
				WarningWindow warning = new WarningWindow (fuse.Window, this);
				warning.Run ();
			}
		}
		
	}
}
