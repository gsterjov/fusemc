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
using System.Collections.Generic;
using Gtk;

namespace Fuse.Plugin.Library.Info
{
	
	/// <summary>
	/// The page navigator box.
	/// </summary>
	public class PageNavigator <T> : HBox
	{
		
		private PagedList <T> list;
		
		
		private HBox page_box = new HBox (false, 0);
		private Label page_label = new Label ();
		private Button prev = new Button ();
		private Button next = new Button ();
		
		
		public event EventHandler PageChanged;
		
		
		public PageNavigator (PagedList <T> list) : base (false, 0)
		{
			this.list = list;
			
			
			prev.Image = new Image (Stock.GoBack, IconSize.Button);
			next.Image = new Image (Stock.GoForward, IconSize.Button);
			
			page_box.PackStart (prev, false, false, 0);
			page_box.PackStart (page_label, true, true, 0);
			page_box.PackStart (next, false, false, 0);
			
			this.PackStart (page_box);
			
			prev.Clicked += prev_clicked;
			next.Clicked += next_clicked;
		}
		
		
		
		
		// update the page number
		public void UpdatePageNumber ()
		{
			int page_number = list.PageNumber;
			int total_pages = list.TotalPages;
			
			if (total_pages > 1)
			{
				page_label.Markup = "<small>Page " + page_number + " of " + total_pages + "</small>";
				prev.Sensitive = list.HasPrevious;
				next.Sensitive = list.HasNext;
				
				if (page_box.Parent == null)
					this.PackStart (page_box, false, false, 0);
				
			}
			else if (page_box.Parent != null)
				this.Remove (page_box);
			
			this.ShowAll ();
		}
		
		
		
		//go to the previous page
		private void prev_clicked (object o, EventArgs args)
		{
			list.PreviousPage ();
			UpdatePageNumber ();
			
			if (PageChanged != null)
				PageChanged (this, new EventArgs ());
		}
		
		
		//go to the next page
		private void next_clicked (object o, EventArgs args)
		{
			list.NextPage ();
			UpdatePageNumber ();
			
			if (PageChanged != null)
				PageChanged (this, new EventArgs ());
		}
		
		
	}
}
