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
using System.Data;
using Mono.Data.SqliteClient;

namespace Fuse.Plugin.Theatre
{
	
	/// <summary>
	/// Manages various aspects of the database for this plugin.
	/// </summary>
	public class DataManager
	{
		IDbConnection dbcon;
		
		
		// initiate the database connection using sqlite
		public DataManager ()
		{
			string path = System.IO.Path.Combine (Global.Core.AppDir, "Theatre.db");
			string connection = "URI=file:" + path + ",version=3";
			
			bool db_exists = System.IO.File.Exists (path);
			
			dbcon = (IDbConnection) new SqliteConnection (connection);
			
			if (!db_exists)
				createTables ();
		}
		
		
		
		// create the database tables
		void createTables ()
		{
			dbcon.Open ();
			
			string sql = "CREATE TABLE db (version INTEGER);";
			
			sql += "CREATE TABLE media (path TEXT);";
			
			executeSql (sql);
			
			
			sql = "INSERT INTO db VALUES ('0.2')";
			executeSql (sql);
		}
		
		
		
		
		/// <summary>
		/// Adds a media file to the sql database.
		/// </summary>
		public void AddMedia (Media media)
		{
			string sql = "INSERT INTO media VALUES ('" + parseSql (media.Path) + "')";
			executeSql (sql);
		}
		
		
		
		/// <summary>
		/// Deletes the specified media file from the database.
		/// </summary>
		public void DeleteMedia (Media media)
		{
			string sql = "DELETE FROM media WHERE path='" + parseSql (media.Path) + "'";
			executeSql (sql);
		}
		
		
		
		/// <summary>
		/// Retrieves all the media files from the database.
		/// </summary>
		public List <Media> GetMedia ()
		{
			List <Media> list = new List <Media> ();
			
			dbcon.Open ();
			
			string sql = "SELECT path FROM media";
			
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader ();
			
			while (reader.Read ())
			{
				string path = reader.GetString (0);
				list.Add (new Media (path));
			}
			
			
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			
			dbcon.Close ();
			
			return list;
		}
		
		
		
		// makes the sql command compatible
		string parseSql (string text)
		{
			if (text == null)
				return "";
			
			text = text.Replace ("'", "''");
			return text;
		}
		
		
		
		// executes the sql command
		void executeSql (string sql)
		{
			dbcon.Open ();
			
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = sql;
			dbcmd.ExecuteNonQuery ();
			
			dbcmd.Dispose ();
			dbcmd = null;
			
			dbcon.Close ();
		}
		
		
	}
}
