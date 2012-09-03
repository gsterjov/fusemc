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

namespace Fuse.Plugin.Library.Info.LyricWiki
{
	
	/// <summary>
	/// Artists searched from the LyricWiki.org database.
	/// </summary>
	public class SearchArtist : Content
	{
		
		Lyrics main;
		QueryInfo last_query;
		
		
		//global widgets
		private VBox box = new VBox (false, 0);
		private Label search_title = new Label ();
		private VBox album_box = new VBox (false, 0);
		
		
		public SearchArtist (Lyrics main)
		{
			this.main = main;
			this.DisplayWidget = box;
			
			search_title.Xalign = 0;
			box.PackStart (search_title, false, false, 0);
			box.PackStart (album_box, false, false, 0);
		}
		
		
		
		/// <summary>
		/// The query to execute.
		/// </summary>
		public override string GetQuery (QueryInfo query)
		{
			if (query.Equals (last_query, QueryField.Artist))
				return null;
			
			string url = "http://lyricwiki.org/api.php?artist={0}&fmt=xml";
			return ParseQuery (url, query.Artist);
		}
		
		
		
		/// <summary>
		/// Load the artist search.
		/// </summary>
		public override void Load (XmlDocument doc, QueryInfo query)
		{
			last_query = query;
			
			foreach (Widget widget in album_box.Children)
				album_box.Remove (widget);
			
			
			XmlNodeList list = doc.GetElementsByTagName ("getArtistResponse");
			if (list.Count == 0)
				return;
			
			
			string artist = null;
			XmlNodeList albums_node = null;
			
			
			//look for artist name
			foreach (XmlNode node in list[0].ChildNodes)
			{
				if (node.Name == "artist")
				{
					artist = node.InnerText;
					search_title.Markup = "<b><big><big>" + Utils.ParseMarkup (artist) + "</big></big></b>\n";
				}
				else if (node.Name == "albums")
					albums_node = node.ChildNodes;
			}
			
			
			if (albums_node == null || albums_node.Count == 0)
				return;
			
			
			string name = null;
			string year = null;
			
			album_box.PackStart (new HSeparator (), false, false, 2);
			
			
			foreach (XmlNode node in albums_node)
			{
				switch (node.Name)
				{
					case "album":
						name = node.InnerText;
						break;
						
					case "year":
						if (node.InnerText != "0")
							year = node.InnerText;
						break;
						
					case "songs":
						Album album = new Album (artist, name, year, node.ChildNodes);
						
						album_box.PackStart (new AlbumBox (album, main), false ,false, 0);
						album_box.PackStart (new HSeparator (), false, false, 2);
						
						name = null;
						year = null;
						break;
				}
			}
			
			
		}
		
		
		
		public override void ShowPage () {}
		
		
		
	}
}
