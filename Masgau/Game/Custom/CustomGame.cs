using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace MASGAU {
    public class CustomGame: Game {
        public bool Submitted { get; protected set; }
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
            if (element.HasAttribute("submitted")&&Boolean.Parse(element.Attributes["submitted"].Value)) {
                Submitted = true;
            }
            base.LoadData(element);
        }

        protected override GameVersion createVersion(Game parent, XmlElement element) {
            return new CustomGameVersion(parent, element);
        }

        public override XmlElement exportXml() {
            this.xml = base.exportXml();
            if (this.xml.HasAttribute("submitted"))
                this.xml.Attributes["submitted"].Value = Submitted.ToString();
            else
                this.addAtribute(xml, "submitted", Submitted.ToString());
            return this.xml;
        }
    }
}
