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

namespace Fuse.Plugin.Library.Info.LyricWiki
{
	
	/// <summary>
	/// A clickable box.
	/// </summary>
	public class AlbumBox : VBox
	{
		
		private Album album;
		private Lyrics main;
		private bool expanded = false;
		private string song_format = "  {0}. {1}";
		
		private EventBox box = new EventBox ();
		private VBox song_box = new VBox (false, 0);
		
		
		public AlbumBox (Album album, Lyrics main) : base (false, 0)
		{
			this.album = album;
			this.main = main;
			
			Label name = new Label ();
			Label year = new Label ();
			
			name.Markup = "<b>" + Utils.ParseMarkup (album.Name) + "</b>";
			year.Markup = "<small>" + album.Year + "</small>";
			
			name.Ellipsize = Pango.EllipsizeMode.End;
			name.Xalign = 0;
			year.Xalign = 0;
			
			
			VBox header = new VBox (false, 0);
			header.PackStart (name, false, false, 0);
			header.PackStart (year, false, false, 0);
			
			header.BorderWidth = 4;
			box.Add (header);
			
			this.PackStart (box, false, false, 0);
			
			
			//events
			box.Realized += realized;
			box.EnterNotifyEvent += mouse_enter;
			box.LeaveNotifyEvent += mouse_leave;
			box.ButtonReleaseEvent += mouse_clicked;
		}
		
		
		
		//change the mouse pointer in the hand
		private void realized (object o, EventArgs args)
		{
			box.GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Hand2);
		}
		
		//highlight on enter
		private void mouse_enter (object o, EnterNotifyEventArgs args)
		{
			box.State = StateType.Selected;
		}
		
		//remove highlight on leave
		private void mouse_leave (object o, LeaveNotifyEventArgs args)
		{
			box.State = StateType.Normal;
		}
		
		
		
		
		//a song link was clicked
		private void link_clicked (object o, ButtonReleaseEventArgs args)
		{
			LinkLabel label = (LinkLabel)o;
			
			QueryInfo query = new QueryInfo (Key.Artist(album.Artist), Key.Title(label.Link));
			main.LoadContent (query, typeof (SongLyrics));
		}
		
		
		
		//the album box was clicked
		private void mouse_clicked (object o, ButtonReleaseEventArgs args)
		{
			if (expanded)
			{
				foreach (Widget child in song_box.Children)
					song_box.Remove (child);
				
				this.Remove (song_box);
			}
			else
			{
				int i=1;
				foreach (string song in album.Songs)
				{
					string markup = String.Format (song_format, i++, Utils.ParseMarkup (song));
					LinkLabel link = new LinkLabel (markup, song);
					
					link.ButtonReleaseEvent += link_clicked;
					song_box.PackStart (link, false, false, 0);
				}
				
				this.PackStart (song_box, false, false, 0);
			}
			
			
			expanded = !expanded;
			this.ShowAll ();
			main.Resize ();
		}
		
	}
}
