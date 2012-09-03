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
	/// Holds information for the media added into the library.
	/// </summary>
	public abstract class MediaCollection <T>
	{
		public delegate bool ForEachDeleteFunc (T node);
		
		protected List <T> list = new List <T> ();
		
		
		/// <summary>
		/// Loop through the media list deleting when the function returns true.
		/// </summary>
		public void ForEachDelete (ForEachDeleteFunc func)
		{
			for (int i=0; i<list.Count; i++)
			{
				T node = list[i];
				if (func(node) == true)
				{
					list.RemoveAt (i);
					i--;
				}
			}
		}
		
		
		/// <summary>
		/// All the media in the collection.
		/// </summary>
		public List <T> MediaList
		{
			get{ return list; }
		}
		
	}
}
