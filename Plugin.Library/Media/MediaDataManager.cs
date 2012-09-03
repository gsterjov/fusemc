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
using System.Text;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// Manages the media database for this plugin.
	/// </summary>
	public class MediaDataManager : Database
	{
		
		
		/// <summary>
		/// Adds the media to the database.
		/// </summary>
		public void AddMedia (FolderMedia media)
		{
			int folder_id = GetFolderID (media.Folder);
			
			if (folder_id > -1)
				addMedia (media, folder_id, -1);
		}
		public void AddMedia (PlaylistMedia media)
		{
			int playlist_id = GetPlaylistID (media.Playlist);
			
			if (playlist_id > -1)
				addMedia (media, -1, playlist_id);
		}
		
		
		
		/// <summary>
		/// Updates the media in the database.
		/// </summary>
		public void UpdateMedia (FileMedia media)
		{
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("UPDATE media SET artist={0},title={1},album={2},comment={3},year={4},track_number={5},track_count={6} WHERE path={7}",
			                 parse(media.Artist), parse(media.Title), parse(media.Album), parse(media.Comment), parse(media.Year),
			                 parse(media.TrackNumber), parse(media.TrackCount), parse(media.Path));
			
			ExecuteQuery (sb.ToString ());
		}
		
        
        
		/// <summary>
		/// Deletes the media from the database.
		/// </summary>
		public void DeleteMedia (FileMedia media)
		{
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("DELETE FROM media WHERE path={0}", parse(media.Path));
			ExecuteQuery (sb.ToString ());
		}
        
        
		
		
		// adds the file media to the database
		private void addMedia (FileMedia media, int folder_id, int playlist_id)
		{
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("INSERT INTO media VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10})",
			                 parse(folder_id), parse(playlist_id), parse(media.Path), parse(media.Artist),
			                 parse(media.Title), parse(media.Album), parse(media.Comment), parse(media.Year),
			                 parse(media.TrackNumber), parse(media.TrackCount), parse(media.Duration.TotalSeconds));
			
			ExecuteQuery (sb.ToString ());
		}
		
		
	}
}
