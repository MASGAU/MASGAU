using System;
using System.IO;
using System.Xml;
using GameSaveInfo;
namespace MASGAU {
    public class CustomGame : GameSaveInfo.Game {
        private bool _submitted = false;
        public bool Submitted {
            get {
                return _submitted;
            }
            set {
                _submitted = value;
                WriteSubmittedValue(this.XML);
            }
        }
        public CustomGame(XmlElement element)
            : base(element) {
        }


        public CustomGame(string title, DirectoryInfo location, string saves, string ignores, XmlDocument doc)
            : base(doc) {
            this.Title = title;
            Name = prepareGameName(title);
            this.Type = GameType.game;
            CustomGameVersion version = new CustomGameVersion(this, location, saves, ignores);
            this.Versions.Add(version);
        }

        protected override void LoadData(XmlElement element) {
            if (element.HasAttribute("submitted")) {
                _submitted = Boolean.Parse(element.Attributes["submitted"].Value);
            }
            base.LoadData(element);
        }

        protected override GameVersion createVersion(GameSaveInfo.Game parent, XmlElement element) {
            return new CustomGameVersion(parent, element);
        }

        protected override XmlElement WriteData(XmlElement element) {
            element = base.WriteData(element);
            WriteSubmittedValue(element);
            return element;
        }


        private void WriteSubmittedValue(XmlElement element) {
            if (element.HasAttribute("submitted")) {
                if (_submitted)
                    element.Attributes["submitted"].Value = "true";
                else
                    element.Attributes["submitted"].Value = "false";
            } else {
                if (_submitted)
                    this.addAtribute(element, "submitted", "true");
                else
                    this.addAtribute(element, "submitted", "false");
            }

        }
    }
}
