﻿using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MVC.WPF;
using Translator;
namespace MASGAU {
    public class GameListViewItem : ModelListViewItem {

        public GameEntry GameDataSource {
            set {
                base.DataSource = value;
            }
            get {
                return base.DataSource as GameEntry;
            }
        }



        protected Grid gameGrid = new Grid();
        protected Label gameTitle = new Label();
        protected Label gameVersion = new Label();

        protected CheckBox monitorCheck = new CheckBox();

        protected ColumnDefinition monitorColumn = new ColumnDefinition();

        public GameListViewItem() {
            gameTitle.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            monitorCheck.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            monitorCheck.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            gameVersion.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            gameVersion.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

            gameVersion.Padding = new System.Windows.Thickness(0);
            gameTitle.Padding = new System.Windows.Thickness(0);


            gameGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            gameGrid.Margin = new System.Windows.Thickness(0);

            gameTitle.Margin = new System.Windows.Thickness(0);
            gameVersion.Margin = new System.Windows.Thickness(0);

            ColumnDefinition col = new ColumnDefinition();
            col.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
            gameGrid.ColumnDefinitions.Add(col);

            col = new ColumnDefinition();
            col.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
            gameGrid.ColumnDefinitions.Add(col);

            monitorColumn.Width = new System.Windows.GridLength(50, System.Windows.GridUnitType.Pixel);
            gameGrid.ColumnDefinitions.Add(monitorColumn);


            RowDefinition row = new RowDefinition();
            gameGrid.RowDefinitions.Add(row);
            row = new RowDefinition();
            gameGrid.RowDefinitions.Add(row);

            Grid.SetColumn(gameTitle, 0);
            Grid.SetRow(gameTitle, 0);
            Grid.SetColumn(gameVersion, 1);
            Grid.SetRow(gameVersion, 0);
            Grid.SetColumn(monitorCheck, 2);
            Grid.SetRow(monitorCheck, 0);


            gameGrid.Children.Add(gameTitle);
            gameGrid.Children.Add(gameVersion);
            gameGrid.Children.Add(monitorCheck);


            Content = gameGrid;
        }

        public override void LoadSourceData() {
            base.LoadSourceData();

            Binding bind = new Binding("IsMonitored");
            bind.Source = DataSource;
            bind.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(monitorCheck, CheckBox.IsCheckedProperty, bind);

            bind = new Binding("CanBeMonitored");
            bind.Source = DataSource;
            bind.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(monitorCheck, CheckBox.IsEnabledProperty, bind);

        }

        void monitorCheck_Click(object sender, System.Windows.RoutedEventArgs e) {
        }


        protected override System.Windows.Media.Brush BackgroundColor {
            get {

                return new SolidColorBrush(convertColor(GameDataSource.BackgroundColor));
            }
        }

        protected override Brush SelectedBackgroundColor {
            get {
                return new SolidColorBrush(convertColor(GameDataSource.SelectedColor));
            }
        }

        protected override void changeForegroundColor(Brush brush) {
            gameTitle.Foreground = brush;
            gameVersion.Foreground = brush;
        }


        protected string processVersionIdentifier(string id) {
            if (String.IsNullOrEmpty(id))
                return "";

            return Strings.GetLabelString(id) + " ";
        }
        protected override void LoadContent() {
            gameTitle.Content = GameDataSource.Title;

            StringBuilder version = new StringBuilder();

            version.Append(processVersionIdentifier(GameDataSource.id.game.Release));
            version.Append(processVersionIdentifier(GameDataSource.id.OS));
            version.Append(processVersionIdentifier(GameDataSource.id.game.Platform));
            version.Append(processVersionIdentifier(GameDataSource.id.game.Media));
            version.Append(processVersionIdentifier(GameDataSource.id.game.Region));
            version.Append(processVersionIdentifier(GameDataSource.id.game.Type));

            gameVersion.Content = version.ToString();

        }

        public override ModelListViewItem CreateItem() {
            return new GameListViewItem();
        }
    }
}
