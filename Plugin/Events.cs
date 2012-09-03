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
using Fuse.Interfaces;

namespace Fuse
{
	
	// used for list navigation
	public enum NavigateType { Next, Previous, Shuffle }
	public delegate void NavigateFunc (NavigateType type);
	
	
	// media engine enumerations
	public enum MediaStatus { Playing, Stopped, Paused, Loaded, Unloaded }
	
	// media engine event handlers
	public delegate void ErrorEventHandler (ErrorEventArgs args);
	public delegate void BufferEventHandler (BufferEventArgs args);
	public delegate void EndOfStreamEventHandler ();
	
	public delegate void StateEventHandler (StateEventArgs args);
	public delegate void VideoInfoEventHandler (VideoInfoEventArgs args);
	public delegate void TagEventHandler (TagEventArgs args);
	
	public delegate void MediaTimerHandler (MediaTimerEventArgs args);
	
	
	public delegate void EmptyHandler ();
	
	
	
	/// <summary>
	/// Arguments for a raised error.
	/// </summary>
	public sealed class ErrorEventArgs
	{
		string error, debug;
		
		public ErrorEventArgs (string error, string debug)
		{
			this.error = error;
			this.debug = debug;
		}
		
        public string Error { get{ return error; } }
		public string Debug { get{ return debug; } }
    }
	
	
	
	/// <summary>
	/// Arguments for a raised buffer event.
	/// </summary>
	public sealed class BufferEventArgs
	{
		int progress;
		
		public BufferEventArgs (int progress)
		{ this.progress = progress; }
		
        public int Progress { get{ return progress; } }
    }
	
	
	
	/// <summary>
	/// Arguments for a raised video info event.
	/// </summary>
	public sealed class VideoInfoEventArgs
	{
		IVideoInfo video_info;
		
		public VideoInfoEventArgs (IVideoInfo video_info)
		{ this.video_info = video_info; }
		
        public IVideoInfo VideoInfo { get{ return video_info; } }
    }
	
	
	
	/// <summary>
	/// Arguments for a raised video info event.
	/// </summary>
	public sealed class TagEventArgs
	{
		IEngineTag tag;
		
		public TagEventArgs (IEngineTag tag)
		{ this.tag = tag; }
		
        public IEngineTag Tag { get{ return tag; } }
    }
	
    
	
	/// <summary>
	/// Arguments for a raised state.
	/// </summary>
    public sealed class StateEventArgs
    {
    	MediaStatus state;
		
    	public StateEventArgs (MediaStatus state)
    	{ this.state = state; }
		
    	public MediaStatus State { get{ return state; } }
    }
	
	
	
	/// <summary>
	/// Arguments for a raised media timer event.
	/// </summary>
    public sealed class MediaTimerEventArgs
    {
    	TimeSpan position;
		TimeSpan duration;
		
    	public MediaTimerEventArgs (TimeSpan position, TimeSpan duration)
    	{
			this.position = position;
			this.duration = duration;
		}
		
    	public TimeSpan Position { get{ return position; } }
		public TimeSpan Duration { get{ return duration; } }
    }
	
    
}
