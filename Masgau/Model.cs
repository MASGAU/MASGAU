using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Specialized;
using MASGAU.Communication.Message;

namespace MASGAU
{

    public class Model<T>: Model<StringID,T> where T: AModelItem<StringID> {
    }

    public class Model<I,T>: ObservableCollection<T>, INotifyPropertyChanged where T: AModelItem<I> where I: AIdentifier
    {
        public Boolean containsId(I id) {
            foreach(T item in this) {
                if(item.id.Equals(id))
                    return true;
            }
            return false;
        }

        public T get(I id) {
            foreach(T item in this) {
                if(item.id.Equals(id))
                    return item;
            }
            return null;
        }

        protected bool _cancelling = false;
        public void cancel() {_cancelling = true;}

        protected override event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string prop)
        {
           if( PropertyChanged != null )
           {
              PropertyChanged(this, new PropertyChangedEventArgs(prop));
           }
        }

        public void AddWithSort(T add_me) {
            if(this.Count==0) {
                this.InsertItem(0,add_me);
            } else {
                for(int i = 0; i < this.Count;i++ ) {
                    T compare = this[i];
                    if(compare.CompareTo(add_me)>0) {
                        this.InsertItem(i,add_me);
                        return;
                    }
                }
                this.InsertItem(this.Count,add_me);
            }
        }


        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    this.NotifyPropertyChanged("IsEnabled");
                }

            }
        }

        public Model() {
        }

        public void refresh() {
            NotifyCollectionChangedEventArgs e = 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
           try {
                this.OnCollectionChanged(e);
           } catch (Exception ex) {
               MessageHandler.SendError("Error While Refreshing List","You heard me",ex);
           }
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var eh = CollectionChanged;
            if (eh != null)
            {
                Dispatcher dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
                        let dpo = nh.Target as DispatcherObject
                        where dpo != null
                        select dpo.Dispatcher).FirstOrDefault();

            if (dispatcher != null && dispatcher.CheckAccess() == false)
            {
                dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
            }
            else
            {
                foreach (NotifyCollectionChangedEventHandler nh in eh.GetInvocationList())
                    nh.Invoke(this, e);
            }
            }
        }


        public List<T> SelectedItems {
            get {
                List<T> return_me = new List<T>();
                foreach(T item in this) {
                    if(item.IsSelected)
                        return_me.Add(item);
                }
                return return_me;
            }
        }

    }
}
