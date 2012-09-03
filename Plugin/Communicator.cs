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
using System.Collections.Generic;

namespace Fuse
{
	
	
	public delegate void BroadcastEvent (BroadcastEventArgs args);
	
	
	/// <summary>
	/// Allows for communication between plugins.
	/// </summary>
	public class Communicator
	{
		List <Broadcaster> broadcasters = new List <Broadcaster> ();
		
		
		/// <summary>
		/// Registers a plugin with the broadcaster.
		/// </summary>
		public Broadcaster RegisterPlugin (string name, string version)
		{
			return RegisterPlugin (name, version, null);
		}
		
		
		/// <summary>
		/// Registers a plugin with the broadcaster.
		/// </summary>
		public Broadcaster RegisterPlugin (string name, string version, BroadcastEvent handler)
		{
			Broadcaster broadcaster = new Broadcaster (name, version, handler);
			broadcasters.Add (broadcaster);
			
			return broadcaster;
		}
		
		
		/// <summary>
		/// Unregisters the specified plugin broadcaster.
		/// </summary>
		public void UnregisterPlugin (Broadcaster broadcaster)
		{
			broadcasters.Remove (broadcaster);
			broadcaster = null;
		}
		
		
		
		
		/// <summary>
		/// Retrieves a broadcaster from the registered list.
		/// </summary>
		public Broadcaster GetBroadcaster (string name, string version)
		{
			foreach (Broadcaster broadcaster in broadcasters)
				if (broadcaster.Name == name && broadcaster.Version == version)
					return broadcaster;
			
			return null;
		}
		
	}
	
	
	
	
	// broadcast event args
	public sealed class BroadcastEventArgs
	{
		Broadcaster from_plugin;
		string command;
		object obj;
		
		public BroadcastEventArgs (Broadcaster from_plugin, string command, object obj)
		{
			this.from_plugin = from_plugin;
			this.command = command;
			this.obj = obj;
		}
		
		
		
		/// <summary>
		/// The plugin the command and data came from.
		/// </summary>
		public Broadcaster FromPlugin
		{
			get{ return from_plugin; }
		}
		
		/// <summary>
		/// What the plugin should do.
		/// </summary>
		public string Command
		{
			get{ return command; }
		}
		
		/// <summary>
		/// Data to be passed to the plugin.
		/// </summary>
		public object Object
		{
			get{ return obj; }
		}
		
	}
	
	
	
	/// <summary>
	/// Information about a plugin used in a broadcast.
	/// </summary>
	public sealed class Broadcaster
	{
		string name;
		string version;
		BroadcastEvent handler;
		
		public Broadcaster (string name, string version, BroadcastEvent handler)
		{
			this.name = name;
			this.version = version;
			this.handler = handler;
		}
		
		
		/// <summary>
		/// The name of the plugin.
		/// </summary>
		public string Name
		{
			get{ return name; }
		}
		
		
		/// <summary>
		/// The version of the plugin.
		/// </summary>
		public string Version
		{
			get{ return version; }
		}
		
		
		/// <summary>
		/// The delegate to handle the broadcast events.
		/// </summary>
		public BroadcastEvent BroadcastHandler
		{
			set{ handler = value; }
		}
		
		
		
		/// <summary>
		/// Send a command to the plugin.
		/// </summary>
		public void SendCommand (string command, object obj)
		{
			SendCommand (null, command, obj);
		}
		
		
		/// <summary>
		/// Send a command to the plugin.
		/// </summary>
		public void SendCommand (Broadcaster from_plugin, string command, object obj)
		{
			if (handler != null)
			{
				BroadcastEventArgs args = new BroadcastEventArgs (from_plugin, command, obj);
				handler (args);
			}
		}
		
		
	}
	
	
	
}
