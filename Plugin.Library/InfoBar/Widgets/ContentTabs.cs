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
using System.Collections.Generic;
using Gtk;

namespace Fuse.Plugin.Library.Info
{
	
	/// <summary>
	/// The content tabs.
	/// </summary>
	public class ContentTabs
	{
		
		private Content active_content;
		private List <Content> content_list = new List <Content> ();
		
		
		//global widgets
		private VBox box = new VBox (false, 0);
		private Notebook tabs = new Notebook ();
		private ContentButtonBox button_box = new ContentButtonBox ();
		
		
		public delegate void ContentChangedHandler (Content content);
		public event ContentChangedHandler ContentChanged;
		
		
		public ContentTabs ()
		{
			tabs.ShowTabs = false;
			tabs.ShowBorder = false;
			
			box.PackStart (button_box.DisplayWidget, false, false, 6);
			box.PackStart (tabs, true, true, 0);
			
			
			tabs.SwitchPage += change_content;
			button_box.ButtonToggled += button_toggled;
		}
		
		
		
		/// <summary>
		/// Add content without a tab.
		/// </summary>
		public void AddContent (Content content)
		{
			content_list.Add (content);
			tabs.AppendPage (content.DisplayWidget, null);
			content.NewSize += content_resize;
		}
		
		
		/// <summary>
		/// Add content with a tab.
		/// </summary>
		public void AddContent (Content content, string title)
		{
			AddContent (content);
			button_box.AddContent (title, content);
		}
		
		
		
		/// <summary>
		/// Load the specified content info.
		/// </summary>
		public void LoadContent (Content content, QueryInfo query)
		{
			string url = content.GetQuery (query);
			if (url == null)
				return;
			
			content.ShowLoading ();
			
			XmlDocument doc = content.LoadXml (url);
			if (doc != null)
			{
				content.Load (doc, query);
				content.HideLoading ();
			}
		}
		
		
		
		/// <summary>
		/// Load the specified content info.
		/// </summary>
		public void LoadTab (Content content, QueryInfo query)
		{
			tabs.Page = tabs.PageNum (content.DisplayWidget);
			LoadContent (active_content, query);
		}
		
		
		/// <summary>
		/// Load the specified content info.
		/// </summary>
		public void LoadTab (QueryInfo query, Type content_type)
		{
			foreach (Content content in content_list)
				if (content.GetType() == content_type)
					LoadTab (content, query);
		}
		
		
		/// <summary>
		/// Load the specified content info.
		/// </summary>
		public void LoadTab (QueryInfo query)
		{
			LoadTab (active_content, query);
		}
		
		
		
		/// <summary>
		/// Resizes the tab widget.
		/// </summary>
		public void Resize ()
		{
			content_resize ();
		}
		
		
		
		/// <summary>The widget to display.</summary>
		public Widget DisplayWidget
		{ get{ return box; } }
		
		
		/// <summary>The displayed content.</summary>
		public Content ActiveContent
		{ get{ return active_content; } }
		
		
		/// <summary>The content list.</summary>
		public List <Content> ContentList
		{ get{ return content_list; } }
		
		
		
		//switch pages
		private void change_content (object o, SwitchPageArgs args)
		{
			//look for the content currently showing
			active_content = null;
			Widget active_widget = tabs.GetNthPage ((int) args.PageNum);
			
			foreach (Content content in content_list)
				if (content.DisplayWidget == active_widget)
					active_content = content;
			
			if (active_content == null)
				return;
			
			content_resize ();
		}
		
		
		
		//the content widget has a new size
		private void content_resize ()
		{
			Requisition size = active_content.WidgetSize;
			tabs.SetSizeRequest (size.Width, size.Height);
		}
		
		
		//a content button was pressed
		private void button_toggled (Content content)
		{
			if (ContentChanged != null)
				ContentChanged (content);
		}
		
	}
}
