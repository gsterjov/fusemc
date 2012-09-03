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
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// Manages the folder database for this plugin.
	/// </summary>
	public class FolderDataManager : Database
	{
		
		
		/// <summary>
		/// Adds a folder to the database.
		/// </summary>
		public void AddFolder (Folder folder)
		{
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("INSERT INTO folders VALUES (NULL,{0},{1},{2})",
			                 parse(folder.Path), parse(folder.Visible), parse(folder.Monitor.Monitoring));
			
			ExecuteQuery (sb.ToString ());
		}
		
		
		
		/// <summary>
		/// Updates the folder in the database.
		/// </summary>
		public void UpdateFolder (Folder folder)
		{
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("UPDATE folders SET visible={0},monitor={1} WHERE path={2}",
			                 parse(folder.Visible), parse(folder.Monitor.Monitoring), parse(folder.Path));
			
			ExecuteQuery (sb.ToString ());
		}
		
		
		
		/// <summary>
		/// Deletes the folder and its media from the database.
		/// </summary>
		public void DeleteFolder (Folder folder)
		{
			int folder_id = GetFolderID (folder);
			
			if (folder_id > -1)
			{
				StringBuilder sb = new StringBuilder ();
				sb.AppendFormat ("DELETE FROM media WHERE folder_id={0}", parse(folder_id));
				ExecuteQuery (sb.ToString ());
				
				sb = new StringBuilder ();
				sb.AppendFormat ("DELETE FROM folders WHERE path={0}", parse(folder.Path));
				ExecuteQuery (sb.ToString ());
			}
		}
		
		
		
		/// <summary>
		/// Retrieves all the folders from the database.
		/// </summary>
		public List <Folder> GetFolders ()
		{
			List <Folder> list = new List <Folder> ();
			string sql = "SELECT path,visible,monitor FROM folders";
			
			ExecuteQuery (sql, delegate (IDataReader reader) {
				while (reader.Read ())
				{
					string path = reader.GetString (0);
					bool visible = reader.GetBoolean (1);
					bool monitor = reader.GetBoolean (2);
					
					Folder folder = new Folder (path);
					folder.Visible = visible;
					folder.Monitor.Ready = monitor;
					
					list.Add (folder);
				}
			});
			
			
			return list;
		}
		
		
		/// <summary>
		/// Retrieves all the media from the database.
		/// </summary>
		public void LoadMedia (Folder folder)
		{
			int folder_id = GetFolderID (folder);
			
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("SELECT path,artist,title,album,comment,year,track_number,track_count,duration FROM media WHERE folder_id={0}", parse(folder_id));
			
			
			ExecuteQuery (sb.ToString (), delegate (IDataReader reader) {
				while (reader.Read ())
				{
					string path = reader.GetString (0);
					string artist = reader.GetString (1);
					string title = reader.GetString (2);
					string album = reader.GetString (3);
					string comment = reader.GetString (4);
					int year = reader.GetInt32 (5);
					int track_number = reader.GetInt32 (6);
					int track_count = reader.GetInt32 (7);
					TimeSpan duration = TimeSpan.FromSeconds (reader.GetDouble (8));
					
					FolderMedia media = new FolderMedia (path, folder);
					media.Artist = artist;
					media.Title = title;
					media.Album = album;
					media.Comment = comment;
					media.Year = year;
					media.TrackNumber = track_number;
					media.TrackCount = track_count;
					media.Duration = duration;
					
					folder.MediaList.Add (media);
				}
			});
		}
		
		
		
	}
}
