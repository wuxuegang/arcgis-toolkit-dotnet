// /*******************************************************************************
//  * Copyright 2017 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UIKit;

namespace Esri.ArcGISRuntime.Toolkit.UI.Controls
{
    internal class LegendItemCell : UITableViewCell
    {
        private readonly UIStackView _rootStackView;
        private readonly UILabel _textLabel;
        private readonly UITableView _listView;
        private readonly LayerLegend _layerLegend;

        public LegendItemCell(IntPtr handle) : base(handle)
        {           
            SelectionStyle = UITableViewCellSelectionStyle.None;
            TranslatesAutoresizingMaskIntoConstraints = false;

            _rootStackView = new UIStackView()
            {
                Axis = UILayoutConstraintAxis.Vertical,
                Alignment = UIStackViewAlignment.Leading,
                Distribution = UIStackViewDistribution.Fill,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Spacing = 0
            };

            _textLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(20, UIFontWeight.Bold),                
                TextColor = UIColor.Black,
                BackgroundColor = UIColor.Clear,
                ContentMode = UIViewContentMode.Center,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _rootStackView.AddArrangedSubview(_textLabel);

            _listView = new UITableView(UIScreen.MainScreen.Bounds)
            {

                TranslatesAutoresizingMaskIntoConstraints = false,
                AutoresizingMask = UIViewAutoresizing.All,
                RowHeight = UITableView.AutomaticDimension,
                EstimatedRowHeight = 40,
            };
            _listView.RegisterClassForCellReuse(typeof(LegendItemCell), LegendTableSource.CellId);

            _rootStackView.AddArrangedSubview(_listView);

            _layerLegend = new LayerLegend();
            _rootStackView.AddArrangedSubview(_layerLegend);
            ContentView.AddSubview(_rootStackView);
        }

        private void Refresh(LayerContentViewModel layerContent, string propertyName)
        {
            if (propertyName == nameof(LayerContentViewModel.Sublayers))
            {
                var subLayers = layerContent?.Sublayers;
                if (subLayers == null)
                {
                    return;
                }

                var source = new LegendTableSource(new List<LayerContentViewModel>(layerContent?.Sublayers));
                _listView.Source = source;
                _listView.ReloadData();
                source.CollectionChanged += (a, b) => InvokeOnMainThread(() => _listView.ReloadData());
            }
            else if (propertyName == nameof(LayerContentViewModel.DisplayLegend))
            {
                Hidden = (layerContent?.DisplayLegend ?? false) ? false : true;
            }
            else if (propertyName == nameof(LayerContentViewModel.IsSublayer))
            {
                _textLabel.Hidden = (layerContent?.IsSublayer ?? false) ? false : true;
            }
        }

        public void Update(LayerContentViewModel layerContent)
        {
            _textLabel.Text = layerContent?.LayerContent?.Name;
            _layerLegend.LayerContent = layerContent?.LayerContent;
            if (layerContent?.Sublayers != null)
            {
                var source = new LegendTableSource(new List<LayerContentViewModel>(layerContent?.Sublayers));
                _listView.Source = source;
                _listView.ReloadData();
                source.CollectionChanged += (a, b) => InvokeOnMainThread(() => _listView.ReloadData());
            }
            if (layerContent is INotifyPropertyChanged)
            {
                var inpc = layerContent as INotifyPropertyChanged;
                var listener = new Internal.WeakEventListener<INotifyPropertyChanged, object, PropertyChangedEventArgs>(inpc)
                {
                    OnEventAction = (instance, source, eventArgs) =>
                    {
                        Refresh(instance as LayerContentViewModel, eventArgs.PropertyName);
                    },
                    OnDetachAction = (instance, weakEventListener) => instance.PropertyChanged -= weakEventListener.OnEvent
                };
                inpc.PropertyChanged += listener.OnEvent;
            }
        }

        private bool _constraintsUpdated = false;

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();



            _rootStackView.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Horizontal);
            _rootStackView.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Vertical);

            //_textLabel.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Horizontal);
            //_textLabel.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Vertical);

            //_listView.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Horizontal);
            //_listView.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Vertical);

            //_layerLegend.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Horizontal);
            //_layerLegend.SetContentCompressionResistancePriority((float)UILayoutPriority.DefaultHigh, UILayoutConstraintAxis.Vertical);

            if (_constraintsUpdated)
            {
                return;
            }

            _constraintsUpdated = true;
            var margin = ContentView.LayoutMarginsGuide;


            _rootStackView.LeadingAnchor.ConstraintEqualTo(margin.LeadingAnchor).Active = true;
            _rootStackView.TopAnchor.ConstraintEqualTo(margin.TopAnchor).Active = true;
            _rootStackView.TrailingAnchor.ConstraintEqualTo(margin.TrailingAnchor).Active = true;
            _rootStackView.BottomAnchor.ConstraintEqualTo(margin.BottomAnchor).Active = true;

            //_textLabel.LeadingAnchor.ConstraintEqualTo(margin.LeadingAnchor).Active = true;
            //_textLabel.TopAnchor.ConstraintEqualTo(margin.TopAnchor).Active = true;

            //_listView.LeadingAnchor.ConstraintEqualTo(margin.LeadingAnchor).Active = true;
            //_listView.TopAnchor.ConstraintEqualTo(_textLabel.BottomAnchor).Active = true;
            //_listView.WidthAnchor.ConstraintEqualTo(margin.WidthAnchor).Active = true;
            //_listView.HeightAnchor.ConstraintEqualTo(margin.HeightAnchor, (nfloat)0.5).Active = true;

            //_layerLegend.LeadingAnchor.ConstraintEqualTo(margin.LeadingAnchor).Active = true;
            //_layerLegend.WidthAnchor.ConstraintEqualTo(margin.WidthAnchor).Active = true;
            //_layerLegend.TopAnchor.ConstraintEqualTo(_listView.BottomAnchor).Active = true;
            //_layerLegend.HeightAnchor.ConstraintEqualTo(margin.HeightAnchor, (nfloat)0.5).Active = true;
        }
    }
}
