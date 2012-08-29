using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVC.WPF;
namespace MASGAU {
    public class GameListViewItem: ModelListViewItem {



        public override void LoadSourceData() {
            Content = "CONTENT MUTHAFUCKA";
        }

        public override ModelListViewItem CreateItem() {
            return new GameListViewItem();
        }
    }
}
