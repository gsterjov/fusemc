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
	/// Information regarding the video file loaded.
	/// </summary>
	public class VideoInfo : IVideoInfo
	{
		int width;
		int height;
    	float frame_rate;
		
		
		public VideoInfo (GStreamer.VideoInfo video_info)
		{
			this.width = video_info.Width;
			this.height = video_info.Height;
			this.frame_rate = video_info.FrameRate;
		}
    	
		
    	public int Width { get{ return width; } }
       	public int Height { get{ return height; } }
       	public float AspectRatio { get { return (float)width/height; } }
    	public float FrameRate { get{ return frame_rate; } }
	}
	
}