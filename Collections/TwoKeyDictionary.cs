using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Collections
{
    public class TwoKeyDictionary<K1,K2,V>: Dictionary<K1,Dictionary<K2,V>>
    {
        public V Get(K1 key_one, K2 key_two) {
            if(ContainsKey(key_one,key_two)) {
                return this[key_one][key_two];
            } else {
                throw new KeyNotFoundException("Collection does not contain keys " + key_one + " and " + key_two);
            }
        }

        public void Add(K1 key_one, K2 key_two, V value) {
            if(!this.ContainsKey(key_one)) {
                this.Add(key_one, new Dictionary<K2,V>());
            }
            this[key_one].Add(key_two,value);
        }

        public bool ContainsKey(K1 key_one, K2 key_two) {
            return this.ContainsKey(key_one)&&this[key_one].ContainsKey(key_two);
        }
    }
}
