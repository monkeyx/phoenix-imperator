﻿//
// EntityPageFactory.cs
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

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;

namespace PhoenixImperator.Pages.Entities
{
	/// <summary>
	/// Entity page builder factory.
	/// </summary>
	public static class EntityPageBuilderFactory
	{
		/// <summary>
		/// Shows the entity page.
		/// </summary>
		/// <param name="manager">Manager.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="tabIndex">Tab index.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ShowEntityPage<T>(NexusManager<T> manager, int id, int tabIndex = 0) where T :   EntityBase, new()
		{
			IEntityPageBuilder<T> builder = EntityPageBuilderFactory.CreateBuilder<T>(manager);
			manager.Get(id,(item) => {
				TabbedPage page = builder.BuildPage((T) item);
				if(tabIndex > 0){
					page.CurrentPage = page.Children[tabIndex];
				}
				Device.BeginInvokeOnMainThread (() => {
					RootPage.Root.NextPage (page);
				});
			});
		}

		private static IEntityPageBuilder<T> CreateBuilder<T>(NexusManager<T> manager) where T :   EntityBase, new()
		{
			IEntityPageBuilder<T> builder;
			switch (typeof(T).Name) {
			case "Item":
				builder = (IEntityPageBuilder<T>)new ItemPageBuilder ();
				break;
			case "OrderType":
				builder = (IEntityPageBuilder<T>)new OrderTypePageBuilder ();
				break;
			case "Position":
				builder = (IEntityPageBuilder<T>)new PositionPageBuilder ();
				break;
			case "StarSystem":
				builder = (IEntityPageBuilder<T>)new StarSystemPageBuilder ();
				break;
			case "Notification":
				builder = (IEntityPageBuilder<T>)new NotificationPageBuilder ();
				break;
			case "MarketBase":
				builder = (IEntityPageBuilder<T>)new MarketBasePageBuilder ();
				break;
			default:
				throw new Exception ("Unsupported type"); 
			}
			builder.Manager = manager;
			return builder;
		}
	}
}

