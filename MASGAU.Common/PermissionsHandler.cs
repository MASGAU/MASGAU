using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MASGAU
{
    public class PermissionsHandler
    {
        public static bool isReadable(string path) {
            try {
                DirectoryInfo read_me = new DirectoryInfo(path);
                if(read_me.Exists) {
                    FileInfo[] infos = read_me.GetFiles();
                    if(infos.Length>0) {
                        FileStream stream = infos[0].Open(FileMode.Open, FileAccess.Read);
                        stream.Close();
                    } 
                    return true;
                } else
                    return false;
            } catch {
                return false;
            }
        }
        public static bool isWritable(string path) {
            try {
                if (Directory.Exists(path)) {
                    string file_name = Path.GetRandomFileName();
                    FileInfo test_file = new FileInfo(Path.Combine(path,file_name));
                    FileStream delete_me = test_file.Create();
                    delete_me.Close();
                    test_file.Delete();
                    return true;
                } else 
                    return false;
            } catch {
                return false;
            }
        }

    }
}
