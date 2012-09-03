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
using System.Threading;
using System.Collections.Generic;
using Gtk;
using RssReader;


namespace Fuse.Plugin.News
{
		
	/// <summary>
	/// Holds the news feeds and content.
	/// </summary>
	public class News
	{
		
		MainPage parent;
		NewsViewer news_viewer;
		DelegateQueue delegate_queue = new DelegateQueue ();
		
		
		// global widgets
		TreeView news_tree = new TreeView ();
		ListStore news_store = new ListStore (typeof (Feed));
		
		HPaned main_splitter = new HPaned ();
		
		
		
		// create the main news widget
		public News (MainPage parent)
		{
			this.parent = parent;
			news_viewer = new NewsViewer (parent);
			
			
			// setting up the news feed tree
			news_tree.Model = news_store;
			
			news_tree.AppendColumn (null, new CellRendererText (), new TreeCellDataFunc (renderNews));
			news_tree.HeadersVisible = false;
			news_tree.RowSeparatorFunc = new TreeViewRowSeparatorFunc (row_separator);
			
			
			
			// box packing
			ScrolledWindow news_scroll = new ScrolledWindow ();
			ScrolledWindow html_scroll = new ScrolledWindow ();
			
			news_scroll.Add (news_tree);
			html_scroll.AddWithViewport (news_viewer.HTML);
			
			news_scroll.ShadowType = ShadowType.In;
			html_scroll.ShadowType = ShadowType.In;
			
			main_splitter.Add1 (news_scroll);
			main_splitter.Add2 (html_scroll);
			
			
			// events
			news_tree.Selection.Changed += news_selected;
			news_tree.ButtonReleaseEvent += feed_tree_button_release;
		}
		
		
		
		// render the news title column
		void renderNews (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Feed node = (Feed) model.GetValue (iter, 0);
			if (node.Name == "ROW_SEP")
				return;
			
			
			string text = Utils.ParseMarkup (node.Name) + "\n";
			if (node.Updating)
				text += "<b>Updating..</b>";
			else
				text += node.UnreadStatus;
			
			(cell as CellRendererText).Markup = text;
		}
		
		
		// determines whether the row is a separator or not
		bool row_separator (TreeModel model, TreeIter iter)
		{
			Feed node = (Feed) model.GetValue (iter, 0);
			return node.Name == "ROW_SEP";
		}
		
		
		
		// when a user selects a new feed
		void news_selected (object o, EventArgs args)
		{
			TreeIter iter;
			if (!news_tree.Selection.GetSelected (out iter))
				return;
			
			Feed node = (Feed) news_store.GetValue (iter, 0);
			news_viewer.LoadFeed (node);
			parent.TopBar.RefreshPageCount ();
		}
		
		
		
		// when a user has clicked on the feed tree
		void feed_tree_button_release (object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button != 3) return;
			
			TreeIter iter;
			if (!news_tree.Selection.GetSelected (out iter)) return;
			
			Feed feed = (Feed) news_store.GetValue (iter, 0);
			
			FeedContextMenu menu = new FeedContextMenu (feed, parent);
			menu.ShowAll ();
			menu.Popup ();
		}
		
		
		
		
		// adds the new feed items
		void addNewItems (Feed feed, RssFeed rss_feed)
		{
			
			// only add in new items. backwards
			for (int i=rss_feed.Channel.Items.Count-1; i>=0; i--)
			{
				RssItem rss_item = rss_feed.Channel.Items [i];
				
				bool exists = false;
				foreach (Item item in feed.Items)
					if (item.Title == rss_item.Title && item.Url == rss_item.Link && item.GUID == rss_item.Guid && item.PubDate == rss_item.PubDate)
						exists = true;
				
				
				// the list doesnt have the news item
				if (!exists)
				{
					Item new_item = new Item (rss_item);
					new_item.IsNew = true;
					
					// add in the new item
					Application.Invoke (delegate {
						feed.Items.Add (new_item);
						parent.DataManager.AddItem (feed, new_item);
					});
				}
			}
			
			
			// update the feed
			Application.Invoke (delegate {
				feed.UpdateStatus ();
				parent.DataManager.UpdateFeed (feed);
			});
		}
		
		
		
		
		/// <summary>
		/// Loads the news feed data into the news store.
		/// </summary>
		public void LoadData ()
		{
			List <Feed> list = parent.DataManager.GetFeeds ();
			
			foreach (Feed feed in list)
			{
				if (news_store.IterNChildren () > 0)
					news_store.AppendValues (new Feed ("ROW_SEP", "", "", "", false));
				
				news_store.AppendValues (feed);
			}
		}
		
		
		
		
		/// <summary>
		/// Adds a feed into the news store.
		/// </summary>
		public void AddFeed (RssFeed rss_feed)
		{
			Feed feed = new Feed (rss_feed);
			
			if (news_store.IterNChildren () > 0)
				news_store.AppendValues (new Feed ("ROW_SEP", "", "", "", false));
			
			news_store.AppendValues (feed);
			parent.DataManager.AddFeed (feed);
		}
		
		
		
