using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Logger
{
    public class Logger
    {

        private static StreamWriter log_writer;


        static Logger()
        {
            log_writer = new StreamWriter("log.txt", true, Encoding.UTF8);
        }

        public static void log(String line)
        {
            StringBuilder output = new StringBuilder(DateTime.Now.ToString());
            output.Append(" - ");
            output.Append(line);

            log_writer.WriteLine(output.ToString());
        }

    }
}
