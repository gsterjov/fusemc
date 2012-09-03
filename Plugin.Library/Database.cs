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
using System.Data;
using Mono.Data.SqliteClient;

namespace Fuse.Plugin.Library
{
	
	
	public delegate void DataReaderDelegate (IDataReader reader);
	
	
	
	/// <summary>
	/// The database for this plugin.
	/// </summary>
	public abstract class Database
	{
		private IDbConnection dbcon;
		
		
		public Database ()
		{
			string path = System.IO.Path.Combine (Global.Core.AppDir, "MediaLibrary.db");
			string connection = "URI=file:" + path + ",version=3";
			
			bool db_exists = System.IO.File.Exists (path);
			dbcon = (IDbConnection) new SqliteConnection (connection);
			
			if (!db_exists)
				CreateTables ();
		}
		
		
		private void CreateTables ()
		{
			string sql = "CREATE TABLE folders (id INTEGER PRIMARY KEY, path TEXT, visible BOOLEAN, monitor BOOLEAN);";
			sql += "CREATE TABLE playlists (id INTEGER PRIMARY KEY, name TEXT);";
			sql += "CREATE TABLE media (folder_id REFERENCES folders(id)," +
			                              "playlist_id REFERENCES playlists(id)," +
			                              "path TEXT," +
			                              "artist TEXT," +
			                              "title TEXT," +
			                              "album TEXT," +
			                              "comment TEXT," +
			                              "year INT," +
			                              "track_number INT," +
			                              "track_count INT," +
					                      "duration INT);";
			
			ExecuteQuery (sql);
		}
		
		
		
		protected string parse (double val)
		{
			return "'" + val + "'";
		}
		protected string parse (int val)
		{
			return "'" + val + "'";
		}
		protected string parse (string val)
		{
			return "'" + parseSql (val) + "'";
		}
		protected string parse (bool val)
		{
			return "'" + val.ToString () + "'";
		}
		
		// makes the sql command compatible
		private string parseSql (string text)
		{
			text = text.Replace ("'", "''");
			return text;
		}
		
		
		
		
		protected void ExecuteQuery (string query)
		{
			dbcon.Open ();
			
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = query;
			dbcmd.ExecuteNonQuery ();
			
			dbcmd.Dispose ();
			dbcmd = null;
			
			dbcon.Close ();
		}
		
		
		protected void ExecuteQuery (string query, DataReaderDelegate reader_func)
		{
			dbcon.Open ();
			
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = query;
			IDataReader reader = dbcmd.ExecuteReader ();
			
			reader_func (reader);
			
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			
			dbcon.Close ();
		}
		
		
		
		protected int GetFolderID (Folder folder)
		{
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("SELECT id FROM folders WHERE path={0}", parse(folder.Path));
			
			
			int folder_id = -1;
			ExecuteQuery (sb.ToString (), delegate (IDataReader reader) {
				if (reader.Read ())
					folder_id = reader.GetInt32 (0);
			});
			
			return folder_id;
		}
		
		
		
		protected int GetPlaylistID (Playlist playlist)
		{
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("SELECT id FROM playlists WHERE name={0}", parse(playlist.Name));
			
			
			int playlist_id = -1;
			ExecuteQuery (sb.ToString (), delegate (IDataReader reader) {
				if (reader.Read ())
					playlist_id = reader.GetInt32 (0);
			});
			
			return playlist_id;
		}
		
		
		
	}
}
