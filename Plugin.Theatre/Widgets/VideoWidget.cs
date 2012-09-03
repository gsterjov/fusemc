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
using System.Runtime.InteropServices;
using Gtk;


namespace Fuse.Plugin.Theatre
{
		
	/// <summary>
	/// The widget that actually shows the video.
	/// </summary>
	public class VideoWidget : EventBox
	{
		
		ulong drawable_xid = 0;
		bool active = false;
		
		// global widgets
		public DrawingArea drawable = new DrawingArea ();
		AspectFrame aspect = new AspectFrame (null, 0.5f, 0.5f, 1, false);
		
		Gdk.Pixbuf logo = new Gdk.Pixbuf (null, "fuse-logo-theatre.png");
		
		
		// create the widget
		public VideoWidget (bool active) : base ()
		{
			this.active = active;
			
			aspect.ShadowType = ShadowType.None;
			aspect.Add (drawable);
			this.Add (aspect);
			
			this.Realized += box_realized;
			drawable.Realized += drawable_realized;
			drawable.ExposeEvent += drawable_exposed;
		}
		
		
		// make the media engine use this window
		void drawable_realized (object o, EventArgs args)
		{
			drawable.ModifyBg (StateType.Normal, new Gdk.Color (0,0,0));
			
			drawable_xid = gdk_x11_drawable_get_xid(drawable.GdkWindow.Handle);
			if (active)
			{
				Global.Core.Fuse.MediaControls.MediaEngine.SetWindow (drawable_xid);
			}
		}
		
		
		// make everything black
		void box_realized (object o, EventArgs args)
		{
			this.ModifyBg (StateType.Normal, new Gdk.Color (0,0,0));
		}
		
		
		// make everything black
		void drawable_exposed (object o, ExposeEventArgs args)
		{
			if (Global.Core.Fuse.MediaControls.MediaEngine.HasVideo) return;
			
			int width, height;
			this.GdkWindow.GetSize (out width, out height);
			
			float ratio = (float)width / (float)height;
			if (aspect.Ratio != ratio)
				aspect.Ratio = ratio;
			
			if (Global.Core.Fuse.MediaControls.MediaEngine.CurrentStatus == MediaStatus.Playing ||
			    Global.Core.Fuse.MediaControls.MediaEngine.CurrentStatus == MediaStatus.Paused)
				return;
			
			
			int x = 0; int y = 0;
			if (width > logo.Width)
				x = (width - logo.Width) / 2;
			if (height > logo.Height)
				y = (height - logo.Height) / 2;
			
			drawable.GdkWindow.DrawPixbuf (drawable.Style.BlackGC, logo, 0,0,x,y, -1, -1, Gdk.RgbDither.None, 0,0);
		}
		
		
		
		/// <summary>
		/// Changes the aspect ratio of the widget.
		/// </summary>
		public void ChangeAspect (float ratio)
		{
			aspect.Ratio = ratio;
		}
		
		
		
		
		/// <summary>
		/// The window ID of the widget to draw on.
		/// </summary>
		public ulong Drawable {
			get { return drawable_xid; }
		}
		
		
		[DllImport("gdk-x11-2.0")]
		static extern ulong gdk_x11_drawable_get_xid(IntPtr window);
		
		
	}
}
