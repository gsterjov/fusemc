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
	public class DynamicTree : OrganizerTree
	{
		
		private ListStore store = new ListStore (typeof (DynamicMedia));
		private DynamicMedia selected;
		
		
		public DynamicTree () : base ()
		{
			CellRendererPixbuf pic = new CellRendererPixbuf ();
			
			this.Model = store;
			this.HeadersVisible = false;
			this.AppendColumn (null, pic, new TreeCellDataFunc (renderPixbuf));
			this.AppendColumn (null, new CellRendererText (), new TreeCellDataFunc (renderText));
			
			store.AppendValues (new AudioCD ());
			
			this.TreeSelected += tree_selected;
		}
		
		
		/// <summary>
		/// The ListStore which stores DynamicMedia classes.
		/// </summary>
		public ListStore ListStore
		{
			get{ return store; }
		}
		
		
		// cannot remove dynamic media so do nothing
		public override void RemoveSelected ()
		{}
		
		
		// filter the media treeview
		public override bool FilterMedia (Media media)
		{
			if (selected is AudioCD)
				return (media is AudioCDMedia);
			
			return false;
		}
		
		
		
		// when a user selected a node from the dynamic tree
		private void tree_selected (TreeModel model, TreeIter iter)
		{
			selected = (DynamicMedia) model.GetValue (iter, 0);
			selected.LoadMedia ();
		}
		
		
		// the text renderer for the treeview
		private void renderText (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			DynamicMedia dynamic = (DynamicMedia) model.GetValue (iter, 0);
			(cell as CellRendererText).Markup = dynamic.Title;
		}
		
		
		// the pixbuf renderer for the treeview
		private void renderPixbuf (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			DynamicMedia dynamic = (DynamicMedia) model.GetValue (iter, 0);
			(cell as CellRendererPixbuf).StockId = dynamic.StockIcon;
			(cell as CellRendererPixbuf).StockSize = (uint) IconSize.LargeToolbar;
			(cell as CellRendererPixbuf).Visible = true;
		}
		
		
		
	}
}
