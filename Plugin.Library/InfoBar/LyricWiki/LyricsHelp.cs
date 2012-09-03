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
using System.Xml;
using System.Collections.Generic;
using Gtk;

namespace Fuse.Plugin.Library.Info.LyricWiki
{
	
	/// <summary>
	/// Shows details on how to use the lyrics panel.
	/// </summary>
	public class LyricsHelp : VBox
	{
		
		
		public LyricsHelp () : base (false, 0)
		{
			Label title = new Label ();
			Label details1 = new Label ();
			Label details2 = new Label ();
			
			
			title.Markup = "<b><big><big>Lyrics</big></big></b>";
			
			details1.Markup = "This panel displays the lyrics of the currently playing song";
			details2.Markup = "You can also search for an artist below which will give you a list of albums and tracks to choose from";
			
			
			details1.Wrap = true;
			details2.Wrap = true;
			
			VBox box = new VBox (false, 40);
			box.PackStart (details1, false, false, 0);
			box.PackStart (details2, false, false, 0);
			
			Alignment align = new Alignment (0.5f, 0.5f, 0, 0);
			align.Add (box);
			
			this.PackStart (title, false, false, 0);
			this.PackStart (new HSeparator (), false, false, 5);
			this.PackStart (align, true, true, 0);
			this.PackStart (new Image (null, "lyricwiki-logo.png"), false, false, 0);
		}
		
		
	}
	
	
	
}
