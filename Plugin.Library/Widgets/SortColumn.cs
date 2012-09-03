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
	/// A column widget that sorts the tree.
	/// </summary>
	public class SortColumn : TreeViewColumn
	{
		
		private TreeView tree;
		
		
		public SortColumn (string title, TreeCellDataFunc cell_data, TreeView tree) : base ()
		{
			this.tree = tree;
			CellRendererText cell = new CellRendererText ();
			this.PackStart (cell, true);
			this.SetCellDataFunc (cell, cell_data);
			
			this.Title = title;
			this.Resizable = true;
			this.Clickable = true;
			this.Reorderable = true;
			this.FixedWidth = 150;
			this.Spacing = 5;
			this.Sizing = TreeViewColumnSizing.Fixed;
			this.Clicked += column_clicked;
		}
		
		
		
		// the column was clicked
		private void column_clicked (object o, EventArgs args)
		{
			foreach (TreeViewColumn col in tree.Columns)
			{
				if (col != this)
				{
					col.SortIndicator = false;
					col.SortOrder = SortType.Ascending;
				}
			}
			
			if (!SortIndicator)
				SortOrder = SortType.Descending;
			
			if (SortOrder == SortType.Ascending)
				SortOrder = SortType.Descending;
			else
				SortOrder = SortType.Ascending;
			
			
			SortIndicator = true;
		}
		
	}
}
