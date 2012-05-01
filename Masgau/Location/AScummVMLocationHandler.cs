using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MASGAU.Location.Holders;

namespace MASGAU.Location
{

    public abstract class AScummVMLocationHandler: ALocationHandler
    {
        public override bool ready
        {
            get { return config_files!=null&&config_files.Count>0; }
        }


        private Dictionary<String,FileInfo> config_files;

        protected abstract Dictionary<String,FileInfo> collectConfigFiles();

        public AScummVMLocationHandler(): base(HandlerType.ScummVM) {
        }


        protected override List<DetectedLocationPathHolder> getPaths(LocationScummVMHolder get_me)
        {
            config_files = collectConfigFiles();
            foreach(String user in config_files.Keys) {
                IniFileHandler ini = new IniFileHandler(config_files[user]);

            }

            List<DetectedLocationPathHolder> return_me = new List<DetectedLocationPathHolder>();
            return return_me;
        }



    }
}
