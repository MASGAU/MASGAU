using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MVC;

namespace MASGAU.Task {
    public class TaskHandler : AModelItem {

        private const string job_args = "-allusers -backup_task";
        private bool schtasks_available = false;

        private Process taskmaster = new Process();

        private Frequency _frequency;
        public Frequency frequency {
            get {
                return _frequency;
            }
            set {
                _frequency = value;
                NotifyPropertyChanged("frequency");
            }
        }

        private DayOfWeek _day_of_week;
        public DayOfWeek day_of_week {
            get {
                return _day_of_week;
            }
            set {
                _day_of_week = value;
                NotifyPropertyChanged("day_of_week");
            }
        }

        private int _day_of_month;
        public int day_of_month {
            get {
                return _day_of_month;
            }
            set {
                _day_of_month = value;
                NotifyPropertyChanged("day_of_month");
            }
        }

        private int _hour;
        public int hour {
            get { return _hour; }
            set {
                _hour = value;
                NotifyPropertyChanged("hour");
            }
        }
        private int _minute;
        public int minute {
            get { return _minute; }
            set {
                _minute = value;
                NotifyPropertyChanged("minute");
            }
        }

        private string task_name;
        public string output;

        private bool _exists = false;
        public bool exists {
            get {
                return _exists;
            }
        }
        private bool _available = false;
        public bool available {
            get {
                return _available;
            }
            set {
                _available = value;
                NotifyPropertyChanged("available");
            }
        }


        public List<DayOfWeek> weekdays {
            get {
                List<DayOfWeek> return_me = new List<DayOfWeek>();
                foreach (DayOfWeek value in Enum.GetValues(typeof(DayOfWeek))) {
                    return_me.Add(value);
                }
                return return_me;
            }
        }

        public List<Frequency> frequencies {
            get {
                List<Frequency> return_me = new List<Frequency>();
                foreach (Frequency value in Enum.GetValues(typeof(Frequency))) {
                    return_me.Add(value);
                }
                return return_me;
            }
        }

        public List<int> days {
            get {
                List<int> return_me = new List<int>();
                for (int i = 1; i < 32; i++) {
                    return_me.Add(i);
                }
                return return_me;
            }
        }
        public List<int> hours {
            get {
                List<int> return_me = new List<int>();
                for (int i = 0; i < 24; i++) {
                    return_me.Add(i);
                }
                return return_me;
            }
        }
        public List<int> minutes {
            get {
                List<int> return_me = new List<int>();
                for (int i = 0; i < 60; i++) {
                    return_me.Add(i);
                }
                return return_me;
            }
        }



        public TaskHandler()
            : base("") {
            task_name = "MASGAU";
            taskmaster.StartInfo.FileName = "schtasks.exe";
            taskmaster.StartInfo.CreateNoWindow = true;
            taskmaster.StartInfo.RedirectStandardOutput = true;
            //            taskmaster.StartInfo.RedirectStandardError = true;
            //            taskmaster.StartInfo.RedirectStandardInput = true;
            taskmaster.StartInfo.UseShellExecute = false;


            if (Core.all_users_mode &&
                File.Exists(Core.programs.backup)) {
                available = true;
            } else {
                return;
            }




            // An important break, for some reason the schtasks takes forever on vista without
            // specifying a specific task to analyze
            if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("LOCALAPPDATA")))
                taskmaster.StartInfo.Arguments = "/Query /TN " + task_name + " /FO LIST /V";
            else
                taskmaster.StartInfo.Arguments = "/Query /FO LIST /V";

