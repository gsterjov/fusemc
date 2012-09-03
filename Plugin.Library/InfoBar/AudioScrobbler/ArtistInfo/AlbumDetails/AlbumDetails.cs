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
using System.Collections.Generic;
using Gtk;

namespace Fuse.Plugin.Library.Info.AudioScrobbler.ArtistInfo
{
	
	/// <summary>
	/// Info loaded from the AudioScrobbler.net database.
	/// </summary>
	public class AlbumDetails : Content
	{
		
		private ArtistInfo main;
		private PagedList <TrackInfo> tracks = new PagedList <TrackInfo> ();
		
		private QueryInfo last_query;
		
		
		private Image album_image = new Image ();
		private Label title_label = new Label ();
		private Label release_label = new Label ();
		private VBox tracks_box = new VBox (false, 0);
		private VBox box = new VBox (false, 20);
		
		
		
		public AlbumDetails (ArtistInfo main)
		{
			this.main = main;
			this.DisplayWidget = box;
			
			
			title_label.Yalign = 1;
			title_label.Xalign = 0;
			release_label.Yalign = 0;
			release_label.Xalign = 0;
			
			VBox title = new VBox (false, 0);
			title.PackStart (title_label, true, true, 0);
			title.PackStart (release_label, true, true, 0);
			
			HBox header = new HBox (false, 10);
			header.PackStart (new BorderWidget (album_image), false, false, 0);
			header.PackStart (title, false, false, 0);
			
			box.PackStart (header, false, false, 0);
			box.PackStart (tracks_box, false, false, 0);
		}
		
		
		
		/// <summary>
		/// The query to execute.
		/// </summary>
		public override string GetQuery (QueryInfo query)
		{
			if (query.Equals (last_query, QueryField.Artist, QueryField.Album))
				return null;
			
			string url = "http://ws.audioscrobbler.com/1.0/album/{0}/{1}/info.xml";
			return ParseQuery (url, query.Artist, query.Album);
		}
		
		
		
		
		
		/// <summary>
		/// Load the album details.
		/// </summary>
		public override void Load (XmlDocument doc, QueryInfo query)
		{
			last_query = query;
			tracks.Clear ();
			
			
			XmlNodeList list = doc.GetElementsByTagName ("album");
			if (list.Count == 0)
				return;
			
			
			string release_date = null;
			string image = null;
			string title = list[0].Attributes["title"].Value;
			
			
			foreach (XmlNode node in list[0].ChildNodes)
			{
				switch (node.LocalName)
				{
					case "releasedate":
						release_date = node.InnerText;
						break;
						
					case "coverart":
						foreach (XmlNode cover_node in node.ChildNodes)
							if (cover_node.LocalName == "medium")
								image = cover_node.InnerText;
						break;
						
					case "tracks":
						foreach (XmlNode track_node in node.ChildNodes)
							tracks.Add (new TrackInfo (track_node));
						break;
				}
			}
			
			
			release_date = release_date.Trim ();
			
			int time_index = release_date.LastIndexOf (",");
			if (time_index > -1)
				release_date = release_date.Substring (0, time_index);
			
			
			//load the header information
			Gdk.Pixbuf pic = this.LoadImage (image);
			album_image.Pixbuf = pic.ScaleSimple (100, 100, Gdk.InterpType.Bilinear);
			title_label.Markup = "<b>" + Utils.ParseMarkup (title) + "</b>";
			release_label.Markup = "<small>Release Date: " + release_date + "</small>";
			
			
			
			
			foreach (Widget widget in tracks_box.Children)
				tracks_box.Remove (widget);
			
			
			//add the album tracks
			tracks_box.PackStart (new HSeparator (), false, false, 0);
			int i=1;
			
			foreach (TrackInfo track in tracks)
			{
				string markup = i + ". " + Utils.ParseMarkup (track.Name);
				LinkLabel label = new LinkLabel (markup, track.Name);
				
				label.ButtonReleaseEvent += track_clicked;
				
				tracks_box.PackStart (label, false, false, 0);
				tracks_box.PackStart (new HSeparator (), false, false, 0);
				i++;
			}
			
			
			ShowPage ();
		}
		
		
		
		/// <summary>
		/// Shows the current page.
		/// </summary>
		public override void ShowPage ()
		{
			box.ShowAll ();
		}
		
		
		
		
		//a track was selected
		private void track_clicked (object o, ButtonReleaseEventArgs args)
		{
			LinkLabel label = (LinkLabel)o;
			
			QueryInfo query = new QueryInfo (Key.Artist(main.Artist), Key.Title(label.Link));
			main.LoadContent (query, typeof (SimilarTracks));
		}
		
		
		
	}
	
}
