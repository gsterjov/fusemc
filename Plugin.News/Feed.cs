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
using System.Net;
using System.Collections.Generic;
using RssReader;

namespace Fuse.Plugin.News
{
		
	/// <summary>
	/// Holds information about a news feed.
	/// </summary>
	public class Feed
	{
		
		string name;
		string url;
		string etag;
		string last_modified;
		bool autorefresh;
		
		bool updating;
		string unread_status;
		int unread_count;
		int new_count;
		List <Item> items = new List <Item> ();
		
		
		
		// mainly used by the database
		public Feed (string name, string url, string etag, string last_modified, bool autorefresh)
		{
			this.name = name;
			this.url = url;
			this.etag = etag;
			this.last_modified = last_modified;
			this.autorefresh = autorefresh;
		}
		
		
		// loads the feed
		public Feed (RssFeed feed)
		{
			this.name = feed.Channel.Title;
			this.url = feed.URL;
			this.etag = feed.ETag;
			this.last_modified = feed.LastModified;
			
			
			for (int i=feed.Channel.Items.Count-1; i>=0; i--)
				items.Add (new Item (feed.Channel.Items [i]));
			
			UpdateStatus ();
		}
		
		
		
		
		
		/// <summary>
		/// Updates the current unread state.
		/// </summary>
		public void UpdateStatus ()
		{
			unread_count = 0;
			new_count = 0;
			
			foreach (Item item in items)
			{
				if (!item.Read)
					unread_count++;
				if (item.IsNew)
					new_count++;
			}
			
			
			unread_status = unread_count.ToString () + " Unread";
			if (unread_count > 0)
				unread_status = "<b>" + unread_status + "</b>";
		}
		
		
		
		
		/// <summary>
		/// The name of the news feed.
		/// </summary>
		public string Name
		{
			get{ return name; }
		}
		
		
		/// <summary>
		/// The url of the news feed.
		/// </summary>
		public string Url
		{
			get{ return url; }
		}
		
		
		
		/// <summary>
		/// A unique ID for the news feed.
		/// </summary>
		public string ETag
		{
			get{ return etag; }
			set{ etag = value; }
		}
		
		
		
		/// <summary>
		/// The last time the feed was modified.
		/// </summary>
		public string LastModified
		{
			get{ return last_modified; }
			set{ last_modified = value; }
		}
		
		
		
		/// <summary>
		/// How often to auto-refresh.
		/// </summary>
		public bool AutoRefresh
		{
			get{ return autorefresh; }
			set{ autorefresh = value; }
		}
		
		
		
		
		
		/// <summary>
		/// Is the current news feed being updated.
		/// </summary>
		public bool Updating
		{
			get{ return updating; }
			set{ updating = value; }
		}
		
		
		
		/// <summary>
		/// The amount of news items that havent been viewed in a presentable string.
		/// </summary>
		public string UnreadStatus
		{
			get{ return unread_status; }
		}
		
		
		/// <summary>
		/// The amount of news items that havent been viewed.
		/// </summary>
		public int UnreadCount
		{
			get{ return unread_count; }
		}
		
		
		/// <summary>
		/// The amount of news items that are new.
		/// </summary>
		public int NewCount
		{
			get{ return new_count; }
		}
		
		
		
		
		/// <summary>
		/// The items within the news feed.
		/// </summary>
		public List <Item> Items
		{
			get{ return items; }
		}
		
		
	}
}
