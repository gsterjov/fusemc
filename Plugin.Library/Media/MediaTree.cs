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
	
	public delegate bool MediaFilterFunc (Media media);
	
	/// <summary>
	/// The media tree view widget.
	/// </summary>
	public class MediaTree : TreeView
	{
		private Random random = new Random ();
		private MediaStore store = new MediaStore ();
		private TreeModelFilter filter;
		
		
		private string media_info_format = "<b>{0}</b>  <i>by</i>  <b>{1}</b>";
		
		private bool thaw;
		private MediaFilterFunc filter_func;
		
		private Media current_media;
		
		
		public MediaTree () : base ()
		{
			filter = new TreeModelFilter (store, null);
			
			this.Model = filter;
			this.RulesHint = true;
			
			
			// custom columns for the media tree
			SortColumn col_title = new SortColumn ("Title", new TreeCellDataFunc (renderTitle), this);
			SortColumn col_artist = new SortColumn ("Artist", new TreeCellDataFunc (renderArtist), this);
			SortColumn col_album = new SortColumn ("Album", new TreeCellDataFunc (renderAlbum), this);
			
			col_title.Clicked += delegate(object o, EventArgs args) {if (thaw) store.SetSortColumnId (0, col_title.SortOrder);};
			col_artist.Clicked += delegate(object o, EventArgs args) {if (thaw) store.SetSortColumnId (1, col_artist.SortOrder);};
			col_album.Clicked += delegate(object o, EventArgs args) {if (thaw) store.SetSortColumnId (2, col_album.SortOrder);};
			
			
			this.AppendColumn (col_title);
			this.AppendColumn (col_artist);
			this.AppendColumn (col_album);
			
			
			//events
			this.RowActivated += tree_activated;
			this.ButtonReleaseEvent += tree_button_release;
		}
		
		
		/// <summary>
		/// Let the treeview begin sorting.
		/// </summary>
		public void ThawTree ()
		{
			thaw = true;
			if (filter_func == null)
				filter_func = Global.Core.Library.FolderTree.FilterMedia;
			filter.VisibleFunc = filterMedia;
			filter.Refilter ();
			store.SetSortFunc (0, sort_title);
			store.SetSortFunc (1, sort_artist);
			store.SetSortFunc (2, sort_album);
			
			int i = 0;
			foreach (TreeViewColumn column in this.Columns)
			{
				if (column.SortIndicator)
					store.SetSortColumnId (i, column.SortOrder);
				i++;
			}
		}
		
		
		/// <summary>
		/// Navigate through the media list.
		/// </summary>
		public void Navigate (NavigateType type)
		{
			if (type == NavigateType.Next)
				next ();
			else if (type == NavigateType.Previous)
				previous ();
			else if (type == NavigateType.Shuffle)
				shuffle ();
		}
		
		
		/// <summary>
		/// Refilter the media library.
		/// </summary>
		public void Refilter ()
		{
			filter.Refilter ();
		}
		
		
		/// <summary>
		/// Sets the filter for the media store.
		/// </summary>
		public void SetFilter (MediaFilterFunc func)
		{
			filter_func = func;
		}
		
		
		/// <summary>
		/// Customized ListStore which stores Media classes.
		/// </summary>
		public MediaStore MediaStore
		{
			get{ return store; }
		}
		
		
		/// <summary>
		/// The currently active media.
		/// </summary>
		public Media CurrentMedia
		{
			get{ return current_media; }
		}
		
		
		
		// gets the tree path of the currently playing media file.
		private TreePath currentPath ()
		{
			TreePath playing_path = null;
			
			filter.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter)
			{
				Media media = (Media) model.GetValue (iter, 0);
				
				if (media.Path == Global.Core.Fuse.MediaControls.CurrentMedia)
				{
					playing_path = path.Copy ();
					return true;
				}
				return false;
			});
			
			return playing_path;
		}
		
		
		// selects the next media file on the list
		private void next ()
		{
			TreePath path = currentPath ();
			TreeIter iter;
			
			if (path == null || !filter.GetIter (out iter, path)) return;
			
			if (filter.IterNext (ref iter))
			{
				path.Next ();
				this.Selection.SelectPath (path);
				this.ActivateRow (path, this.Columns[0]);
			}
		}
		
		
		// selects the previous media file on the list
		private void previous ()
		{
			TreePath path = currentPath ();
			
			if (path == null) return;
			
			if (path.Prev ())
			{
				this.Selection.SelectPath (path);
				this.ActivateRow (path, this.Columns[0]);
			}
		}
		
		
		// selects a random media file on the list
		private void shuffle ()
		{
			int num = random.Next (filter.IterNChildren () - 1);
			TreePath path = new TreePath (num.ToString ());
			this.Selection.SelectPath (path);
			this.ActivateRow (path, this.Columns[0]);
		}
		
		
		
		// when a user has clicked on the media tree
		private void tree_button_release (object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button != 3) return;
			
			TreeIter iter;
			if (!this.Selection.GetSelected (out iter)) return;
			
			Media media = (Media) filter.GetValue (iter, 0);
			
			MediaContextMenu menu = new MediaContextMenu (media);
			menu.ShowAll ();
			menu.Popup ();
		}
		
		
		// the user double clicked on a row in the media library
		private void tree_activated (object o, RowActivatedArgs args)
		{
			TreeIter iter;
			filter.GetIter (out iter, args.Path);
			
			current_media = (Media) filter.GetValue (iter, 0);
			
			if (current_media is AudioCDMedia)
				Global.Core.Fuse.MediaControls.LoadTrack (current_media.TrackNumber, Navigate);
			else
				Global.Core.Fuse.MediaControls.LoadMedia (current_media.Path, Navigate);
			
			
			Gdk.Pixbuf cover_art = null;
			if (current_media is FileMedia)
				cover_art = Utils.LoadCoverArt ((current_media as FileMedia).Picture);
			
			Global.Core.Library.SetCoverArt (cover_art);
			
			
			string artist = Utils.ParseMarkup (current_media.Artist);
			string title = Utils.ParseMarkup (current_media.TitleOrFilename);
			
			Global.Core.Fuse.MediaControls.MediaInfo = string.Format (media_info_format, title, artist);
			this.QueueDraw ();
			
			Global.Core.PopupWidget (new MediaPopup (current_media, cover_art));
			Global.Core.Library.InfoBar.LoadMedia (current_media);
		}
		
		
		
		
		
		// filter the media treeview
		private bool filterMedia (TreeModel model, TreeIter iter)
		{
			Media media = (Media) model.GetValue (iter, 0);
			if (media == null) return false;
			return filter_func (media);
		}
		
		
		
		// sort by title for the media tree
		private int sort_title (TreeModel model, TreeIter tia, TreeIter tib)
		{
			Media media1 = (Media) model.GetValue (tia, 0);
			Media media2 = (Media) model.GetValue (tib, 0);
			
			if (media1.Title == media2.Title)
			{
				if (media1.Artist == media2.Artist)
					return media1.Album.CompareTo (media2.Album);
				else
					return media1.Artist.CompareTo (media2.Artist);
			}
			else
				return media1.Title.CompareTo (media2.Title);
		}
		
		
		
		// sort by artist for the media tree
		private int sort_artist (TreeModel model, TreeIter tia, TreeIter tib)
		{
			Media media1 = (Media) model.GetValue (tia, 0);
			Media media2 = (Media) model.GetValue (tib, 0);
			
			if (media1.Artist == media2.Artist)
			{
				if (media1.Album == media2.Album)
					return media1.TrackNumber.CompareTo (media2.TrackNumber);
				else
					return media1.Album.CompareTo (media2.Album);
			}
			else
				return media1.Artist.CompareTo (media2.Artist);
		}
		
		
		
		// sort by album for the media tree
		private int sort_album (TreeModel model, TreeIter tia, TreeIter tib)
		{
			Media media1 = (Media) model.GetValue (tia, 0);
			Media media2 = (Media) model.GetValue (tib, 0);
			
			if (media1.Album == media2.Album)
			{
				if (media1.Artist == media2.Artist)
					return media1.TrackNumber.CompareTo (media2.TrackNumber);
				else
					return media1.Artist.CompareTo (media2.Artist);
			}
			else
				return media1.Album.CompareTo (media2.Album);
		}
		
		
		
		
		
		// makes text bold if it is currently playing
		private string isPlaying (string text, string path)
		{
			if (Global.Core.Fuse.MediaControls.CurrentMedia == path)
				return "<b>" + Utils.ParseMarkup (text) + "</b>";
			return Utils.ParseMarkup (text);
		}
		
		
		// render the media artist column
		private void renderTitle (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Media media = (Media) model.GetValue (iter, 0);
			(cell as CellRendererText).Markup = isPlaying (media.TitleOrFilename, media.Path);
		}
		
		
		// render the media artist column
		private void renderArtist (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Media media = (Media) model.GetValue (iter, 0);
			(cell as CellRendererText).Markup = isPlaying (media.Artist, media.Path);
		}
		
		
		// render the media artist column
		private void renderAlbum (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Media media = (Media) model.GetValue (iter, 0);
			(cell as CellRendererText).Markup = isPlaying (media.Album, media.Path);
		}
		
	}
}
