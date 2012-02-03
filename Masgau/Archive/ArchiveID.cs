using System;
using System.Text;
using MASGAU.Game;

namespace MASGAU.Archive {
    public class ArchiveID : AIdentifier {
        public readonly String owner;
        public readonly GameID game;
        public readonly String type;

        public ArchiveID(GameID game, String owner, String type) {
            this.game = game;
            this.owner = owner;
            this.type = type;
        }

        public override Boolean Equals(AIdentifier obj) {
            ArchiveID id = obj as ArchiveID;
            return this.owner == id.owner &&
                this.game == id.game &&
                this.type == id.type;
        }

        public override int CompareTo(object obj) {
                ArchiveID id = obj as ArchiveID;
                int result = compare(game, id.game);
                if (result == 0)
                    result = compare(owner, id.owner);
                if (result == 0)
                    result = compare(type, id.type);

                return result;
        }

        public override String ToString() {
            StringBuilder return_me = new StringBuilder(game.ToString());
            if (owner != null)
                return_me.Append(Core.seperator + owner);

            if (type != null)
                return_me.Append(Core.seperator + type);

            return return_me.ToString();
        }

    }
}
