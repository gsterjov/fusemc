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


namespace Fuse.Plugin.Library.Info.AudioScrobbler.Profile
{
	
	/// <summary>
	/// Profile info loaded from the AudioScrobbler.net database.
	/// </summary>
	public class Friends : Content
	{
		
		private QueryInfo last_query;
		private PagedList <Friend> list = new PagedList <Friend> ();
		private PageNavigator <Friend> page_navigator;
		
		private VBox box = new VBox (false, 0);
		
		
		
		public Friends ()
		{
			page_navigator = new PageNavigator <Friend> (list);
			
			
			VBox page_box = new VBox (false, 10);
			page_box.PackStart (box, false, false, 0);
			page_box.PackStart (page_navigator, false, false, 0);
			
			list.AmountToShow = 5;
			this.DisplayWidget = page_box;
			
			
			page_navigator.PageChanged += page_changed;
		}
		
		
		
		
		/// <summary>
		/// The query to execute.
		/// </summary>
		public override string GetQuery (QueryInfo query)
		{
			if (query.Equals (last_query, QueryField.Username))
				return null;
			
			string url = "http://ws.audioscrobbler.com/1.0/user/{0}/friends.xml";
			return ParseQuery (url, query.Username);
		}
		
		
		
		/// <summary>
		/// Load the friends list.
		/// </summary>
		public override void Load (XmlDocument doc, QueryInfo query)
		{
			this.last_query = query;
			list.Clear ();
			
			XmlNodeList node_list = doc.GetElementsByTagName ("friends");
			if (node_list.Count == 0)
				return;
			
			foreach (XmlNode node in node_list[0].ChildNodes)
				if (node.LocalName == "user")
					list.Add (new Friend (node));
			
			
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
			
			
			//add the friends
			box.PackStart (new HSeparator (), false, false, 2);
			
			foreach (Friend friend in list.CurrentPage)
			{
				FriendBox friend_box = new FriendBox (friend, this);
				
				box.PackStart (friend_box, false, false, 0);
				box.PackStart (new HSeparator (), false, false, 2);
			}
			
			box.ShowAll ();
			this.HideLoading ();
		}
		
		
		
		//the page was changed
		private void page_changed (object o, EventArgs args)
		{
			ShowPage ();
		}
		
		
	}
	
	
	
}
