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

namespace Fuse
{
		
	/// <summary>
	/// Utility functions used throughout the application.
	/// </summary>
	public class Utils
	{
		
		
		
		/// <summary>
		/// Makes the specified text pango compatible.
		/// </summary>
		public static string ParseMarkup (string text)
		{
			text = text.Replace ("&", "&amp;");
			text = text.Replace ("<", "&lt;");
			return text.Replace (">", "&gt;");
		}
		
		
		
		/// <summary>
		/// Convenience function to convert a timespan into a presentable string.
		/// </summary>
		public static string PrettyTime (TimeSpan span)
		{
			string sec = span.Seconds.ToString ();
			if(span.Seconds < 10)
				sec = "0" + sec;
			return span.Minutes + ":" + sec;
		}
		
		
	}
}
