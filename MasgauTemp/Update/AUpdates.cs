using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVC.Communication;
using MVC.Translator;
namespace MASGAU.Update {
    abstract class AUpdates<T>: Dictionary<string,T> where T: AUpdate {

        public void Add(T item) {
            if(this.ContainsKey(item.getName())) {
                T existing = this[item.getName()];

                int result = existing.CompareTo(item);
                if (result == 0) {
                    existing.addURL(item);
                } else if (result < 0) {
                    this[item.getName()] = item;
                }
            } else {
                this.Add(item.getName(),item);
            }
        }

        public bool UpdateAvailable {
            get {
                foreach (T item in this.Values) {
                    if (item.UpdateAvailable)
                        return true;
                }
                return false;
            }
        }

        public bool Update() {
            ProgressHandler.value = 0;
            ProgressHandler.max = this.Count;
            try {
                foreach (T item in this.Values) {
                    ProgressHandler.value++;
                    TranslatingProgressHandler.setTranslatedMessage("UpdatingFile", item.getName());
                    if(item.UpdateAvailable)
                        item.Update();
                }
                return true;
            } catch (Exception e) {
                Logger.Logger.log(e);
                return false;
            }
        }

    }
}
