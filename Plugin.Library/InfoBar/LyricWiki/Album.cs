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
using System.Xml;
using Gtk;

namespace Fuse.Plugin.Library.Info.LyricWiki
{
	
	/// <summary>
	/// Info about a similar artist.
	/// </summary>
	public class Album
	{
		private string artist, name, year;
		private string[] songs;
		
		
		public Album (string artist, string name, string year, XmlNodeList items)
		{
			this.name = string.IsNullOrEmpty(name) ? "Other Songs" : name;
			this.year = string.IsNullOrEmpty(year) ? "No Year": year;
			
			this.artist = artist;
			this.songs = new string[items.Count];
			
			for (int i=0; i<items.Count; i++)
				songs[i] = items[i].InnerText;
		}
		
		
		public string Artist { get{ return artist; } }
		public string Name { get{ return name; } }
		public string Year { get{ return year; } }
		
		public string[] Songs { get{ return songs; } }
		
	}
}