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
using System.Runtime.InteropServices;
using Gtk;
using Fuse.Interfaces;

namespace Fuse
{
	
	/// <summary>
	/// The main window.
	/// </summary>
	public class MainWindow : Window
	{
		FuseApp fuse;
		
		// global user interface widgets
		Notebook pages = new Notebook ();
		
		VBox bottom = new VBox (false, 0);
		HBox core_box = new HBox (false, 0);
		Image info_image = new Image (null, "fuse-logo.png");
		
		
		
		// creates the main window user interface
		public MainWindow (FuseApp fuse) : base ("Fuse")
		{
			this.fuse = fuse;
			this.DeleteEvent += window_deleted;
			
			
			bottom.PackStart (fuse.Controls, false, false, 0);
			core_box.PackStart (info_image, true, true, 0);
			
			VBox backbone = new VBox (false, 0);
			backbone.PackStart (fuse.Menu, false, false, 0);
			backbone.PackStart (core_box, true, true, 0);
			backbone.PackStart (bottom, false, false, 0);
			
			
			this.Icon = new Gdk.Pixbuf (null, "fuse-tray.png");
			this.Add (backbone);
		}
		
		
		
		/// <summary>
		/// Creates a new page in the main Notebook and
		/// adds the supplied widget to it.
		/// </summary>
		public void AddWidget (Widget widget, string title)
		{
			
			if (pages.NPages == 0)
			{
				core_box.Remove (info_image);
				core_box.PackStart (pages);
			}
			
			Label label = new Label (title);
			pages.AppendPage (widget, label);
			
			this.ShowAll ();
		}
		
		
		/// <summary>
		/// Removes the page in the main Notebook.
		/// </summary>
		public void RemoveWidget (Widget widget)
		{
			int index = pages.PageNum (widget);
			pages.RemovePage (index);
			
			if (pages.NPages == 0)
			{
				core_box.Remove (pages);
				core_box.PackStart (info_image);
			}
			
			this.ShowAll ();
		}
		
		
		
		/// <summary>
		/// The label of the current tab.
		/// </summary>
		public string CurrentTab
		{
			get{ return pages.GetTabLabelText (pages.CurrentPageWidget); }
		}
		
		
		/// <summary>
		/// The amount of tabs visible.
		/// </summary>
		public int TabCount
		{
			get{ return pages.NPages; }
		}
		
		
		
		public void SwitchToTab (string tab_name)
		{
			for (int i=0; i < pages.NPages; i++)
			{
				Widget page = pages.GetNthPage (i);
				if (pages.GetTabLabelText (page) == tab_name)
					pages.CurrentPage = i;
			}
		}
		
		
		
		/// <summary>
		/// The box at the bottom of the window.
		/// </summary>
		public VBox BottomBox
		{
			get{ return bottom; }
		}
		
		
		
		// when the user clicks the close button
		void window_deleted (object o, EventArgs args)
		{
			fuse.Quit ();
		}
		
		
	}
}


