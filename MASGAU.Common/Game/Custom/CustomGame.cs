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
                if (this.XML.HasAttribute("submitted"))
                    this.XML.Attributes["submitted"].Value = value.ToString();
                else
                    this.addAtribute(this.XML, "submitted", value.ToString());
                _submitted = value;
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
            if (element.HasAttribute("submitted"))
                element.Attributes["submitted"].Value = _submitted.ToString();
            else
                this.addAtribute(element, "submitted", _submitted.ToString());
            return element;
        }
    }
}
