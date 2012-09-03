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
using Fuse.Interfaces;

namespace Fuse
{
	
	/// <summary>
	/// A widget that controls the MediaEngine.
	/// </summary>
	public class Controls : HBox, IMediaControls
	{
		private FuseApp fuse;
		
		private MediaEngine mediaEngine1;
		private MediaEngine mediaEngine2;
		private MediaEngine probeEngine;
		
		private SeekBar seekbar = new SeekBar ();
		private Timer timer = new Timer ();
		private Timer crossfade_timer = new Timer ();
		
		private int fade_volume;
		private bool switch_engines;
		
		private NavigateFunc nav_func;
		
		
		// global widgets
		private Label time = new Label ();
		private Label mediaInfo = new Label ();
		private Button play = new Button ();
		
		// media status variables
		private bool shuffle;
		private bool crossfade;
		private bool playing;
		private string current_media;
		private string media_info;
		
		
		// events
		public event TagEventHandler ProbeTag;
		public event MediaTimerHandler MediaTimer;
		
		
		
		// creates the main controls such as play, time left, etc.
		public Controls (FuseApp fuse) : base (false, 0)
		{
			this.fuse = fuse;
			
			// the buttons
			Button prev = new Button ();
			Button next = new Button ();
			Button volume = new Button ();
			
			play.Image = new Image (Stock.MediaPlay, IconSize.Button);
			prev.Image = new Image (Stock.MediaPrevious, IconSize.Button);
			next.Image = new Image (Stock.MediaNext, IconSize.Button);
			
			//FIXME: wont work in windows
			Gdk.Pixbuf vol = IconTheme.Default.LoadIcon ("audio-volume-high", (int)IconSize.Button, IconLookupFlags.ForceSvg);
			volume.Image = new Image (vol);
			
			
			
			// media information box
			mediaInfo.Markup = "<small> </small>";
			time.Markup = "<small> </small>";
			
			VBox info_box = new VBox (false, 0);
			HBox mediaInfo_box = new HBox (false, 0);
			info_box.PackStart (mediaInfo_box, true, true, 0);
			info_box.PackStart (seekbar, true, true, 0);
			
			mediaInfo_box.PackStart (mediaInfo, true, true, 0);
			mediaInfo_box.PackStart (time, false, false, 2);
			
			
			// hook in the media timer
			timer.Elapsed += timer_elapsed;
			crossfade_timer.Elapsed += crossfade_count;
			
			
			// hook button events
			play.Clicked += play_clicked;
			prev.Clicked += prev_clicked;
			next.Clicked += next_clicked;
			seekbar.PositionChanged += seek_changed;
			
			
			// pack the widgets in
			this.PackStart (prev, false, false, 0);
			this.PackStart (play, false, false, 2);
			this.PackStart (next, false, false, 0);
			this.PackStart (info_box, true, true, 5);
			this.PackStart (volume, false, false, 0);
			this.BorderWidth = 3;
		}
		
		
		
		
		
		/// <summary>
		/// The user selected MediaEngine.
		/// </summary>
		public MediaEngine Engine
		{
			get{ return mediaEngine1; }
			set
			{
				mediaEngine1 = value;
				mediaEngine2 = null;
				
				if (mediaEngine1 != null)
				{
					mediaEngine1.Instance.Initiate ();
					mediaEngine1.Instance.Error += engine_error;
					mediaEngine1.Instance.EndOfStream += engine_eos;
					
					mediaEngine2 = new MediaEngine (mediaEngine1.Path);
					mediaEngine2.Load ();
					mediaEngine2.Instance.Initiate ();
					mediaEngine2.Instance.Error += engine_error;
					mediaEngine2.Instance.EndOfStream += engine_eos;
					
					
					probeEngine = new MediaEngine (mediaEngine1.Path);
					if (probeEngine.Load ())
                        probeEngine.Instance.FoundTag += probe_tag;
                    else
                        probeEngine = null;
					
				}
			}
		}
		
		
		/// <summary>
		/// The MediaEngine instance.
		/// </summary>
		public IMediaEngine MediaEngine
		{
			get{ return switch_engines ? mediaEngine2.Instance : mediaEngine1.Instance;; }
		}
		
		
		/// <summary>
		/// The MediaEngine instance.
		/// </summary>
		public IMediaEngine MediaEngineOld
		{
			get{ return switch_engines ? mediaEngine1.Instance : mediaEngine2.Instance; }
		}
		
		
		
		
		/// <summary>
		/// Load the specified file into the MediaEngine.
		/// </summary>
		public void LoadMedia (string path)
		{
			LoadMedia (path, null);
		}
		
