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

namespace Fuse.Plugin.Library
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
			
			string path = System.IO.Path.Combine (base_path, "library_settings.ini");
			
			
			if (!File.Exists (path))
				File.WriteAllText (path, "");
			
			source =  new IniConfigSource (path);
			
			
			bool addWindow = true;
			bool addOptions = true;
			bool addColumns = true;
			bool addSorting = true;
			bool addScrobbler = true;
			
			foreach (IConfig config in source.Configs)
			{
				if (config.Name == "Window")
					addWindow = false;
				else if (config.Name == "Options")
					addOptions = false;
				else if (config.Name == "Tree Columns")
					addColumns = false;
				else if (config.Name == "Sorting")
					addSorting = false;
				else if (config.Name == "AudioScrobbler")
					addScrobbler = false;
			}
			
			if (addWindow)
				source.AddConfig ("Window");
			if (addOptions)
				source.AddConfig ("Options");
			if (addColumns)
				source.AddConfig ("Tree Columns");
			if (addSorting)
				source.AddConfig ("Sorting");
			if (addScrobbler)
				source.AddConfig ("AudioScrobbler");
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
		/// Options configuration.
		/// </summary>
		public IConfig Options
		{
			get{ return source.Configs["Options"]; }
		}
		
		
		/// <summary>
		/// AudioScrobbler configuration.
		/// </summary>
		public IConfig AudioScrobbler
		{
			get{ return source.Configs["AudioScrobbler"]; }
		}
		
		
		/// <summary>
		/// Tree columns configuration.
		/// </summary>
		public IConfig TreeColumns
		{
			get{ return source.Configs["Tree Columns"]; }
		}
		
		
		/// <summary>
		/// Sorting configuration.
		/// </summary>
		public IConfig Sorting
		{
			get{ return source.Configs["Sorting"]; }
		}
		
	}
	
}
