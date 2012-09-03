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


using System.Xml;
using System.Collections.Generic;

namespace RssReader
{
	
	
	/// <summary>
	/// The RSS channel class.
	/// </summary>
	public class RssChannel
	{
		
		
		string title;
		string link;
		string description;
		string pubdate;
		List <RssItem> items = new List <RssItem> ();
		
		
		
		public RssChannel (XmlNodeList node_list)
		{
			readChannel (node_list);
		}
		
		
		
		public string Title
		{
			get{ return title; }
		}
		
		
		public string Link
		{
			get{ return link; }
		}
		
		
		public string Description
		{
			get{ return description; }
		}
		
		
		public string PubDate
		{
			get{ return pubdate; }
		}
		
		
		public List <RssItem> Items
		{
			get{ return items; }
		}
		
		
		
		
		// reads the channel section of an rss document
		void readChannel (XmlNodeList node_list)
		{
			
			foreach (XmlNode node in node_list)
			{
				switch (node.Name.ToLower ())
				{
					case "title":
						title = node.InnerText;
						break;
					case "link":
						link = node.InnerText;
						break;
					case "description":
						description = node.InnerText;
						break;
					case "pubdate":
						pubdate = node.InnerText;
						break;
					
					case "item":
						RssItem item = new RssItem (node.ChildNodes);
						items.Add (item);
						break;
				}
			}
			
		}
		
		
	}
	
}