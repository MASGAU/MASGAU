using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using XmlData;

namespace GameSaveInfo {
    public class GameXmlFile: AXmlDataFile<Game> {
        public static Version data_format_version = new Version(2, 0);

        public DateTime date;
        public Version Version { get; protected set; }

        public GameXmlFile(FileInfo file): base(file,true) {
            if (DocumentElement.HasAttribute("date"))
                date = DateTime.Parse(DocumentElement.Attributes["date"].Value);
            else
                date = DateTime.Parse("November 5, 1955");


            if (DocumentElement.HasAttribute("majorVersion") && DocumentElement.HasAttribute("minorVersion"))
                Version = new Version(Int32.Parse(DocumentElement.Attributes["majorVersion"].Value), Int32.Parse(DocumentElement.Attributes["minorVersion"].Value));
        }


        protected override Game CreateDataEntry(System.Xml.XmlElement element) {
            return new Game(element);
        }

        public Game getGame(string name) {
            foreach (Game game in this.entries) {
                if (game.Name == name)
                    return game;
            }
            return null;
        }

        protected override XmlElement CreatRootNode() {
            XmlElement ele = this.CreateElement("programs");

            XmlAttribute attr = this.CreateAttribute("majorVersion");
            attr.Value = data_format_version.Major.ToString();
            ele.Attributes.Append(attr);

            attr = this.CreateAttribute("minorVersion");
            attr.Value = data_format_version.Minor.ToString();
            ele.Attributes.Append(attr);

            attr = this.CreateAttribute("xmlns:xsi");
            attr.Value = @"http://www.w3.org/2001/XMLSchema-instance";
            ele.Attributes.Append(attr);

            attr = this.CreateAttribute("xsi:noNamespaceSchemaLocation");
            attr.Value = "games.xsd";
            ele.Attributes.Append(attr);

            return ele;
        }
    }
}
