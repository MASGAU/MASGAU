using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
namespace MASGAU {
    public class ArchiveListViewItem: GameListViewItem {
        private Label typeLabel = new Label();
        private Label dateLabel = new Label();


        public ArchiveListViewItem() {



            monitorCheck.Visibility = System.Windows.Visibility.Collapsed;
            monitorColumn.Width = new System.Windows.GridLength(0);

            dateLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            dateLabel.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

            typeLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            typeLabel.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;


            dateLabel.Padding = new System.Windows.Thickness(0);
            typeLabel.Padding = new System.Windows.Thickness(0);

            gameVersion.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;

            Grid.SetColumn(typeLabel, 1);
            Grid.SetRow(typeLabel, 0);
            Grid.SetColumn(dateLabel, 1);
            Grid.SetRow(dateLabel, 1);

            Grid.SetColumn(gameVersion, 0);
            Grid.SetRow(gameVersion, 1);

            gameGrid.Children.Add(typeLabel);
            gameGrid.Children.Add(dateLabel);

        }

        public new Archive ArchiveDataSource {
            set {
                this.DataSource = value;
            }
            get {
                return base.DataSource as Archive;
            }
        }


        protected override void changeForegroundColor(Brush brush) {
            base.changeForegroundColor(brush);
            dateLabel.Foreground = brush;
            typeLabel.Foreground = brush;
        }

        protected override void LoadContent() {
            gameTitle.Content = ArchiveDataSource.Title;

            StringBuilder version = new StringBuilder();

            version.Append(processVersionIdentifier(ArchiveDataSource.id.Game.game.Release));
            version.Append(processVersionIdentifier(ArchiveDataSource.id.Game.OS));
            version.Append(processVersionIdentifier(ArchiveDataSource.id.Game.game.Platform));
            version.Append(processVersionIdentifier(ArchiveDataSource.id.Game.game.Media));
            version.Append(processVersionIdentifier(ArchiveDataSource.id.Game.game.Region));
            version.Append(processVersionIdentifier(ArchiveDataSource.id.Game.game.Type));

            gameVersion.Content = version.ToString();

            dateLabel.Content = ArchiveDataSource.LastModified.ToString();
            typeLabel.Content = ArchiveDataSource.Type;
        }

        protected override System.Windows.Media.Brush BackgroundColor {
            get {

                return new SolidColorBrush(convertColor(ArchiveDataSource.BackgroundColor));
            }
        }

        protected override Brush SelectedBackgroundColor {
            get {
                return new SolidColorBrush(convertColor(ArchiveDataSource.SelectedColor));
            }
        }


        public override MVC.WPF.ModelListViewItem CreateItem() {
            return new ArchiveListViewItem();
        }
    }
}
