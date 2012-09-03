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
using System.Collections.Generic;
using Gtk;


namespace Fuse.Plugin.News
{
		
	/// <summary>
	/// The popup for new news feed items.
	/// </summary>
	public class NewsPopup : VBox
	{
		
		MainPage parent;
		
		List <Feed> feeds = new List <Feed> ();
		List <Item> unread_items = new List <Item> ();
		
		int feed_index = 0;
		int page_count = 0;
		int page_number = 0;
		int show_total = 4;
		LinkButton[] links;
		
		
		// global widgets
		Label unread = new Label ();
		Label page_label = new Label ();
		Label title = new Label ();
		
		Button feed_prev = new Button ();
		Button feed_next = new Button ();
		Button page_prev = new Button ();
		Button page_next = new Button ();
		
		VBox item_box = new VBox (false, 0);
		
		
		// creates the popup
		public NewsPopup (List <Feed> feeds, MainPage parent) : base (false, 5)
		{
			this.feeds = feeds;
			this.parent = parent;
			
			HBox feed_box = new HBox (false, 3);
			VBox title_box = new VBox (false, 0);
			HBox page_box = new HBox (false, 0);
			
			
			//the top feed browsing bar
			feed_prev.Image = new Image (Stock.GoBack, IconSize.Button);
			feed_next.Image = new Image (Stock.GoForward, IconSize.Button);
			
			title_box.PackStart (title, false, false, 0);
			title_box.PackStart (unread, false, false, 0);
			
			feed_box.PackStart (feed_prev, false, false, 0);
			feed_box.PackStart (title_box, true, true, 0);
			feed_box.PackStart (feed_next, false, false, 0);
			
			
			//the bottom page browsing bar
			page_prev.Image = new Image (Stock.GoBack, IconSize.Button);
			page_next.Image = new Image (Stock.GoForward, IconSize.Button);
			
			
			page_box.PackStart (page_prev, false, false, 0);
			page_box.PackStart (page_label, true, true, 0);
			page_box.PackStart (page_next, false, false, 0);
			
			
			this.BorderWidth = 5;
			this.PackStart (feed_box, false, false, 0);
			this.PackStart (new HSeparator (), false, false, 5);
			this.PackStart (item_box, true, true, 0);
			this.PackStart (new HSeparator (), false, false, 5);
			this.PackStart (page_box, false, false, 0);
			
			
			loadFeed (feeds[0]);
			
			
			
			//events
			feed_prev.EnterNotifyEvent += button_enter;
			feed_prev.LeaveNotifyEvent += button_leave;
			feed_next.EnterNotifyEvent += button_enter;
			feed_next.LeaveNotifyEvent += button_leave;
			
			page_prev.EnterNotifyEvent += button_enter;
			page_prev.LeaveNotifyEvent += button_leave;
			page_next.EnterNotifyEvent += button_enter;
			page_next.LeaveNotifyEvent += button_leave;
			
			feed_prev.Clicked += feed_prev_clicked;
			feed_next.Clicked += feed_next_clicked;
			page_prev.Clicked += page_prev_clicked;
			page_next.Clicked += page_next_clicked;
		}
		
		
		
		// loads the feed into the popup
		private void loadFeed (Feed feed)
		{
			page_number = 0;
			unread_items.Clear ();
			
			//find all the unread items
			foreach (Item item in feed.Items)
				if (item.IsNew)
					unread_items.Add (item);
			
			
			//create the links to be used in the popup
			links = new LinkButton[unread_items.Count];
			
			for (int i=0; i<unread_items.Count; i++)
			{
				Item item = unread_items[i];
				
				links[i] = new LinkButton (item.Url, item.Title);
				links[i].Xalign = 0;
				links[i].EnterNotifyEvent += button_enter;
				links[i].LeaveNotifyEvent += button_leave;
				
				//the link has been clicked
				links[i].Clicked += delegate (object o, EventArgs args) {
					parent.News.NewsViewer.LoadItem (item, feed);
					feed.UpdateStatus ();
					unread.Markup = feed.UnreadStatus;
					parent.News.NewsTree.QueueDraw ();
				};
			}
			
			
			//get the amount of pages
			if (unread_items.Count > 0)
			{
				double item_count = (double) unread_items.Count;
				double show_count = (double) show_total+1;
				page_count = (int) Math.Ceiling (item_count / show_count);
			}
			else
				page_count = 0;
			
			
			//update the feed details
			title.Markup = Utils.ParseMarkup (feed.Name);
			unread.Markup = feed.UnreadStatus;
			refreshFeed ();
			
			//create the items
			generateItems ();
		}
		
		
		// loads the feed items into the popup
		private void generateItems ()
		{
			foreach (Widget widget in item_box.Children)
				item_box.Remove (widget);
			
			
			//only get items for this page
			int index = unread_items.Count-1 - (page_number * (show_total+1));
			
			for (int i=index; i>=index-show_total && i>=0; i--)
				item_box.PackStart (links[i], false, false, 0);
			
			refreshPageCount ();
		}
		
		
		
		
		// gets the new feed details for the selected news feed.
		private void refreshFeed ()
		{
			feed_prev.Sensitive = feed_index > 0;
			feed_next.Sensitive = feed_index < feeds.Count-1;
		}
		
		
		// gets the new page count for the selected news feed.
		private void refreshPageCount ()
		{
			if (page_count == 1)
			{
				page_label.Markup = "<small>No Pages</small>";
				page_label.Sensitive = false;
			}
			else
			{
				page_label.Markup = "<small>Page " + (page_number+1) + " of " + page_count + "</small>";
				page_label.Sensitive = true;
			}
			
			page_prev.Sensitive = page_number > 0;
			page_next.Sensitive = page_number < page_count-1;
		}
		
		
		
		// a mouse entered a button
		private void button_enter (object o, EventArgs args)
		{
			parent.SendCommand (parent.TrayIcon, "StopTimer", null);
		}
		
		// a mouse left a button
		private void button_leave (object o, EventArgs args)
		{
			parent.SendCommand (parent.TrayIcon, "StartTimer", null);
		}
		
		
		
		
		// go to the previous feed
		private void feed_prev_clicked (object o, EventArgs args)
		{
			if (feed_index > 0)
			{
				feed_index--;
				loadFeed (feeds[feed_index]);
				this.ShowAll ();
			}
		}
		
		// go to the next feed
		private void feed_next_clicked (object o, EventArgs args)
		{
			if (feed_index < feeds.Count-1)
			{
				feed_index++;
				loadFeed (feeds[feed_index]);
				this.ShowAll ();
			}
		}
		
		
		// go to the previous page
		private void page_prev_clicked (object o, EventArgs args)
		{
			if (page_number > 0)
			{
				page_number--;
				generateItems ();
				this.ShowAll ();
			}
		}
		
		// go to the next page
		private void page_next_clicked (object o, EventArgs args)
		{
			if (page_number < page_count-1)
			{
				page_number++;
				generateItems ();
				this.ShowAll ();
			}
		}
		
		
	}
}
