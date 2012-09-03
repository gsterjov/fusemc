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
using System.Threading;
using System.Collections.Generic;

namespace Fuse.Plugin.Library
{
	
	
	/// <summary>
	/// Monitors a folder for any changes to its contents.
	/// </summary>
	public class FolderMonitor
	{
		
		bool ready;
		bool monitoring;
		Folder folder;
		
		List <FileSystemWatcher> watcher_list = new List <FileSystemWatcher> ();
		
		
		
		public FolderMonitor (Folder folder)
		{
			this.folder = folder;
		}
		
		
		
		/// <summary>
		/// Start monitoring the folder.
		/// </summary>
		public void Start ()
		{
			if (!Directory.Exists (folder.Path))
			{
				Global.Core.Fuse.ThrowError ("Cannot monitor the directory:\n" + folder.Path + "\n\nPath does not exist");
				return;
			}
			
			monitoring = true;
			
			Thread thread = new Thread (scanStart);
			thread.Start ();
		}
		
		
		// enable monitoring on all sub-directories and scan for changes
		void scanStart ()
		{
			scanDelete ();
			scanAdd (folder.Path);
		}
		
		
		//adds new files and watches new directories
		void scanAdd (string path)
		{
			foreach (string dir in Directory.GetDirectories (path))
				scanAdd (dir);
			
			//add in newly found files
			foreach (string file in Directory.GetFiles (path))
				if (!Global.Core.Library.MediaTree.MediaStore.MediaExists (file, folder))
					pathCreated (file);
			
			
			//make sure its not already being monitored
			foreach (FileSystemWatcher watcher in watcher_list)
				if (watcher.Path == path)
					return;
			
			createWatcher (path);
		}
		
		
		//delets non-existant media files
		void scanDelete ()
		{
			folder.ForEachDelete (delegate (FolderMedia media) {
				if (!File.Exists (media.Path))
				{
					Global.Core.Library.MediaTree.MediaStore.RemoveMedia (media);
					Global.Core.Library.MediaTree.MediaStore.DataManager.DeleteMedia (media);
					return true;
				}
				
				return false;
			});
		}
		
		
		
		/// <summary>
		/// Stop monitoring the folder.
		/// </summary>
		public void Stop ()
		{
			foreach (FileSystemWatcher watcher in watcher_list)
				watcher.EnableRaisingEvents = false;
			
			watcher_list.Clear ();
			monitoring = false;
		}
		
		
		
		/// <summary>
		/// Is the folder currently being monitored.
		/// </summary>
		public bool Monitoring
		{
			get{ return monitoring; }
		}
		
		
		/// <summary>
		/// Is the folder ready to start being monitored.
		/// </summary>
		public bool Ready
		{
			get{ return ready; }
			set{ ready = value; }
		}
		
		
		
		// sets up the file system watcher and add it to the list
		void createWatcher (string path)
		{
			FileSystemWatcher watcher = new FileSystemWatcher ();
			watcher.Path = path;
			
			// event handling
			watcher.Created += watcher_created;
			watcher.Deleted += watcher_deleted;
			watcher.Renamed += watcher_renamed;
			
			watcher.EnableRaisingEvents = monitoring;
			
			watcher_list.Add (watcher);
		}
		
		
		
		// add the file to the library when it can be read
		void pathCreated (string path)
		{
            //its a directory
			if (Directory.Exists (path))
				scanAdd (path);
			
			//its a file
			else if (Utils.ValidExt (path))
			{
                Stream stream = null;
                
				//try to get a read lock
				while (true)
				{
					try
					{
						stream = new FileStream (path, FileMode.Open);
						stream.Close ();
						stream = null;
						break;
					}
					catch
					{ Thread.Sleep (5000); }
					finally
					{
						if (stream != null)
						{
							stream.Close ();
							stream = null;
						}
					}
				}//end loop
				
				
				Global.Core.Library.MediaTree.MediaStore.AddMedia (path, folder);
			}
		}
		
		
		// remove the path from the library
		void pathDeleted (string path)
		{
			bool isDirectory = false;
			int i;
			
			//look for the path in the watcher list
			//if its there then it must be a directory
			for (i = 0; i < watcher_list.Count; i++)
			{
				FileSystemWatcher watcher = watcher_list[i];
				if (watcher.Path == path)
				{
					watcher.EnableRaisingEvents = false;
					isDirectory = true;
					break;
				}
			}
			
			
			// remove directory from watch list
			// or the media file from the library
			if (isDirectory)
				watcher_list.RemoveAt (i);
			else
			{
				
				folder.ForEachDelete (delegate (FolderMedia media) {
					if (media.Path == path)
					{
						Global.Core.Library.MediaTree.MediaStore.RemoveMedia (media);
						Global.Core.Library.MediaTree.MediaStore.DataManager.DeleteMedia (media);
						return true;
					}
					
					return false;
				});
			}
		}
		
		
		// remove the file from the library and then add it back
		void pathRenamed (string old_path, string new_path)
		{
			pathDeleted (old_path);
			pathCreated (new_path);
		}
		
		
		
		
		
		// a file has been created
		void watcher_created (object o, FileSystemEventArgs args)
		{
			string path = args.FullPath;
			Thread thread = new Thread (delegate (){ pathCreated (path); });
			thread.Start ();
		}
		
		
		// a file has been deleted
		void watcher_deleted (object o, FileSystemEventArgs args)
		{
			string path = args.FullPath;
			Thread thread = new Thread (delegate (){ pathDeleted (path); });
			thread.Start ();
		}
		
		
		// a file has been renamed
		void watcher_renamed (object o, RenamedEventArgs args)
		{
			string old_path = args.OldFullPath;
			string new_path = args.FullPath;
			Thread thread = new Thread (delegate (){ pathRenamed (old_path, new_path); });
			thread.Start ();
		}
		
		
	}
	
}
