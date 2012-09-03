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
	
	/// <summary>
	/// A list used for paging.
	/// </summary>
	public class PagedList <T> : List <T>
	{
		
		private int page_number = 0;
		private int show_amount = 0;
		
		
		
		/// <summary>
		/// Returns the items within the current page.
		/// </summary>
		public List <T> CurrentPage
		{
			get
			{
				int index = page_number * show_amount;
				int count = show_amount;
				
				if (this.Count == 0 || index > this.Count-1)
					return new List <T> ();
				
				if (index + count > this.Count-1)
					count = this.Count - index;
				
				return this.GetRange (index, count);
			}
		}
		
		
		
		/// <summary>
		/// How many items to show in a page.
		/// </summary>
		public int AmountToShow
		{
			get{ return show_amount; }
			set{ show_amount = value; }
		}
		
		
		/// <summary>
		/// The current page number.
		/// </summary>
		public int PageNumber
		{
			get{ return page_number+1; }
		}
		
		
		/// <summary>
		/// Total amount of pages.
		/// </summary>
		public int TotalPages
		{
			get
			{
				//get the amount of pages
				if (this.Count > show_amount)
				{
					double item_count = (double) this.Count;
					double show_count = (double) show_amount;
					
					return (int) Math.Ceiling (item_count / show_count);
				}
				else
					return 1;
			}
		}
		
		
		
		
		/// <summary>
		/// Can go to the previous page.
		/// </summary>
		public bool HasPrevious
		{
			get{ return page_number > 0; }
		}
		
		
		/// <summary>
		/// Can go to the next page.
		/// </summary>
		public bool HasNext
		{
			get{ return page_number < TotalPages-1; }
		}
		
		
		
		/// <summary>
		/// Clear all elements.
		/// </summary>
		public new void Clear ()
		{
			page_number = 0;
			base.Clear ();
		}
		
		
		
		/// <summary>
		/// Move to the previous page.
		/// </summary>
		public void PreviousPage ()
		{
			if (HasPrevious)
				page_number--;
		}
		
		
		/// <summary>
		/// Move to the next page.
		/// </summary>
		public void NextPage ()
		{
			if (HasNext)
				page_number++;
		}
		
		
		
	}
}
