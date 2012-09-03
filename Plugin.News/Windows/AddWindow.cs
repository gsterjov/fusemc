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
using System.Net;
using System.Xml;
using System.Threading;
using System.Collections.Generic;
using Gtk;
using RssReader;

namespace Fuse.Plugin.News
{
	
	/// <summary>
	/// The dialog window for adding news feeds.
	/// </summary>
	public class AddWindow : DialogBase
	{
		
		MainPage parent;
		
		// global widgets
		VBox backbone = new VBox (false, 0);
		Entry custom_url = new Entry ();
		Button add_feed = new Button (Stock.Add);
		
		ProgressBar progress = new ProgressBar ();
		
		Thread thread;
		RssFeed feed = null;
		bool loading;
		
		
		// creates the add window user interface
		public AddWindow (MainPage parent) : base (parent.Fuse.MainWindow, "Add News Feed")
		{
			this.parent = parent;
			
			
			// event hooks
			add_feed.Clicked += add_clicked;
			this.DeleteEvent += window_delete;
			
			
			
			// pack widgets
			backbone.PackStart (new Label ("Enter the URL of the News Feed:"), false, false, 0);
			backbone.PackStart (custom_url, false, true, 0);
			backbone.PackStart (new HSeparator (), false, true, 10);
			backbone.PackStart (add_feed, false, true, 0);
			
			backbone.BorderWidth = 15;
			this.WidthRequest = 300;
			this.Resizable = false;
			this.SkipPagerHint = true;
			this.SkipTaskbarHint = true;
			this.Add (backbone);
		}
		
		
		
		/// <summary>
		/// The loaded news feed.
		/// </summary>
		public RssFeed Feed
		{
			get{ return feed; }
		}
		
		
		// return a value indicating that the loading failed
		void failRespond ()
		{
			feed = null;
			loading = false;
			Application.Invoke (delegate{ this.Destroy (); });
			retVal = 2;
		}
		
		
		// start the loading process
		void startAdd ()
		{
			add_feed.Sensitive = false;
			custom_url.Sensitive = false;
			
			backbone.PackStart (progress, false, true, 0);
			backbone.ShowAll ();
			
			loading = true;
			GLib.Timeout.Add (500, pulseProgress);
		}
		
		
		
		// load the feed
		void loadFeed ()
		{
			feed = null;
			
			try {
				feed = new RssFeed (custom_url.Text, null, null);
				
				if (feed.Channel != null)
				{
					loading = false;
					Application.Invoke (delegate{ this.Destroy (); });
					retVal = 1;
					return;
				}
			}
			catch (Exception e)
			{
				parent.Fuse.ThrowWarning ("News.loadFeed:: Could not load the feed - " + custom_url.Text, e.ToString ());
			}
			
			failRespond ();
		}
		
		
		// load the news feed
		void add_clicked (object o, EventArgs args)
		{
			startAdd ();
			if (custom_url.Text.Length > 0)
			{
				thread = new Thread (new ThreadStart (loadFeed));
				thread.Start ();
			}
			else
				failRespond ();
		}
		
		
		
		// pulses the progress
		bool pulseProgress ()
		{
			progress.Pulse ();
			return loading;
		}
		
		
		
		// the window has closed
		void window_delete (object o, DeleteEventArgs a)
		{
			if (thread != null && thread.IsAlive)
				thread.Abort ();
		}
		
		
	}
}
