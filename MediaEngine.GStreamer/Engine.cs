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
using System.Runtime.InteropServices;
using Fuse.Interfaces;

namespace Fuse.MediaEngine.GST
{
	
	/// <summary>
	/// The GStreamer media engine.
	/// </summary>
	public class Engine : IMediaEngine
	{
		GStreamer.Playbin playbin = new GStreamer.Playbin ();
		
		
		// events
		public event ErrorEventHandler Error;
		public event BufferEventHandler Buffer;
		public event EndOfStreamEventHandler EndOfStream;
		public event StateEventHandler StateChanged;
		public event VideoInfoEventHandler FoundVideoInfo;
		public event TagEventHandler FoundTag;
		
		
		
		// media engine description properties
		public string Name { get{ return "GStreamer 0.10 Media Engine"; } }
		public string Description { get{ return "Plays media through the GStreamer framework"; } }
		public string Author { get{ return "Goran Sterjov"; } }
		public string Version { get{ return "0.2"; } }
		public string Website { get{ return "http://fusemc.sourceforge.net"; } }
		
		
		
		
		public Engine ()
		{
			playbin.Error += error;
			playbin.Buffer += buffer;
			playbin.EndOfStream += end_of_stream;
			playbin.StateChanged += state_changed;
			playbin.FoundVideoInfo += found_video_info;
			playbin.FoundTag += found_tag;
		}
		
		
		
		/// <summary>
		/// Loads up the GStreamer library.
		/// </summary>
		public bool Initiate ()
		{
			return playbin.Initiate ();
		}
		
		
		
		
		/// <summary>
		/// Disposes the GStreamer library.
		/// </summary>
		public void Dispose ()
		{
			playbin.Dispose ();
		}
		
		/// <summary>
		/// Loads the specified path into the GStreamer library.
		/// </summary>
		public bool Load (string path)
		{
			return playbin.Load ("file://" + path);
		}
		
		/// <summary>
		/// Plays the loaded media file.
		/// </summary>
		public void Play ()
		{
			playbin.Play ();
		}
		
		/// <summary>
		/// Pauses the loaded media file.
		/// </summary>
		public void Pause ()
		{
			playbin.Pause ();
		}
		
		/// <summary>
		/// Stops the loaded media file and unloads it.
		/// </summary>
		public void Stop ()
		{
			playbin.Stop ();
		}
		
		/// <summary>
		/// Changes the window to which video playback is attached to.
		/// </summary>
		public void SetWindow (ulong window_id)
		{
			playbin.SetWindow (window_id);
		}
		
		
		
		/// <summary>
		/// Seeks to the nearest millisecond on the media file.
		/// </summary>
		public void Seek (TimeSpan time)
		{
			playbin.Seek (time);
		}
		
		
		/// <summary>
		/// Seeks to the specified track number.
		/// </summary>
		public void SeekToTrack (int track_number)
		{
			playbin.SeekToTrack (track_number);
		}
		
		
		
		
		/// <summary>
		/// Returns the current position that the media file is in.
		/// </summary>
		public TimeSpan CurrentPosition
		{
			get{ return playbin.CurrentPosition; }
		}
		
		/// <summary>
		/// Returns the current volume of the GStreamer library.
		/// </summary>
		public double Volume
		{
			get{ return playbin.Volume; }
			set{ playbin.Volume = value; }
		}
		
		/// <summary>
		/// Returns the total duration of the media file.
		/// </summary>
		public TimeSpan Duration
		{
			get{ return playbin.Duration; }
		}
		
		
		
		/// <summary>
		/// Returns a string array of all the visualisations available
		/// </summary>
		public string[] VisualisationList
		{
			get
			{
				List <string> list = new List <string> (playbin.VisualisationList);
				list.Sort ();
				return list.ToArray ();
			}
		}
		
		
		/// <summary>
		/// Sets the visualisation
		/// </summary>
		public string Visualisation
		{
			set{ playbin.Visualisation = value; }
		}
		
		
		
		/// <summary>
		/// Video information regarding the video file loaded.
		/// </summary>
		public IVideoInfo VideoInfo
		{
			get
			{
				GStreamer.VideoInfo info = playbin.VideoInfo;
				if (info != null)
					return new VideoInfo (info);
				else
					return null;
			}
		}
		
		
		/// <summary>
		/// Media tag of the loaded file.
		/// </summary>
		public IEngineTag Tag
		{
			get
			{
				GStreamer.Tag tag = playbin.Tag;
				if (tag != null)
					return new Tag (tag);
				else
					return null;
			}
		}
		
		
		
		/// <summary>
		/// Returns a value determining if the media file is a video file.
		/// </summary>
		public bool HasVideo
		{
			get{return playbin.HasVideo;}
		}
		
		/// <summary>
		/// Returns the current status of the media engine.
		/// </summary>
		public MediaStatus CurrentStatus
		{
			get
			{
				switch (playbin.CurrentStatus)
				{
					case GStreamer.MediaStatus.Loaded:
						return MediaStatus.Loaded;
					case GStreamer.MediaStatus.Paused:
						return MediaStatus.Paused;
					case GStreamer.MediaStatus.Playing:
						return MediaStatus.Playing;
					case GStreamer.MediaStatus.Stopped:
						return MediaStatus.Stopped;
				}
				return MediaStatus.Unloaded;
			}
		}
		
		
		
		//raised error
		void error (GStreamer.ErrorEventArgs args)
		{
			if (Error != null)
				Error (new ErrorEventArgs (args.Error, args.Debug));
		}
		
		
		//buffer event raised
		void buffer (GStreamer.BufferEventArgs args)
		{
			if (Buffer != null)
				Buffer (new BufferEventArgs (args.Progress));
		}
		
		
		//finished playing stream
		void end_of_stream ()
		{
			if (EndOfStream != null)
				EndOfStream ();
		}
		
		
		//state changed
		void state_changed (GStreamer.StateEventArgs args)
		{
			if (StateChanged == null) return;
			
			switch (args.State)
			{
				case GStreamer.MediaStatus.Loaded:
					StateChanged(new StateEventArgs (MediaStatus.Loaded));
					break;
				case GStreamer.MediaStatus.Paused:
					StateChanged(new StateEventArgs (MediaStatus.Paused));
					break;
				case GStreamer.MediaStatus.Playing:
					StateChanged(new StateEventArgs (MediaStatus.Playing));
					break;
				case GStreamer.MediaStatus.Stopped:
					StateChanged(new StateEventArgs (MediaStatus.Stopped));
					break;
				case GStreamer.MediaStatus.Unloaded:
					StateChanged(new StateEventArgs (MediaStatus.Unloaded));
					break;
			}
			
		}
		
		
		//found video information
		void found_video_info (GStreamer.VideoInfoEventArgs args)
		{
			if (FoundVideoInfo != null)
			{
				VideoInfo info = new VideoInfo (args.VideoInfo);
				FoundVideoInfo (new VideoInfoEventArgs (info));
			}
		}
		
		
		//found a tag
		void found_tag (GStreamer.TagEventArgs args)
		{
			if (FoundTag != null)
			{
				Tag tag = new Tag (args.Tag);
				FoundTag (new TagEventArgs (tag));
			}
		}
		
		
	}
	
}