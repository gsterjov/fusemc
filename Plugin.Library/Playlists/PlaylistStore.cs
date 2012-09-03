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
using Gtk;

namespace Fuse.Plugin.Library
{
		
	/// <summary>
	/// Adds specific playlist functionality to the ListStore class.
	/// </summary>
	public class PlaylistStore : ListStore
	{
		
		PlaylistDataManager data_manager = new PlaylistDataManager ();
		
		
		public PlaylistStore () : base (typeof (Playlist))
		{}
		
		
		
		/// <summary>
		/// Checks to see if the playlist is already in the library.
		/// </summary>
		public bool PlaylistExists (string name)
		{
			bool exists = false;
			this.Foreach (delegate (TreeModel model, TreePath tree_path, TreeIter iter)
			{
				Playlist playlist = (Playlist) model.GetValue (iter, 0);
				if (playlist.Name == name)
					exists = true;
				return exists;
			});
			
			return exists;
		}
		
		
		
		/// <summary>
		/// Create a new playlist and add it to the store.
		/// </summary>
		public void CreatePlaylist ()
		{
			int i = 1;
			string new_playlist = "New Playlist";
			string name = new_playlist;
			
			while (PlaylistExists (name))
				name = new_playlist + " " + i++;
			
			Playlist playlist = new Playlist (name);
			this.AppendValues (playlist);
			data_manager.AddPlaylist (playlist);
		}
		
		
		
		
		/// <summary>
		/// Add files into the playlist.
		/// </summary>
		public void AddFiles (string[] files, Playlist playlist)
		{
			if (files == null || files.Length == 0) return;
			string path = Path.GetDirectoryName (files[0]);
			
			
			// load the files within the directory
			Progress progress = new Progress (Global.Core.Library.MediaBox);
			progress.Start ((double) files.Length);
			progress.Push ("Waiting in queue:  " + Utils.GetFolderName (path));
			
			// queue process
			Global.Core.Library.DelegateQueue.Enqueue (delegate {
				foreach (string file in files)
				{
					if (progress.Canceled) break;
					progress.Push ("Loading File: " + Path.GetFileName (file));
					
					Global.Core.Library.MediaTree.MediaStore.AddMedia (file, playlist);
					progress.Step ();
				}
				progress.End ();
			});
			
		}
		
		
		
		
		/// <summary>
		/// Add the folder into the playlist.
		/// </summary>
		public void AddFolder (string path, Playlist playlist)
		{
			// load the files within the directory
			Progress progress = new Progress (Global.Core.Library.MediaBox);
			progress.Start (Utils.FileCount (path));
			progress.Push ("Waiting in queue:  " + Utils.GetFolderName (path));
			
			// queue process
			Global.Core.Library.DelegateQueue.Enqueue (delegate {
				addDirRecurse (path, progress, playlist);
				progress.End ();
			});
			
		}
		
		
		
		/// <summary>
		/// The data manager for the store.
		/// </summary>
		public PlaylistDataManager DataManager
		{
			get{ return data_manager; }
		}
		
		
		
		
		// recursively add directories into the media library
		private void addDirRecurse (string path, Progress progress, Playlist playlist)
		{
			// add all files within the directory
			foreach (string file in Directory.GetFiles (path))
			{
				if (progress.Canceled) return;
				progress.Push ("Loading File: " + Path.GetFileName (file));
				Global.Core.Library.MediaTree.MediaStore.AddMedia (file, playlist);
				progress.Step ();
			}
			
			
			// recurse into directories, if any
			foreach (string dir in Directory.GetDirectories (path))
			{
				if (progress.Canceled) return;
				addDirRecurse (dir, progress, playlist);
			}
		}
		
		
	}
}
