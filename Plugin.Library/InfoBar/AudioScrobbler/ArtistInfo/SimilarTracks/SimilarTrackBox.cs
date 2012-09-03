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
using System.Net;
using Gtk;

namespace Fuse.Plugin.Library.Info.AudioScrobbler.ArtistInfo
{
	
	/// <summary>
	/// A clickable box.
	/// </summary>
	public class SimilarTrackBox : HBox
	{
		
		private EventBox eb = new EventBox ();
		
		
		public SimilarTrackBox (SimilarTrack track) : base (false, 0)
		{
			
			Label name = new Label ();
			Label match = new Label ();
			VBox box = new VBox (false, 2);
			
			name.Markup = Utils.ParseMarkup (track.Artist) + " - " + Utils.ParseMarkup (track.Title);
			match.Markup = "<small>Similarity: <b>% " + track.Match + "</b></small>";
			
			
			name.Ellipsize = Pango.EllipsizeMode.End;
			
			name.Xalign = 0;
			name.Yalign = 1;
			match.Xalign = 0;
			match.Yalign = 0;
			
			box.PackStart (name, false, false, 0);
			box.PackStart (match, false, false, 0);
			
			eb.Add (box);
			this.PackStart (eb, true, true, 0);
			
			eb.BorderWidth = 2;
			eb.Realized += realized;
			eb.EnterNotifyEvent += mouse_enter;
			eb.LeaveNotifyEvent += mouse_leave;
		}
		
		
		
		//change the mouse pointer in the hand
		private void realized (object o, EventArgs args)
		{
			eb.GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Hand2);
		}
		
		//highlight on enter
		private void mouse_enter (object o, EnterNotifyEventArgs args)
		{
			eb.State = StateType.Selected;
		}
		
		//remove highlight on leave
		private void mouse_leave (object o, LeaveNotifyEventArgs args)
		{
			eb.State = StateType.Normal;
		}
		
		
	}
}
