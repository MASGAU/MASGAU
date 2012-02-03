using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Masgau
{
    class TaskHandler
    {
        public bool exists = false, schtasks_available = false;
        private Process taskmaster = new Process();
        public string frequency;
        public int day;
        public DateTime the_times;
        private string task_name;
        public string output;

        public TaskHandler() {
            task_name = "MASGAU";
            taskmaster.StartInfo.FileName = "schtasks.exe";
            taskmaster.StartInfo.CreateNoWindow = true;
            taskmaster.StartInfo.RedirectStandardOutput = true;
//            taskmaster.StartInfo.RedirectStandardError = true;
//            taskmaster.StartInfo.RedirectStandardInput = true;
            taskmaster.StartInfo.UseShellExecute = false;

            // An important break, for some reason the schtasks takes forever on vista without
            // specifying a specific task to analyze
            if(System.Environment.GetEnvironmentVariable("LOCALAPPDATA")!=null)
                taskmaster.StartInfo.Arguments = "/Query /TN " + task_name + " /FO LIST /V";
            else 
                taskmaster.StartInfo.Arguments = "/Query /FO LIST /V";

            day = 1;
            the_times = DateTime.Parse("12:00");
            try {
                Console.WriteLine(taskmaster.StartInfo.Arguments);
                taskmaster.Start();
                string output = taskmaster.StandardOutput.ReadLine();
                while(output!=null&&(!output.StartsWith("TaskName:")||!output.Contains(task_name)))
                    output = taskmaster.StandardOutput.ReadLine();

                output = taskmaster.StandardOutput.ReadLine();

                while(output!=null) {
                    exists = true;
                    Console.WriteLine(output);
                    if(output.StartsWith("Schedule Type:")||output.StartsWith("Scheduled Type:")) {
                        if(output.Contains("Daily")) {
                            frequency = "daily";
                        } else if (output.Contains("Weekly")) {
                            frequency = "weekly";
                        } else if (output.Contains("Monthly")) {
                            frequency = "monthly";
                        }
                    } else if(output.StartsWith("Start Time:")) {
                        string current_time = output.Substring(38);
                        the_times = DateTime.Parse(current_time);
                    } else if(output.StartsWith("Days:")) {
                        if(frequency=="weekly") {
                            if(output.EndsWith("SUN")||output.EndsWith("SUNDAY"))
                                day = 0;
                            else if (output.EndsWith("MON")||output.EndsWith("MONDAY"))
                                day = 1;
                            else if (output.EndsWith("TUE")||output.EndsWith("TUESDAY"))
                                day = 2;
                            else if (output.EndsWith("WED")||output.EndsWith("WEDNESDAY"))
                                day = 3;
                            else if (output.EndsWith("THU")||output.EndsWith("THURSDAY"))
                                day = 4;
                            else if (output.EndsWith("FRI")||output.EndsWith("FRIDAY"))
                                day = 5;
                            else if (output.EndsWith("SAT")||output.EndsWith("SATURDAY"))
                                day = 6;
                        }
                        else if (frequency == "monthly")
                        {
                            day = int.Parse(output.Substring(38).Trim());
                        }
                    } else if(output.StartsWith("TaskName:")) {
                        break;
                    }
                    output = taskmaster.StandardOutput.ReadLine();
                }
                taskmaster.WaitForExit();
                schtasks_available = true;
            } catch {
                Console.WriteLine("SCHTASKS Not Found");
            } 
        }


        public bool createTask(string username, string password) {
            deleteTask();

            string arguments;
            arguments = "/Create ";
            // The user to run the task as
//                arguments += "/RU \"SYSTEM\" ";
            arguments += "/RU " + username + " ";
            arguments += "/RP " + password + " ";

            // Sets the task schedule
            arguments += "/SC " + frequency.ToUpper() + " ";
            
            // Refines the task schedule
//            arguments += "/MO ";
            if(frequency=="monthly") {
                arguments += "/D " + day + " ";
            }
            if(frequency=="weekly") {
                switch(day) {
                    case 0:
                        arguments += "/D SUN ";
                        break;
                    case 1:
                        arguments += "/D MON ";
                        break;
                    case 2:
                        arguments += "/D TUE ";
                        break;
                    case 3:
                        arguments += "/D WED ";
                        break;
                    case 4:
                        arguments += "/D THU ";
                        break;
                    case 5:
                        arguments += "/D FRI ";
                        break;
                    case 6:
                        arguments += "/D SAT ";
                        break;
                }
            }
            if(frequency=="monthly") {
                arguments += "/M * ";
            }

            // Names the task
            arguments += "/TN " + task_name + " ";

            // Points to the binary
            arguments += "/TR \"\"\"\"" + Application.StartupPath + "\\MasgauTask.exe\"\"\" /allusers\" ";

            // Sets the run time
            arguments += "/ST " + the_times.Hour.ToString("00") + ":" + the_times.Minute.ToString("00") + ":" + the_times.Second.ToString("00") + " ";

            // This is for Vista, makes sure it runs with maximum priveleges
            if(System.Environment.GetEnvironmentVariable("LOCALAPPDATA")!=null)
                arguments += "/RL HIGHEST";

            taskmaster.StartInfo.RedirectStandardError = true;
            taskmaster.StartInfo.RedirectStandardOutput = true;
            taskmaster.StartInfo.CreateNoWindow = true;
            taskmaster.StartInfo.Arguments = arguments;
            taskmaster.StartInfo.UseShellExecute = false;
//            taskmaster.StartInfo.Verb = "runas";
            Console.WriteLine(arguments);
            taskmaster.Start();
            string error_output = taskmaster.StandardError.ReadToEnd();
            string standard_output = taskmaster.StandardOutput.ReadToEnd();
            taskmaster.WaitForExit();
            Console.Write("TOODLENOOB:" + output);
            if(error_output.Contains("ERROR")) {
                output = error_output;
                return false;
            } else if(standard_output.Contains("WARNING")) {
                output = standard_output;
                deleteTask();
                return false;
            } else {
                exists = true;
                return true;
            }
        }

        public void deleteTask() {
            taskmaster.StartInfo.Arguments = "/Delete /TN " + task_name + " /F";
            taskmaster.StartInfo.RedirectStandardError = false;
            taskmaster.StartInfo.RedirectStandardOutput = false;
            taskmaster.StartInfo.CreateNoWindow = true;
            taskmaster.StartInfo.UseShellExecute = false;
//            taskmaster.StartInfo.Verb = "runas";
            taskmaster.Start();
            taskmaster.WaitForExit();
            exists = false;
        }
    }
}
