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
using System.IO;
using System.Net;

namespace Fuse.Plugin.Library
{
		
	/// <summary>
	/// Utility functions used throughout the plugin.
	/// </summary>
	public class Utils : Fuse.Utils
	{
		
		public const string RootNode = "root_node";
        private static string[] extensions = {".mp3", ".ogg", ".wma", ".wav", ".flac", };
		
		
		/// <summary>
		/// Checks if the specified file is supported.
		/// </summary>
		public static bool ValidExt (string path)
		{
			if (!File.Exists (path))
				return false;
			
			foreach (string ext in extensions)
				if (Path.GetExtension(path).ToLower() == ext)
					return true;
			return false;
		}
		
		
		
		/// <summary>
		/// Returns the amount of files there are in a folder. Recursive.
		/// </summary>
		public static double FileCount (string folder)
		{
			double total = Directory.GetFiles (folder).Length;
			
			foreach (string dir in Directory.GetDirectories (folder))
				total += FileCount (dir);
			
			return total;
		}
		
		
		/// <summary>
		/// Gets the folder name from the absolute path.
		/// </summary>
		public static string GetFolderName (string path)
		{
			char separator = Path.DirectorySeparatorChar;
			return path.Substring (path.LastIndexOf (separator)+1);
		}
		
		
		
		/// <summary>
		/// Gets the file name from the absolute path.
		/// </summary>
		public static string GetFileName (string path)
		{
			return Path.GetFileNameWithoutExtension (path);
		}
		
		
		
		/// <summary>
		/// Returns a pixbuf from the specified bytes
		/// </summary>
		public static Gdk.Pixbuf LoadCoverArt (byte[] pic)
		{
			if (pic == null || pic.Length == 0) return null;
			
			try { return new Gdk.Pixbuf (pic, 85, 85); }
			catch { return null; }
		}
		
		
		
	}
}
