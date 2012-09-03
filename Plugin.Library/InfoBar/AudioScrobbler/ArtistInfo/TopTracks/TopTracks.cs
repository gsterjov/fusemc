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
	public class TopTracks : Content
	{
		
		private ArtistInfo main;
		private PagedList <TopTrack> top_tracks = new PagedList <TopTrack> ();
		private PageNavigator <TopTrack> page_navigator;
		
		
		private QueryInfo last_query;
		private VBox box = new VBox (false, 0);
		
		
		
		public TopTracks (ArtistInfo main)
		{
			this.main = main;
			
			page_navigator = new PageNavigator <TopTrack> (top_tracks);
			
			
			VBox page_box = new VBox (false, 10);
			page_box.PackStart (box, false, false, 0);
			page_box.PackStart (page_navigator, false, false, 0);
			
			top_tracks.AmountToShow = 10;
			this.DisplayWidget = page_box;
			
			
			page_navigator.PageChanged += page_changed;
		}
		
		
		
		/// <summary>
		/// The query to execute.
		/// </summary>
		public override string GetQuery (QueryInfo query)
		{
			if (query.Equals (last_query, QueryField.Artist))
				return null;
			
			string url = "http://ws.audioscrobbler.com/1.0/artist/{0}/toptracks.xml";
			return ParseQuery (url, query.Artist);
		}
		
		
		
		
		/// <summary>
		/// Load the top tracks.
		/// </summary>
		public override void Load (XmlDocument doc, QueryInfo query)
		{
			last_query = query;
			top_tracks.Clear ();
			
			
			XmlNodeList list = doc.GetElementsByTagName ("mostknowntracks");
			if (list.Count == 0)
				return;
			
			
			foreach (XmlNode node in list[0].ChildNodes)
				if (node.LocalName == "track")
					top_tracks.Add (new TopTrack (node.ChildNodes));
			
			
			page_navigator.UpdatePageNumber ();
			ShowPage ();
		}
		
		
		
		/// <summary>
		/// Shows the current page.
		/// </summary>
		public override void ShowPage ()
		{
			foreach (Widget widget in box.Children)
				box.Remove (widget);
			
			
			//add the top tracks
			box.PackStart (new HSeparator (), false, false, 0);
			int i=1;
			
			foreach (TopTrack track in top_tracks.CurrentPage)
			{
				
				int index = (top_tracks.PageNumber-1) * top_tracks.AmountToShow;
				index += i;
				
				string markup = index + ". " + Utils.ParseMarkup (track.Name);
				LinkLabel label = new LinkLabel (markup, track.Name);
				
				label.ButtonReleaseEvent += track_clicked;
				
				box.PackStart (label, false, false, 0);
				box.PackStart (new HSeparator (), false, false, 0);
				i++;
			}
			
			
			box.ShowAll ();
		}
		
		
		
		//a track was selected
		private void track_clicked (object o, ButtonReleaseEventArgs args)
		{
			LinkLabel label = (LinkLabel)o;
			
			QueryInfo query = new QueryInfo (Key.Artist(main.Artist), Key.Title(label.Link));
			main.LoadContent (query, typeof (SimilarTracks));
		}
		
		
		
		//the page was changed
		private void page_changed (object o, EventArgs args)
		{
			ShowPage ();
		}
		
		
	}
	
	
	
}
