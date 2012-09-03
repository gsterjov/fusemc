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
	/// A low level class giving a window the functionalities of a dialog.
	/// </summary>
	public class DialogBase : Window
	{
		
		protected int retVal = -1;
		
		/// <summary>
		/// Changes the specified window into a dialog.
		/// </summary>
		protected DialogBase (Window transient, string title) : base (title)
		{
			this.DeleteEvent += window_delete;
			this.TransientFor = transient;
			this.Modal = true;
			this.WindowPosition = Gtk.WindowPosition.CenterOnParent;
		}
		
		
		/// <summary>
		/// Run the pseudo dialog
		/// </summary>
		public int Run ()
		{
			this.ShowAll ();
			while (retVal == -1)
				Application.RunIteration ();
			return retVal;
		}
		
		
		// default functionality for the closing of a dialog
		void window_delete (object o, EventArgs a)
		{
			this.Destroy ();
			retVal = 0;
		}
	}
}
