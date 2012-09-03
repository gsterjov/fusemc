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

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// Information loaded from the MusicBrainz.org database.
	/// </summary>
	public class MusicBrainzRelease
	{
		
		private string musicbrainz_releases = "http://musicbrainz.org/ws/1/release/?type=xml&discid={0}";
	    private string musicbrainz_tracks = "http://musicbrainz.org/ws/1/release/{0}?type=xml&inc=tracks+artist";
		
		
		private int track_count;
		private string id;
		private string album;
		private string artist;
		private string[] titles;
		
		
		public MusicBrainzRelease (string toc, int track_count)
		{
			this.track_count = track_count;
			titles = new string[track_count];
			
			
			XmlTextReader reader = new XmlTextReader (String.Format (musicbrainz_releases, toc));
			
            while (reader.Read ())
            {
				if (reader.LocalName == "release" && reader["ext:score"] == "100")
					this.id = reader["id"];
            }
			
			reader.Close ();
			reader = null;
			
			if (this.id == null)
			{
				this.track_count = 0;
				this.album = null;
				this.artist = null;
				this.titles = null;
			}
			else
				loadRelease ();
		}
		
		
		
		public int TrackCount { get{ return track_count; } }
		public string ReleaseID { get{ return id; } }
		public string Album { get{ return album; } }
		public string Artist { get{ return artist; } }
		public string[] Titles { get{ return titles; } }
		
		
		
		// load the release information
		private void loadRelease ()
		{
			XmlTextReader reader = new XmlTextReader (String.Format (musicbrainz_tracks, id));
			int i=0;
			bool isTrack = false;
			
			while (reader.Read ())
			{
				if (reader.NodeType != XmlNodeType.Element)
                   continue;
				
				switch (reader.LocalName)
				{
					case "title":
						if (isTrack)
						{
							titles[i] = reader.ReadString ();
						    i++;
						}
						else
							album = reader.ReadString ();
						break;
						
					case "name":
						artist = reader.ReadString ();
						break;
						
					case "track-list":
						isTrack = true;
						break;
				}
				
				
				if (i == titles.Length)
					break;
			}
			
			reader.Close ();
		    reader = null;
		}
		
		
	}
}