            day_of_month = 1;
            hour = 12;
            minute = 0;
            try {
                taskmaster.Start();
                string output = taskmaster.StandardOutput.ReadLine();
                while (output !=null && (!output.StartsWith("TaskName:") || !output.Contains(task_name)))
                    output = taskmaster.StandardOutput.ReadLine();

                output = taskmaster.StandardOutput.ReadLine();

                while (output!=null) {
                    _exists = true;
                    if (output.StartsWith("Schedule Type:") || output.StartsWith("Scheduled Type:")) {
                        if (output.Contains("Daily")) {
                            frequency = Frequency.Daily;
                        } else if (output.Contains("Weekly")) {
                            frequency = Frequency.Weekly;
                        } else if (output.Contains("Monthly")) {
                            frequency = Frequency.Monthly;
                        }
                    } else if (output.StartsWith("Start Time:")) {
                        string current_time = output.Substring(38);
                        hour = Int32.Parse(current_time.Substring(0, 2));
                        minute = Int32.Parse(current_time.Substring(3, 2));
                    } else if (output.StartsWith("Days:")) {
                        switch (frequency) {
                            case Frequency.Weekly:
                                if (output.EndsWith("SUN") || output.EndsWith("SUNDAY"))
                                    day_of_week = DayOfWeek.Sunday;
                                else if (output.EndsWith("MON") || output.EndsWith("MONDAY"))
                                    day_of_week = DayOfWeek.Monday;
                                else if (output.EndsWith("TUE") || output.EndsWith("TUESDAY"))
                                    day_of_week = DayOfWeek.Tuesday;
                                else if (output.EndsWith("WED") || output.EndsWith("WEDNESDAY"))
                                    day_of_week = DayOfWeek.Wednesday;
                                else if (output.EndsWith("THU") || output.EndsWith("THURSDAY"))
                                    day_of_week = DayOfWeek.Thursday;
                                else if (output.EndsWith("FRI") || output.EndsWith("FRIDAY"))
                                    day_of_week = DayOfWeek.Friday;
                                else if (output.EndsWith("SAT") || output.EndsWith("SATURDAY"))
                                    day_of_week = DayOfWeek.Saturday;
                                break;
                            case Frequency.Monthly:
                                day_of_month = int.Parse(output.Substring(38).Trim());
                                break;
                        }
                    } else if (output.StartsWith("TaskName:")) {
                        break;
                    }
                    output = taskmaster.StandardOutput.ReadLine();
                }
                taskmaster.WaitForExit();
                schtasks_available = true;
            } catch {
            }
        }


        public bool createTask(string username, string password) {
            deleteTask();

            StringBuilder arguments = new StringBuilder("/Create ");
            // The user to run the task as
            //                arguments += "/RU \"SYSTEM\" ";
            arguments.Append("/RU " + username + " ");
            arguments.Append("/RP " + password + " ");

            // Sets the task schedule
            arguments.Append("/SC " + frequency.ToString().ToUpper() + " ");

            // Refines the task schedule
            //            arguments += "/MO ";
            switch (frequency) {
                case Frequency.Monthly:
                    arguments.Append("/D " + day_of_month + " ");
                    arguments.Append("/M * ");
                    break;
                case Frequency.Weekly:
                    switch (day_of_week) {
                        case DayOfWeek.Sunday:
                            arguments.Append("/D SUN ");
                            break;
                        case DayOfWeek.Monday:
                            arguments.Append("/D MON ");
                            break;
                        case DayOfWeek.Tuesday:
                            arguments.Append("/D TUE ");
                            break;
                        case DayOfWeek.Wednesday:
                            arguments.Append("/D WED ");
                            break;
                        case DayOfWeek.Thursday:
                            arguments.Append("/D THU ");
                            break;
                        case DayOfWeek.Friday:
                            arguments.Append("/D FRI ");
                            break;
                        case DayOfWeek.Saturday:
                            arguments.Append("/D SAT ");
                            break;
                    }
                    break;
            }

            // Names the task
            arguments.Append("/TN " + task_name + " ");

            // Points to the binary
            arguments.Append("/TR \"\"\"\"" + Core.programs.backup + "\"\"\" " + job_args + "\" ");

            // Sets the run time
            arguments.Append("/ST " + hour.ToString("00") + ":" + minute.ToString("00") + ":00 ");

            // This is for Vista, makes sure it runs with maximum priveleges
            if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("LOCALAPPDATA")))
                arguments.Append("/RL HIGHEST");

            taskmaster.StartInfo.RedirectStandardError = true;
            taskmaster.StartInfo.RedirectStandardOutput = true;
            taskmaster.StartInfo.CreateNoWindow = true;
            taskmaster.StartInfo.Arguments = arguments.ToString();
            taskmaster.StartInfo.UseShellExecute = false;
            //            taskmaster.StartInfo.Verb = "runas";
            taskmaster.Start();
            string error_output = taskmaster.StandardError.ReadToEnd();
            string standard_output = taskmaster.StandardOutput.ReadToEnd();
            taskmaster.WaitForExit();
            if (error_output.Contains("ERROR")) {
                output = error_output;
                return false;
            } else if (standard_output.Contains("WARNING")) {
                output = standard_output;
                deleteTask();
                return false;
            } else {
                _exists = true;
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
            _exists = false;
        }
    }
}
