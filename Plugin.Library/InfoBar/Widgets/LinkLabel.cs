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
	/// A clickable label.
	/// </summary>
	public class LinkLabel : Alignment
	{
		
		private string link;
		private Label label = new Label ();
		private EventBox eb = new EventBox ();
		
		
		
		public LinkLabel (string markup, string link) : base (0, 0, 1, 1)
		{
			this.link = link;
			
			label.Xalign = 0;
			label.Ellipsize = Pango.EllipsizeMode.End;
			label.Markup = markup;
			
			eb.Add (label);
			this.Add (eb);
			
			eb.Realized += realized;
			eb.EnterNotifyEvent += mouse_enter;
			eb.LeaveNotifyEvent += mouse_leave;
		}
		
		
		
		private void mouse_enter (object o, EnterNotifyEventArgs args)
		{
			eb.State = StateType.Selected;
		}
		
		private void mouse_leave (object o, LeaveNotifyEventArgs args)
		{
			eb.State = StateType.Normal;
		}
		
		
		private void realized (object o, EventArgs args)
		{
			eb.GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Hand2);
		}
		
		
		public string Link
		{
			get{ return link;}
			set{ link = value; }
		}
		
		
		
		public Label Label
		{
			get{ return label; }
		}
		
	}
}
