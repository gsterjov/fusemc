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
using Gtk;

namespace Fuse.Plugin.Library.Info.AudioScrobbler.ArtistInfo
{
	
	/// <summary>
	/// The main panel for the artist info panel.
	/// </summary>
	public class ArtistInfo : InfoPanel
	{
		
		private ArtistBox artist_header = new ArtistBox ();
		
		
		//global widgets
		private Notebook tabs = new Notebook ();
		private ContentTabs content_tabs = new ContentTabs ();
		
		private VBox box = new VBox (false, 0);
		private VBox artist_box = new VBox (false, 0);
		
		
		
		public ArtistInfo (InfoBar info_bar) : base (info_bar)
		{
			Viewport view = new Viewport ();
			SearchBox search_box = new SearchBox ();
			ScrolledWindow scroll = new ScrolledWindow ();
			
			box.PackStart (scroll, true, true, 0);
			box.PackStart (new HSeparator (), false ,false, 0);
			box.PackStart (search_box, false, true, 0);
			
			
			artist_box.PackStart (artist_header.DisplayWidget, false, false, 0);
			artist_box.PackStart (content_tabs.DisplayWidget, true, true, 0);
			artist_box.BorderWidth = 20;
			
			
			content_tabs.AddContent (new SimilarArtists(this), "Similar Artists");
			content_tabs.AddContent (new TopTracks(this), "Top Tracks");
			content_tabs.AddContent (new TopAlbums(this), "Top Albums");
			content_tabs.AddContent (new SimilarTracks());
			content_tabs.AddContent (new AlbumDetails(this));
			
			
			tabs.ShowTabs = false;
			tabs.ShowBorder = false;
			tabs.AppendPage (new ArtistInfoHelp (), null);
			tabs.AppendPage (artist_box, null);
			
			
			scroll.ShadowType = ShadowType.None;
			scroll.Add (view);
			
			view.ShadowType = ShadowType.None;
			view.Add (tabs);
			
			
			search_box.Search += search;
			content_tabs.ContentChanged += content_changed;
		}
		
		
		
		/// <summary>The audioscrobbler panel widget.</summary>
		public override Widget DisplayWidget
		{ get{ return box; } }
		
		
		/// <summary>The audioscrobbler panel type.</summary>
		public override Type InfoType
		{ get{ return this.GetType(); } }
		
		
		
		/// <summary>The currently loaded artist.</summary>
		public string Artist
		{ get{ return artist_header.Artist; } }
		
		
		
		/// <summary>
		/// Load the specified media artist's info.
		/// </summary>
		public override void LoadMedia (Media media)
		{
			QueryInfo query = new QueryInfo (media);
			LoadArtist (query);
		}
		
		
		/// <summary>
		/// Load the specified artist info.
		/// </summary>
		public void LoadArtist (QueryInfo query)
		{
			content_tabs.ActiveContent.ShowLoading ();
			tabs.Page = tabs.PageNum (artist_box);
			
			content_tabs.LoadContent (artist_header, query);
			content_tabs.LoadTab (query);
		}
		
		
		
		/// <summary>
		/// Load the specified content info.
		/// </summary>
		public void LoadContent (QueryInfo query, Type content_type)
		{
			foreach (Content content in content_tabs.ContentList)
				if (content.GetType() == content_type)
					content_tabs.LoadTab (content, query);
		}
		
		
		
		
		//the content has been changed
		private void content_changed (Content content)
		{
			QueryInfo query = new QueryInfo (Key.Artist(this.Artist));
			content_tabs.LoadTab (content, query);
		}
		
		
		//a search has been requested
		private void search (string search_string)
		{
			QueryInfo query = new QueryInfo (Key.Artist(search_string));
			LoadArtist (query);
		}
		
		
	}
}
