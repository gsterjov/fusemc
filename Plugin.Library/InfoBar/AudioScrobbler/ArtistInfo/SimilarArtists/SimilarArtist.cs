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

namespace Fuse.Plugin.Library.Info.AudioScrobbler.ArtistInfo
{
	
	/// <summary>
	/// Info about a similar artist.
	/// </summary>
	public class SimilarArtist
	{
		private string name, match, url, image;
		
		public SimilarArtist (XmlNodeList list)
		{
			foreach (XmlNode node in list)
			{
				switch (node.LocalName)
				{
					case "name":
						this.name = node.InnerText;
						break;
						
					case "url":
						this.url = node.InnerText;
						break;
						
					case "image_small":
						this.image = node.InnerText;
						break;
						
					case "match":
						int idx = node.InnerText.IndexOf (".");
						if (idx > -1)
							this.match = node.InnerText.Substring (0, idx);
						else
							this.match = node.InnerText;
						
						break;
				}
			}
			
		}
		
		public string Name { get{ return name; } }
		public string Match { get{ return match; } }
		public string Url { get{ return url; } }
		public string Image { get{ return image; } }
	}
}