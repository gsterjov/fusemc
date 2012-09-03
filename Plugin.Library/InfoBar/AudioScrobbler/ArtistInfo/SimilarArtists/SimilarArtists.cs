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
	public class SimilarArtists : Content
	{
		
		private ArtistInfo main;
		private PagedList <SimilarArtist> similar_artists = new PagedList <SimilarArtist> ();
		private PageNavigator <SimilarArtist> page_navigator;
		
		
		private QueryInfo last_query;
		private VBox box = new VBox (false, 0);
		
		
		public SimilarArtists (ArtistInfo main)
		{
			this.main = main;
			
			page_navigator = new PageNavigator <SimilarArtist> (similar_artists);
			
			
			VBox page_box = new VBox (false, 10);
			page_box.PackStart (box, false, false, 0);
			page_box.PackStart (page_navigator, false, false, 0);
			
			similar_artists.AmountToShow = 5;
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
			
			string url = "http://ws.audioscrobbler.com/1.0/artist/{0}/similar.xml";
			return ParseQuery (url, query.Artist);
		}
		
		
		
		
		/// <summary>
		/// Load the similar artists.
		/// </summary>
		public override void Load (XmlDocument doc, QueryInfo query)
		{
			last_query = query;
			similar_artists.Clear ();
			
			
			XmlNodeList list = doc.GetElementsByTagName ("similarartists");
			if (list.Count == 0)
				return;
			
			foreach (XmlNode node in list[0].ChildNodes)
				if (node.LocalName == "artist")
					similar_artists.Add (new SimilarArtist (node.ChildNodes));
			
			
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
			
			this.ShowLoading ();
			
			
			//add the similar artists
			box.PackStart (new HSeparator (), false, false, 2);
			
			foreach (SimilarArtist artist in similar_artists.CurrentPage)
			{
				SimilarArtistBox event_box = new SimilarArtistBox (artist, this);
				event_box.ButtonReleaseEvent += artist_clicked;
				
				box.PackStart (event_box, false, false, 0);
				box.PackStart (new HSeparator (), false, false, 2);
			}
			
			
			box.ShowAll ();
			this.HideLoading ();
		}
		
		
		
		//an artist was selected
		private void artist_clicked (object o, ButtonReleaseEventArgs args)
		{
			SimilarArtistBox box = (SimilarArtistBox)o;
			
			QueryInfo query = new QueryInfo (Key.Artist(box.Artist.Name));
			main.LoadArtist (query);
		}
		
		
		
		//the page was changed
		private void page_changed (object o, EventArgs args)
		{
			ShowPage ();
		}
		
		
	}
	
	
	
}
