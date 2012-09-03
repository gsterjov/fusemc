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
	/// A widget that controls the position of the playing media file.
	/// </summary>
	public class SeekBar : HScale
	{
		
		// status variables
		bool canSeek = true;
		bool isIdle = true;
		
		
		/// <summary>
		/// Raised when the user changes the seek bar position.
		/// </summary>
		public event EventHandler PositionChanged;
		

		public SeekBar () : base (0.0,0.0,0.0) {
			this.DrawValue = false;
			this.Sensitive = false;
		}
		
		
		// overrides which allow us to know when the user is moving the slider
		protected override bool OnButtonPressEvent(Gdk.EventButton evnt)
		{
			canSeek = false;
			return base.OnButtonPressEvent (evnt);
		}
		protected override bool OnButtonReleaseEvent(Gdk.EventButton evnt)
		{
			canSeek = true;
			raisePositionChanged ();
			return base.OnButtonPressEvent (evnt);
		}
		
		
		// raise the PositionChanged event
		void raisePositionChanged ()
		{
			EventHandler handler = PositionChanged;
			if (handler != null)
				PositionChanged (this, new EventArgs ());
		}
		
		
		/// <summary>
		/// Gets or sets the seek bar to idle making it unusable.
		/// </summary>
		public bool Idle
		{
			get{ return isIdle; }
			set
			{
				isIdle = value;
				this.Sensitive = !value;
			}
		}
		
		
		/// <summary>
		/// Gets or sets the current position of the seek bar.
		/// </summary>
		public double Position
		{
			get{ return base.Value; }
			set
			{
				if(canSeek && !isIdle) base.Value = value;
			}
		}
		
		
	}
}
