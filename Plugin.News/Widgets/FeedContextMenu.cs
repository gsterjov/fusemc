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
using System.Threading;
using Gtk;

namespace Fuse.Plugin.News
{
	
	/// <summary>
	/// The context menu for the Feed tree.
	/// </summary>
	public class FeedContextMenu : Menu
	{
		Feed feed;
		MainPage page;
		
		
		// create the context menu
		public FeedContextMenu (Feed feed, MainPage page) : base ()
		{
			this.feed = feed;
			this.page = page;
			
			ImageMenuItem refresh_feed = new ImageMenuItem (Stock.Refresh, null);
			ImageMenuItem remove_feed = new ImageMenuItem (Stock.Remove, null);
			CheckMenuItem autorefresh = new CheckMenuItem ("Auto-Refresh");
			
			this.Add (refresh_feed);
			this.Add (autorefresh);
			this.Add (new SeparatorMenuItem ());
			this.Add (remove_feed);
			
			
			autorefresh.Active = feed.AutoRefresh;
			
			
			refresh_feed.Activated += refresh_activated;
			remove_feed.Activated += remove_activated;
			autorefresh.Toggled += autorefresh_toggled;
		}
		
		
		
		// refresh was clicked
		void refresh_activated (object o, EventArgs args)
		{
			Thread thread = new Thread (delegate(){ page.News.Refresh (feed); });
			thread.Start ();
			
			page.News.NewsViewer.Refresh ();
		}
		
		
		// remove was clicked
		void remove_activated (object o, EventArgs args)
		{
			page.News.Remove ();
		}
		
		
		// autorefresh was toggled
		void autorefresh_toggled (object o, EventArgs args)
		{
			feed.AutoRefresh = !feed.AutoRefresh;
			page.DataManager.UpdateFeed (feed);
		}
		
		
	}
}
