﻿//
// OrderEditPage.cs
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
using System.Threading.Tasks;

using Xamarin.Forms;

using Phoenix.BL.Entities;
using Phoenix.BL.Managers;

using PhoenixImperator.Pages.Entities;

namespace PhoenixImperator.Pages
{
	/// <summary>
	/// Order edit page.
	/// </summary>
	public class OrderEditPage : PhoenixPage
	{
		/// <summary>
		/// Gets or sets the current order.
		/// </summary>
		/// <value>The current order.</value>
		public Order CurrentOrder { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PhoenixImperator.Pages.OrderEditPage"/> class.
		/// </summary>
		/// <param name="order">Order.</param>
		public OrderEditPage (Order order) : base(order.OrderType.Name)
		{
			CurrentOrder = order;

			formTable = new TableView {
				Root = new TableRoot(),
				Intent = TableIntent.Form,
				BackgroundColor = Color.White,
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Fill
			};

			formTable.Root.Add (new TableSection ());

			Button saveButton = new Button {
				Text = "Save",
				TextColor = Color.White,
				BackgroundColor = Color.Blue
			};
			Button deleteButton = new Button {
				Text = "Delete Order",
				TextColor = Color.White,
				BackgroundColor = Color.Red
			};

			saveButton.Clicked += (sender, e) => {
				deleteButton.IsEnabled = false;
				saveButton.IsEnabled = false;
				SaveOrder();
			};

			deleteButton.Clicked += async(sender, e) => {
				bool confirm = await DisplayAlert("Delete Order","Are you sure?","Yes","No");
				if(confirm){
					deleteButton.IsEnabled = false;
					saveButton.IsEnabled = false;
					DeleteOrder();
				}
					
			};

			ShowParameters ();

			AddSection ("Description");
			LastSection.Add (new ViewCell () {
				View = new StackLayout {
					Children = {
						new Label {
							FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)),
							VerticalOptions = LayoutOptions.Fill,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							Text = CurrentOrder.OrderType.Description,
							BackgroundColor = Color.White,
							TextColor = Color.Black
						}
					}
				}
			});

			PageLayout.Children.Add (deleteButton);
			PageLayout.Children.Add (new ScrollView { Content = formTable });
			PageLayout.Children.Add (saveButton);
		}

		private void SaveOrder()
		{
			Phoenix.Application.OrderManager.SaveOrder(CurrentOrder,(results) => {
				PositionPageBuilder.UpdateOrders(results);
				Device.BeginInvokeOnMainThread(() => {
					RootPage.Root.PreviousPage();
				});
			});
		}

		private void DeleteOrder()
		{
			Phoenix.Application.OrderManager.DeleteOrder(CurrentOrder,(results) => {
				PositionPageBuilder.UpdateOrders(results);
				Device.BeginInvokeOnMainThread(() => {
					RootPage.Root.PreviousPage();
				});
			});
		}

		private void ShowParameters ()
		{
			if (CurrentOrder.OrderType.Parameters.Count < 1) {
				LastSection.Add (new TextCell{
					Text = "No parameters"
				});
				return;
			}
			int i = 0;
			foreach (OrderParameterType pt in CurrentOrder.OrderType.Parameters) {
				OrderParameter param = null;
				if (i < CurrentOrder.Parameters.Count) {
					param = CurrentOrder.Parameters [i];
				}
				AddParameter (pt, param);
				i += 1;
			}
		}

		private TableSection AddSection()
		{
			TableSection section = new TableSection ();
			formTable.Root.Add (section);
			return section;
		}

		private TableSection AddSection(string title)
		{
			TableSection section = new TableSection (title);
			formTable.Root.Add (section);
			return section;
		}

		private TableSection LastSection{
			get {
				return formTable.Root [(formTable.Root.Count - 1)];
			}
		}

		private void AddParameter(OrderParameterType paramType, OrderParameter param = null)
		{
			if (param == null) {
				param = new OrderParameter ();
				switch (paramType.DataType) {
				case OrderType.DataTypes.Integer:
					param.Value = "0";
					break;
				case OrderType.DataTypes.Boolean:
					param.Value = "False";
					break;
				case OrderType.DataTypes.String:
					param.Value = "";
					break;
				default:
					param.Value = "";
					break;
				}
				CurrentOrder.Parameters.Add (param);
			}
			if (paramType.InfoType > 0) {
				AddInfoParam (paramType, param);
				return;
			}
			switch (paramType.DataType) {
			case OrderType.DataTypes.Integer:
				AddIntegerParam (paramType, param);
				break;
			case OrderType.DataTypes.String:
				AddStringParam (paramType, param);
				break;
			case OrderType.DataTypes.Boolean:
				AddBooleanParam (paramType, param);
				break;
			default:
				AddIntegerParam (paramType, param);
				break;
			}
		}

