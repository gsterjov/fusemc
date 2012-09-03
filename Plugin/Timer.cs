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

namespace Fuse
{
	
	/// <summary>
	/// Raises an event at certain time intervals.
	/// </summary>
	public class Timer
	{
		
		private TimerElement element;
		
		
		
		// the class controlling the counter.
		// this is necessary in order to eliminate
		// the problem of running more than one timer because
		// glib.timeout has no remove function.
		private class TimerElement
		{
			
			public bool running;
			private EmptyHandler func;
			
			public TimerElement (EmptyHandler func)
			{
				this.func = func;
			}
			
			// the actual timer
			public bool timer_count ()
			{
				if (running)
					this.func ();
				
				return running;
			}
			
			
		}
		
		
		/// <summary>
		/// Raised every time the timer interval has elapsed.
		/// </summary>
		public event EmptyHandler Elapsed;
		
		
		/// <summary>
		/// Stops the timer.
		/// </summary>
		public void Stop () {
			
			if (element != null)
			{
				element.running = false;
				element = null;
			}
			
		}
		
		/// <summary>
		/// Starts the timer with the specified interval.
		/// </summary>
		public void Start (int interval) {
			Stop ();
			element = new TimerElement (raise_timer);
			
			element.running = true;
			GLib.Timeout.Add ((uint) interval, element.timer_count);
		}
		
		
		
		/// <summary>
		/// The status of the timer.
		/// </summary>
		public bool Running
		{
			get{ return element != null ? element.running : false; }
		}
		
		
		
		// raises the timer elapsed event
		private void raise_timer ()
		{
			if (Elapsed != null)
				Elapsed ();
		}
		
	}
}
