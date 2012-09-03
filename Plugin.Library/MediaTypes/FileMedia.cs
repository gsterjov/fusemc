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
using Fuse.Interfaces;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// A media file.
	/// </summary>
	public class FileMedia : Media
	{
		
		protected string comment;
		protected int year;
		protected string pic_uri;
		
		
		
		public FileMedia (string path) : base (path)
		{}
		
		
		
		/// <summary>
		/// Load the tag from the media file.
		/// </summary>
		public bool LoadTag ()
		{
            if (!Utils.ValidExt (path))
                return false;
            
            
			try
			{
				TagLib.File parser = TagLib.File.Create (path);
				if (parser.Properties.MediaTypes == TagLib.MediaTypes.Audio)
				{
					artist = parser.Tag.JoinedPerformers;
					title = parser.Tag.Title;
					album = parser.Tag.Album;
					
					year = (int) parser.Tag.Year;
					track_number = (int) parser.Tag.Track;
					track_count = (int) parser.Tag.TrackCount;
					duration = parser.Properties.Duration;
					
					
					// replace nulls
					if (artist == null) artist = "";
					if (title == null) title = "";
					if (album == null) album = "";
					if (comment == null) comment = "";
					
					return true;
				}
			}
			catch (Exception e)
			{
                if (e is TagLib.UnsupportedFormatException)
                    return false;
                
				Application.Invoke (delegate{
					string message = "Media.LoadTag:: Failed to load media tag - " + path;
					Global.Core.Fuse.ThrowWarning (message, e.ToString ());
				});
			}
			
			return false;
		}
		
		
		
		/// <summary>
		/// Save the current tag to the media file.
		/// </summary>
		public bool SaveTag ()
		{
			if (!System.IO.File.Exists (path))
				return false;
			
			try
			{
				TagLib.File parser = TagLib.File.Create (path);
				
				if (parser.Properties.MediaTypes == TagLib.MediaTypes.Audio)
				{
					parser.Tag.Performers = new string[] {artist};
					parser.Tag.Title = title;
					parser.Tag.Album = album;
					parser.Tag.Comment = comment;
					parser.Tag.Year = (uint) year;
					parser.Tag.Track = (uint) track_number;
					parser.Tag.TrackCount = (uint) track_count;
					
					// save the picture
					if (pic_uri != null && System.IO.File.Exists (pic_uri))
						parser.Tag.Pictures = new TagLib.IPicture [] {TagLib.Picture.CreateFromPath (pic_uri)};
					
					parser.Save ();
					Global.Core.Library.MediaTree.MediaStore.DataManager.UpdateMedia (this);
					return true;
				}
				
			}
			catch (Exception e)
			{
				Application.Invoke (delegate{
					string message = "Media.SaveTag:: Failed to save media tag to the file - " + path;
					Global.Core.Fuse.ThrowWarning (message, e.ToString ());
				 });
			}
			
			
			return false;
		}
		
		
		
		
		/// <summary>
		/// Sets the path to the picture file used when saving.
		/// </summary>
		public void SetPicture (string pic_uri)
		{
			this.pic_uri = pic_uri;
		}
		
		
		
		
		/// <summary>
		/// The comment within the media file.
		/// </summary>
		public string Comment
		{
			get{ return comment; }
			set{ comment = value; }
		}
		
		/// <summary>
		/// The year of the media file.
		/// </summary>
		public int Year
		{
			get{ return year; }
			set{ year = value; }
		}
		
		
		
		/// <summary>
		/// The cover art of the media file.
		/// </summary>
		public byte[] Picture
		{
			get
			{
				try
				{
					TagLib.File parser = TagLib.File.Create (path);
					if (parser.Properties.MediaTypes == TagLib.MediaTypes.Audio)
					{
						// get cover art
						if (parser.Tag.Pictures.Length > 0)
						{
							TagLib.IPicture pic = parser.Tag.Pictures [0];
							if (pic.Data != null && pic.Data.Data != null)
								return pic.Data.Data;
						}
					}
				}
				catch {}
				
				return null;
			}
		}
		
		
	}
}