		private void AddIntegerParam(OrderParameterType paramType, OrderParameter param)
		{
			EntryCell entry = new EntryCell {
				Label = paramType.Name,
				Text = param.Value,
				Keyboard = Keyboard.Numeric
			};
			entry.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
				if(e.PropertyName == "Text"){
					param.Value = entry.Text;
				}
			};
			LastSection.Add (entry);
		}

		private void AddStringParam(OrderParameterType paramType, OrderParameter param)
		{
			EntryCell entry = new EntryCell {
				Label = paramType.Name,
				Text = param.Value
			};
			entry.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
				if(e.PropertyName == "Text"){
					param.Value = entry.Text;
				}
			};
			LastSection.Add (entry);
		}

		private void AddBooleanParam(OrderParameterType paramType, OrderParameter param)
		{
			SwitchCell switchCell = new SwitchCell {
				Text = paramType.Name,
				On = param.Value == "True"
			};

			switchCell.OnChanged += (sender, e) => {
				param.Value = switchCell.On ? "True" : "False";
			};
			LastSection.Add (switchCell);
		}

		private void AddInfoParam(OrderParameterType paramType, OrderParameter param)
		{
			if (paramType.InfoType == 0) {
				AddIntegerParam (paramType, param);
				return;
			}

			string displayValue = string.IsNullOrWhiteSpace (param.DisplayValue) ? "Select " + paramType.Name : param.DisplayValue;
			Picker infoPicker = new Picker {
				Title = displayValue,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Silver
			};

			EntryCell entry = new EntryCell {
				Text = param.Value,
				Label = paramType.Name
			};

			if (string.IsNullOrWhiteSpace (param.Value)) {
				entry.Placeholder = paramType.DataType.ToString();
			}

			if (paramType.DataType != OrderType.DataTypes.String) {
				entry.Keyboard = Keyboard.Numeric;
			}

//			entry.Completed += (sender, e) => {
//				InfoEntryTextChanged(entry,paramType,param,infoPicker);
//			};

			entry.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
				if(e.PropertyName == "Text"){
					InfoEntryTextChanged(entry,paramType,param,infoPicker);
				}
			};

			LastSection.Add (entry);
			LastSection.Add (new ViewCell { View = infoPicker });

			if (infoData.ContainsKey (paramType.InfoType)) {
				Task.Factory.StartNew (() => {
					Dictionary<string,InfoData> data = infoData [paramType.InfoType];
					if (data.Count > 0) {
						int i = 0;
						foreach (string value in data.Keys) {
							infoPicker.Items.Add (value.ToString());
							if (param.Value != null && param.Value == data[value].ToString()) {
								Device.BeginInvokeOnMainThread(() => {
									infoPicker.SelectedIndex = i;
								});
							}
							i += 1;
						}
					}
				});
				

			} else {
				Phoenix.Application.InfoManager.GetInfoDataByGroupId (paramType.InfoType, (results) => {
					Dictionary<string,InfoData> data = new Dictionary<string,InfoData>();
					int i = 0;
					foreach(InfoData info in results){
						if(!data.ContainsKey(info.ToString())){
							data.Add(info.ToString(),info);
							Device.BeginInvokeOnMainThread(() => {
								infoPicker.Items.Add(info.ToString());
								if (param.Value != null && param.Value == info.NexusId.ToString()) {
									infoPicker.SelectedIndex = i;
								}
							});
							i += 1;
						}
					}
					infoData.Add(paramType.InfoType,data);
					if(data.Count < 1){
						Device.BeginInvokeOnMainThread(() => {
							infoPicker.IsEnabled = false;
							infoPicker.IsVisible = false;
						});
					}
				});
			}
			infoPicker.Unfocused += (sender, e) => {
				if(infoPicker.SelectedIndex > 0){
					if (infoData.ContainsKey (paramType.InfoType)) {
						Dictionary<string,InfoData> data = infoData [paramType.InfoType];
						string value = infoPicker.Items[infoPicker.SelectedIndex];
						if(data.ContainsKey(value)){
							param.Value = data[value].NexusId.ToString();
							param.DisplayValue = data[value].ToString();
							Device.BeginInvokeOnMainThread(() => {
								entry.Text = data[value].NexusId.ToString();
							});
						}
					}
				}
			};
		}

		private void InfoEntryTextChanged(EntryCell entry, OrderParameterType paramType, OrderParameter param, Picker infoPicker)
		{
			param.Value = entry.Text;
			if (infoData.ContainsKey (paramType.InfoType)) {
				Dictionary<string,InfoData> data = infoData [paramType.InfoType];
				if(data.Count > 0) {
					Task.Factory.StartNew(() => {
						int i = 0;
						foreach(string value in data.Keys){
							if(entry.Text == data[value].NexusId.ToString()){
								if(infoPicker.Items.Count > i){
									Device.BeginInvokeOnMainThread(() => {
										infoPicker.SelectedIndex = i;
										infoPicker.Title = data[value].ToString();
									});
								}
								break;
							}
							i += 1;
						}
					});
				}
				else {
					Device.BeginInvokeOnMainThread(() => {
						infoPicker.Title = param.Value;
					});
				}

			} else {
				Device.BeginInvokeOnMainThread(() => {
					infoPicker.Title = param.Value;
				});
			}
		}

		private TableView formTable;
		private Dictionary<int, Dictionary<string,InfoData>> infoData = new Dictionary<int, Dictionary<string,InfoData>>();
	}
}


