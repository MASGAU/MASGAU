using System;
using MVC;
namespace MASGAU {
    public class QuickHash : AComparable {
        public readonly string Hash;

        public override int GetHashCode() {
            return Hash.GetHashCode();
        }

        public QuickHash(object source) {
            Hash = createHash(source);
        }
        private QuickHash(string source, bool something) {
            this.Hash = source;
        }

        public static QuickHash CreateFromExistingHash(string hash) {
            QuickHash thing = new QuickHash(hash, true);
            return thing;
        }

        public override bool Equals(AComparable to_me) {
            if (to_me == null)
                return false;
            return this.Hash.Equals(to_me.ToString());
        }

        public override int CompareTo(object to_me) {
            if (to_me == null)
                return Hash.CompareTo(null);
            return Hash.CompareTo(to_me.ToString());
        }

        public static string createHash(object source) {
            return String.Format("{0:X}", source.GetHashCode());
        }
        public override string ToString() {
            return Hash;
        }
    }
}
