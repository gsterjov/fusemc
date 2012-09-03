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

namespace Fuse.Interfaces
{
	/// <summary>
	/// The interface used to communicate media engines with
	/// Fuse's main window
	/// </summary>
	public interface IMediaEngine
	{
		
		event ErrorEventHandler Error;
		event BufferEventHandler Buffer;
		event EndOfStreamEventHandler EndOfStream;
		event StateEventHandler StateChanged;
		event VideoInfoEventHandler FoundVideoInfo;
		event TagEventHandler FoundTag;
		
		bool Initiate ();
		bool Load (string path);
		
		void Dispose ();
		void Play ();
		void Stop ();
		void Pause ();
		void Seek (TimeSpan time);
		void SeekToTrack (int track_number);
		void SetWindow (ulong window_id);
		
		double Volume {get;set;}
		TimeSpan CurrentPosition {get;}
		TimeSpan Duration {get;}
		IVideoInfo VideoInfo {get;}
		bool HasVideo {get;}
		string[] VisualisationList {get;}
		string Visualisation {set;}
		
		MediaStatus CurrentStatus {get;}
		
		
		string Name {get;}
		string Description {get;}
		string Author {get;}
		string Version {get;}
		string Website {get;}
	}
}