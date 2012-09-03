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
	/// The context menu for the Playlist tree.
	/// </summary>
	public class PlaylistContextMenu : Menu
	{
		
		Playlist playlist;
		
		
		// create the context menu
		public PlaylistContextMenu (Playlist playlist) : base ()
		{
			this.playlist = playlist;
			
			ImageMenuItem add_dir = new ImageMenuItem ("Add Directory");
			ImageMenuItem add_files = new ImageMenuItem ("Add Files");
			ImageMenuItem remove_folder = new ImageMenuItem (Stock.Remove, null);
			
			add_dir.Image = new Image (Stock.Add, IconSize.Menu);
			add_files.Image = new Image (Stock.Add, IconSize.Menu);
			
			
			this.Add (add_dir);
			this.Add (add_files);
			this.Add (remove_folder);
			
			
			add_dir.Activated += add_dir_activated;
			add_files.Activated += add_files_activated;
			remove_folder.Activated += remove_folder_activated;
		}
		
		
		
		// add dir was clicked
		void add_dir_activated (object o, EventArgs args)
		{
			string folder = Dialogs.ChooseFolder ();
			if (folder != null)
				Global.Core.Library.PlaylistTree.PlaylistStore.AddFolder (folder, playlist);
		}
		
		
		// add files was clicked
		void add_files_activated (object o, EventArgs args)
		{
			string[] files = Dialogs.ChooseFiles ();
			Global.Core.Library.PlaylistTree.PlaylistStore.AddFiles (files, playlist);
		}
		
		
		// remove was clicked
		void remove_folder_activated (object o, EventArgs args)
		{
			Global.Core.Library.PlaylistTree.RemoveSelected ();
		}
		
		
		
	}
}
