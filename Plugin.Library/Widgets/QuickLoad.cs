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


using Gtk;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// The quick loading widget.
	/// </summary>
	public class QuickLoad
	{
		
		int current;
		int max;
		
		
		// global widgets
		EventBox box = new EventBox ();
		Label status = new Label ();
		
		
		
		// create the widget
		public QuickLoad (int max)
		{
			this.max = max;
			
			
			VBox backbone = new VBox (false, 5);
			Label header = new Label ();
			Label click_info = new Label ();
			
			click_info.Markup = "<i>(Click to see the media library)</i>";
			header.Markup = "<big><b>Quick Loading..</b></big>";
			
			click_info.Yalign = 0;
			header.Yalign = 1;
			status.Yalign = 0;
			
			backbone.PackStart (header, true, true, 0);
			backbone.PackStart (status, true, true, 0);
			backbone.PackStart (click_info, true, true, 20);
			box.Add (backbone);
		}
		
		
		
		
		/// <summary>
		/// The current progress.
		/// </summary>
		public int Current
		{
			get{ return current; }
			set
			{
				current = value;
				status.Text = current.ToString () + " of " + max.ToString ();
			}
		}
		
		
		
		
		/// <summary>
		/// The actual widget.
		/// </summary>
		public EventBox MainBox
		{
			get{ return box; }
		}
		
		
	}
}