		/// <summary>
		/// Load the specified file into the media engine and navigate using navFunc.
		/// </summary>
		public void LoadMedia (string path, NavigateFunc navFunc)
		{
			nav_func = navFunc;
			
			if (!validEngine ())
				return;
			
			
			if (System.IO.File.Exists (path))
			{
				switch_engines = !switch_engines;
				
				Stop ();
				MediaEngine.Load (path);
				current_media = path;
				
				fadePlay ();
			}
			else
				fuse.ThrowError ("The file doesn't exist:\n\n" + path);
		}
		
		
		
		/// <summary>
		/// Loads a specific track number.
		/// </summary>
		public void LoadTrack (int track_number)
		{
			LoadTrack (track_number, null);
		}
		
		
		/// <summary>
		/// Loads a specific track number.
		/// </summary>
		public void LoadTrack (int track_number, NavigateFunc navFunc)
		{
			nav_func = navFunc;
			
			if (!validEngine ())
				return;
			
			
			if (current_media == null || !current_media.StartsWith ("cdda://"))
			{
				Stop ();
				MediaEngine.Load ("cdda://");
			}
			
			current_media = "cdda://" + track_number;
			MediaEngine.SeekToTrack (track_number);
			
			if (MediaEngine.CurrentStatus != MediaStatus.Playing)
				Play ();
		}
		
		
        
		/// <summary>
		/// Probes an audio cd to get its info.
		/// </summary>
		public bool ProbeAudioCD ()
		{
			if (probeEngine == null)
				return false;
            
            
            if (probeEngine.Instance.Initiate () && probeEngine.Instance.Load ("cdda://1"))
                return true;
            
            return false;
		}
		
		
		
		/// <summary>
		/// Disposes the MediaEngine.
		/// </summary>
		public void DisposeEngine ()
		{
			if (Engine != null)
			{
				MediaEngine.Stop ();
				MediaEngine.Dispose ();
				Engine = null;
			}
		}
		
		
		
		/// <summary>
		/// Stops the loaded media file.
		/// </summary>
		public void Stop ()
		{
			stopTimer ();
			MediaEngine.Stop ();
		}
		
		
		
		/// <summary>
		/// Plays the loaded media file.
		/// </summary>
		public void Play ()
		{
			MediaEngine.Play ();
			
			timer.Start (1000);
			seekbar.Idle = false;
			
			playing = true;
			play.Image = new Image (Stock.MediaPause, IconSize.Button);
		}
		
		
		/// <summary>
		/// Pauses the loaded media file.
		/// </summary>
		public void Pause ()
		{
			MediaEngine.Pause ();
			
			stopTimer ();
			
			playing = false;
			play.Image = new Image (Stock.MediaPlay, IconSize.Button);
		}
		
		
		/// <summary>
		/// Plays the previous media file on the list.
		/// </summary>
		public void Previous ()
		{
			if (nav_func == null) return;
			
			if (shuffle)
				nav_func (NavigateType.Shuffle);
			else
				nav_func (NavigateType.Previous);
		}
		
		
		/// <summary>
		/// Plays the next media file on the list.
		/// </summary>
		public void Next ()
		{
			if (nav_func == null) return;
			
			if (shuffle)
				nav_func (NavigateType.Shuffle);
			else
				nav_func (NavigateType.Next);
		}
		
		
		