		/// <summary>
		/// Removes the selected feed from the news store.
		/// </summary>
		public void Remove ()
		{
			TreeIter iter;
			if (!news_tree.Selection.GetSelected (out iter))
				return;
			
			
			TreePath path = news_store.GetPath (iter);
			if (path.Prev ())
			{
				TreeIter separator_iter;
				if (news_store.GetIter (out separator_iter, path))
					news_store.Remove (ref separator_iter);
			}
			
			Feed feed = (Feed) news_store.GetValue (iter, 0);
			news_store.Remove (ref iter);
			parent.DataManager.DeleteFeed (feed);
		}
		
		
		
		/// <summary>
		/// Refreshes the specified news feed.
		/// </summary>
		public void Refresh (Feed feed)
		{
			feed.Updating = true;
			Application.Invoke (delegate{ news_tree.QueueDraw (); });
			
			
			//clear new item count
			foreach (Item item in feed.Items)
				item.IsNew = false;
			
			//update feed
			try
			{
				RssFeed rss_feed = new RssFeed (feed.Url, feed.ETag, feed.LastModified);
				
				if (rss_feed.Channel != null)
					addNewItems (feed, rss_feed);
				
			}
			catch (Exception e)
			{
				parent.Fuse.ThrowWarning ("News.Refresh:: Could not update the feed - " + feed.Url, e.ToString ());
			}
			
			
			feed.Updating = false;
			
			//update view
			Application.Invoke (delegate{
				
				news_tree.QueueDraw ();
				
				TreeIter iter;
				if (news_tree.Selection.GetSelected (out iter))
				{
					Feed node = (Feed) news_store.GetValue (iter, 0);
					if (node.Url == feed.Url)
						news_viewer.Refresh ();
				}
				
			});
		}
		
		
		
		/// <summary>
		/// Refreshes the news feeds flagged as auto-refresh.
		/// </summary>
		public void AutoRefresh ()
		{
			foreach (object[] row in news_store)
			{
				Feed feed = (Feed) row[0];
				if (feed.AutoRefresh)
					delegate_queue.Enqueue (delegate(){ Refresh (feed); });
			}
			
			delegate_queue.Enqueue (PopupNewItems);
		}
		
		
		
		/// <summary>
		/// Creates a popup if a feed has new items.
		/// </summary>
		public void PopupNewItems ()
		{
			Application.Invoke (delegate{
				
				List <Feed> list = new List <Feed> ();
				
				foreach (object[] row in news_store)
				{
					Feed feed = (Feed) row[0];
					if (feed.NewCount > 0)
						list.Add (feed);
				}
				
				if (list.Count > 0)
					parent.PopupWidget (new NewsPopup (list, parent));
				
			});
		}
		
		
		
		/// <summary>
		/// The core widget which contains all the child widgets.
		/// </summary>
		public HPaned MainSplitter
		{
			get{ return main_splitter; }
		}
		
		
		/// <summary>
		/// The store containing the news feeds.
		/// </summary>
		public ListStore NewsStore
		{
			get{ return news_store; }
		}
		
		
		
		/// <summary>
		/// The tree containing the news feeds.
		/// </summary>
		public TreeView NewsTree
		{
			get{ return news_tree; }
		}
		
		
		
		/// <summary>
		/// The news viewer widget displaying the news content.
		/// </summary>
		public NewsViewer NewsViewer
		{
			get{ return news_viewer; }
		}
		
	}
}
