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
using Gtk;


namespace Fuse.Plugin.Library.Info.AudioScrobbler.Profile
{
	
	/// <summary>
	/// Profile info loaded from the AudioScrobbler.net database.
	/// </summary>
	public class ProfileInfo : Content
	{
		
		private QueryInfo last_query;
		
		//profile box
		private Image image = new Image ();
		private Label username = new Label ();
		private Label real_name = new Label ();
		private Label play_count = new Label ();
		
		
		
		public ProfileInfo ()
		{
			username.Xalign = 0;
			real_name.Xalign = 0;
			play_count.Xalign = 0;
			
			
			VBox info = new VBox (false, 0);
			info.PackStart (username, false, false, 0);
			info.PackStart (real_name, false, false, 5);
			info.PackStart (play_count, false, false, 0);
			
			Alignment align = new Alignment (0, 0.5f, 0, 0);
			align.Add (info);
			
			
			HBox box = new HBox (false, 0);
			box.PackStart (new BorderWidget (image), false, false, 0);
			box.PackStart (align, true, true, 20);
			
			this.DisplayWidget = box;
		}
		
		
		
		
		/// <summary>
		/// The query to execute.
		/// </summary>
		public override string GetQuery (QueryInfo query)
		{
			if (query.Equals (last_query, QueryField.Artist))
				return null;
			
			string url = "http://ws.audioscrobbler.com/1.0/user/{0}/profile.xml";
			return ParseQuery (url, query.Username);
		}
		
		
		
		/// <summary>
		/// Load the profile details.
		/// </summary>
		public override void Load (XmlDocument doc, QueryInfo query)
		{
			last_query = query;
			
			XmlNodeList list = doc.GetElementsByTagName ("profile");
			if (list.Count == 0)
				return;
			
			
			string user = list[0].Attributes["username"].Value;
			username.Markup = "<big><b>" + Utils.ParseMarkup (user) + "</b></big>";
			
			
			foreach (XmlNode node in list[0].ChildNodes)
			{
				switch (node.LocalName)
				{
					case "realname":
						real_name.Markup = Utils.ParseMarkup (node.InnerText);
						break;
						
					case "playcount":
						play_count.Markup = "<small>Play Count: <b>" + node.InnerText + "</b></small>";
						break;
						
					case "avatar":
						image.Pixbuf = this.LoadImage (node.InnerText);
						break;
				}
			}
			
		}
		
		
		
		public override void ShowPage () {}
		
		
	}
	
	
	
}
