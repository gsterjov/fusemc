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

namespace Fuse.Plugin.Library.Info.AudioScrobbler.Profile
{
	
	/// <summary>
	/// A clickable box.
	/// </summary>
	public class TopArtistBox : ImageBox
	{
		
		
		private TopArtist artist;
		
		
		
		public TopArtistBox (TopArtist artist, WebService web_service) : base ()
		{
			this.artist = artist;
			
			
			this.Image.Pixbuf = web_service.LoadImage (artist.Image);
			
			Label name = new Label ();
			Label playcount = new Label ();
			
			name.Markup = "<b>" + Utils.ParseMarkup (artist.Name) + "</b>";
			playcount.Markup = "<small>Play Count: " + artist.PlayCount + "</small>";
			
			name.Xalign = 0;
			playcount.Xalign = 0;
			
			VBox box = new VBox (false, 2);
			box.PackStart (name, false, false, 0);
			box.PackStart (playcount, false, false, 0);
			
			this.InformationBox.Add (box);
		}
		
		
		
		/// <summary>
		/// The artist details.
		/// </summary>
		public TopArtist Artist
		{
			get{ return artist; }
		}
		
		
		
	}
}
