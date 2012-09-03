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
using System.Web;
using System.Text;
using System.Collections.Generic;

namespace Fuse.Plugin.Library.Info.AudioScrobbler.Profile
{
	
	/// <summary>
	/// The scrobbler queue.
	/// </summary>
	public class ScrobblerQueue
	{
		
		private string cache_file;
		
		private Queue <string> queue = new Queue <string> ();
		private string format = "&a[{0}]={1}&t[{0}]={2}&i[{0}]={3}&o[{0}]=P&r[{0}]=&l[{0}]={4}&b[{0}]={5}&n[{0}]={6}&m[{0}]=";
		
		
		public ScrobblerQueue ()
		{
			cache_file = Path.Combine (Global.Core.AppDir, "scrobbler_cache.txt");
		}
		
		
		
		//adds the media to the queue and saves it
		public void Add (Media media, string timestamp)
		{
			
			string duration = Math.Ceiling (media.Duration.TotalSeconds).ToString ();
			
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat (format,
			                 "{0}",
			                 HttpUtility.UrlEncode (media.Artist),
			                 HttpUtility.UrlEncode (media.Title),
			                 timestamp,
			                 duration,
			                 HttpUtility.UrlEncode (media.Album),
			                 media.TrackNumber);
			
			
			StreamWriter writer = new StreamWriter (cache_file, true);
			writer.WriteLine (sb.ToString ());
			writer.Close ();
		}
		
		
		
		//returns the query to execute
		public string GetQuery (string session_id, string url)
		{
			queue.Clear ();
			
			StreamReader reader = new StreamReader (cache_file);
			while (!reader.EndOfStream)
				queue.Enqueue (reader.ReadLine ());
			
			reader.Close ();
			
			
			StringBuilder sb = new StringBuilder ();
			sb.Append (url);
			sb.AppendFormat ("?s={0}", session_id);
			
			int count = queue.Count;
			
			for (int i=0; i<count && i<50; i++)
				sb.AppendFormat (queue.Dequeue (), i);
			
			return sb.ToString ();
		}
		
		
		//saves the current queue into the cache file
		//only really works after GetQuery is called..
		//otherwise the queue would be empty... stupid.
		public void Save ()
		{
			StreamWriter writer = new StreamWriter (cache_file, false);
			
			foreach (string str in queue)
				writer.WriteLine (str);
			
			writer.Close ();
		}
		
		
		
	}
}
