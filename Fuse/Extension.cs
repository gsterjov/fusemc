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
using System.Reflection;
using Fuse.Interfaces;


namespace Fuse
{
	
	/// <summary>
	/// Creates a extension class from a specified assembly file.
	/// </summary>
	public abstract class Extension <T>
	{
		string path;
		string interface_name;
		T instance;
		
		
		/// <summary>
		/// Creates the extension class but does not load it.
		/// </summary>
		public Extension (string path, string interface_name) {
			this.path = path;
			this.interface_name = interface_name;
		}
		
		
		/// <summary>
		/// The absolute path to the assembly.
		/// </summary>
		public string Path {
			get{ return path; }
		}
		
		
		/// <summary>
		/// The instance of the extension.
		/// </summary>
		public T Instance {
			get { return instance; }
		}
		
		/// <summary>
		/// Creates an instance of the specified assembly
		/// and stores it in Extension.Instance.
		/// </summary>
		public bool Load () {
			
			// make sure the file exists
			if (!File.Exists (path)) return false;
			try {
				Assembly assemb = Assembly.LoadFrom (path);
				
				// loop through all the available interfaces and check
				// to see if it has the correct interface
				foreach (Type type in assemb.GetTypes ()) {
					if (hasInterface (type)) {
						instance = (T) Activator.CreateInstance (assemb.GetType (type.ToString ()));
						return true;
					}
				}
			}
			// something went wrong. throw an error
			catch (Exception e) {
				string message = "Extension.Load:: Load Failed - " + System.IO.Path.GetFileName (path);
				Console.WriteLine (message + "\n\n" + e.Message);
				return false;
			}
			return false;
		}
		
		
		// see if the specified type matches the interface name.
		// it helps clean up the Load function
		bool hasInterface (Type type) {
			if (type.IsPublic && !type.IsAbstract) {
				Type interface_type = type.GetInterface (interface_name, false);
				if (interface_type != null) return true;
			}
			return false;
		}
		
	}
}
