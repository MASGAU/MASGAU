using MVC;
namespace MASGAU {
    class RibbonComboItem<T> : AModelItem {

        T item;
        public RibbonComboItem(T item)
            : base(item.ToString()) {
            this.item = item;
        }
    }
}
