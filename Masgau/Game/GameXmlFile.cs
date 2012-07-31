using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using XmlData;

namespace MASGAU {
    public class GameXmlFile: AXmlDataFile<Game> {
        public DateTime date;
        public Version Version { get; protected set; }

        public GameXmlFile(FileInfo file): base(file,"programs",true) {
            if (RootNode.HasAttribute("date"))
                date = DateTime.Parse(RootNode.Attributes["date"].Value);
            else
                date = DateTime.Parse("November 5, 1955");


            if (RootNode.HasAttribute("majorVersion") && RootNode.HasAttribute("minorVersion"))
                Version = new Version(Int32.Parse(RootNode.Attributes["majorVersion"].Value), Int32.Parse(RootNode.Attributes["minorVersion"].Value));
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

        protected virtual XmlElement CreatRootNode(string name) {
            XmlElement ele = base.CreatRootNode(name);
            XmlAttribute attr = this.CreateAttribute("majorVersion");
            attr.Value = Core.data_format_version.Major.ToString();
            ele.Attributes.Append(attr);

            attr = this.CreateAttribute("minorVersion");
            attr.Value = Core.data_format_version.Minor.ToString();
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
