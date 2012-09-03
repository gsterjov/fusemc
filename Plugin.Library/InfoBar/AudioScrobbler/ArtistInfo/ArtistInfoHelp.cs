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

namespace Fuse.Plugin.Library.Info.AudioScrobbler.ArtistInfo
{
	
	/// <summary>
	/// Shows details on how to use the audioscrobbler panel.
	/// </summary>
	public class ArtistInfoHelp : VBox
	{
		
		
		public ArtistInfoHelp () : base (false, 0)
		{
			Label title = new Label ();
			Label details1 = new Label ();
			Label details2 = new Label ();
			Label details3 = new Label ();
			
			
			title.Markup = "<b><big><big>Artist Info</big></big></b>";
			
			details1.Markup = "This panel displays information about the currently playing artist such as:";
			details2.Markup = "<i>Similar Artists\nSimilar Tracks\nTop Albums\nTop Tracks\nAlbum Details</i>";
			details3.Markup = "You can also search for an artist below";
			
			
			details1.Wrap = true;
			details2.Wrap = true;
			details3.Wrap = true;
			
			
			VBox details_box = new VBox (false, 0);
			details_box.PackStart (details1, false, false, 0);
			details_box.PackStart (details2, false, false, 2);
			details_box.PackStart (details3, false, false, 40);
			
			Alignment align = new Alignment (0.5f, 0.5f, 0, 0);
			align.Add (details_box);
			
			this.PackStart (title, false, false, 0);
			this.PackStart (new HSeparator (), false, false, 5);
			this.PackStart (align, true, true, 0);
			this.PackStart (new Image (null, "audioscrobbler-logo.png"), false, false, 0);
		}
		
		
	}
	
	
	
}
