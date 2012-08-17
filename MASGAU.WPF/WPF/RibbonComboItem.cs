using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVC;
namespace MASGAU {
    class RibbonComboItem<T>: AModelItem {

        T item;
        public RibbonComboItem(T item): base(item.ToString()) {
            this.item = item;
        }
    }
}
