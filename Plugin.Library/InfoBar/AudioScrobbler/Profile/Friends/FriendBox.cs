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
using System.Net;
using Gtk;

namespace Fuse.Plugin.Library.Info.AudioScrobbler.Profile
{
	
	/// <summary>
	/// A clickable box.
	/// </summary>
	public class FriendBox : ImageBox
	{
		
		
		private Friend friend;
		
		
		
		public FriendBox (Friend friend, WebService web_service) : base ()
		{
			this.friend = friend;
			
			Gdk.Pixbuf pic = web_service.LoadImage (friend.Image);
			this.Image.Pixbuf = pic.ScaleSimple (45, 45, Gdk.InterpType.Bilinear);
			
			Label username = new Label ();
			username.Markup = "<b>" + Utils.ParseMarkup (friend.Username) + "</b>";
			
			this.InformationBox.Add (username);
		}
		
		
		
		/// <summary>
		/// The friend details.
		/// </summary>
		public Friend Friend
		{
			get{ return friend; }
		}
		
		
		
	}
}
