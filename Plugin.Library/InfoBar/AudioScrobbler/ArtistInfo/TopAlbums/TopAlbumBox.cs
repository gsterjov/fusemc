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
	public class TopAlbumBox : ImageBox
	{
		
		
		private TopAlbum album;
		
		
		
		public TopAlbumBox (TopAlbum album, WebService web_service, int rank) : base ()
		{
			this.album = album;
			
			Gdk.Pixbuf pic = web_service.LoadImage (album.Image);
			this.Image.Pixbuf = pic.ScaleSimple (45, 45, Gdk.InterpType.Bilinear);
			
			
			Label name = new Label ();
			VBox info_box = new VBox (false, 5);
			
			name.Markup = rank + ". " + Utils.ParseMarkup (album.Name);
			name.Ellipsize = Pango.EllipsizeMode.End;
			name.Xalign = 0;
			
			info_box.PackStart (name, false, false, 0);
			this.InformationBox.Add (info_box);
		}
		
		
		
		/// <summary>
		/// The album details.
		/// </summary>
		public TopAlbum Album
		{
			get{ return album; }
		}
		
		
		
	}
}
