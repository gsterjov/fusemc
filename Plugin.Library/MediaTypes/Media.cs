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
using Gtk;

namespace Fuse.Plugin.Library
{
	
	
	/// <summary>
	/// Media.
	/// </summary>
	public abstract class Media
	{
		
		protected string path;
		protected string artist;
		protected string title;
		protected string album;
		protected int track_number;
		protected int track_count;
		protected TimeSpan duration;
		
		protected TreeIter iter;
		
		
		
		public Media (string path)
		{
			this.path = path;
		}
		
		
		/// <summary>
		/// The absolute path to the media file.
		/// </summary>
		public string Path
		{
			get{ return path; }
		}
		
		
		/// <summary>
		/// The artist of the media file.
		/// </summary>
		public string Artist
		{
			get{ return artist; }
			set{ artist = value; }
		}
		
		
		/// <summary>
		/// The title of the media file.
		/// </summary>
		public string Title
		{
			get{ return title; }
			set{ title = value; }
		}
		
		
		/// <summary>
		/// returns the title or filename if the title is empty.
		/// </summary>
		public string TitleOrFilename
		{
			get
			{
				if (string.IsNullOrEmpty (title))
					return Utils.GetFileName (path);
				else
					return title;
			}
		}
		
		
		/// <summary>
		/// The album of the media file.
		/// </summary>
		public string Album
		{
			get{ return album; }
			set{ album = value; }
		}
		
		
		/// <summary>
		/// The track number of the media file.
		/// </summary>
		public int TrackNumber
		{
			get{ return track_number; }
			set{ track_number = value; }
		}
		
		/// <summary>
		/// The track count of the media file.
		/// </summary>
		public int TrackCount
		{
			get{ return track_count; }
			set{ track_count = value; }
		}
		
		/// <summary>
		/// The duration of the media file.
		/// </summary>
		public TimeSpan Duration
		{
			get{ return duration; }
			set{ duration = value; }
		}
		
		
		
		/// <summary>
		/// The TreeIter to allow for quick removal.
		/// </summary>
		public TreeIter Iter
		{
			get{ return iter; }
			set{ iter = value; }
		}
		
		
	}
}