		/// <summary>
		/// Play a random media file.
		/// </summary>
		public bool Shuffle
		{
			get{ return shuffle; }
			set{ shuffle = value; }
		}
		
		
		/// <summary>
		/// Crossfade between tracks.
		/// </summary>
		public bool Crossfade
		{
			get{ return crossfade; }
			set{ crossfade = value; }
		}
		
		
		
		/// <summary>
		/// The media title that is to be displayed.
		/// </summary>
		public string MediaInfo
		{
			get{ return media_info; }
			set
			{
				media_info = value;
				mediaInfo.Markup = "<small>" + value + "</small>";
			}
		}
		
		
		/// <summary>
		/// The path of the currently playing media file.
		/// </summary>
		public string CurrentMedia
		{
			get{ return current_media; }
		}
		
		
		
		
		// stops the media timer
		private void stopTimer ()
		{
			timer.Stop ();
			seekbar.Idle = true;
		}
		
		
		// checks if the media engine is valid
		private bool validEngine ()
		{
			if (Engine != null)
				return true;
			
			fuse.ThrowError ("Could not load a media MediaEngine.\nGo to  Options -> Media Engines  to select a working engine");
			return false;
		}
		
		
		// fade into the next song
		private void fadePlay ()
		{
			if (MediaEngineOld.CurrentStatus == MediaStatus.Playing && crossfade)
			{
				crossfade_timer.Stop ();
				
				fade_volume = 0;
				MediaEngine.Volume = 0;
				
				Play ();
				crossfade_timer.Start (50);
			}
			else
			{
				MediaEngineOld.Stop ();
				MediaEngine.Volume = 100;
				Play ();
			}
		}
		
		
		
		// the crossfade timer
		private void crossfade_count ()
		{
			fade_volume++;
			
			if (fade_volume > 100)
			{
				crossfade_timer.Stop ();
				MediaEngineOld.Stop ();
			}
			else
			{
				MediaEngine.Volume = fade_volume;
				MediaEngineOld.Volume = 100-fade_volume;
			}
		}
		
		
		// executed after each interval has passed
		private void timer_elapsed ()
		{
			TimeSpan position = MediaEngine.CurrentPosition;
			TimeSpan duration = MediaEngine.Duration;
			
			// make sure the range is valid
			if (duration.TotalSeconds <= 0 || duration.TotalSeconds < position.TotalSeconds)
				return;
			
			// start crossfading
			else if (duration.TotalSeconds-3 <= position.TotalSeconds && crossfade)
				Next ();
			
			
			string pretty_time = Utils.PrettyTime (position) + " of " + Utils.PrettyTime (duration);
			time.Markup = "<small>" + pretty_time + "</small>";
			
			
			seekbar.SetRange (0, duration.TotalSeconds);
			seekbar.Position = position.TotalSeconds;
			
			if (MediaTimer != null)
				MediaTimer (new MediaTimerEventArgs (position, duration));
		}
		
		
		
		
		// a media tag was found
		private void probe_tag (TagEventArgs args)
		{
			if (ProbeTag != null)
				ProbeTag (args);
			
			probeEngine.Instance.Dispose ();
		}
		
		
		// the user changed the position of the seek bar
		private void seek_changed (object o, EventArgs args)
		{
			MediaEngine.Seek (TimeSpan.FromSeconds (seekbar.Position));
		}
		
		
		// the user clicked the play/pause button
		private void play_clicked (object o, EventArgs args)
		{
			if (string.IsNullOrEmpty (current_media))
				return;
			
			if (!playing)
				Play ();
			else
				Pause ();
		}
		
		
		// an error was raised with the engine
		private void engine_error (ErrorEventArgs args)
		{
			fuse.ThrowWarning ("GStreamer Engine:: " + args.Error, args.Debug);
		}
		
		
		// finished playing the stream
		private void engine_eos ()
		{
			if (!crossfade)
				Next ();
		}
		
		
		// the user clicked the previous button
		private void prev_clicked (object o, EventArgs args)
		{
			Previous ();
		}
		
		
		// the user clicked the next button
		private void next_clicked (object o, EventArgs args)
		{
			Next ();
		}
		
		
	}
}
