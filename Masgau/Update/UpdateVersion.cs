using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MASGAU.Update
{
    public class UpdateVersion: IComparable<UpdateVersion>
    {
        public int major {
            get;
            protected set;
        }
        public int minor {
            get;
            protected set;
        }
        public int revision {
            get;
            protected set;
        }

        public UpdateVersion(int major, int minor, int revision) {
            this.major = major;
            this.minor = minor;
            this.revision = revision;
        }

        // Allow the constructors to do their things
        protected UpdateVersion() {}

        public static UpdateVersion getVersionFromXml(XmlElement element) {
            UpdateVersion return_me = new UpdateVersion();
            if(element.HasAttribute("majorVersion"))
                return_me.major = Int32.Parse(element.GetAttribute("majorVersion"));
            else
                throw new MException("Version Read Error","Could not find majorVersion attribute",true);

            if(element.HasAttribute("minorVersion"))
                return_me.minor = Int32.Parse(element.GetAttribute("minorVersion"));
            else
                throw new MException("Version Read Error","Could not find minorVersion attribute", true);

            if(element.HasAttribute("revisionVersion"))
                return_me.revision = Int32.Parse(element.GetAttribute("revisionVersion"));
            else
                throw new MException("Version Read Error","Could not find revisionVersion attribute", true);
            return return_me;
        }


        public int CompareTo(UpdateVersion version) {
            int result = major.CompareTo(version.major);
            
            if(result==0)
                result = minor.CompareTo(version.minor);

            if(result==0)
                result = revision.CompareTo(version.revision);

            return result;
        }

        public bool compatibleWith(UpdateVersion version) {
            return this.major==version.major&&this.minor==version.minor;
        }

        public override string ToString()
        {
            if(major==0&&major==0&&revision==0)
                return "Not Present";
            else
                return major + "." + minor + "." + revision;
        }

       
    }

}
