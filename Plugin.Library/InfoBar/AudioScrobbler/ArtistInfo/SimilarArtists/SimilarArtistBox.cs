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
	public class SimilarArtistBox : ImageBox
	{
		
		
		private SimilarArtist artist;
		
		
		
		public SimilarArtistBox (SimilarArtist artist, WebService web_service) : base ()
		{
			this.artist = artist;
			
			this.Image.Pixbuf = web_service.LoadImage (artist.Image);
			
			
			Label name = new Label ();
			Label match = new Label ();
			VBox info_box = new VBox (false, 5);
			
			name.Markup = "<b>" + Utils.ParseMarkup (artist.Name) + "</b>";
			match.Markup = "<small>Similarity: <b>% " + artist.Match + "</b></small>";
			
			name.Xalign = 0;
			match.Xalign = 0;
			
			info_box.PackStart (name, false, false, 0);
			info_box.PackStart (match, false, false, 0);
			
			this.InformationBox.Add (info_box);
		}
		
		
		
		/// <summary>
		/// The artist details.
		/// </summary>
		public SimilarArtist Artist
		{
			get{ return artist; }
		}
		
		
		
	}
}
