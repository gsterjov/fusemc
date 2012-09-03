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
	
	
	public enum SelectStatus { All, None, Some };
	
	
	/// <summary>
	/// Adds specific folder functionality to the TreeStore class.
	/// </summary>
	public class FolderStore : TreeStore
	{
		
		TreeIter root_iter;
		SelectStatus folder_status = SelectStatus.None;
		FolderDataManager data_manager = new FolderDataManager ();
		
		
		public FolderStore () : base (typeof (Folder))
		{
			root_iter = this.AppendValues (new Folder (Utils.RootNode));
		}
		
		
		
		/// <summary>
		/// Make all the folders visible.
		/// </summary>
		public void SelectAll ()
		{
			this.Foreach (delegate (TreeModel model, TreePath tree_path, TreeIter iter)
			{
				Folder folder = (Folder) model.GetValue (iter, 0);
				if (folder.Path != Utils.RootNode)
				{
					folder.Visible = true;
					data_manager.UpdateFolder (folder);
				}
				return false;
			});
		}
		
		
		/// <summary>
		/// Make no folders visible.
		/// </summary>
		public void SelectNone ()
		{
			this.Foreach (delegate (TreeModel model, TreePath tree_path, TreeIter iter)
			{
				Folder folder = (Folder) model.GetValue (iter, 0);
				if (folder.Path != Utils.RootNode)
				{
					folder.Visible = false;
					data_manager.UpdateFolder (folder);
				}
				return false;
			});
		}
		
		
		
		/// <summary>
		/// Return the status of the folder selection.
		/// </summary>
		public SelectStatus SelectionStatus ()
		{
			int visible_count = 0;
			
			this.Foreach (delegate (TreeModel model, TreePath tree_path, TreeIter iter)
			{
				Folder folder = (Folder) model.GetValue (iter, 0);
				if (folder.Path != Utils.RootNode && folder.Visible)
					visible_count++;
				
				return false;
			});
			
			
			if (visible_count == 0)
				return SelectStatus.None;
			else if (visible_count == this.IterNChildren (root_iter))
				return SelectStatus.All;
			
			
			return SelectStatus.Some;
		}
		
		
		
		/// <summary>
		/// Checks to see if the folder is already in the library.
		/// </summary>
		public bool FolderExists (string path)
		{
			bool exists = false;
			this.Foreach (delegate (TreeModel model, TreePath tree_path, TreeIter iter)
			{
				Folder folder = (Folder) model.GetValue (iter, 0);
				if (folder.Path == path)
					exists = true;
				return exists;
			});
			
			return exists;
		}
		
		
		
		/// <summary>
		/// Add files into the media library.
		/// </summary>
		public void AddFiles (string[] files)
		{
			if (files == null || files.Length == 0) return;
			string path = Path.GetDirectoryName (files[0]);
			
			
			// add the folder if it isnt already in the list
			if (FolderExists (path))
				Global.Core.Fuse.ThrowError ("The folder is already in the library:\n" + path);
			else
			{
				Folder folder = new Folder (path);
				this.AppendValues (root_iter, folder);
				data_manager.AddFolder (folder);
				
				
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
						
						Global.Core.Library.MediaTree.MediaStore.AddMedia (file, folder);
						progress.Step ();
					}
					progress.End ();
				});
			}
			
			Global.Core.Library.FolderTree.UpdateFolderStatus ();
		}
		
		
		
		
		/// <summary>
		/// Add the folder into the media library.
		/// </summary>
		public void AddFolder (string path)
		{
			// add the folder if it isnt already in the list
			if (FolderExists (path))
				Global.Core.Fuse.ThrowError ("The folder is already in the library:\n" + path);
			else
			{
				Folder folder = new Folder (path);
				this.AppendValues (root_iter, folder);
				data_manager.AddFolder (folder);
				
				
				// load the files within the directory
				Progress progress = new Progress (Global.Core.Library.MediaBox);
				progress.Start (Utils.FileCount (path));
				progress.Push ("Waiting in queue:  " + Utils.GetFolderName (path));
				
				// queue process
				Global.Core.Library.DelegateQueue.Enqueue (delegate {
					addDirRecurse (path, progress, folder);
					progress.End ();
				});
			}
			
			Global.Core.Library.FolderTree.UpdateFolderStatus ();
		}
		
		
		
		/// <summary>
		/// Return the status of the folder selection.
		/// </summary>
		public SelectStatus Status
		{
			get{ return folder_status; }
			set{ folder_status = value; }
		}
		
		
		/// <summary>
		/// The root node of the store.
		/// </summary>
		public TreeIter RootIter
		{
			get{ return root_iter; }
		}
		
		
		/// <summary>
		/// The data manager for the store.
		/// </summary>
		public FolderDataManager DataManager
		{
			get{ return data_manager; }
		}
		
		
		
		// recursively add directories into the media library
		private void addDirRecurse (string path, Progress progress, Folder folder)
		{
			// add all files within the directory
			foreach (string file in Directory.GetFiles (path))
			{
				if (progress.Canceled) return;
				progress.Push ("Loading File: " + Path.GetFileName (file));
				Global.Core.Library.MediaTree.MediaStore.AddMedia (file, folder);
				progress.Step ();
			}
			
			
			// recurse into directories, if any
			foreach (string dir in Directory.GetDirectories (path))
			{
				if (progress.Canceled) return;
				addDirRecurse (dir, progress, folder);
			}
		}
		
		
	}
}
