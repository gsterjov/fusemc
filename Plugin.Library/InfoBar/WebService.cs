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
using System.Xml;
using System.Web;
using System.Threading;
using Gtk;

namespace Fuse.Plugin.Library.Info
{
	
	/// <summary>
	/// Reads stuff from the web.
	/// </summary>
	public class WebService
	{
		
		
		private Notebook tabs = new Notebook ();
		
		private Alignment align = new Alignment (0.5f, 0.5f, 0, 0);
		private Label title = new Label ();
		private Timer timer = new Timer ();
		private ProgressBar progress = new ProgressBar ();
		
		
		public delegate void NewSizeHandler ();
		public event NewSizeHandler NewSize;
		
		
		public WebService ()
		{
			title.Markup = "<b><big><big>Loading</big></big></b>";
			
			VBox loading_box = new VBox (false, 2);
			
			loading_box.PackStart (title, false, false, 0);
			loading_box.PackStart (progress, false, false, 0);
			align.Add (loading_box);
			
			tabs.AppendPage (align, null);
			
			tabs.ShowTabs = false;
			tabs.ShowBorder = false;
			
			timer.Elapsed += timer_count;
			title.SizeRequested += title_resized;
		}
		
		
		
		//makes the query http compatible
		protected string ParseQuery (string format, params string[] values)
		{
			for (int i=0; i<values.Length; i++)
				values[i] = HttpUtility.UrlEncode (values[i]);
			
			return String.Format (format, values);
		}
		
		
		
		/// <summary>
		/// Show the loading widget.
		/// </summary>
		public void ShowLoading ()
		{
			tabs.CurrentPage = 0;
			throwNewSize ();
			tabs.ShowAll ();
		}
		
		/// <summary>
		/// Hide the loading widget.
		/// </summary>
		public void HideLoading ()
		{
			tabs.CurrentPage = 1;
			throwNewSize ();
			tabs.ShowAll ();
		}
		
		
		
		/// <summary>
		/// Load an XML document from the specified query.
		/// </summary>
		public XmlDocument LoadXml (string query)
		{
			Stream stream = null;
			XmlDocument doc = new XmlDocument ();
			
			try
			{
				stream = threadLoad (query);
				if (stream == null)
					return null;
				
				doc.Load (stream);
			}
			
			//an error occured. catch it cleanly
			catch (Exception e)
			{
				string message = "WebService.LoadXml:: Failed to load the URL - " + query;
				Global.Core.Fuse.ThrowWarning (message, e.ToString ());
			}
			
			//always close the stream afterwards
			finally
			{
				if (stream != null)
				{
					stream.Close ();
					stream.Dispose ();
				}
			}
			
			return doc;
		}
		
		
		
		
		/// <summary>
		/// Loads an image from the web.
		/// </summary>
		public Gdk.Pixbuf LoadImage (string query)
		{
			Stream stream = null;
			Gdk.Pixbuf pic = null;
			
			try
			{
				stream = threadLoad (query);
				pic = new Gdk.Pixbuf (stream);
			}
			
			//an error occured. catch it cleanly
			catch (Exception e)
			{
				string message = "WebService.LoadImage:: Failed to load the image - " + query;
				Global.Core.Fuse.ThrowWarning (message, e.ToString ());
			}
			
			//always close the stream afterwards
			finally
			{
				if (stream != null)
				{
					stream.Close ();
					stream.Dispose ();
				}
			}
			
			return pic;
		}
		
		
		
		
		/// <summary>
		/// The widget to display.
		/// </summary>
		public Widget DisplayWidget
		{
			get{ return tabs; }
			set
			{
				if (tabs.NPages == 2)
					tabs.RemovePage (1);
				
				tabs.AppendPage (value, null);
			}
		}
		
		
		/// <summary>
		/// The size of the displayed widget.
		/// </summary>
		public Requisition WidgetSize
		{
			get{ return tabs.CurrentPageWidget.SizeRequest (); }
		}
		
		
		
		//throws the new size event
		private void throwNewSize ()
		{
			if (NewSize != null)
				NewSize ();
		}
		
		
		
		//start the loading process
		private void start_timer ()
		{
			title.Markup = "<b><big><big>Loading</big></big></b>";
			
			//prevents erratic timers
			if (!timer.Running)
				timer.Start (100);
		}
		
		//the loading process failed
		private void fail_timer ()
		{
			title.Markup = "<b><big>Loading Failed</big></b>";
			timer.Stop ();
			progress.Fraction = 0;
		}
		
		
		
		//update the progress bar
		private void timer_count ()
		{
			progress.Pulse ();
			
			if (tabs.CurrentPage == 1)
				timer.Stop ();
		}
		
		
		
		//resize the progress bar
		private void title_resized (object o, SizeRequestedArgs args)
		{
			progress.WidthRequest = args.Requisition.Width;
			progress.HeightRequest = 5;
		}
		
		
		
		//load the query in a separate thread
		private Stream threadLoad (string query)
		{
			start_timer ();
			
			Stream stream = null;
			WebDownload downloader = new WebDownload ();
			bool finished = false;
			
			
			Thread thread = new Thread (delegate() {
				
				//catch any errors. ugly, but works.
				try
				{ stream = downloader.ReadUrl (query); }
				catch
				{
					Application.Invoke (delegate{ fail_timer (); });
				}
				
				finished = true;
			});
			thread.Start ();
			
			
			
			//blocks the main thread waiting for the
			//downloading thread to finish. this keeps the
			//user interface responsive.
			while (!finished)
				while (Application.EventsPending ())
					Application.RunIteration ();
			
			
			return stream;
		}
		
	}
}
