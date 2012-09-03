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
	/// The main panel for lyrics.
	/// </summary>
	public class Lyrics : InfoPanel
	{
		
		
		//global widgets
		private Notebook tabs = new Notebook ();
		private ContentTabs content_tabs = new ContentTabs ();
		private VBox box = new VBox (false, 0);
		private VBox lyrics_box = new VBox (false, 0);
		
		
		public Lyrics (InfoBar info_bar) : base (info_bar)
		{
			Viewport view = new Viewport ();
			SearchBox search_box = new SearchBox ();
			ScrolledWindow scroll = new ScrolledWindow ();
			
			
			box.PackStart (scroll, true, true, 0);
			box.PackStart (new HSeparator (), false ,false, 0);
			box.PackStart (search_box, false, true, 0);
			
			lyrics_box.PackStart (content_tabs.DisplayWidget, true, true, 0);
			lyrics_box.BorderWidth = 20;
			
			tabs.ShowTabs = false;
			tabs.ShowBorder = false;
			tabs.AppendPage (new LyricsHelp (), null);
			tabs.AppendPage (lyrics_box, null);
			
			
			content_tabs.AddContent (new SongLyrics ());
			content_tabs.AddContent (new SearchArtist (this));
			
			
			scroll.ShadowType = ShadowType.None;
			scroll.Add (view);
			
			view.ShadowType = ShadowType.None;
			view.Add (tabs);
			
			
			search_box.Search += search;
		}
		
		
		/// <summary>The lyrics panel widget.</summary>
		public override Widget DisplayWidget
		{ get{ return box; } }
		
		
		/// <summary>The lyrics panel type.</summary>
		public override Type InfoType
		{ get{ return this.GetType(); } }
		
		
		
		/// <summary>
		/// Load the specified media's lyrics.
		/// </summary>
		public override void LoadMedia (Media media)
		{
			QueryInfo query = new QueryInfo (media);
			LoadContent (query, typeof (SongLyrics));
		}
		
		
		/// <summary>
		/// Load the specified content info.
		/// </summary>
		public void LoadContent (QueryInfo query, Type content_type)
		{
			tabs.Page = tabs.PageNum (lyrics_box);
			
			foreach (Content content in content_tabs.ContentList)
				if (content.GetType() == content_type)
					content_tabs.LoadTab (content, query);
		}
		
		
		
		/// <summary>
		/// Resizes the tabs widget.
		/// </summary>
		public void Resize ()
		{
			content_tabs.Resize ();
		}
		
		
		
		//a search has been requested
		private void search (string search_string)
		{
			QueryInfo query = new QueryInfo (Key.Artist (search_string));
			LoadContent (query, typeof (SearchArtist));
		}
		
	}
}
