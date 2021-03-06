﻿//
// OrderTypePage.cs
//
// Author:
//       Seyed Razavi <monkeyx@gmail.com>
//
// Copyright (c) 2015 Seyed Razavi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;

namespace PhoenixImperator.Pages.Entities
{
	/// <summary>
	/// Order type page builder.
	/// </summary>
	public class OrderTypePageBuilder : BaseEntityPageBuilder<OrderType>
	{
		/// <summary>
		/// Displaies the entity.
		/// </summary>
		/// <param name="item">Item.</param>
		protected override void DisplayEntity(OrderType item)
		{
			AddContentTab ("General", "icon_general.png");
			currentTab.AddPropertyDoubleLine ("Type", item.TypeDescription);
			currentTab.AddPropertyDoubleLine ("Position(s)", item.PositionType);
			currentTab.AddProperty ("TU Cost", item.TUCost.ToString ());

			AddContentTab ("Description", "icon_techmanual.png");
			currentTab.AddLabel (item.Description);

			if (item.Parameters.Count > 0) {
				AddContentTab ("Parameters", "icon_positions.png");
				currentTab.AddListView (typeof(TextCell), item.Parameters, (sender, e) => {
				});
			}
		}
	}
}


