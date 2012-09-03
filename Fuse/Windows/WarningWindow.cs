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

namespace Fuse
{
	
	/// <summary>
	/// The window which displays the error log.
	/// </summary>
	public class WarningWindow : DialogBase
	{
		
		StatusBar status;
		TreeView tree = new TreeView ();
		Label message = new Label ();
		
		
		// creates the exception window
		public WarningWindow (Window transient, StatusBar status) : base (transient, "Error Log")
		{
			this.status = status;
			
			tree.Model = status.ErrorLog;
			tree.HeadersVisible = false;
			tree.AppendColumn (null, new CellRendererText (), new TreeCellDataFunc (render_error));
			
			message.Selectable = true;
			message.Xalign = 0;
			message.Yalign = 0;
			message.WidthChars = -1;
			
			
			Label tree_title = new Label ();
			Label message_title = new Label ();
			tree_title.Xalign = 0;
			tree_title.Markup = "<b>Error Log:</b>";
			message_title.Xalign = 0;
			message_title.Markup = "<b>Error Message:</b>";
			
			
			VBox box = new VBox (false, 0);
			Button remove_button = new Button (Stock.Remove);
			
			ScrolledWindow tree_scroll = new ScrolledWindow ();
			ScrolledWindow message_scroll = new ScrolledWindow ();
			tree_scroll.Add (tree);
			message_scroll.AddWithViewport (message);
			
			
			tree_scroll.ShadowType = ShadowType.In;
			message_scroll.ShadowType = ShadowType.In;
			
			
			tree.Selection.Changed += tree_selected;
			remove_button.Clicked += remove_clicked;
			
			
			box.PackStart (tree_title, false, false, 5);
			box.PackStart (tree_scroll, false, false, 0);
			box.PackStart (message_title, false, false, 5);
			box.PackStart (message_scroll, true, true, 0);
			box.PackStart (remove_button, false, true, 5);
			
			box.BorderWidth = 5;
			
			this.Resize (600, 400);
			this.Add (box);
		}
		
		
		// the user clicked on the remove button
		void remove_clicked (object o, EventArgs args)
		{
			TreeIter iter;
			if (tree.Selection.GetSelected (out iter))
			{
				status.ErrorLog.Remove (ref iter);
				
				if (status.ErrorLog.IterNChildren () == 0)
				{
					status.Icon = StatusIcon.None;
					status.Pop ();
					
					// close the window
					this.Destroy ();
					retVal = 0;
				}
				else if (tree.Model.GetIterFirst (out iter))
					tree.Selection.SelectIter (iter);
			}
			
		}
		
		
		// the user selected an error message from the tree
		void tree_selected (object o, EventArgs args)
		{
			TreeIter iter;
			if (tree.Selection.GetSelected (out iter))
			{
				string error_message = (string) status.ErrorLog.GetValue (iter, 1);
				message.Text = error_message;
			}
			
		}
		
		
		
		void render_error (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			string error = (string) model.GetValue (iter, 0);
			(cell as CellRendererText).Markup = parseMarkup (error);
		}
		
		
		
		
		// makes the input pango compatible
		string parseMarkup (string text)
		{
			text = Utils.ParseMarkup (text);
			
			bool make_bold = text.IndexOf ("::") >= 0;
			if (make_bold)
				text = "<b>" + text.Replace ("::", "::</b>");
			
			return text;
		}
		
		
	}
}
