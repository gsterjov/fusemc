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
	public class SimilarTracks : Content
	{
		
		private VBox tracks_box = new VBox (false, 0);
		private PagedList <SimilarTrack> similar_tracks = new PagedList <SimilarTrack> ();
		private PageNavigator <SimilarTrack> page_navigator;
		
		
		private QueryInfo last_query;
		private VBox box = new VBox (false, 20);
		
		
		
		public SimilarTracks ()
		{
			Label title_label = new Label ();
			title_label.Markup = "<big><b>Similar Tracks:</b></big>";
			title_label.Xalign = 0;
			
			box.PackStart (title_label, false, false, 0);
			box.PackStart (tracks_box, false, false, 0);
			
			
			page_navigator = new PageNavigator <SimilarTrack> (similar_tracks);
			
			
			VBox page_box = new VBox (false, 10);
			page_box.PackStart (box, false, false, 0);
			page_box.PackStart (page_navigator, false, false, 0);
			
			similar_tracks.AmountToShow = 10;
			this.DisplayWidget = page_box;
			
			
			page_navigator.PageChanged += page_changed;
		}
		
		
		
		/// <summary>
		/// The query to execute.
		/// </summary>
		public override string GetQuery (QueryInfo query)
		{
			if (query.Equals (last_query, QueryField.Artist, QueryField.Title))
				return null;
			
			string url = "http://ws.audioscrobbler.com/1.0/track/{0}/{1}/similar.xml";
			return ParseQuery (url, query.Artist, query.Title);
		}
		
		
		
		
		/// <summary>
		/// Load the similar tracks.
		/// </summary>
		public override void Load (XmlDocument doc, QueryInfo query)
		{
			last_query = query;
			similar_tracks.Clear ();
			
			
			XmlNodeList list = doc.GetElementsByTagName ("similartracks");
			if (list.Count == 0)
				return;
			
			foreach (XmlNode node in list[0].ChildNodes)
				if (node.LocalName == "track")
					similar_tracks.Add (new SimilarTrack (node.ChildNodes));
			
			
			page_navigator.UpdatePageNumber ();
			ShowPage ();
		}
		
		
		
		/// <summary>
		/// Shows the current page.
		/// </summary>
		public override void ShowPage ()
		{
			foreach (Widget widget in tracks_box.Children)
				tracks_box.Remove (widget);
			
			
			//add the top tracks
			tracks_box.PackStart (new HSeparator (), false, false, 0);
			
			foreach (SimilarTrack track in similar_tracks.CurrentPage)
			{
				SimilarTrackBox event_box = new SimilarTrackBox (track);
				
				tracks_box.PackStart (event_box, false, false, 0);
				tracks_box.PackStart (new HSeparator (), false, false, 0);
			}
			
			
			box.ShowAll ();
		}
		
		
		
		//the page was changed
		private void page_changed (object o, EventArgs args)
		{
			ShowPage ();
		}
		
		
	}
	
	
	
}
