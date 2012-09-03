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
using System.Xml;
using Gtk;

namespace Fuse.Plugin.Library.Info.AudioScrobbler.ArtistInfo
{
	
	/// <summary>
	/// Info loaded from the AudioScrobbler.net database.
	/// </summary>
	public class ArtistBox : Content
	{
		
		
		QueryInfo last_query;
		
		
		//global widgets
		private VBox box = new VBox (false, 0);
		
		private Label artist_label = new Label ();
		private Image artist_image = new Image ();
		
		
		
		public ArtistBox ()
		{
			this.DisplayWidget = box;
			
			BorderWidget image_border = new BorderWidget (artist_image);
			artist_label.Xalign = 0;
			
			box.PackStart (artist_label, false, false, 0);
			box.PackStart (image_border, false, false, 5);
		}
		
		
		
		
		/// <summary>The currently loaded artist.</summary>
		public string Artist
		{ get{ return last_query.Artist; } }
		
		
		
		/// <summary>
		/// The query to execute.
		/// </summary>
		public override string GetQuery (QueryInfo query)
		{
			if (query.Equals (last_query, QueryField.Artist))
				return null;
			
			string url = "http://ws.audioscrobbler.com/1.0/artist/{0}/similar.xml";
			return ParseQuery (url, query.Artist);
		}
		
		
		
		
		/// <summary>
		/// Load the artist details.
		/// </summary>
		public override void Load (XmlDocument doc, QueryInfo query)
		{
			last_query = query;
			
			XmlNodeList list = doc.GetElementsByTagName ("similarartists");
			if (list.Count == 0)
				return;
			
			
			string name = list[0].Attributes["artist"].Value;
			string image = list[0].Attributes["picture"].Value;
			
			artist_label.Markup = "<b><big><big>" + Utils.ParseMarkup (name) + "</big></big></b>";
			artist_image.Pixbuf = this.LoadImage (image);
		}
		
		
		
		public override void ShowPage () {}
		
		
	}
	
	
}
