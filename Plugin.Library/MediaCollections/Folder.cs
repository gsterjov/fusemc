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

namespace Fuse.Plugin.Library
{
		
	/// <summary>
	/// Holds information for the folders added into the library.
	/// </summary>
	public class Folder : MediaCollection <FolderMedia>
	{
		
		private	string path;
		private	bool visible;
		private	FolderMonitor monitor;
		
		
		public Folder (string path)
		{
			this.path = path;
			if (path != Utils.RootNode)
				monitor = new FolderMonitor (this);
		}
		
		
		/// <summary>
		/// The absolute path to the folder.
		/// </summary>
		public string Path
		{
			get{ return path; }
		}
		
		
		/// <summary>
		/// Is the folder currently visible.
		/// </summary>
		public bool Visible
		{
			get{ return visible; }
			set{ visible = value; }
		}
		
		
		/// <summary>
		/// The folder monitoring class.
		/// </summary>
		public FolderMonitor Monitor
		{
			get{ return monitor; }
		}
		
	}
}
