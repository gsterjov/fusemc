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


namespace Fuse.Plugin.Theatre
{
	
	/// <summary>
	/// The widget that shows the video controls in fullscreen.
	/// </summary>
	public class FullscreenControls : Window
	{
		
		// global widgets
		EventBox eb = new EventBox ();
		Button play = new Button ();
		SeekBar seek_bar = new SeekBar ();
		Label current_media = new Label ();
		Label current_time = new Label ();
		
		int pos_y = -1;
		
		
		// create the window
		public FullscreenControls () : base (WindowType.Popup)
		{
			HBox box = new HBox (false, 0);
			
			box.BorderWidth = 5;
			
			//buttons
			Button next = new Button ();
			Button previous = new Button ();
			
			play.Image = new Image (Stock.MediaPlay, IconSize.Button);
			next.Image = new Image (Stock.MediaNext, IconSize.Button);
			previous.Image = new Image (Stock.MediaPrevious, IconSize.Button);
			
			
			HBox info_box = new HBox (false, 0);
			info_box.PackStart (current_media, true, true, 0);
			info_box.PackStart (current_time, false, false, 0);
			
			VBox seek_box = new VBox (false, 0);
			seek_box.PackStart (info_box, false, true, 0);
			seek_box.PackStart (seek_bar, true, true, 0);
			
			
			box.PackStart (previous, false, false, 3);
			box.PackStart (play, false, false, 3);
			box.PackStart (next, false, false, 3);
			box.PackStart (seek_box, true, true, 3);
			eb.Add (box);
			this.Add (eb);
			
			this.Realized += realized;
			
			
			play.Clicked += play_clicked;
			next.Clicked += next_clicked;
			previous.Clicked += previous_clicked;
			seek_bar.PositionChanged += seek_changed;
			
			Global.Core.Fuse.MediaControls.MediaEngine.StateChanged += state_changed;
			Global.Core.Fuse.MediaControls.MediaTimer += media_timer;
			
			seek_bar.Idle = false;
		}
		
		
		/// <summary>
		/// The height of the fullscreen controls.
		/// </summary>
		public int PositionY
		{
			get{ return pos_y; }
		}
		
		
		
		// a media state has changed
		private void state_changed (StateEventArgs args)
		{
			if (args.State == MediaStatus.Playing)
				play.Image = new Image (Stock.MediaPause, IconSize.Button);
			else
				play.Image = new Image (Stock.MediaPlay, IconSize.Button);
		}
		
		
		// a media timer event has been raised
		private void media_timer (MediaTimerEventArgs args)
		{
			seek_bar.SetRange (0, args.Duration.TotalSeconds);
			seek_bar.Position = args.Position.TotalSeconds;
			
			string pretty_time = Utils.PrettyTime (args.Position) + " of " + Utils.PrettyTime (args.Duration);
			current_time.Markup = "<small>" + pretty_time + "</small>";
			
			current_media.Markup = "<small>" + Global.Core.Fuse.MediaControls.MediaInfo + "</small>";
		}
		
		
		
		// play the media
		private void play_clicked (object o, EventArgs args)
		{
			if (Global.Core.Fuse.MediaControls.MediaEngine.CurrentStatus == MediaStatus.Playing)
				Global.Core.Fuse.MediaControls.Pause ();
			else
				Global.Core.Fuse.MediaControls.Play ();
		}
		
		
		// go to the next media
		private void next_clicked (object o, EventArgs args)
		{
			Global.Core.Fuse.MediaControls.Next ();
		}
		
		
		// go to the previous media
		private void previous_clicked (object o, EventArgs args)
		{
			Global.Core.Fuse.MediaControls.Previous ();
		}
		
		
		
		// seek to the new position
		private void seek_changed (object o, EventArgs args)
		{
			Global.Core.Fuse.MediaControls.MediaEngine.Seek (TimeSpan.FromSeconds (seek_bar.Position));
		}
		
		
		
		// the widget window has been created
		private void realized (object o, EventArgs args)
		{
			resizeControls ();
		}
		
		
		// resize and move the controls
		private void resizeControls ()
		{
			int width, height;
			this.GetSize (out width, out height);
			this.pos_y = this.Screen.Height - height;
			
			this.Resize (this.Screen.Width, height);
			this.Move (0, pos_y);
		}
		
		
	}
}
