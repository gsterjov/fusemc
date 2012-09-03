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
	/// The popup for playing media.
	/// </summary>
	public class MediaPopup : HBox
	{
		
		Label time = new Label ();
		
		
		// creates the popup
		public MediaPopup (Media media, Gdk.Pixbuf cover_art) : base (false, 5)
		{
			VBox media_info = new VBox (false, 0);
			HBox controls = new HBox (false, 0);
			Image image;
			
			if (cover_art != null)
				image = new Image (cover_art);
			else
				image = new Image (null, "fuse-noart.png");
			
			
			Label artist = new Label ();
			Label title = new Label ();
			artist.Markup = "<b>" + Utils.ParseMarkup (media.Artist) + "</b>";
			title.Markup = Utils.ParseMarkup (media.Title);
			
			
			time.Markup = "<small>0:00 of 0:00</small>";
			
			
			Button previous = new Button ();
			Button next = new Button ();
			previous.Image = new Image (Stock.MediaPrevious, IconSize.Button);
			next.Image = new Image (Stock.MediaNext, IconSize.Button);;
			
			
			// button events
			previous.Clicked += previous_clicked;
			next.Clicked += next_clicked;
			
			this.DestroyEvent += destroy_widget;
			
			
			controls.PackStart (previous, false, false, 0);
			controls.PackStart (time, true, true, 20);
			controls.PackStart (next, false, false, 0);
			
			media_info.PackStart (artist, false, false, 0);
			media_info.PackStart (title, false, false, 5);
			media_info.PackStart (controls, false, false, 0);
			
			this.BorderWidth = 5;
			this.PackStart (image, false, false, 0);
			this.PackStart (media_info, false, false, 0);
			
			previous.EnterNotifyEvent += button_enter;
			previous.LeaveNotifyEvent += button_leave;
			next.EnterNotifyEvent += button_enter;
			next.LeaveNotifyEvent += button_leave;
			
			Global.Core.Fuse.MediaControls.MediaTimer += media_timer;
		}
		
		
		
		// a mouse entered a button
		void button_enter (object o, EventArgs args)
		{
			Global.Core.SendCommand (Global.Core.TrayIcon, "StopTimer", null);
		}
		
		// a mouse left a button
		void button_leave (object o, EventArgs args)
		{
			Global.Core.SendCommand (Global.Core.TrayIcon, "StartTimer", null);
		}
		
		
		
		
		// move back to the previous track
		void previous_clicked (object o, EventArgs args)
		{
			Global.Core.Fuse.MediaControls.Previous ();
		}
		
		
		// move forward to the next track
		void next_clicked (object o, EventArgs args)
		{
			Global.Core.Fuse.MediaControls.Next ();
		}
		
		
		// the widget is destroyed
		void destroy_widget (object o, EventArgs args)
		{
			Global.Core.Fuse.MediaControls.MediaTimer -= media_timer;
		}
		
		
		
		// update the current time
		private void media_timer (MediaTimerEventArgs args)
		{
			string pretty_time = Utils.PrettyTime (args.Position) + " of " + Utils.PrettyTime (args.Duration);
			time.Markup = "<small>" + pretty_time + "</small>";
		}
		
		
	}
}
