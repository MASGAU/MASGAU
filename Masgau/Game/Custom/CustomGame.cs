using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace MASGAU {
    public class CustomGame: Game {
        private bool _submitted = false;
        public bool Submitted {
            get {
                return _submitted;
            }
            set {
                if (this.xml == null)
                    this.xml = exportXml();

                if (this.xml.HasAttribute("submitted"))
                    this.xml.Attributes["submitted"].Value = value.ToString();
                else
                    this.addAtribute(xml, "submitted", value.ToString());
                _submitted = value;
            }
        }
        public CustomGame(XmlElement element)
            : base(element) {
        }

        public static string prepareGameName(string title) {
            string name = Regex.Replace(title, @"[^A-Za-z0-9]+", "");
            return name;
        }

        public CustomGame(string title, DirectoryInfo location, string saves, string ignores, XmlDocument doc): base(doc){
            this.Title = title;
            Name =  prepareGameName(title);
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

        protected override GameVersion createVersion(Game parent, XmlElement element) {
            return new CustomGameVersion(parent, element);
        }

        public override XmlElement exportXml() {
            this.xml = base.exportXml();
            if (this.xml.HasAttribute("submitted"))
                this.xml.Attributes["submitted"].Value = _submitted.ToString();
            else
                this.addAtribute(xml, "submitted", _submitted.ToString());
            return this.xml;
        }
    }
}
