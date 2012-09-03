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
using Gtk;

namespace Fuse.Plugin.Library
{
	
	/// <summary>
	/// The media edit window.
	/// </summary>
	public class MediaEdit : DialogBase
	{
		FileMedia media;
		
		string pic_uri;
		TreePath current_path;
		
		
		// global widgets;
		Entry path_entry = new Entry ();
		Entry artist_entry = new Entry ();
		Entry title_entry = new Entry ();
		Entry album_entry = new Entry ();
		
		TextView comment_textview = new TextView ();
		ComboBox genre_combo = new ComboBox ();
		
		SpinButton year_spin = new SpinButton (0, 3000, 1);
		SpinButton tracknumber_spin = new SpinButton (0, 3000, 1);
		SpinButton trackcount_spin = new SpinButton (0, 3000, 1);
		
		Image picture = new Image ();
		
		Button open_button = new Button (Stock.Open);
		Button back_button = new Button (Stock.GoBack);
		Button forward_button = new Button (Stock.GoForward);
		Button save_button = new Button (Stock.Save);
		Button close_button = new Button (Stock.Close);
		
		
		
		// create the window
		public MediaEdit (FileMedia media) : base (Global.Core.Fuse.MainWindow, "Edit Tag")
		{
			this.media = media;
			
			
			// get the current path
			TreeIter iter;
			if (Global.Core.Library.MediaTree.Selection.GetSelected (out iter))
				current_path = Global.Core.Library.MediaTree.Model.GetPath (iter);
			
			
			
			ScrolledWindow comment_scroll = new ScrolledWindow ();
			comment_scroll.Add (comment_textview);
			comment_scroll.ShadowType = ShadowType.EtchedIn;
			
			genre_combo.Sensitive = false;
			path_entry.IsEditable = false;
			
			
			// labels
			Label path_label = new Label ("File:");
			Label artist_label = new Label ("Artist:");
			Label title_label = new Label ("Title:");
			Label album_label = new Label ("Album:");
			Label genre_label = new Label ("Genre:");
			Label comment_label = new Label ("Comment:");
			Label year_label = new Label ("Year:");
			Label tracknumber_label = new Label ("Track Number:");
			Label trackcount_label = new Label ("Total Tracks:");
			Label picture_label = new Label ("Picture:");
			
			path_label.Xalign = 0;
			artist_label.Xalign = 0;
			title_label.Xalign = 0;
			album_label.Xalign = 0;
			genre_label.Xalign = 0;
			comment_label.Xalign = 0;
			year_label.Xalign = 0;
			tracknumber_label.Xalign = 0;
			trackcount_label.Xalign = 0;
			
			
			// containers
			VBox backbone = new VBox (false, 5);
			
			HBox path_box = new HBox (false, 5);
			path_box.PackStart (path_label, false, false, 0);
			path_box.PackStart (path_entry, true, true, 0);
			
			
			Table left_table = new Table (5, 2, false);
			Table right_table = new Table (3, 2, false);
			
			left_table.ColumnSpacing = 3;
			left_table.RowSpacing = 2;
			right_table.ColumnSpacing = 3;
			right_table.RowSpacing = 2;
			
			
			// left table attachments
			left_table.Attach (artist_label, 0, 1, 0, 1);
			left_table.Attach (title_label, 0, 1, 1, 2);
			left_table.Attach (album_label, 0, 1, 2, 3);
			left_table.Attach (genre_label, 0, 1, 3, 4);
			left_table.Attach (comment_label, 0, 1, 4, 5);
			
			left_table.Attach (artist_entry, 1, 2, 0, 1);
			left_table.Attach (title_entry, 1, 2, 1, 2);
			left_table.Attach (album_entry, 1, 2, 2, 3);
			left_table.Attach (genre_combo, 1, 2, 3, 4);
			left_table.Attach (comment_scroll, 1, 2, 4, 5);
			
			
			// right table attachments
			right_table.Attach (year_label, 0, 1, 0, 1);
			right_table.Attach (tracknumber_label, 0, 1, 1, 2);
			right_table.Attach (trackcount_label, 0, 1, 2, 3);
			
			right_table.Attach (year_spin, 1, 2, 0, 1);
			right_table.Attach (tracknumber_spin, 1, 2, 1, 2);
			right_table.Attach (trackcount_spin, 1, 2, 2, 3);
			
			
			
			// pack into other boxes
			VBox right_box = new VBox (false, 0);
			right_box.PackStart (right_table, false, false, 0);
			right_box.PackStart (picture_label, false, false, 0);
			right_box.PackStart (picture, true, true, 0);
			right_box.PackStart (open_button, false, false, 0);
			
			
			HBox table_box = new HBox (false, 5);
			table_box.PackStart (left_table, true, true, 0);
			table_box.PackStart (right_box, true, true, 0);
			
			
			// button box
			HBox button_box = new HBox (true, 5);
			button_box.PackStart (back_button, true, true, 0);
			button_box.PackStart (close_button, true, true, 0);
			button_box.PackStart (save_button, true, true, 0);
			button_box.PackStart (forward_button, true, true, 0);
			
			
			
			// button events
			close_button.Clicked += close_clicked;
			save_button.Clicked += save_clicked;
			back_button.Clicked += back_clicked;
			forward_button.Clicked += forward_clicked;
			open_button.Clicked += open_clicked;
			
			
			
			loadMedia ();
			
			
			this.Resizable = false;
			this.SkipPagerHint = true;
			this.SkipTaskbarHint = true;
			
			backbone.BorderWidth = 10;
			backbone.PackStart (path_box, false, true, 0);
			backbone.PackStart (table_box, true, true, 0);
			backbone.PackStart (button_box, false, true, 0);
			this.Add (backbone);
		}
		
		
		
		
		// loads all the media information
		void loadMedia ()
		{
			path_entry.Text = media.Path;
			artist_entry.Text = media.Artist;
			title_entry.Text = media.Title;
			album_entry.Text = media.Album;
			comment_textview.Buffer.Text = media.Comment;
			
			year_spin.Value = media.Year;
			tracknumber_spin.Value = media.TrackNumber;
			trackcount_spin.Value = media.TrackCount;
			
			pic_uri = null;
			picture.Clear ();
			picture.Pixbuf = Utils.LoadCoverArt (media.Picture);
		}
		
		
		
		
		
		
		// the user clicked the close button
		void close_clicked (object o, EventArgs args)
		{
			this.Destroy ();
			retVal = 0;
		}
		
		
		
		
		// the user clicked the save button
		void save_clicked (object o, EventArgs args)
		{
			media.Artist = artist_entry.Text;
			media.Title = title_entry.Text;
			media.Album = album_entry.Text;
			media.Comment = comment_textview.Buffer.Text;
			
			media.Year = (int) year_spin.Value;
			media.TrackNumber = (int) tracknumber_spin.Value;
			media.TrackCount = (int) trackcount_spin.Value;
			
			media.SetPicture (pic_uri);
			media.SaveTag ();
			
			Global.Core.Library.MediaTree.QueueDraw ();
		}
		
		
		
		
		// the user clicked the back button
		void back_clicked (object o, EventArgs args)
		{
			if (current_path.Prev ())
			{
				TreeIter iter;
				if (Global.Core.Library.MediaTree.Model.GetIter (out iter, current_path))
				{
					media = (FileMedia) Global.Core.Library.MediaTree.Model.GetValue (iter, 0);
					
					Global.Core.Library.MediaTree.Selection.SelectPath (current_path);
					loadMedia ();
				}
			}
		}
		
		
		// the user clicked the forward button
		void forward_clicked (object o, EventArgs args)
		{
			TreeIter iter;
			if (Global.Core.Library.MediaTree.Model.GetIter (out iter, current_path))
			{
				if (Global.Core.Library.MediaTree.Model.IterNext (ref iter))
				{
					media = (FileMedia) Global.Core.Library.MediaTree.Model.GetValue (iter, 0);
					
					current_path.Next ();
					Global.Core.Library.MediaTree.Selection.SelectPath (current_path);
					loadMedia ();
				}
			}
		}
		
		
		
		
		// the user clicked the open button
		void open_clicked (object o, EventArgs args)
		{
			string file = Dialogs.ChooseFile ("Select Image");
			if (file != null)
			{
				pic_uri = file;
				picture.Clear ();
				picture.Pixbuf = new Gdk.Pixbuf (pic_uri, 85, 85, false);
			}
		}
		
		
	}
}
