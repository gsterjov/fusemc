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
	/// Manages the playlist database for this plugin.
	/// </summary>
	public class PlaylistDataManager : Database
	{
		
		
		/// <summary>
		/// Adds a playlist to the database.
		/// </summary>
		public void AddPlaylist (Playlist playlist)
		{
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("INSERT INTO playlists VALUES (NULL,{0})", parse(playlist.Name));
			
			ExecuteQuery (sb.ToString ());
		}
		
		
		
		/// <summary>
		/// Updates the playlist in the database.
		/// </summary>
		public void UpdatePlaylist (Playlist playlist, string old_name)
		{
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("UPDATE playlist SET name={0} WHERE name={1}", parse(playlist.Name), parse(old_name));
			
			ExecuteQuery (sb.ToString ());
		}
		
		
		
		/// <summary>
		/// Deletes the playlist and its media from the database.
		/// </summary>
		public void DeletePlaylist (Playlist playlist)
		{
			int playlist_id = GetPlaylistID (playlist);
			
			if (playlist_id > -1)
			{
				StringBuilder sb = new StringBuilder ();
				sb.AppendFormat ("DELETE FROM media WHERE playlist_id={0}", parse(playlist_id));
				ExecuteQuery (sb.ToString ());
				
				sb = new StringBuilder ();
				sb.AppendFormat ("DELETE FROM playlists WHERE name={0}", parse(playlist.Name));
				ExecuteQuery (sb.ToString ());
			}
		}
		
		
		
		/// <summary>
		/// Retrieves all the playlists from the database.
		/// </summary>
		public List <Playlist> GetPlaylists ()
		{
			List <Playlist> list = new List <Playlist> ();
			string sql = "SELECT name FROM playlists";
			
			ExecuteQuery (sql, delegate (IDataReader reader) {
				while (reader.Read ())
				{
					string name = reader.GetString (0);
					list.Add (new Playlist (name));
				}
			});
			
			
			return list;
		}
		
		
		/// <summary>
		/// Retrieves all the media from the database.
		/// </summary>
		public void LoadMedia (Playlist playlist)
		{
			int playlist_id = GetPlaylistID (playlist);
			
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("SELECT path,artist,title,album,comment,year,track_number,track_count,duration FROM media WHERE playlist_id={0}", parse(playlist_id));
			
			
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
					
					PlaylistMedia media = new PlaylistMedia (path, playlist);
					media.Artist = artist;
					media.Title = title;
					media.Album = album;
					media.Comment = comment;
					media.Year = year;
					media.TrackNumber = track_number;
					media.TrackCount = track_count;
					media.Duration = duration;
					
					playlist.MediaList.Add (media);
				}
			});
		}
		
		
		
	}
}
