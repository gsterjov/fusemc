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
using System.Threading;
using System.Collections.Generic;
using Gtk;

namespace Fuse.Plugin.Library.Info
{
	
	using LyricWiki;
	using AudioScrobbler.ArtistInfo;
	using AudioScrobbler.Profile;
	
	
	/// <summary>
	/// Shows information about an artist or song.. or whatever.
	/// </summary>
	public class InfoBar : VBox
	{
		
		private List <InfoButton> tabs = new List <InfoButton> ();
		private InfoButton active_button;
		
		private bool expanded;
		
		
		//global widgets
		private EventBox close_box = new EventBox ();
		private HBox main_box = new HBox (false, 0);
		private VBox button_box = new VBox (true, 5);
		private VBox panel_box = new VBox (false, 0);
		private Frame panel_frame = new Frame ();
		
		private Alignment custom_align = new Alignment (0, 0, 0, 0);
		
		
		
		public InfoBar () : base ()
		{
			// top panel bar
			Image close = new Image (Stock.Close, IconSize.Menu);
			close_box.Realized += close_box_realized;
			close_box.ButtonReleaseEvent += close_box_released;
			close_box.Add (close);
			
			Label title = new Label ();
			title.Markup = "<small>Information Bar</small>";
			title.Angle = 90;
			
			
			VBox left_box = new VBox (false, 0);
			left_box.PackStart (button_box, false, false, 0);
			left_box.PackStart (title, true, true, 0);
			
			
			Alignment align = new Alignment (1, 0.5f, 0, 0);
			align.Add (close_box);
			
			HBox panel_header = new HBox (false, 0);
			panel_header.PackStart (custom_align, true, true, 0);
			panel_header.PackStart (align, false, false, 0);
			
			panel_box.PackStart (panel_header, false, false, 0);
			
			
			panel_frame.Add (panel_box);
			main_box.PackStart (left_box, false, false, 0);
			this.PackStart (main_box, true, true, 0);
			
			
			addInfoPanel (new Lyrics (this), "Lyrics");
			addInfoPanel (new ArtistInfo (this), "Artist Info");
			addInfoPanel (new Profile (this), "Profile");
		}
		
		
		
		/// <summary>
		/// Adds a custom menu to the panel.
		/// </summary>
		public void AddCustomMenu (Widget widget)
		{
			if (custom_align.Child != null)
				custom_align.Remove (custom_align.Child);
			
			custom_align.Add (widget);
		}
		
		
		
		/// <summary>
		/// Loads the specified media's information.
		/// </summary>
		public void LoadMedia (Media media)
		{
			if (active_button != null && media != null)
				active_button.Panel.LoadMedia (media);
		}
		
		
		/// <summary>
		/// Loads the specified media's information.
		/// </summary>
		public void LoadMedia (Media media, Type panel_type)
		{
			foreach (InfoButton button in tabs)
				if (button.Panel.InfoType == panel_type)
					button.Active = true;
			
			LoadMedia (media);
		}
		
		
		
		// adds an info panel to the info bar
		private void addInfoPanel (InfoPanel panel, string title)
		{
			InfoButton button = new InfoButton (panel, title);
			button.Clicked += tab_clicked;
			
			tabs.Add (button);
			button_box.PackStart (button, false, true, 0);
		}
		
		
		
		//expands the info bar
		private void expandBar ()
		{
			if (expanded)
				return;
			
			main_box.PackStart (panel_frame, true, true, 0);
			Global.Core.Library.ExpandInfoBar ();
			expanded = true;
		}
		
		
		//collapses the info bar
		private void collapseBar ()
		{
			if (!expanded)
				return;
			
			panel_box.Remove (active_button.Panel.DisplayWidget);
			active_button = null;
			
			main_box.Remove (panel_frame);
			Global.Core.Library.CollapseInfoBar ();
			expanded = false;
		}
		
		
		
		//a tab has been selected
		private void tab_clicked (object o, EventArgs args)
		{
			InfoButton button = (InfoButton)o;
			
			if (button != active_button && !button.Active)
				return;
			
			
			if (active_button != null)
				panel_box.Remove (active_button.Panel.DisplayWidget);
			
			
			
			panel_box.PackStart (button.Panel.DisplayWidget, true, true, 0);
			active_button = button;
			
			
			
			foreach (InfoButton tab in tabs)
				if (tab != button)
					tab.Active = false;
			
			
			if (button.Active)
				expandBar ();
			else
				collapseBar ();
			
			
			LoadMedia (Global.Core.Library.MediaTree.CurrentMedia);
			this.ShowAll ();
		}
		
		
		
		//make the close image clickable
		private void close_box_realized (object o, EventArgs args)
		{
			close_box.GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Hand2);
		}
		
		
		//close the lyric panel
		private void close_box_released (object o, ButtonReleaseEventArgs args)
		{
			active_button.Active = false;
		}
		
	}
}
