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
	
	
	public delegate void TreeSelectedDelegate (TreeModel model, TreeIter iter);
	
	
	/// <summary>
	/// The folder tree view widget.
	/// </summary>
	public abstract class OrganizerTree : TreeView
	{
		
		
		public event TreeSelectedDelegate TreeSelected;
		
		
		public OrganizerTree () : base ()
		{
			this.Selection.SelectFunction = tree_select;
		}
		
		
		
		// the treeview is selected
		private bool tree_select (TreeSelection selection, TreeModel model, TreePath path, bool selected)
		{
			
			if (this != Global.Core.Library.DynamicTree)
				Global.Core.Library.DynamicTree.Selection.UnselectAll ();
			
			if (this != Global.Core.Library.FolderTree)
				Global.Core.Library.FolderTree.Selection.UnselectAll ();
			
			if (this != Global.Core.Library.PlaylistTree)
				Global.Core.Library.PlaylistTree.Selection.UnselectAll ();
			
			
            //if it isnt already selected
			if (!selected)
			{
				Global.Core.TopBar.SelectedTree = this;
				Global.Core.Library.MediaTree.SetFilter (FilterMedia);
				Global.Core.Library.MediaTree.Refilter ();
                
                //raise the event
                if (TreeSelected != null)
                {
                    TreeIter iter;
    				if (model.GetIter (out iter, path))
    					TreeSelected (model, iter);
                }
			}
			
			
			return true;
		}
		
		
		public abstract bool FilterMedia (Media media);
		public abstract void RemoveSelected ();
		
		
		// convenience function to apply the search string
		protected bool find (params string[] search_strings)
		{
			string search_value = Global.Core.TopBar.Search.Text.ToLower ();
			if (search_value.Length == 0)
				return true;
			
			foreach (string text in search_strings)
				if (text.ToLower().IndexOf (search_value) > -1)
					return true;
			
			
			return false;
		}
		
		
	}
}
