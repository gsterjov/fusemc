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
using Nini.Config;

namespace Fuse.Plugin.Theatre
{
	
	/// <summary>
	/// Configuration class for the application.
	/// </summary>
	public class Config
	{
		
		
		IConfigSource source;
		
		
		//creates a config file in the specified base path
		public Config (string base_path)
		{
			if (!Directory.Exists (base_path))
				Directory.CreateDirectory (base_path);
			
			string path = System.IO.Path.Combine (base_path, "theatre_settings.ini");
			
			
			if (!File.Exists (path))
				File.WriteAllText (path, "");
			
			source =  new IniConfigSource (path);
			
			
			bool addWindow = true;
			bool addTheatre = true;
			
			foreach (IConfig config in source.Configs)
			{
				if (config.Name == "Window")
					addWindow = false;
				if (config.Name == "Theatre")
					addTheatre = false;
			}
			
			if (addWindow)
				source.AddConfig ("Window");
			if (addTheatre)
				source.AddConfig ("Theatre");
		}
		
		
		
		/// <summary>
		/// Save the configuration.
		/// </summary>
		public void Save ()
		{
			source.Save ();
		}
		
		
		/// <summary>
		/// Window configuration.
		/// </summary>
		public IConfig Window
		{
			get{ return source.Configs["Window"]; }
		}
		
		
		/// <summary>
		/// Theatre configuration.
		/// </summary>
		public IConfig Theatre
		{
			get{ return source.Configs["Theatre"]; }
		}
		
	}
	
}
