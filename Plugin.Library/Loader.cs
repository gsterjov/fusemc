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

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// Loads the library.
	/// </summary>
	public class Loader
	{
		QuickLoad quickload;
		
		List <Folder> folder_list;
		List <Playlist> playlist_list;
		
		int current_media = 0;
		int current_index = 0;
		
		int total = 0;
		int total_index = 0;
		bool finished;
		
		
		
		/// <summary>
		/// Loads the media library from the database.
		/// </summary>
		public void Load ()
		{
			folder_list = Global.Core.Library.FolderTree.FolderStore.DataManager.GetFolders ();
			playlist_list = Global.Core.Library.PlaylistTree.PlaylistStore.DataManager.GetPlaylists ();
			
			
			if (folder_list.Count == 0 && playlist_list.Count == 0)
			{
				if (Global.Core.QuickLoad)
					Global.Core.Library.MediaTree.ThawTree ();
				
				return;
			}
			
			
			TreeIter root_iter = Global.Core.Library.FolderTree.FolderStore.RootIter;
			// load the folders from the database
			foreach (Folder folder in folder_list)
			{
				Global.Core.Library.FolderTree.FolderStore.AppendValues (root_iter, folder);
				Global.Core.Library.FolderTree.FolderStore.DataManager.LoadMedia (folder);
			}
			Global.Core.Library.FolderTree.UpdateFolderStatus ();
			
			
			
			// load the playlists from the database
			foreach (Playlist playlist in playlist_list)
			{
				Global.Core.Library.PlaylistTree.PlaylistStore.AppendValues (playlist);
				Global.Core.Library.PlaylistTree.PlaylistStore.DataManager.LoadMedia (playlist);
			}
			
			
			
			
			// count how much media there is
			total = 0;
			foreach (Folder folder in folder_list)
				total += folder.MediaList.Count;
			foreach (Playlist playlist in playlist_list)
				total += playlist.MediaList.Count;
			
			
			
			// start loading
			if (Global.Core.QuickLoad)
				setupQuickLoad ();
			else
				GLib.Timeout.Add (500, updateProgress);
			
			if (folder_list.Count > 0)
				GLib.Idle.Add (new GLib.IdleHandler (loadFolder));
			else
				GLib.Idle.Add (new GLib.IdleHandler (loadPlaylist));
		}
		
		
		
		
		// updates user interface with the current load progress
		bool updateProgress ()
		{
			if (finished)
				return false;
			
			Global.Core.Fuse.StatusPush ("Loading media:  " + total_index.ToString () + " of " + total.ToString ());
			return true;
		}
		
		
		// updates the load mask widget
		bool quickLoadProgress ()
		{
			if (quickload == null)
				return false;
			
			quickload.Current = total_index;
			return true;
		}
		
		
		
		
		// sets up the quick load widget
		void setupQuickLoad ()
		{
			quickload = new QuickLoad (total);
			quickload.MainBox.ButtonReleaseEvent += quickload_click;
			
			Global.Core.Library.MediaScroll.Remove (Global.Core.Library.MediaTree);
			Global.Core.Library.MediaScroll.AddWithViewport (quickload.MainBox);
			Global.Core.Library.MediaScroll.ShowAll ();
			GLib.Timeout.Add (100, quickLoadProgress);
		}
		
		
		void stopQuickLoad ()
		{
			Global.Core.Library.MediaScroll.Remove (quickload.MainBox.Parent);
			Global.Core.Library.MediaScroll.Add (Global.Core.Library.MediaTree);
			Global.Core.Library.MediaScroll.ShowAll ();
			Global.Core.Library.MediaTree.ThawTree ();
			quickload = null;
		}
		
		
		
		// the user wants to see the media tree
		void quickload_click (object o, ButtonReleaseEventArgs args)
		{
			stopQuickLoad ();
			GLib.Timeout.Add (500, updateProgress);
		}
		
		
		
		
		// loads a single media file from the folder list into the store
		bool loadFolder ()
		{
			
			Folder folder = folder_list [current_index];
			
			if (folder.MediaList.Count > 0)
			{
				Media media = folder.MediaList [current_media];
				media.Iter = Global.Core.Library.MediaTree.MediaStore.AppendValues (media);
				total_index++;
			}
			
			if (current_media >= folder.MediaList.Count - 1)
			{
				if (current_index >= folder_list.Count - 1)
				{
					current_index = 0;
					current_media = 0;
					
					if (playlist_list.Count > 0)
						GLib.Idle.Add (new GLib.IdleHandler (loadPlaylist));
					return finishLoad ();
				}
				else
				{
					current_index++;
					current_media = 0;
				}
			}
			else
				current_media++;
			
			
			return true;
		}
		
		
		
		// loads a single media file from the folder list into the store
		bool loadPlaylist ()
		{
			Playlist playlist = playlist_list [current_index];
			
			if (playlist.MediaList.Count > 0)
			{
				Media media = playlist.MediaList [current_media];
				media.Iter = Global.Core.Library.MediaTree.MediaStore.AppendValues (media);
				total_index++;
			}
			
			if (current_media >= playlist.MediaList.Count - 1)
			{
				if (current_index >= playlist_list.Count - 1)
					return finishLoad ();
				else
				{
					current_index++;
					current_media = 0;
				}
			}
			else
				current_media++;
			
			
			return true;
		}
		
		
		
		
		// the media loading is completed
		bool finishLoad ()
		{
			if (quickload != null)
				stopQuickLoad ();
			else
				Global.Core.Fuse.StatusPop ();
			
			finished = true;
			
			
			foreach (Folder folder in folder_list)
				if (folder.Monitor.Ready)
					folder.Monitor.Start ();
			
			return false;
		}
		
		
	}
}
