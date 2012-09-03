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
using System.Web;
using Gtk;

namespace Fuse.Plugin.Library.Info.LyricWiki
{
	
	/// <summary>
	/// Lyrics loaded from the LyricWiki.org database.
	/// </summary>
	public class SongLyrics : Content
	{
		
		
		private QueryInfo last_query;
		
		
		//global widgets
		private VBox box = new VBox (false, 0);
		private Label artist_label = new Label ();
		private Label title_label = new Label ();
		private Label lyrics_label = new Label ();
		
		
		public SongLyrics ()
		{
			this.DisplayWidget = box;
			
			Alignment align = new Alignment (0, 0, 0, 0);
			align.Add (lyrics_label);
			align.BorderWidth = 10;
			
			box.PackStart (artist_label, false, false, 0);
			box.PackStart (title_label, false, false, 3);
			box.PackStart (new HSeparator (), false, false, 0);
			box.PackStart (align, true, true, 0);
			
			artist_label.Xalign = 0;
			title_label.Xalign = 0;
		}
		
		
		
		/// <summary>
		/// The query to execute.
		/// </summary>
		public override string GetQuery (QueryInfo query)
		{
			if (query.Equals (last_query, QueryField.Artist, QueryField.Title))
				return null;
			
			string url = "http://lyricwiki.org/api.php?artist={0}&song={1}&fmt=xml";
			return ParseQuery (url, query.Artist, query.Title);
		}
		
		
		
		/// <summary>
		/// Load the song lyrics.
		/// </summary>
		public override void Load (XmlDocument doc, QueryInfo query)
		{
			last_query = query;
			
			XmlNodeList list = doc.GetElementsByTagName ("LyricsResult");
			if (list.Count == 0)
				return;
			
			
			//look for the elements
			foreach (XmlNode node in list[0].ChildNodes)
			{
				switch (node.Name)
				{
					case "artist":
						artist_label.Markup = "<b><big>" + Utils.ParseMarkup (node.InnerText) + "</big></b>";
						break;
						
					case "song":
						title_label.Markup = "<i><big>" + Utils.ParseMarkup (node.InnerText) + "</big></i>\n";
						break;
						
					case "lyrics":
						lyrics_label.Markup = Utils.ParseMarkup (node.InnerText);
						break;
				}
			}
			
		}
		
		
		
		public override void ShowPage () {}
		
		
	}
}
