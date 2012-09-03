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
using Fuse.Interfaces;

namespace Fuse.Plugin.News
{
	
	/// <summary>
	/// The top bar for the news feeds.
	/// </summary>
	public class TopBar : HBox
	{
		
		MainPage parent;
		DelegateQueue delegate_queue = new DelegateQueue ();
		
		// global widgets
		Button prev_page_button = new Button ();
		Button next_page_button = new Button ();
		Label page_numbers = new Label ();
		
		
		// create the TopBar widget
		public TopBar (MainPage parent)
		{
			this.parent = parent;
			
			// create the widgets
			Button add_button = new Button (Stock.Add);
			Button remove_button = new Button (Stock.Remove);
			Button refresh_button = new Button (Stock.Refresh);
			
			
			prev_page_button.Image = new Image (Stock.GoBack, IconSize.Menu);
			next_page_button.Image = new Image (Stock.GoForward, IconSize.Menu);
			page_numbers.Markup = "<small>No Pages</small>";
			prev_page_button.Sensitive = false;
			next_page_button.Sensitive = false;
			page_numbers.Sensitive = false;
			
			
			// hook up the widget events
			add_button.Clicked += new EventHandler (add_clicked);
			remove_button.Clicked += remove_clicked;
			refresh_button.Clicked += refresh_clicked;
			prev_page_button.Clicked += prev_page_clicked;
			next_page_button.Clicked += next_page_clicked;
			
			
			// homogeneous button box
			HBox button_box = new HBox (true, 0);
			button_box.PackStart (add_button, false, true, 0);
			button_box.PackStart (remove_button, false, true, 0);
			
			
			// page box
			HBox page_box = new HBox (false, 0);
			page_box.PackStart (prev_page_button, false, false, 0);
			page_box.PackStart (page_numbers, false, false, 5);
			page_box.PackStart (next_page_button, false, false, 0);
			
			
			// pack widgets
			this.PackStart (button_box, false, false, 0);
			this.PackStart (new VSeparator (), false, true, 5);
			this.PackStart (refresh_button, false, false, 0);
			this.PackStart (new HBox (), true, true, 0);
			this.PackStart (page_box, false, false, 0);
		}
		
		
		
		/// <summary>
		/// Gets the new page count for the selected news feed.
		/// </summary>
		public void RefreshPageCount ()
		{
			int page_count = parent.News.NewsViewer.PageCount;
			int page_number = parent.News.NewsViewer.PageNumber;
			
			if (page_count == 0)
			{
				page_numbers.Markup = "<small>No Pages</small>";
				page_numbers.Sensitive = false;
			}
			else
			{
				page_numbers.Markup = "<small>Page " + page_number + " of " + page_count + "</small>";
				page_numbers.Sensitive = true;
			}
			
			prev_page_button.Sensitive = parent.News.NewsViewer.HasPrevious;
			next_page_button.Sensitive = parent.News.NewsViewer.HasNext;
		}
		
		
		
		// the user clicked on the add button
		void add_clicked (object o, EventArgs args)
		{
			AddWindow dialog = new AddWindow (parent);
			int ret = dialog.Run ();
			
			if (ret == 1)
				parent.News.AddFeed (dialog.Feed);
			else if (ret == 2)
				parent.Fuse.ThrowError ("Failed to load the news feed");
		}
		
		
		// the user clicked on the remove button
		void remove_clicked (object o, EventArgs args)
		{
			parent.News.Remove ();
		}
		
		
		// the user clicked on the refresh button
		void refresh_clicked (object o, EventArgs args)
		{
			foreach (object[] row in parent.News.NewsStore)
			{
				Feed feed = (Feed) row[0];
				if (feed.Name != "ROW_SEP")
					delegate_queue.Enqueue (delegate(){ parent.News.Refresh (feed); });
			}
			
			delegate_queue.Enqueue (parent.News.PopupNewItems);
		}
		
		
		// go to the previous page
		void prev_page_clicked (object o, EventArgs args)
		{
			parent.News.NewsViewer.PreviousPage ();
			parent.News.NewsTree.QueueDraw ();
		}
		
		
		// go to the next page
		void next_page_clicked (object o, EventArgs args)
		{
			parent.News.NewsViewer.NextPage ();
			parent.News.NewsTree.QueueDraw ();
		}
		
		
	}
}
