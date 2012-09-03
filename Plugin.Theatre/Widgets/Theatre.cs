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
using Gtk;

using Fuse.Interfaces;


namespace Fuse.Plugin.Theatre
{
		
	/// <summary>
	/// The theatre widget.
	/// </summary>
	public class Theatre
	{
		
		private Random random = new Random ();
		
		// global widgets
		private VideoWidget video_widget;
		private VideoWidget video_widget_fullscreen;
		private Fullscreen fullscreen;
		
		private TreeView media_tree;
		
		private ListStore media_store = new ListStore (typeof (Media));
		private HPaned main_splitter = new HPaned ();
		
		private float ratio;
		private string visualisation;
		
		
		// create the main theatre widget
		public Theatre ()
		{
			video_widget = new VideoWidget (true);
			video_widget_fullscreen = new VideoWidget (false);
			fullscreen = new Fullscreen (video_widget_fullscreen);
			
			media_tree = new TreeView (media_store);
			
			media_tree.AppendColumn (null, new CellRendererText (), new TreeCellDataFunc (renderMedia));
			media_tree.HeadersVisible = false;
			
			
			ScrolledWindow scrolled_tree = new ScrolledWindow ();
			scrolled_tree.Add (media_tree);
			scrolled_tree.ShadowType = ShadowType.In;
			
			main_splitter.Add1 (scrolled_tree);
			main_splitter.Add2 (video_widget);
			
			
			media_tree.RowActivated += tree_activated;
			
			video_widget.ButtonPressEvent += video_button_press;
			video_widget_fullscreen.ButtonPressEvent += video_button_press;
			
			video_widget.ButtonReleaseEvent += video_button_release;
			video_widget_fullscreen.ButtonReleaseEvent += video_button_release;
			
			Global.Core.Fuse.MediaControls.MediaEngine.FoundVideoInfo += found_video_info;
		}
		
		
		
		
		/// <summary>
		/// Toggle between fullscreen modes.
		/// </summary>
		public void Fullscreen ()
		{
			if (!fullscreen.Visible)
			{
				if (Global.Core.Fuse.MediaControls.MediaEngine.CurrentStatus == MediaStatus.Playing ||
				    Global.Core.Fuse.MediaControls.MediaEngine.CurrentStatus == MediaStatus.Paused)
				{
					fullscreen.Show ();
					Global.Core.Fuse.MediaControls.MediaEngine.SetWindow (video_widget_fullscreen.Drawable);
				}
			}
			else if (fullscreen.Visible)
			{
				fullscreen.Hide ();
				Global.Core.Fuse.MediaControls.MediaEngine.SetWindow (video_widget.Drawable);
			}
		}
		
		
		
		/// <summary>
		/// Change the aspect ratio of the displayed video.
		/// </summary>
		public float AspectRatio
		{
			get{ return ratio; }
			set
			{
				ratio = value;
				
				if (ratio == 0)
				{
					if (Global.Core.Fuse.MediaControls.MediaEngine.VideoInfo != null)
					{
						float auto_aspect = Global.Core.Fuse.MediaControls.MediaEngine.VideoInfo.AspectRatio;
						video_widget.ChangeAspect (auto_aspect);
						video_widget_fullscreen.ChangeAspect (auto_aspect);
					}
				}
				else
				{
					video_widget.ChangeAspect (ratio);
					video_widget_fullscreen.ChangeAspect (ratio);
				}
			}
		}
		
		
		
		/// <summary>
		/// The currently selected visualisation.
		/// </summary>
		public string Visualisation
		{
			get{ return visualisation; }
			set
			{
				visualisation = value;
				Global.Core.Fuse.MediaControls.MediaEngine.Visualisation = value;
			}
		}
		
		
		
		
		/// <summary>
		/// Loads all the media files from the database.
		/// </summary>
		public void LoadData ()
		{
			List <Media> list = Global.Core.DataManager.GetMedia ();
			
			foreach (Media media in list)
				media_store.AppendValues (media);
		}
		
		
		
		
		/// <summary>
		/// Adds a media file to the store.
		/// </summary>
		public void Add (string path)
		{
			Media media = new Media (path);
			
			media_store.AppendValues (media);
			Global.Core.DataManager.AddMedia (media);
		}
		
		
		
