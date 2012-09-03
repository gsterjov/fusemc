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
using System.IO;
using System.Reflection;
using Gtk;
using Gecko;


namespace Fuse.Plugin.News
{
		
	/// <summary>
	/// Shows the news on a feed in a newspaper fashion.
	/// </summary>
	public class NewsViewer
	{
		
		MainPage parent;
		WebControl html = new WebControl ();
		Feed feed = null;
		
		string template;
		int page_count = 0;
		int page_number = 0;
		int show_total = 4;
		
		
		// create the news viewer widget
		public NewsViewer (MainPage parent)
		{
			this.parent = parent;
			
			Stream template_stream = Assembly.GetExecutingAssembly().GetManifestResourceStream ("template.html");
			StreamReader reader = new StreamReader (template_stream);
			template = reader.ReadToEnd ();
			reader.Close ();
			template_stream.Close ();
		}
		
		
		
		
		/// <summary>
		/// Load the feed items into the html widget.
		/// </summary>
		public void LoadFeed (Feed feed)
		{
			this.feed = feed;
			page_number = 0;
			
			if (feed.Items.Count > 0)
			{
				double item_count = (double) feed.Items.Count;
				double show_count = (double) show_total+1;
				page_count = (int) Math.Ceiling (item_count / show_count);
			}
			else
				page_count = 0;
			
			
			showPage ();
		}

		
		/// <summary>
		/// Loads a single item into the html widget.
		/// </summary>
		public void LoadItem (Item item, Feed feed)
		{
			if (item == null || feed == null)
				return;
			
			
			page_number = 0;
			page_count = 0;
			this.feed = null;
			
			// render item into html
			string output = template.Replace ("@fuse_feed_name@", feed.Name);
			output = output.Replace ("@fuse_link@", item.Url);
			output = output.Replace ("@fuse_header@", item.Title);
			output = output.Replace ("@fuse_description@", item.Description);
			
			item.Read = true;
			parent.DataManager.UpdateItem (item);
			
			
			html.RenderData (output, feed.Url, "text/html");
			feed.UpdateStatus ();
		}
		
		
		/// <summary>
		/// Refreshes the current display.
		/// </summary>
		public void Refresh ()
		{
			showPage ();
		}
		
		
		
		/// <summary>
		/// Shows the previous page of the news feed.
		/// </summary>
		public void PreviousPage ()
		{
			if (page_number > 0)
			{
				page_number--;
				parent.TopBar.RefreshPageCount ();
				showPage ();
			}
		}
		
		
		
		/// <summary>
		/// Shows the next page of the news feed.
		/// </summary>
		public void NextPage ()
		{
			if (page_number < page_count-1)
			{
				page_number++;
				parent.TopBar.RefreshPageCount ();
				showPage ();
			}
		}
		
		
		
		/// <summary>
		/// Whether or not there is a previous page.
		/// </summary>
		public bool HasPrevious
		{
			get{ return page_number > 0; }
		}
		
		
		/// <summary>
		/// Whether or not there is a next page.
		/// </summary>
		public bool HasNext
		{
			get{ return page_number < page_count-1; }
		}
		
		
		/// <summary>
		/// The total amount of pages available in the feed.
		/// </summary>
		public int PageCount
		{
			get{ return page_count; }
		}
		
		
		/// <summary>
		/// The current page being displayed.
		/// </summary>
		public int PageNumber
		{
			get{ return page_number+1; }
		}
		
		
		
		
		/// <summary>
		/// The HTML widget displaying the news content.
		/// </summary>
		public WebControl HTML
		{
			get{ return html; }
		}
		
		
		
		
		//shows the current page
		void showPage ()
		{
			if (feed == null)
				return;
			
			int start_item = template.IndexOf ("<!-- START_ITEM_TEMPLATE -->");
			int end_item = template.IndexOf ("<!-- END_ITEM_TEMPLATE -->");
			
			
			string item_template = template.Substring (start_item, end_item - start_item);
			string output = template.Substring (0, start_item);
			
			
			//only get items for this page
			int index = feed.Items.Count-1 - (page_number * (show_total+1));
			
			for (int i=index; i>=index-show_total && i>=0; i--)
			{
				Item item = feed.Items [i];
				
				string item_details = item_template.Replace ("@fuse_link@", item.Url);
				item_details = item_details.Replace ("@fuse_header@", item.Title);
				item_details = item_details.Replace ("@fuse_description@", item.Description);
				output += item_details;
				item.Read = true;
				parent.DataManager.UpdateItem (item);
			}
			
			
			output += template.Substring (end_item);
			output = output.Replace ("@fuse_feed_name@", feed.Name);
			
			
			html.RenderData (output, feed.Url, "text/html");
			
			feed.UpdateStatus ();
		}
		
		
	}
}
