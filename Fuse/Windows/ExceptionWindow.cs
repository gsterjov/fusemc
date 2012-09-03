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
	/// The window which displays the critical exception.
	/// </summary>
	public class ExceptionWindow : DialogBase
	{
		
		
		// creates the exception window
		public ExceptionWindow (Window transient, string exception) : base (transient, "An error has occured..")
		{
			VBox box = new VBox (false, 0);
			Label title = new Label ();
			title.Markup = "A critical error has occured within the application\n\n<b>Error Details:</b>";
			title.Xalign = 0;
			
			Button quit = new Button (Stock.Quit);
			TextView message = new TextView ();
			
			ScrolledWindow scroll = new ScrolledWindow ();
			scroll.AddWithViewport (message);
			
			quit.Clicked += quit_clicked;
			message.Buffer.Text = exception;
			
			box.PackStart (title, false, false, 0);
			box.PackStart (scroll, true, true, 0);
			box.PackStart (quit, false, false, 0);
			
			box.BorderWidth = 10;
			this.Add (box);
		}
		
		
		// the user clicked on the quit button
		void quit_clicked (object o, EventArgs args)
		{
			this.Destroy ();
			retVal = 0;
		}
		
	}
}
