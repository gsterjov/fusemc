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
	/// The button used in the info bar.
	/// </summary>
	public class InfoButton : ToggleButton
	{
		
		private InfoPanel panel;
		
		
		public InfoButton (InfoPanel panel, string title)
		{
			this.panel = panel;
			
			Label label = new Label ();
			label.Markup = Utils.ParseMarkup (title);
			label.Angle = 90;
			
			this.Child = label;
			this.Relief = ReliefStyle.None;
			this.FocusOnClick = false;
			this.CanFocus = false;
		}
		
		
		
		/// <summary>
		/// The info panel.
		/// </summary>
		public InfoPanel Panel
		{
			get{ return panel; }
		}
		
		
		
	}
}
