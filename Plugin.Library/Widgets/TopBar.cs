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
using Fuse.Interfaces;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// The top bar for the library.
	/// </summary>
	public class TopBar : HBox
	{
		// global widgets
		private Entry search = new Entry ();
		private EventBox clear_box = new EventBox ();
		private OrganizerTree selected_tree;
		
		
		// create the TopBar widget
		public TopBar ()
		{
			// create the widgets
			Button add_button = new Button (Stock.Add);
			Button remove_button = new Button (Stock.Remove);
			Label search_label = new Label ("Search:");
			
			Image clear_image = new Image (Stock.Clear, IconSize.Menu);
			clear_box.Add (clear_image);
			
			
			// hook up the widget events
			add_button.Clicked += add_clicked;
			remove_button.Clicked += remove_clicked;
			search.Changed += search_changed;
			clear_box.ButtonReleaseEvent += clear_released;
			clear_box.Realized += clear_realized;
			
			
			// homogeneous button box
			HBox button_box = new HBox (true, 0);
			button_box.PackStart (add_button, false, true, 0);
			button_box.PackStart (remove_button, false, true, 0);
			
			
			// pack widgets
			this.PackStart (button_box, false, true, 0);
			this.PackStart (search_label, false, false, 5);
			this.PackStart (search, true, true, 0);
			this.PackStart (clear_box, false, false, 0);
		}
		
		
		
		// the user click on the add button
		void add_clicked (object o, EventArgs args)
		{
			AddWindow window = new AddWindow ();
			int ret = window.Run ();
			Playlist playlist = window.SelectedPlaylist;
			
			
			// 1 = add directory; 2 = add files; 3 = create playlist
			if (ret == 1)
			{
				string folder = Dialogs.ChooseFolder ();
				if (folder != null)
				{
					if (playlist == null)
						Global.Core.Library.FolderTree.FolderStore.AddFolder (folder);
					else
						Global.Core.Library.PlaylistTree.PlaylistStore.AddFolder (folder, playlist);
				}
			}
			else if (ret == 2)
			{
				string[] files = Dialogs.ChooseFiles ();
				if (playlist == null)
					Global.Core.Library.FolderTree.FolderStore.AddFiles (files);
				else
					Global.Core.Library.PlaylistTree.PlaylistStore.AddFiles (files, playlist);
			}
			else if (ret == 3)
				Global.Core.Library.PlaylistTree.PlaylistStore.CreatePlaylist ();
			
		}
		
		
		// the user click on the remove button
		void remove_clicked (object o, EventArgs args)
		{
			if (selected_tree != null)
				selected_tree.RemoveSelected ();
		}
		
		
		// the user click on the clear box
		void clear_released (object o, ButtonReleaseEventArgs args)
		{
			search.Text = "";
		}
		
		// make the clear image clickable
		void clear_realized (object o, EventArgs args)
		{
			clear_box.GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Hand2);
		}
		
		
		// the user changed the search string
		void search_changed (object o, EventArgs args)
		{
			Global.Core.Library.MediaTree.Refilter ();
		}
		
		
		
		/// <summary>
		/// The search text field used to filter the tree.
		/// </summary>
		public Entry Search
		{
			get{ return search; }
		}
		
		
		/// <summary>
		/// The currently selected tree.
		/// </summary>
		public OrganizerTree SelectedTree
		{
			set{ selected_tree = value;; }
		}
		
	}
}
