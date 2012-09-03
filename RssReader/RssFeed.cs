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

namespace RssReader
{
	
	
	public enum RssVersion {RSS090, RSS091, RSS092, RSS10, RSS20, NotSupported}
	
	
	
	/// <summary>
	/// The RSS feed class.
	/// </summary>
	public class RssFeed
	{
		
		
		RssVersion version;
		RssChannel channel;
		string etag;
		string last_modified;
		string url;
		
		
		
		public RssFeed (string url, string etag, string last_modified)
		{
			this.url = url;
			Stream rss_stream = null;
			bool modified = true;
			
			HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create (url);
			
			if (!string.IsNullOrEmpty (etag))
				request.Headers.Add ("If-None-Match", etag);
			
			if (!string.IsNullOrEmpty (last_modified))
				request.IfModifiedSince = DateTime.Parse (last_modified);
			
			
			// get the rss web stream
			try
			{
				HttpWebResponse response = (HttpWebResponse) request.GetResponse ();
				last_modified = response.LastModified.ToString ();
				etag = response.Headers ["ETag"];
				
				rss_stream = response.GetResponseStream ();
			}
			catch { modified = false; }
			
			
			// load the modified feed
			if (modified && rss_stream != null)
			{
				XmlDocument doc = new XmlDocument ();
				doc.Load (rss_stream);
				
				readFeed (doc);
			}
		}
		
		
		
		public RssVersion Version
		{
			get{ return version; }
		}
		
		
		public RssChannel Channel
		{
			get{ return channel; }
		}
		
		
		public string ETag
		{
			get{ return etag; }
		}
		
		
		public string LastModified
		{
			get{ return last_modified; }
		}
		
		
		public string URL
		{
			get{ return url; }
		}
		
		
		
		
		// parses the rss file
		void readFeed (XmlDocument doc)
		{
			
			// make sure its a valid rss document
			XmlNodeList elements = doc.GetElementsByTagName ("rss");
			if (elements.Count == 0)
			{
				elements = doc.GetElementsByTagName ("rdf");
				if (elements.Count == 0)
					return;
			}
			
			
			// get the rss version
			string rss_version = elements [0].Attributes ["version"].Value;
			switch (rss_version)
			{
				case "0.90":
					version = RssVersion.RSS090;
					break;
				case "0.91":
					version = RssVersion.RSS091;
					break;
				case "0.92":
					version = RssVersion.RSS092;
					break;
				case "1.0":
					version = RssVersion.RSS10;
					break;
				case "2.0":
					version = RssVersion.RSS20;
					break;
				default:
					version = RssVersion.NotSupported;
					break;
			}
			
			
			
			// get the channels
			elements = doc.GetElementsByTagName ("channel");
			if (elements.Count == 0)
				return;
			
			channel = new RssChannel (elements [0].ChildNodes);
			
		}
		
		
		
		
	}
	
}