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

namespace Fuse.Plugin.Library.Info.AudioScrobbler.Profile
{
	
	/// <summary>
	/// Info about a friend.
	/// </summary>
	public class Friend
	{
		private string username, url, image;
		
		public Friend (XmlNode friend)
		{
			
			username = friend.Attributes["username"].Value;
			
			
			foreach (XmlNode node in friend.ChildNodes)
			{
				switch (node.LocalName)
				{
					case "url":
						this.url = node.InnerText;
						break;
						
					case "image":
						this.image = node.InnerText;
						break;
				}
			}
			
		}
		
		public string Username { get{ return username; } }
		public string Url { get{ return url; } }
		public string Image { get{ return image; } }
	}
}