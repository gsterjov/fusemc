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
using System.IO;
using System.Net;

namespace Fuse
{
	
	/// <summary>
	/// Downloads from the web.
	/// </summary>
	public class WebDownload
	{
		
		long contentLength = 0;
		long bytesRead = 0;
		
		
		/// <summary>
		/// Read the contents of the URL.
		/// </summary>
		public Stream ReadUrl (string url)
		{
			
			WebRequest request = WebRequest.Create (url);
			WebResponse response = request.GetResponse ();
			Stream stream = response.GetResponseStream ();
			
			//save bytes to memory
			MemoryStream memory = new MemoryStream ();
			
			contentLength = response.ContentLength;
			
			
			try
			{
				byte[] buffer = new byte[1024];
				int read = 0;
				bool reading = true;
				
				while (reading)
				{
					read = stream.Read (buffer, 0, buffer.Length);
					
					if (read != 0)
						memory.Write (buffer, 0, read);
					
					reading = read != 0;
					bytesRead += read;
				}
				
				memory.Seek (0, SeekOrigin.Begin);
			}
			
			catch (Exception e)
			{
				memory.Close ();
				throw e;
			}
			
			finally
			{
				stream.Close ();
				stream.Dispose ();
				response.Close ();
			}
			
			
			return memory;
		}
		
		
		
		/// <summary>
		/// The length of the download.
		/// </summary>
		public long ContentLength
		{
			get{ return contentLength; }
		}
		
		
		/// <summary>
		/// The amount of bytes received.
		/// </summary>
		public long BytesRead
		{
			get{ return bytesRead; }
		}
		
	}
}
