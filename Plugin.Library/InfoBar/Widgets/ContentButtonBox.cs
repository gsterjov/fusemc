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
	/// The content button box.
	/// </summary>
	public class ContentButtonBox
	{
		
		public delegate void ButtonToggledHandler (Content button);
		public event ButtonToggledHandler ButtonToggled;
		
		
		private HBox box = new HBox (false, 0);
		private ContentButton selected_button;
		private bool first_button = true;
		
		
		
		public ContentButtonBox ()
		{
			box.PackStart (new VSeparator (), false, false, 2);
		}
		
		
		/// <summary>
		/// The panel content.
		/// </summary>
		public void AddContent (string title, Content content)
		{
			ContentButton button = new ContentButton (title, content);
			box.PackStart (button, false, false, 0);
			box.PackStart (new VSeparator (), false, false, 2);
			
			if (first_button)
			{
				button.Active = true;
				first_button = false;
			}
			
			button.Toggled += button_toggled;
		}
		
		
		
		/// <summary>The button box.</summary>
		public HBox DisplayWidget
		{ get{ return box; } }
		
		
		
		//a button was toggled
		private void button_toggled (object o, EventArgs args)
		{
			ContentButton active_button = (ContentButton)o;
			
			if (selected_button == active_button && active_button.Active == false)
				active_button.Active = true;
			
			
			if (active_button.Active == false || active_button == selected_button)
				return;
			
			
			selected_button = active_button;
			foreach (Widget widget in box.Children)
			{
				if (widget is ContentButton)
				{
					ContentButton button = (ContentButton)widget;
					if (button != active_button)
						button.Active = false;
				}
			}
			
			if (ButtonToggled != null)
				ButtonToggled (active_button.PanelContent);
		}
		
	}
}
