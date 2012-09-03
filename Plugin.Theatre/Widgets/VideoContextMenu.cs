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
	/// The context menu for the video widget.
	/// </summary>
	public class VideoContextMenu : Menu
	{
		
		// aspect ratio menu items
		RadioMenuItem aspect_auto;
		RadioMenuItem aspect_4x3;
		RadioMenuItem aspect_16x9;
		RadioMenuItem aspect_16x10;
		
		
		// create the context menu
		public VideoContextMenu () : base ()
		{
			MenuItem aspect_ratio = new MenuItem ("Aspect Ratio");
			ImageMenuItem fullscreen = new ImageMenuItem (Stock.Fullscreen, null);
			
			
			Menu aspect_menu = new Menu ();
			aspect_auto = new RadioMenuItem ("Auto");
			aspect_4x3 = new RadioMenuItem (aspect_auto, "4:3");
			aspect_16x9 = new RadioMenuItem (aspect_auto, "16:9");
			aspect_16x10 = new RadioMenuItem (aspect_auto, "16:10");
			
			aspect_menu.Add (aspect_auto);
			aspect_menu.Add (aspect_4x3);
			aspect_menu.Add (aspect_16x9);
			aspect_menu.Add (aspect_16x10);
			
			aspect_ratio.Submenu = aspect_menu;
			
			this.Add (aspect_ratio);
			
			showVisualisations ();
			
			this.Add (new SeparatorMenuItem ());
			this.Add (fullscreen);
			
			
			fullscreen.Activated += fullscreen_activated;
			
			aspect_auto.ButtonReleaseEvent += aspect_auto_toggled;
			aspect_4x3.ButtonReleaseEvent += aspect_4x3_toggled;
			aspect_16x9.ButtonReleaseEvent += aspect_16x9_toggled;
			aspect_16x10.ButtonReleaseEvent += aspect_16x10_toggled;
			
			toggle_aspect_value ();
		}
		
		
		private void showVisualisations ()
		{
			MenuItem visuals = new MenuItem ("Visualisations");
			Menu vis_menu = new Menu ();
			visuals.Submenu = vis_menu;
			
			RadioMenuItem no_vis = new RadioMenuItem ("No Visualisation");
			if (Global.Core.Theatre.Visualisation == null)
				no_vis.Active = true;
			else
				no_vis.Active = false;
			
			no_vis.ButtonReleaseEvent += delegate (object o, ButtonReleaseEventArgs args)
			{ Global.Core.Theatre.Visualisation = ""; };
			
			vis_menu.Add (no_vis);
			vis_menu.Add (new SeparatorMenuItem ());
			
			
			string[] list = Global.Core.Fuse.MediaControls.MediaEngine.VisualisationList;
			foreach (string vis in list)
			{
				RadioMenuItem vis_item = new RadioMenuItem (no_vis, vis);
				vis_menu.Add (vis_item);
				
				string vis_clone = vis;
				vis_item.ButtonReleaseEvent += delegate (object o, ButtonReleaseEventArgs args)
				{ Global.Core.Theatre.Visualisation = vis_clone; };
				
				if (vis == Global.Core.Theatre.Visualisation)
					vis_item.Active = true;
				else
					vis_item.Active = false;
			}
			
			this.Add (visuals);
		}
		
		
		
		// fullscreen was clicked
		void fullscreen_activated (object o, EventArgs args)
		{
			Global.Core.Theatre.Fullscreen ();
		}
		
		
		// toggle the right radio menu item
		void toggle_aspect_value ()
		{
			aspect_auto.Active = false;
			aspect_4x3.Active = false;
			aspect_16x9.Active = false;
			aspect_16x10.Active = false;
			
			
			//ToString() gives us a true statement since comparing to floats
			//that have the same value doesnt always work. not precise enough?
			
			if (Global.Core.Theatre.AspectRatio.ToString() == (0).ToString())
				aspect_auto.Active = true;
			
			else if (Global.Core.Theatre.AspectRatio.ToString() == (4f/3f).ToString())
				aspect_4x3.Active = true;
			
			else if (Global.Core.Theatre.AspectRatio.ToString() == (16f/9f).ToString())
				aspect_16x9.Active = true;
			
			else if (Global.Core.Theatre.AspectRatio.ToString() == (16f/10f).ToString())
				aspect_16x10.Active = true;
			
		}
		
		
		// auto aspect ratio
		void aspect_auto_toggled (object o, ButtonReleaseEventArgs args)
		{
			Global.Core.Theatre.AspectRatio = 0;
		}
		
		// 4x3 aspect ratio
		void aspect_4x3_toggled (object o, ButtonReleaseEventArgs args)
		{
			Global.Core.Theatre.AspectRatio = 4f/3f;
		}
		
		// 16x9 aspect ratio
		void aspect_16x9_toggled (object o, ButtonReleaseEventArgs args)
		{
			Global.Core.Theatre.AspectRatio = 16f/9f;
		}
		
		// 16x10 aspect ratio
		void aspect_16x10_toggled (object o, ButtonReleaseEventArgs args)
		{
			Global.Core.Theatre.AspectRatio = 16f/10f;
		}
		
		
	}
}
