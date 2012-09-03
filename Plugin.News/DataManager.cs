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
using RssReader;

namespace Fuse.Plugin.News
{
	
	/// <summary>
	/// Manages various aspects of the database for this plugin.
	/// </summary>
	public class DataManager
	{
		IDbConnection dbcon;
		
		
		// initiate the database connection using sqlite
		public DataManager (MainPage parent)
		{
			string path = System.IO.Path.Combine (parent.AppDir, "News.db");
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
			
			sql += "CREATE TABLE feed (id INTEGER PRIMARY KEY, name STRING, url STRING, etag STRING, last_modified STRING, autorefresh BOOL);";
			
			sql += "CREATE TABLE item (feed_id REFERENCES feed(id), " +
				                      "title STRING, " +
					                  "description STRING, " +
					                  "url STRING, " +
					                  "guid STRING, " +
					                  "read BOOL, " +
					                  "pub_date STRING);";
			
			executeSql (sql);
			
			
			sql = "INSERT INTO db VALUES ('0.2')";
			executeSql (sql);
		}
		
		
		
		
		/// <summary>
		/// Adds a news feed to the sql database.
		/// </summary>
		public void AddFeed (Feed feed)
		{
			string sql = "INSERT INTO feed VALUES (NULL, '" + 
				parseSql (feed.Name) + "','" +
				parseSql (feed.Url) + "','" +
				parseSql (feed.ETag) + "','" +
				parseSql (feed.LastModified) + "','" +
				feed.AutoRefresh.ToString () + "')";
			
			executeSql (sql);
			
			
			string feed_id = getFeedID (feed);
			if (feed_id == "-1") return;
			
			
			foreach (Item item in feed.Items)
				addItem (item, feed_id);
		}
		
		
		// add an item into the database
		void addItem (Item item, string feed_id)
		{
			string sql = "INSERT INTO item VALUES ('" +
				feed_id + "','" +
				parseSql (item.Title) + "','" +
				parseSql (item.Description) + "','" +
				parseSql (item.Url) + "','" +
				parseSql (item.GUID) + "','" +
				item.Read.ToString () + "','" +
				parseSql (item.PubDate) + "')";
			
			executeSql (sql);
		}
		
		
		
		/// <summary>
		/// Adds a news item into the sql database.
		/// </summary>
		public void AddItem (Feed feed, Item item)
		{
			string feed_id = getFeedID (feed);
			addItem (item, feed_id);
		}
		
		
		
		/// <summary>
		/// Deletes the specified news feed and its items from the database.
		/// </summary>
		public void DeleteFeed (Feed feed)
		{
			string feed_id = getFeedID (feed);
			
			string sql = "DELETE FROM feed WHERE url='" + feed.Url + "'";
			executeSql (sql);
			
			sql = "DELETE FROM item WHERE feed_id='" + feed_id + "'";
			executeSql (sql);
		}
		
		
		
		
		/// <summary>
		/// Updates the specified news feed in the database.
		/// </summary>
		public void UpdateFeed (Feed feed)
		{
			string sql = "UPDATE feed SET etag='" + parseSql (feed.ETag) + 
				         "', last_modified='" + parseSql (feed.LastModified) +
					     "', autorefresh='" + feed.AutoRefresh.ToString () +
				         "' WHERE url='" + parseSql (feed.Url) + "'";
			
			executeSql (sql);
		}
		
		
		
		
		/// <summary>
		/// Updates the specified news item in the database.
		/// </summary>
		public void UpdateItem (Item item)
		{
			string sql = "UPDATE item SET read='" + item.Read.ToString () + "' WHERE " +
				         "title='" + parseSql (item.Title) + "' AND " +
					     "url='" + parseSql (item.Url) + "' AND " +
					     "guid='" + parseSql (item.GUID) + "' AND " +
					     "pub_date='" + parseSql (item.PubDate) + "'";
			executeSql (sql);
		}
		
		
		
		
		/// <summary>
		/// Retrieves all the news feeds from the database.
		/// </summary>
		public List <Feed> GetFeeds ()
		{
			List <Feed> list = new List <Feed> ();
			
			dbcon.Open ();
			
			string sql = "SELECT name, url, etag, last_modified, autorefresh FROM feed";
			
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader ();
			
			while (reader.Read ())
			{
				string name = reader.GetString (0);
				string url = reader.GetString (1);
				string etag = reader.GetString (2);
				string last_modified = reader.GetString (3);
				bool autorefresh = reader.GetBoolean (4);
				
				list.Add (new Feed (name, url, etag, last_modified, autorefresh));
			}
			
			
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			
			dbcon.Close ();
			
			
			foreach (Feed feed in list)
				loadItems (feed);
			
			return list;
		}
		
		
		
		// load the feed's items
		void loadItems (Feed feed)
		{
			string feed_id = getFeedID (feed);
			string sql = "SELECT title, description, url, guid, read, pub_date FROM item WHERE feed_id='" + feed_id + "'";
			
			
			dbcon.Open ();
			
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader ();
			
			while (reader.Read ())
			{
				string title = reader.GetString (0);
				string description = reader.GetString (1);
				string url = reader.GetString (2);
				string guid = reader.GetString (3);
				bool read = reader.GetBoolean (4);
				string pub_date = reader.GetString (5);
				
				feed.Items.Add (new Item (title, description, url, guid, read, pub_date));
			}
			
			
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			
			dbcon.Close ();
			
			feed.UpdateStatus ();
		}
		
		
		
		
		// gets the database id for the specified playlist
		string getFeedID (Feed feed)
		{
			long feed_id = -1;
			string sql = "SELECT id FROM feed WHERE url='" + parseSql (feed.Url) + "'";
			
			dbcon.Open ();
			
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader ();
			
			if (reader.Read ())
				feed_id = reader.GetInt64 (0);
			
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			
			dbcon.Close ();
			
			return feed_id.ToString ();
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
