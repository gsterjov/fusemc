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

namespace Fuse.Plugin.Library.Info.AudioScrobbler.Profile
{
	
	/// <summary>
	/// The main panel for the Last.fm profile.
	/// </summary>
	public class Profile : InfoPanel
	{
		
		private ProfileInfo profile_header = new ProfileInfo ();
		private ProfileOptions options;
		
		
		//global widgets
		private Notebook tabs = new Notebook ();
		private ContentTabs content_tabs = new ContentTabs ();
		private VBox profile_box = new VBox (false, 10);
		private ScrolledWindow scroll = new ScrolledWindow ();
		
		private EventBox signout_box = new EventBox ();
		private HBox signout = new HBox (false, 5);
		
		
		public Profile (InfoBar info_bar) : base (info_bar)
		{
			Viewport view = new Viewport ();
			options = new ProfileOptions (this);
			
			profile_box.PackStart (profile_header.DisplayWidget, false, false, 0);
			profile_box.PackStart (content_tabs.DisplayWidget, true, true, 0);
			profile_box.BorderWidth = 20;
			
			content_tabs.AddContent (new RecommendedArtists (), "Recommendations");
			content_tabs.AddContent (new TopArtists (), "Top Artists");
			content_tabs.AddContent (new Friends (), "Friends");
			
			tabs.ShowTabs = false;
			tabs.ShowBorder = false;
			tabs.AppendPage (options.DisplayWidget, null);
			tabs.AppendPage (profile_box, null);
			
			scroll.ShadowType = ShadowType.None;
			scroll.Add (view);
			
			view.ShadowType = ShadowType.None;
			view.Add (tabs);
			
			
			Image signout_image = new Image (Stock.Disconnect, IconSize.Menu);
			Label signout_label = new Label ();
			signout_label.Markup = "<small>Sign Out</small>";
			
			signout.PackStart (signout_image, false, false, 0);
			signout.PackStart (signout_label, false, false, 0);
			
			info_bar.AddCustomMenu (signout_box);
			
			
			content_tabs.ContentChanged += content_changed;
			signout_box.Realized += signout_realized;
			signout_box.ButtonReleaseEvent += signout_released;
		}
		
		
		
		
		/// <summary>The profile panel widget.</summary>
		public override Widget DisplayWidget
		{ get{ return scroll; } }
		
		
		/// <summary>The profile panel type.</summary>
		public override Type InfoType
		{ get{ return this.GetType(); } }
		
		
		public override void LoadMedia (Media media) {}
		
		
		
		
		/// <summary>
		/// Load the specified profile info.
		/// </summary>
		public void LoadProfile ()
		{
			signout_box.Add (signout);
			signout_box.ShowAll ();
			tabs.ShowAll ();
			
			tabs.Page = tabs.PageNum (profile_box);
			QueryInfo query = new QueryInfo (Key.Username(options.Username));
			
			content_tabs.LoadContent (profile_header, query);
			content_tabs.LoadTab (query, typeof (RecommendedArtists));
		}
		
		
		
		//switch pages
		private void content_changed (Content content)
		{
			QueryInfo query = new QueryInfo (Key.Username(options.Username));
			content_tabs.LoadTab (content, query);
		}
		
		
		
		
		//make the close image clickable
		private void signout_realized (object o, EventArgs args)
		{
			signout_box.GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Hand2);
		}
		
		
		//close the lyric panel
		private void signout_released (object o, ButtonReleaseEventArgs args)
		{
			tabs.Page = tabs.PageNum (options.DisplayWidget);
			options.SignOut ();
			signout_box.Remove (signout);
		}
		
	}
}
