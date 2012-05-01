using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace MASGAU.Location
{
    public class ScummVMLocationHandler: AScummVMLocationHandler
    {

        protected override Dictionary<string, FileInfo> collectConfigFiles()
        {
            Dictionary<string, FileInfo> files = new Dictionary<string,FileInfo>();
            ASystemLocationHandler handler = Core.locations.getHandler(HandlerType.System) as ASystemLocationHandler;

            foreach(UserData user in handler) {
                if(user.hasFolderFor(EnvironmentVariable.AppData)) {
                    FileInfo file = new FileInfo(
                        Path.Combine(user.getFolder(EnvironmentVariable.AppData),
                        Path.Combine("ScummVM","scummvm.ini")));
                    if(file.Exists) {
                        files.Add(user.name,file);
                    }
                }
            }
            return files;
        }
    }
}
