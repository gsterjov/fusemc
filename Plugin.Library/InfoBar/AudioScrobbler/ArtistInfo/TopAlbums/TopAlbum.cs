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
	/// Info about a top album.
	/// </summary>
	public class TopAlbum
	{
		private string name, reach, url, image;
		
		public TopAlbum (XmlNodeList list)
		{
			foreach (XmlNode node in list)
			{
				switch (node.LocalName)
				{
					case "name":
						this.name = node.InnerText;
						break;
						
					case "reach":
						this.reach = node.InnerText;
						break;
						
					case "url":
						this.url = node.InnerText;
						break;
						
					case "image":
						foreach (XmlNode image_node in node.ChildNodes)
							if (image_node.LocalName == "large")
								image = image_node.InnerText;
						break;
				}
			}
			
		}
		
		public string Name { get{ return name; } }
		public string Reach { get{ return reach; } }
		public string Url { get{ return url; } }
		public string Image { get{ return image; } }
	}
}