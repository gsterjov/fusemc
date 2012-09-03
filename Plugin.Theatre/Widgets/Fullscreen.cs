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
	/// The window that shows the video in fullscreen.
	/// </summary>
	public class Fullscreen
	{
		
		Window window;
		FullscreenControls controls;
		
		
		// create the window
		public Fullscreen (VideoWidget video_widget)
		{
			window = new Window ("Fuse - Fullscreen");
			controls = new FullscreenControls ();
			
			window.Add (video_widget);
			
			window.Events = Gdk.EventMask.PointerMotionMask;
			window.MotionNotifyEvent += window_motion;
			window.EnterNotifyEvent += window_enter;
		}
		
		
		
		/// <summary>
		/// Show the fullscreen window.
		/// </summary>
		public void Show ()
		{
			window.ShowAll ();
			window.Fullscreen ();
		}
		
		
		/// <summary>
		/// Hide the fullscreen window.
		/// </summary>
		public void Hide ()
		{
			window.HideAll ();
		}
		
		
		
		/// <summary>
		/// Whether or not the fullscreen window is visible.
		/// </summary>
		public bool Visible
		{
			get{ return window.Visible; }
		}
		
		
		
		// the mouse has moved in the window
		private void window_motion (object o, MotionNotifyEventArgs args)
		{
			if (args.Event.Y >= controls.PositionY)
				controls.ShowAll ();
		}
		
		// the mouse has entered the window
		private void window_enter (object o, EnterNotifyEventArgs args)
		{
			controls.HideAll ();
		}
		
		
	}
}
