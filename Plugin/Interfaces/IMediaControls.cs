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
	/// The main media control functions such as play, pause, load, etc.
	/// </summary>
	public interface IMediaControls
	{
		event TagEventHandler ProbeTag;
		event MediaTimerHandler MediaTimer;
		
		
		void LoadMedia (string path);
		void LoadMedia (string path, NavigateFunc navFunc);
		void LoadTrack (int track_number);
		void LoadTrack (int track_number, NavigateFunc navFunc);
		bool ProbeAudioCD ();
		void Play ();
		void Pause ();
		void Previous ();
		void Next ();
		
		
		IMediaEngine MediaEngine {get;}
		
		
		bool Shuffle {get;set;}
		string CurrentMedia {get;}
		string MediaInfo {get;set;}
	}
}