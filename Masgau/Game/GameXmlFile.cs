using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XmlData;
namespace MASGAU {

    public class GameXmlFile: AXmlDataFile<IXmlDataEntry> {

        public GameXmlFile(FileInfo file): base(file,"programs") {

        }

        protected override IXmlDataEntry CreateDataEntry() {
            return new Game();
        }
    }
}
