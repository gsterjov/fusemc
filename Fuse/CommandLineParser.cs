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


namespace Fuse
{
		
	/// <summary>
	/// Parses the command line arguments.
	/// </summary>
	public class CommandLineParser
	{
		
		string data_dir;
		
		// parse the command line
		public CommandLineParser (string[] args)
		{
			
			foreach (string arg in args)
			{
				if (arg.StartsWith ("--data-dir="))
					data_dir = parseArgument (arg);
			}
			
		}
		
		
		
		/// <summary>
		/// The directory containing all the users data.
		/// </summary>
		public string DataDir
		{
			get{ return data_dir; }
		}
		
		
		
		
		// parses the specific argument
		string parseArgument (string arg)
		{
			string val = arg.Substring (arg.IndexOf ("=") + 1);
			
			char[] trails = {char.Parse ("\"")};
			return val.Trim (trails);
		}
		
		
	}
}
