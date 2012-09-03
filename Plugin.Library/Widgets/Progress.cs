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

namespace Fuse.Plugin.Library
{
		
	/// <summary>
	/// The progress bar widget that displays the media scan.
	/// </summary>
	public class Progress : Statusbar
	{
		Button stop = new Button (Stock.Stop);
		ProgressBar bar = new ProgressBar ();
		double maximum;
		double count;
		bool canceled;
		Box parent;
		
		
		/// <summary>
		/// Cancel the media scan.
		/// </summary>
		public event EventHandler Cancel;
		
		
		public Progress(Box parent) {
			this.parent = parent;
			this.HasResizeGrip = false;
			this.PackStart (stop, false, false, 0);
			this.PackStart (bar, false, false, 0);
			
			stop.Clicked += cancel_clicked;
		}
		
		
		
		/// <summary>
		/// The total amount of media to scan.
		/// </summary>
		public double Maximum {
			get { return maximum; }
			set { maximum = value; }
		}
		
		/// <summary>
		/// The current position of the media scan.
		/// </summary>
		public double Count {
			get { return count; }
			set { count = value; }
		}
		
		/// <summary>
		/// Whether or not it was canceled.
		/// </summary>
		public bool Canceled {
			get{ return canceled; }
		}
		
		
		
		
		/// <summary>
		/// Setup the progress bar.
		/// </summary>
		public void Start (double maximum) {
			this.maximum = maximum;
			Application.Invoke (delegate{
				parent.PackEnd (this, false, false, 0);
				parent.ShowAll ();
			});
		}
		
		/// <summary>
		/// Remove the progress bar.
		/// </summary>
		public void End () {
			maximum = 0;
			count = 0;
			Application.Invoke (delegate{
				parent.Remove (this);
				this.Destroy ();
			});
		}
		
		
		
		/// <summary>
		/// Push the new status bar text onto the stack.
		/// </summary>
		public void Push (string text) {
			Application.Invoke (delegate {
				this.Push (0, text);
			});
		}
		
		/// <summary>
		/// Count up 1 in the progress bar.
		/// </summary>
		public void Step () {
			count++;
			
			double percent = count / maximum;
			if (percent >= 0 && percent < 1)
				Application.Invoke (delegate{ bar.Fraction = percent; });
		}
		
		
		
		// when the user clicks the cancel button
		void cancel_clicked (object sender, EventArgs args) {
			canceled = true;
			if (Cancel != null) Cancel (sender, args);
		}
		
		
		
	}
}
