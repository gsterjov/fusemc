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

namespace Fuse.Plugin.Theatre
{
	
	/// <summary>
	/// The top bar for the theatre.
	/// </summary>
	public class TopBar : HBox
	{
		
		// create the TopBar widget
		public TopBar ()
		{
			// create the widgets
			Button add_button = new Button (Stock.Add);
			Button remove_button = new Button (Stock.Remove);
			
			
			// hook up the widget events
			add_button.Clicked += new EventHandler (add_clicked);
			remove_button.Clicked += remove_clicked;
			
			// homogeneous button box
			HBox button_box = new HBox (true, 0);
			button_box.PackStart (add_button, false, true, 0);
			button_box.PackStart (remove_button, false, true, 0);
			
			
			// pack widgets
			this.PackStart (button_box, false, false, 0);
		}
		
		
		
		
		// the user clicked on the add button
		void add_clicked (object o, EventArgs args)
		{
			string[] paths = Dialogs.ChooseFiles ();
			if (paths == null)
				return;
			
			foreach (string path in paths)
				Global.Core.Theatre.Add (path);
		}
		
		
		// the user clicked on the remove button
		void remove_clicked (object o, EventArgs args)
		{
			Global.Core.Theatre.Remove ();
		}
		
	}
}
