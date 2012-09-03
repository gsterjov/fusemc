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


using Gtk;
using System;

namespace Fuse.Interfaces
{
	/// <summary>
	/// The main functions for Fuse that is
	/// to be exposed to extensions.
	/// </summary>
	public interface IFuse
	{
		event EventHandler Quiting;
		
		void AddWidget (Widget widget, string title);
		void RemoveWidget (Widget widget);
		
		void StatusPush (string message);
		void StatusPop ();
		
		void Quit ();
		void ThrowWarning (string brief_warning, string detailed_warning);
		void ThrowException (string exception);
		void ThrowError (string message);
		
		Window MainWindow {get;}
		IMediaControls MediaControls {get;}
		Communicator PluginCommunicator {get;}
		
		string DataDir {get;}
		string ConfigDir {get;}
	}
}