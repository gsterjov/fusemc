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
	/// The context menu for the Folder tree.
	/// </summary>
	public class FolderContextMenu : Menu
	{
		
		Folder folder;
		
		
		// create the context menu
		public FolderContextMenu (Folder folder) : base ()
		{
			this.folder = folder;
			
			ImageMenuItem add_dir = new ImageMenuItem ("Add Directory");
			ImageMenuItem add_files = new ImageMenuItem ("Add Files");
			ImageMenuItem remove_folder = new ImageMenuItem (Stock.Remove, null);
			CheckMenuItem visible = new CheckMenuItem ("Visible");
			CheckMenuItem monitor = new CheckMenuItem ("Monitor");
			
			add_dir.Image = new Image (Stock.Add, IconSize.Menu);
			add_files.Image = new Image (Stock.Add, IconSize.Menu);
			
			visible.Active = folder.Visible;
			monitor.Active = folder.Monitor.Monitoring;
			
			
			this.Add (add_dir);
			this.Add (add_files);
			this.Add (remove_folder);
			this.Add (new SeparatorMenuItem ());
			this.Add (visible);
			this.Add (monitor);
			
			
			add_dir.Activated += add_dir_activated;
			add_files.Activated += add_files_activated;
			remove_folder.Activated += remove_folder_activated;
			visible.Toggled += visible_toggled;
			monitor.Toggled += monitor_toggled;
		}
		
		
		
		// add dir was clicked
		void add_dir_activated (object o, EventArgs args)
		{
			string folder = Dialogs.ChooseFolder ();
			if (folder != null)
				Global.Core.Library.FolderTree.FolderStore.AddFolder (folder);
		}
		
		
		// add files was clicked
		void add_files_activated (object o, EventArgs args)
		{
			string[] files = Dialogs.ChooseFiles ();
			Global.Core.Library.FolderTree.FolderStore.AddFiles (files);
		}
		
		
		// remove was clicked
		void remove_folder_activated (object o, EventArgs args)
		{
			Global.Core.Library.FolderTree.RemoveSelected ();
		}
		
		
		// visible was toggled
		void visible_toggled (object o, EventArgs args)
		{
			folder.Visible = !folder.Visible;
			Global.Core.Library.FolderTree.FolderStore.DataManager.UpdateFolder (folder);
			Global.Core.Library.MediaTree.Refilter ();
		}
		
		
		// monitor was toggled
		void monitor_toggled (object o, EventArgs args)
		{
			if (folder.Monitor.Monitoring)
				folder.Monitor.Stop ();
			else
				folder.Monitor.Start ();
			
			Global.Core.Library.FolderTree.FolderStore.DataManager.UpdateFolder (folder);
		}
		
		
		
	}
}
