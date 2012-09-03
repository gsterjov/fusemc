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

namespace Fuse.Plugin.Library.Info
{
	
	/// <summary>
	/// The search box used in the info bar.
	/// </summary>
	public class SearchBox : HBox
	{
		
		public delegate void SearchHandler (string search_string);
		public event SearchHandler Search;
		
		
		private Entry search_entry = new Entry ("Search Artist..");
		private Button search_button = new Button ();
		
		
		
		public SearchBox () : base (false, 0)
		{
			search_button.Image = new Image (Stock.Find, IconSize.Menu);
			search_button.Clicked += search_button_clicked;
			search_entry.Activated += search_button_clicked;
			search_entry.FocusGrabbed += search_entry_focus;
			search_entry.FocusOutEvent += search_entry_unfocus;
			
			
			this.BorderWidth = 5;
			this.PackStart (search_entry, true, true, 0);
			this.PackStart (search_button, false, false, 0);
		}
		
		
		
		//clear the default text automatically
		private void search_entry_focus (object o, EventArgs args)
		{
			if (search_entry.Text == "Search Artist..")
				search_entry.Text = "";
		}
		
		//enter the default text automatically
		private void search_entry_unfocus (object o, FocusOutEventArgs args)
		{
			if (search_entry.Text == "")
				search_entry.Text = "Search Artist..";
		}
		
		
		//begin the search
		private void search_button_clicked (object o, EventArgs args)
		{
			if (Search != null)
				Search (search_entry.Text);
		}
		
		
	}
}
