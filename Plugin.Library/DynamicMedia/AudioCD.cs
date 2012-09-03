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

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// Dynamic media information from an audio cd.
	/// </summary>
	public class AudioCD : DynamicMedia
	{
		
		private bool loaded;
		private string disc_id;
		private string musicbrainz_id;
        
		
		/// <summary>
		/// Load the content from the dynamic media.
		/// </summary>
		public override void LoadMedia ()
		{
            Global.Core.Fuse.MediaControls.ProbeTag += found_tag;
            if (!Global.Core.Fuse.MediaControls.ProbeAudioCD ())
                clear_list ();
		}
		
		
		
		/// <summary>
		/// The stock icon ID to use.
		/// </summary>
		public override string StockIcon
		{
			get{ return Gtk.Stock.Cdrom; }
		}
		
		
		/// <summary>
		/// The name of the dynamic media.
		/// </summary>
		public override string Title
		{
			get{ return "Audio CD"; }
		}
		
		
		
		/// <summary>
		/// The ID of the audio track.
		/// </summary>
		public string DiscID
		{
			get{ return disc_id; }
			set{ disc_id = value; }
		}
		
		
		/// <summary>
		/// The MusicBrainz ID of the audio track.
		/// </summary>
		public string MusicBrainzID
		{
			get{ return musicbrainz_id; }
			set{ musicbrainz_id = value; }
		}
		
		
		
        // clears the audio cd list
        private void clear_list ()
        {
            disc_id = "";
            musicbrainz_id = "";
            
            foreach (Media media in this.list)
				Global.Core.Library.MediaTree.MediaStore.RemoveMedia (media);
			
			this.list.Clear ();
			loaded = false;
        }
        
        
		
		// a media tag has been found
		private void found_tag (TagEventArgs args)
		{
            Global.Core.Fuse.MediaControls.ProbeTag -= found_tag;
            
			if (disc_id != args.Tag.DiscID || musicbrainz_id != args.Tag.MusicBrainzID)
                clear_list ();
            
			
            //load audio cd details
			if (!loaded)
            {
                loaded = true;
    			disc_id = args.Tag.DiscID;
    			musicbrainz_id = args.Tag.MusicBrainzID;
    			
				MusicBrainzRelease release = new MusicBrainzRelease (musicbrainz_id, args.Tag.TrackCount);
				if (release.ReleaseID == null)
					release = null;
				
    			for (int i=1; i <= args.Tag.TrackCount; i++)
    			{
    				AudioCDMedia cd_media = new AudioCDMedia (i);
					
					if (release != null)
					{
    					cd_media.Artist = release.Artist;
    					cd_media.Album = release.Album;
						cd_media.Title = release.Titles[i-1];
					}
					
    				Global.Core.Library.MediaTree.MediaStore.AddMedia (cd_media);
    				this.list.Add (cd_media);
    			}
            }
			
		}
		
		
	}
}
