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
using System.Collections.Generic;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// Holds information for the playlists added into the library.
	/// </summary>
	public class Playlist : MediaCollection <PlaylistMedia>
	{
		
		private	string name;
		
		public Playlist (string name)
		{
			this.name = name;
		}
		
		
		/// <summary>
		/// The name of the playlist.
		/// </summary>
		public string Name
		{
			get{ return name; }
			set{ name = value; }
		}
		
	}
}
