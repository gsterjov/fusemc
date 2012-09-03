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
	/// The folder tree view widget.
	/// </summary>
	public class FolderTree : OrganizerTree
	{
		private FolderStore store = new FolderStore ();
		
		
		public FolderTree () : base ()
		{
			CellRendererToggle crt = new CellRendererToggle ();
			CellRendererPixbuf pic = new CellRendererPixbuf ();
			crt.Activatable = true;
			
			this.Model = store;
			this.HeadersVisible = false;
			this.AppendColumn (null, pic, new TreeCellDataFunc (renderPixbuf));
			this.AppendColumn (null, crt, new TreeCellDataFunc (renderCheckbox));
			this.AppendColumn (null, new CellRendererText (), new TreeCellDataFunc (renderText));
			
			// events
			crt.Toggled += visible_toggled;
			this.ButtonReleaseEvent += tree_button_release;
		}
		
		
		/// <summary>
		/// Removes the folder from the media library.
		/// </summary>
		public override void RemoveSelected ()
		{
			TreeIter iter;
			if (!this.Selection.GetSelected (out iter)) return;
			
			
			Folder folder = (Folder) store.GetValue (iter, 0);
			if (folder.Path == Utils.RootNode) return;
			store.Remove (ref iter);
			
			// remove all media within the folder
			foreach (Media media in folder.MediaList)
				Global.Core.Library.MediaTree.MediaStore.RemoveMedia (media);
			folder.MediaList.Clear ();
			
			
			store.DataManager.DeleteFolder (folder);
			UpdateFolderStatus ();
			folder = null;
		}
		
		
		
		/// <summary>
		/// Updates the current status of the folders.
		/// </summary>
		public void UpdateFolderStatus ()
		{
			store.Status = store.SelectionStatus ();
			this.QueueDraw ();
		}
		
		
		/// <summary>
		/// Customized TreeStore which stores Folder classes.
		/// </summary>
		public FolderStore FolderStore
		{
			get{ return store; }
		}
		
		
		
		
		
		
		// filter the media treeview
		public override bool FilterMedia (Media media)
		{
			FolderMedia folder_media = (media as FolderMedia);
			
			if (folder_media == null || folder_media.Folder == null)
				return false;
			else if (folder_media.Folder.Visible)
				return find (folder_media.Title, folder_media.Artist, folder_media.Album);
			
			return false;
		}
		
		
		// when a user has clicked on the folder tree
		private void tree_button_release (object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button != 3) return;
			
			TreeIter iter;
			if (!this.Selection.GetSelected (out iter)) return;
			
			Folder folder = (Folder) store.GetValue (iter, 0);
			if (folder.Path == Utils.RootNode) return;
			
			FolderContextMenu menu = new FolderContextMenu (folder);
			menu.ShowAll ();
			menu.Popup ();
		}
		
		
		// change the visibility of the selected folder
		private void visible_toggled (object o, ToggledArgs args)
		{
			TreeIter iter;
			store.GetIter (out iter, new TreePath (args.Path));
			
			Folder folder = (Folder) store.GetValue (iter, 0);
			
			
			if (folder.Path == Utils.RootNode)
			{
				if (store.Status != SelectStatus.All)
					store.SelectAll ();
				else
					store.SelectNone ();
			}
			else
			{
				folder.Visible = !folder.Visible;
				store.DataManager.UpdateFolder (folder);
			}
			
			UpdateFolderStatus ();
			Global.Core.Library.MediaTree.Refilter ();
		}
		
		
		
		
		// render the folder text column
		private void renderText (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Folder folder = (Folder) model.GetValue (iter, 0);
			
			// if its the root node
			if (folder.Path == Utils.RootNode)
				(cell as CellRendererText).Markup = "<b>Library</b>";
			else
				(cell as CellRendererText).Text = Utils.GetFolderName (folder.Path);
		}
		
		
		// render the folder picture column
		private void renderPixbuf (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Folder folder = (Folder) model.GetValue (iter, 0);
			
			// if its the root node
			if (folder.Path == Utils.RootNode)
			{
				(cell as CellRendererPixbuf).StockId = Stock.Harddisk;
				(cell as CellRendererPixbuf).StockSize = (uint) IconSize.LargeToolbar;
				(cell as CellRendererPixbuf).Visible = true;
			}
			else (cell as CellRendererPixbuf).Visible = false;
		}
		
		
		// render the folder checkbox column
		private void renderCheckbox (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Folder folder = (Folder) model.GetValue (iter, 0);
			(cell as CellRendererToggle).Inconsistent = false;
			
			if (folder.Path == Utils.RootNode)
			{
				if (store.Status == SelectStatus.None)
					(cell as CellRendererToggle).Active = false;
				else if (store.Status == SelectStatus.All)
					(cell as CellRendererToggle).Active = true;
				else
					(cell as CellRendererToggle).Inconsistent = true;
			}
			else
				(cell as CellRendererToggle).Active = folder.Visible;
		}
		
	}
}
