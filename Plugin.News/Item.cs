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
using RssReader;

namespace Fuse.Plugin.News
{
		
	/// <summary>
	/// Holds information about a news feed item.
	/// </summary>
	public class Item
	{
		
		string title;
		string description;
		string url;
		string guid;
		bool read;
		bool isNew;
		string pub_date;
		
		
		public Item (string title, string description, string url, string guid, bool read, string pub_date)
		{
			this.title = title;
			this.description = description;
			this.url = url;
			this.guid = guid;
			this.read = read;
			this.pub_date = pub_date;
		}
		
		
		public Item (RssItem item)
		{
			this.title = item.Title;
			this.description = item.Description;
			this.url = item.Link;
			this.guid = item.Guid;
			this.pub_date = item.PubDate;
		}
		
		
		
		/// <summary>
		/// The title of the news item.
		/// </summary>
		public string Title
		{
			get{ return title; }
		}
		
		
		
		/// <summary>
		/// The content of the news item.
		/// </summary>
		public string Description
		{
			get{ return description; }
		}
		
		
		
		/// <summary>
		/// The url of the news item.
		/// </summary>
		public string Url
		{
			get{ return url; }
		}
		
		
		
		/// <summary>
		/// The unique ID of the news item.
		/// </summary>
		public string GUID
		{
			get{ return guid; }
		}
		
		
		
		
		/// <summary>
		/// Has the news item already been viewed.
		/// </summary>
		public bool Read
		{
			get{ return read; }
			set{ read = value; }
		}
		
		
		/// <summary>
		/// Has this item already been in the popup.
		/// </summary>
		public bool IsNew
		{
			get{ return isNew; }
			set{ isNew = value; }
		}
		
		
		
		
		/// <summary>
		/// The date this item was published.
		/// </summary>
		public string PubDate
		{
			get{ return pub_date; }
		}
		
		
	}
}