		/// <summary>
		/// Removes the selected media file from the store.
		/// </summary>
		public void Remove ()
		{
			TreeIter iter;
			if (media_tree.Selection.GetSelected (out iter))
			{
				Media media = (Media) media_store.GetValue (iter, 0);
				
				Global.Core.DataManager.DeleteMedia (media);
				media_store.Remove (ref iter);
			}
		}
		
		
		
		/// <summary>
		/// The core widget which contains all the child widgets.
		/// </summary>
		public HPaned MainSplitter
		{
			get{ return main_splitter; }
		}
		
		
		
		// Navigate through the media list.
		private void Navigate (NavigateType type)
		{
			if (type == NavigateType.Next)
				next ();
			else if (type == NavigateType.Previous)
				previous ();
			else if (type == NavigateType.Shuffle)
				shuffle ();
		}
		
		
		
		// gets the tree path of the currently playing media file.
		private TreePath currentPath ()
		{
			TreePath playing_path = null;
			
			media_store.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter)
			{
				Media media = (Media) model.GetValue (iter, 0);
				
				if (media.Path == Global.Core.Fuse.MediaControls.CurrentMedia)
				{
					playing_path = path.Copy ();
					return true;
				}
				return false;
			});
			
			return playing_path;
		}
		
		
		// selects the next media file on the list
		private void next ()
		{
			TreePath path = currentPath ();
			TreeIter iter;
			
			if (path == null || !media_store.GetIter (out iter, path)) return;
			
			if (media_store.IterNext (ref iter))
			{
				path.Next ();
				media_tree.Selection.SelectPath (path);
				media_tree.ActivateRow (path, media_tree.Columns[0]);
			}
		}
		
		
		// selects the previous media file on the list
		private void previous ()
		{
			TreePath path = currentPath ();
			
			if (path == null) return;
			
			if (path.Prev ())
			{
				media_tree.Selection.SelectPath (path);
				media_tree.ActivateRow (path, media_tree.Columns[0]);
			}
		}
		
		
		// selects a random media file on the list
		private void shuffle ()
		{
			int num = random.Next (media_store.IterNChildren () - 1);
			TreePath path = new TreePath (num.ToString ());
			media_tree.Selection.SelectPath (path);
			media_tree.ActivateRow (path, media_tree.Columns[0]);
		}
		
		
		
		// user double clicked on the video widget
		private void video_button_press (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Type == Gdk.EventType.TwoButtonPress)
				Fullscreen ();
		}
		
		
		// user right clicked on the video widget
		private void video_button_release (object o, ButtonReleaseEventArgs args)
		{
			if (args.Event.Button != 3)
				return;
			
			VideoContextMenu menu = new VideoContextMenu ();
			menu.ShowAll ();
			menu.Popup ();
		}
		
		
		
		// a media event has occured
		private void found_video_info (VideoInfoEventArgs args)
		{
			// auto aspect ratio is selected
			if (ratio == 0)
				AspectRatio = 0;
		}
		
		
		
		
		// render the media file into the treeview
		private void renderMedia (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Media media = (Media) model.GetValue (iter, 0);
			string text = System.IO.Path.GetFileNameWithoutExtension (media.Path);
			(cell as CellRendererText).Markup = Utils.ParseMarkup (text);
		}
		
		
		
		// user double clicked on a media file
		private void tree_activated (object o, RowActivatedArgs args)
		{
			TreeIter iter;
			if (media_store.GetIter (out iter, args.Path))
			{
				Media media = (Media) media_store.GetValue (iter, 0);
				Global.Core.Fuse.MediaControls.LoadMedia (media.Path, Navigate);
				
				string text = System.IO.Path.GetFileNameWithoutExtension (media.Path);
				Global.Core.Fuse.MediaControls.MediaInfo = "<b>" + Utils.ParseMarkup (text) + "</b>";
				
				video_widget.QueueDraw ();
				video_widget_fullscreen.QueueDraw ();
			}
		}
		
		
	}
}
