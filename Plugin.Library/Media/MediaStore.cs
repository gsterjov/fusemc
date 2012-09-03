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
	/// Adds specific media functionality to the ListStore class.
	/// </summary>
	public class MediaStore : ListStore
	{
		MediaDataManager data_manager = new MediaDataManager ();
		
		public MediaStore () : base (typeof (Media))
		{}
		
		
		
		// checks to see if the media is already in the folder
		public bool MediaExists (string path, Folder collection)
		{
			foreach (Media media in collection.MediaList)
				if (media.Path == path)
					return true;
			
			return false;
		}
		
		// checks to see if the media is already in the playlist
		public bool MediaExists (string path, Playlist collection)
		{
			foreach (Media media in collection.MediaList)
				if (media.Path == path)
					return true;
			
			return false;
		}
		
		
		
		// add media to the folder
		public void AddMedia (string path, Folder folder)
		{
			FolderMedia media = new FolderMedia (path, folder);
			if (media.LoadTag ())
			{
				folder.MediaList.Add (media);
				AddMedia (media);
				data_manager.AddMedia (media);
			}
		}
		
		// add media to the playlist
		public void AddMedia (string path, Playlist playlist)
		{
			PlaylistMedia media = new PlaylistMedia (path, playlist);
			if (media.LoadTag ())
			{
				playlist.MediaList.Add (media);
				AddMedia (media);
				data_manager.AddMedia (media);
			}
		}
		
		
		
		
		// add media to the store
		public void AddMedia (Media media)
		{
			Application.Invoke (delegate{
				
				TreeIter iter = this.AppendValues (media);
				media.Iter = iter;
				
			});
		}
		
		
		// remove media by path
		public void RemoveMedia (string path)
		{
			Application.Invoke (delegate{
				
				foreach (object[] row in this)
				{
					Media media = (Media) row[0];
					if (media.Path == path)
					{
						RemoveMedia (media);
						break;
					}
				}
				
			});
		}
		
		
		// the actual removing
		public void RemoveMedia (Media media)
		{
			Application.Invoke (delegate{
				
				TreeIter iter = media.Iter;
				this.Remove (ref iter);
				
			});
		}
		
		
		
		// the media data manager
		public MediaDataManager DataManager
		{
			get{ return data_manager; }
		}
		
		
	}
}
