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
	/// The dialog window for adding media.
	/// </summary>
	public class AddWindow : DialogBase
	{
		// global widgets
		ComboBox combo = new ComboBox ();
		
		
		// creates the add window user interface
		public AddWindow () : base (Global.Core.Fuse.MainWindow, "Add Media")
		{
			VBox backbone = new VBox (false, 0);
			
			Button directory = new Button ("Add Directory");
			Button files = new Button ("Add Files");
			Button create_list = new Button ("Create Playlist");
			
			RadioButton library = new RadioButton ("Add to Library");
			RadioButton playlist = new RadioButton (library, "Add to Playlist");
			
			
			// hook up widget events
			directory.Clicked += directory_clicked;
			files.Clicked += files_clicked;
			create_list.Clicked += create_list_clicked;
			
			library.Toggled += library_toggled;
			playlist.Toggled += playlist_toggled;
			
			
			// setup playlist combo box
			combo.Sensitive = false;
			combo.Model = Global.Core.Library.PlaylistTree.PlaylistStore;
			
			CellRendererText text = new CellRendererText ();
			combo.PackStart (text, true);
			combo.SetCellDataFunc (text, render);
			
			TreeIter iter;
			bool not_empty = combo.Model.GetIterFirst (out iter);
			playlist.Sensitive = not_empty;
			if (not_empty) combo.SetActiveIter (iter);
			
			
			// pack widgets
			backbone.PackStart (directory, false, false, 0);
			backbone.PackStart (files, false, false, 5);
			backbone.PackStart (create_list, false, false, 0);
			
			backbone.PackStart (new HSeparator (), false, false, 10);
			
			backbone.PackStart (library, false, false, 0);
			backbone.PackStart (playlist, false, false, 0);
			backbone.PackStart (combo, false, false, 0);
			
			
			backbone.BorderWidth = 15;
			this.Resizable = false;
			this.SkipPagerHint = true;
			this.SkipTaskbarHint = true;
			this.Add (backbone);
		}
		
		
		
		/// <summary>
		/// The playlist to add media in.
		/// </summary>
		public Playlist SelectedPlaylist
		{
			get
			{
				if (combo.Sensitive == false) return null;
				
				TreeIter iter;
				combo.GetActiveIter (out iter);
				return (Playlist) combo.Model.GetValue (iter, 0);
			}
		}
		
		
		
		// render the combo box
		void render (CellLayout layout, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Playlist node = (Playlist) model.GetValue (iter, 0);
			if (node != null)
				(cell as CellRendererText).Text = node.Name;
		}
		
		
		
		// the user clicked on the Add Directory button
		void directory_clicked (object o, EventArgs args)
		{
			this.Destroy ();
			retVal = 1;
		}
		
		// the user clicked on the Add Files button
		void files_clicked (object o, EventArgs args)
		{
			this.Destroy ();
			retVal = 2;
		}
		
		// the user clicked on the Create Playlist button
		void create_list_clicked (object o, EventArgs args)
		{
			this.Destroy ();
			retVal = 3;
		}
		
		
		
		// the user selected the Add to Library radio button
		void library_toggled (object o, EventArgs args)
		{
			combo.Sensitive = false;
		}
		
		// the user clicked on the Add to Playlist Button
		void playlist_toggled (object o, EventArgs args)
		{
			combo.Sensitive = true;
		}
		
	}
}
