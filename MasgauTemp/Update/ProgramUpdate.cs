using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace MASGAU.Update {
    class ProgramUpdate: AUpdate {
        private bool Portable {
            get {
                return Edition == "portable";
            }
        }

        public override bool UpdateAvailable {
            get {
                if (OS != "windows")
                    return false;

                if(Portable!=Core.portable_mode)
                    return false;

                if (Stable != Core.Stable)
                    return false;

                if (Version <= Core.program_version)
                    return false;

                return true;
            }
        }

        public override int CompareTo(AUpdate update) {
            ProgramUpdate prog = update as ProgramUpdate;
            return this.Version.CompareTo(prog.Version);
        }

        public override string getName() {
            if (Stable)
                return OS + Edition + "stable";
            else
                return OS + Edition + "unstable";
        }

        public Version Version { get; protected set; }

        public string Edition { get; protected set; }
        public string OS { get; protected set; }
        public bool Stable { get; protected set; }

        public ProgramUpdate(XmlElement xml): base(xml) {
            this.Date = DateTime.Parse(xml.Attributes["date"].Value);


            this.Version = new Version(Int32.Parse(xml.Attributes["majorVersion"].Value),Int32.Parse(xml.Attributes["minorVersion"].Value),Int32.Parse(xml.Attributes["revision"].Value));

            this.Edition = xml.Attributes["edition"].Value;
            this.OS = xml.Attributes["os"].Value;
            this.Stable = Boolean.Parse(xml.Attributes["stable"].Value);

        }

        public override bool Update() {
            foreach (Uri url in URLs) {
                try {
                    System.Diagnostics.Process.Start(url.ToString());
                        return true;
                } catch (Exception e) {
                    Logger.Logger.log(e);
                    continue;
                }
            }
            return false;
        }

    }
}
