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
using System.Collections.Generic;
using Gtk;
using Fuse.Interfaces;
using Fuse.Plugin.Library.Info;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// The widget that contains the main library content.
	/// </summary>
	public class Library
	{
		
		DelegateQueue delegate_queue = new DelegateQueue ();
		
		// global widgets
		MediaTree media_tree = new MediaTree ();
		FolderTree folder_tree = new FolderTree ();
		PlaylistTree playlist_tree = new PlaylistTree ();
		DynamicTree dynamic_tree = new DynamicTree ();
		
		ScrolledWindow media_scroll = new ScrolledWindow ();
		InfoBar info_bar = new InfoBar ();
		
		HPaned main_splitter = new HPaned ();
		HPaned info_splitter = new HPaned ();
		
		HBox info_box = new HBox (false, 0);
		VBox media_box = new VBox (false, 0);
		Image cover_art = new Image ();
		
		
		
		// creates the library user interface
		public Library ()
		{
			// create widgets
			ScrolledWindow library_scroll = new ScrolledWindow ();
			VBox library_box = new VBox (false, 0);
			
			
			// pack widgets
			library_box.PackStart (dynamic_tree, false, false, 0);
			library_box.PackStart (new HSeparator (), false, false, 0);
			library_box.PackStart (folder_tree, false, false, 0);
			library_box.PackStart (new HSeparator (), false, false, 0);
			library_box.PackStart (playlist_tree, true, true, 0);
			
			library_scroll.AddWithViewport (library_box);
			library_scroll.ShadowType = ShadowType.In;
			
			
			media_scroll.Add (media_tree);
			media_scroll.ShadowType = ShadowType.In;
			media_box.PackStart (media_scroll, true, true, 0);
			
			
			// cover art box
			VBox image_box = new VBox (false, 0);
			image_box.PackStart (library_scroll, true, true, 0);
			image_box.PackStart (cover_art, false, false, 0);
			
			info_box.PackStart (media_box, true, true, 0);
			info_box.PackStart (info_bar, false, false, 0);
			
			info_splitter.Pack1 (info_box, true, true);
			
			main_splitter.Add1 (image_box);
			main_splitter.Add2 (info_splitter);
		}
		
		
		
		/// <summary>
		/// Shows the lyrics panel.
		/// </summary>
		public void ExpandInfoBar ()
		{
			info_box.Remove (info_bar);
			info_splitter.Pack2 (info_bar, false, false);
			info_splitter.ShowAll ();
		}
		
		/// <summary>
		/// Hides the lyrics panel.
		/// </summary>
		public void CollapseInfoBar ()
		{
			info_splitter.Remove (info_bar);
			info_box.PackStart (info_bar, false, false, 0);
			info_splitter.ShowAll ();
		}
		
		
		
		/// <summary>
		/// Sets image box pixbuf displaying the cover art.
		/// </summary>
		public void SetCoverArt (Gdk.Pixbuf pixbuf)
		{
			cover_art.Clear ();
			cover_art.Pixbuf = pixbuf;
		}
		
		
		
		
		/// <summary>The core widget which contains all the child widgets.</summary>
		public HPaned MainSplitter
		{ get{ return main_splitter; } }
		
		
		/// <summary>The VBox where the progress bars appear.</summary>
		public VBox MediaBox
		{ get{ return media_box; } }
		
		
		/// <summary>A queue of delegates waiting to be executed.</summary>
		public DelegateQueue DelegateQueue
		{ get{ return delegate_queue; } }
		
		
		/// <summary>The treeview which holds all the dynamic media.</summary>
		public DynamicTree DynamicTree
		{ get{ return dynamic_tree; } }
		
		
		/// <summary>The treeview which holds all the folders.</summary>
		public FolderTree FolderTree
		{ get{ return folder_tree; } }
		
		
		/// <summary>The treeview which holds all the playlists.</summary>
		public PlaylistTree PlaylistTree
		{ get{ return playlist_tree; } }
		
		
		/// <summary>The treeview which holds all the media files.</summary>
		public MediaTree MediaTree
		{ get{ return media_tree; } }
		
		
		/// <summary>The scrolled window which contains the media tree.</summary>
		public ScrolledWindow MediaScroll
		{ get{ return media_scroll; } }
		
		
		/// <summary>The information bar.</summary>
		public InfoBar InfoBar
		{ get{ return info_bar; } }
		
		
		/// <summary>The information bar splitter.</summary>
		public HPaned InfoSplitter
		{ get{ return info_splitter; } }
		
		
	}
}
