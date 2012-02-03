using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MASGAU
{
    public class CheckedTreeViewItem: INotifyPropertyChanged {
        protected string _tooltip = null;
        public string ToolTip {
            get {
                return _tooltip;
            }
            set {
                _tooltip = value;
            }
        }


        protected string _name = null;
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
          PropertyChangedEventHandler handler = PropertyChanged;
          if (handler != null)
          {
              handler(this, new PropertyChangedEventArgs(name));
          }
        }

        protected CheckedTreeViewItem _parent;
        public CheckedTreeViewItem Parent {
            get {
                return _parent;
            }
        }

        protected ObservableCollection<CheckedTreeViewItem> _children;
        public ObservableCollection<CheckedTreeViewItem> Children {
            get {
                return _children;
            }
        }

        public CheckedTreeViewItem(CheckedTreeViewItem parent) {
            _parent = parent;
            _children = new ObservableCollection<CheckedTreeViewItem>();
        }

        protected bool _isExpanded = true;
        public bool IsExpanded {
            get { return _isExpanded; }
            set {
                if(value!=_isExpanded) {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }
                if(_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
                _isExpanded = true;
            }
        }

        protected bool? _isChecked = true;
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }
        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            //if (value == _isChecked)
                //return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue) {
                foreach(CheckedTreeViewItem item in _children) {
                    item.SetIsChecked(_isChecked, true, false);
                }
            }

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }


        protected bool _isSelected;
        public bool IsSelected {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        protected CheckedTreeViewItem DummyChild;
        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }
    }
}
