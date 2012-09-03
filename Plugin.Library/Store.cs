/*

	Copyright (c)  Goran Sterjov

    This file is part of the Dissent Project.

    The Dissent Project is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    The Dissent Project is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with the Dissent Project; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/


using System;
using Gtk;

namespace Dissent.Plugin.Library
{
		
	/// <summary>
	/// Adds specific functionality to the TreeStore class.
	/// </summary>
	public class Store : TreeStore
	{
		
		Library library;
		TreeIter root_iter;
		DelegateQueue delegate_queue = new DelegateQueue ();
		
		
		
		public FolderStore (Library library) : base (typeof (Folder))
		{
			this.library = library;
			root_iter = this.AppendValues (new Folder (Utils.RootPath));
		}
		
		
		
		
		/// <summary>
		/// Add the folder into the media library.
		/// </summary>
		public void AddFolder (string path)
		{
			
			
			// make sure the folder isnt already in the list
			bool exists = false;
			this.Foreach (delegate (TreeModel model, TreePath tree_path, TreeIter iter)
			{
				Folder node = (Folder) model.GetValue (iter, 0);
				if (node.Path == path) exists = true;
				return exists;
			});
			
			
			
			// add the folder if it isnt already in the list
			if (exists)
				library.MainPage.ThrowError ("The folder is already in the library:\n" + path);
			else
			{
				Folder folder = new Folder (path);
				this.AppendValues (root_iter, folder);
				library.MainPage.DataManager.AddFolder (folder);
				
				// load the files within the directory
				Progress progress = new Progress (library.MediaBox);
				progress.Start (Utils.FileCount (path));
				progress.Push ("Waiting in queue:  " + Utils.GetFolderName (path));
				
				// queue process
				delegate_queue.Enqueue (delegate {
					addDirRecurse (path, progress, folder);
					progress.End ();
				});
			}
			
		}
		
		
		
		
		// recursively add directories into the media library
		void addDirRecurse (string path, Progress progress, Folder folder)
		{
			// add all files within the directory
			foreach (string file in Directory.GetFiles (path))
			{
				if (progress.Canceled) return;
				progress.Push ("Loading File: " + System.IO.Path.GetFileName (file));
				
				library.MediaStore.AddMedia (file, folder);
				
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
