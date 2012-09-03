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
using Gtk;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// A widget with a border.
	/// </summary>
	public class BorderWidget : HBox
	{
		
		private EventBox box = new EventBox ();
		
		public BorderWidget (Widget widget)
		{
			this.PackStart (box, false, false, 0);
			box.Add (widget);
			box.ExposeEvent += expose_box;
			box.SizeRequested += size_requested;
		}
		
		
		// make it big enough for the border
		private void size_requested (object o, SizeRequestedArgs args)
		{
			int width = args.Requisition.Width;
			int height = args.Requisition.Height;
			
			box.SetSizeRequest (width + 2, height + 2);
		}
		
		
		
		// give it a border
		private void expose_box (object o, ExposeEventArgs args)
		{
			int width, height;
			box.GetSizeRequest (out width, out height);
			box.GdkWindow.DrawRectangle (this.Style.ForegroundGC (this.State), false, 0, 0, width-1, height-1);
		}
		
		
	}
}
