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

namespace Fuse.Plugin.Library.Info
{
	
	
	public enum QueryField { Artist, Title, Album, Username };
	
	
	//paired value keys
	public struct Key
	{
		public QueryField Field;
		public string Value;
		
		public Key (string val, QueryField field)
		{
			Value = val;
			Field = field;
		}
		
		
		public static Key Artist (string val)
		{ return new Key (val, QueryField.Artist); }
		
		public static Key Title (string val)
		{ return new Key (val, QueryField.Title); }
		
		public static Key Album (string val)
		{ return new Key (val, QueryField.Album); }
		
		public static Key Username (string val)
		{ return new Key (val, QueryField.Username); }
	}
	
	
	
	/// <summary>
	/// The button used in the info bar.
	/// </summary>
	public class QueryInfo
	{
		
		private string artist, title, album, username;
		
		
		
		public QueryInfo (params Key[] keys)
		{
			foreach (Key key in keys)
			{
				switch (key.Field)
				{
					case QueryField.Artist:
						artist = key.Value; break;
						
					case QueryField.Title:
						title = key.Value; break;
						
					case QueryField.Album:
						album = key.Value; break;
						
					case QueryField.Username:
						username = key.Value; break;
				}
			}
		}
		
		
		
		public QueryInfo (Media media)
		{
			this.artist = media.Artist;
			this.title = media.Title;
			this.album = media.Album;
		}
		
		
		public string Artist { get{ return artist; } }
		public string Title { get{ return title; } }
		public string Album { get{ return album; } }
		public string Username { get{ return username; } }
		
		
		
		public bool Equals (QueryInfo query, params QueryField[] fields)
		{
			if (query == null)
				return false;
			
			foreach (QueryField field in fields)
			{
				switch (field)
				{
					case QueryField.Artist:
						if (this.Artist != query.Artist)
							return false;
						break;
						
					case QueryField.Title:
						if (this.Title != query.Title)
							return false;
						break;
						
					case QueryField.Album:
						if (this.Album != query.Album)
							return false;
						break;
						
					case QueryField.Username:
						if (this.Username != query.Username)
							return false;
						break;
				}
			}
			
			return true;
		}
		
		
	}
}
