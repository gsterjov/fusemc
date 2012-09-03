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
using System.Xml;
using System.Threading;
using Gtk;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// The context menu for the Media tree.
	/// </summary>
	public class MediaContextMenu : Menu
	{
		
		Media media;
		
		
		// create the context menu
		public MediaContextMenu (Media media) : base ()
		{
			this.media = media;
			
			ImageMenuItem play = new ImageMenuItem (Stock.MediaPlay, null);
			ImageMenuItem edit = new ImageMenuItem (Stock.Edit, null);
			MenuItem lyrics = new MenuItem ("View Lyrics");
			MenuItem info = new MenuItem ("View Artist Info");
			MenuItem add_to_playlist = new MenuItem ("Add To Playlist");
			
			play.Activated += play_activated;
			edit.Activated += edit_activated;
			lyrics.Activated += lyrics_activated;
			info.Activated += info_activated;
			
			
			// the "Add To Playlist" menu
			Menu playlist_menu = new Menu ();
			add_to_playlist.Submenu = playlist_menu;
			
			foreach (object[] row in Global.Core.Library.PlaylistTree.PlaylistStore)
			{
				Playlist playlist = (Playlist) row[0];
				MenuItem playlist_item = new MenuItem (playlist.Name);
				playlist_menu.Add (playlist_item);
				
				
				playlist_item.Activated += delegate (object o, EventArgs args) {
					Global.Core.Library.MediaTree.MediaStore.AddMedia (media.Path, playlist);
				};
				
			}
			
			
			if (Global.Core.Library.PlaylistTree.PlaylistStore.IterNChildren () == 0)
				add_to_playlist.Sensitive = false;
			
			
			this.Add (play);
			this.Add (edit);
			this.Add (lyrics);
			this.Add (info);
			this.Add (new SeparatorMenuItem ());
			this.Add (add_to_playlist);
		}
		
		
		
		// play was clicked
		void play_activated (object o, EventArgs args)
		{
			TreeIter iter;
			if (!Global.Core.Library.MediaTree.Selection.GetSelected (out iter))
				return;
			
			TreePath path = Global.Core.Library.MediaTree.Model.GetPath (iter);
			Global.Core.Library.MediaTree.ActivateRow (path, Global.Core.Library.MediaTree.Columns[0]);
		}
		
		
		// edit was clicked
		void edit_activated (object o, EventArgs args)
		{
			if (media is FileMedia)
			{
				MediaEdit edit = new MediaEdit ((FileMedia) media);
				edit.Run ();
			}
			else
				Global.Core.Fuse.ThrowError ("Cant edit this kind of media. Only files can be edited.");
		}
		
		
		// lyrics was clicked
		private void lyrics_activated (object o, EventArgs args)
		{
			Global.Core.Library.InfoBar.LoadMedia (media, typeof (Info.LyricWiki.Lyrics));
		}
		
		
		// info was clicked
		private void info_activated (object o, EventArgs args)
		{
			Global.Core.Library.InfoBar.LoadMedia (media, typeof (Info.AudioScrobbler.ArtistInfo.ArtistInfo));
		}
		
	}
}
