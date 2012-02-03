using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Collections
{
    public class DictionaryList<K,V>: Dictionary<K,List<V>>  {


        public DictionaryList() {
        }

        public virtual List<V> Flatten() {
            List<V> flat = new List<V>();
            foreach(K key in this.Keys) {
                foreach (V item in this[key]) {
                    flat.Add(item);
                }
            }
            return flat;
        }

        public int IndexOf(K key, V get_me) {
            if(this.ContainsKey(key)) {
                if (this[key].Contains(get_me))
                    return this[key].IndexOf(get_me);
            }
            return -1;
        }

        public void AddRange(K key,ICollection<V> items) {
            GetList(key).AddRange(items);
        }

        public void Add(K key, V value) {
            GetList(key).Add(value);
        }

        public Boolean Contains(K key, V value) {
            if(this.ContainsKey(key)) {
                if(this[key].Contains(value))
                    return true;
                else
                    return false;
            } else {
                return false;
            }
        }

        public List<V> GetList(K key) {
            if (!this.ContainsKey(key))
                this.Add(key, new List<V>());
            return this[key];
        }

    }
}
