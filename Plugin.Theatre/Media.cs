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
	/// Stores information about specific media files.
	/// </summary>
	public class Media
	{
		string path;
		
		public Media (string path)
		{
			this.path = path;
		}
		
		
		/// <summary>
		/// The path to the media file.
		/// </summary>
		public string Path
		{
			get{ return path; }
		}
		
		
	}
}
