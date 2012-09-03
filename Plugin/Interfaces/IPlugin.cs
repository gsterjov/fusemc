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


namespace Fuse.Interfaces
{
	/// <summary>
	/// The interface used to communicate plugins with
	/// Fuse's main window
	/// </summary>
	public interface IPlugin
	{
		void Initiate (IFuse fuse);
		void Deinitiate (IFuse fuse);
		void InitiateOptions (IPluginOptions plugin_window);
		string Name {get;}
		string Description {get;}
		string Author {get;}
		string Version {get;}
		string Website {get;}
	}
}