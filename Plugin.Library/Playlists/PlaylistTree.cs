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
	/// The playlist tree view widget.
	/// </summary>
	public class PlaylistTree : OrganizerTree
	{
		private PlaylistStore store = new PlaylistStore ();
		private Playlist selected;
		
		
		public PlaylistTree () : base ()
		{
			CellRendererText playlist = new CellRendererText ();
			playlist.Editable = true;
			
			this.Model = store;
			this.HeadersVisible = false;
			this.AppendColumn (null, playlist, new TreeCellDataFunc (renderText));
			
			
			playlist.Edited += playlist_edited;
			this.ButtonReleaseEvent += tree_button_release;
			this.TreeSelected += tree_selected;
		}
		
		
		
		/// <summary>
		/// Removes the playlist from the media library.
		/// </summary>
		public override void RemoveSelected ()
		{
			TreeIter iter;
			if (!this.Selection.GetSelected (out iter)) return;
			
			
			Playlist playlist = (Playlist) store.GetValue (iter, 0);
			store.Remove (ref iter);
			
			// remove all media within the playlist
			foreach (Media media in playlist.MediaList)
				Global.Core.Library.MediaTree.MediaStore.RemoveMedia (media);
			playlist.MediaList.Clear ();
			
			store.DataManager.DeletePlaylist (playlist);
			playlist = null;
		}
		
		
		
		/// <summary>
		/// Customized ListStore which stores Playlist classes.
		/// </summary>
		public PlaylistStore PlaylistStore
		{
			get{ return store; }
		}
		
		
		
		
		// filter the media treeview
		public override bool FilterMedia (Media media)
		{
			PlaylistMedia playlist_media = (media as PlaylistMedia);
			
			if (playlist_media == null || playlist_media.Playlist == null)
				return false;
			else if (playlist_media.Playlist == selected)
				return find (playlist_media.Title, playlist_media.Artist, playlist_media.Album);
			
			return false;
		}
		
		
		
		// when a user selected a node from the playlist tree
		private void tree_selected (TreeModel model, TreeIter iter)
		{
			selected = (Playlist) model.GetValue (iter, 0);
		}
		
		
		// when a user has clicked on the playlist tree
		private void tree_button_release (object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button != 3) return;
			
			TreeIter iter;
			if (!this.Selection.GetSelected (out iter)) return;
			
			Playlist playlist = (Playlist) store.GetValue (iter, 0);
			
			PlaylistContextMenu menu = new PlaylistContextMenu (playlist);
			menu.ShowAll ();
			menu.Popup ();
		}
		
		
		
		// the user has edited the playlist name
		private void playlist_edited (object o, EditedArgs args)
		{
			TreeIter iter;
			store.GetIter (out iter, new TreePath (args.Path));
			Playlist playlist = (Playlist) store.GetValue (iter, 0);
			
			if (playlist.Name == args.NewText) return;
			
			if (!store.PlaylistExists (args.NewText))
			{
				string old_name = playlist.Name;
				playlist.Name = args.NewText;
				store.DataManager.UpdatePlaylist (playlist, old_name);
			}
			else
				Global.Core.Fuse.ThrowError ("Another playlist already has the name:\n" + args.NewText);
		}
		
		
		
		// render the playlist text column
		private void renderText (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Playlist playlist = (Playlist) model.GetValue (iter, 0);
			(cell as CellRendererText).Text = playlist.Name;
		}
		
	}
}
