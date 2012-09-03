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


using Fuse.Interfaces;

namespace Fuse.MediaEngine.GST
{
	/// <summary>
	/// Media tag of the loaded file.
	/// </summary>
	public class Tag : IEngineTag
	{
		string disc_id;
		string music_brainz_id;
    	int current_track;
		int track_count;
		int duration;
		
		public Tag (GStreamer.Tag tag)
		{
			this.disc_id = tag.DiscID;
			this.music_brainz_id = tag.MusicBrainzID;
			this.current_track = tag.CurrentTrack;
			this.track_count = tag.TrackCount;
			this.duration = tag.Duration;
		}
    	
    	public string DiscID { get{ return disc_id; } }
       	public string MusicBrainzID { get{ return music_brainz_id; } }
       	public int CurrentTrack { get { return current_track; } }
    	public int TrackCount { get{ return track_count; } }
		public int Duration { get{ return duration; } }
	}
	
}