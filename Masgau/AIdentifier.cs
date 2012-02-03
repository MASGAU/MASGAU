using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU {
    public abstract class AIdentifier: IEquatable<AIdentifier>, IComparable {
        public abstract bool Equals(AIdentifier to_me);

        public abstract int CompareTo(object id);

        public abstract override string ToString();

        public override bool Equals(object obj)
        {
            if(obj.GetType()==this.GetType())
                return this.Equals(obj as AIdentifier);
            else
                throw new NotSupportedException("Cannot compare type " + this.GetType().ToString() + " to type " + obj.GetType().ToString());
        }

        public static bool operator ==(AIdentifier a, AIdentifier b) {
            return a.Equals(b);
        }

        public static bool operator !=(AIdentifier a, AIdentifier b) {
            return !(a == b);
        }

        protected static int compare(IComparable a, IComparable b) {
            if (a == null) {
                if (b == null)
                    return 0;
                else
                    return -1;
            }
            else {
                return a.CompareTo(b);
            }
        }


    }
}
