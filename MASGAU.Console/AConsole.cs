using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using MASGAU.Communication;
using MASGAU.Communication.Progress;
using MASGAU.Communication.Message;
using MASGAU.Communication.Request;
using MASGAU.Backup;

namespace MASGAU.Console {
    public abstract class AConsole<L> : ICommunicationReceiver where L: MASGAU.Location.ALocationsHandler {
        private ABackupProgramHandler<L> task;
        private AConsoleProgramHandler<L> program;

        private bool debug = false;

        public SynchronizationContext context {
            get {
                return null;
            }
        }

        public bool available {
            get {
                return true;
            }
        }

        public bool non_interactive = true;
        private EventLog myLog;

        protected AConsole(string[] args) {
            CommunicationHandler.addReceiver(this);
            if (args.Contains("-help") || args.Contains("-h") || args.Contains("/?")) {
                System.Console.WriteLine("MASGAU whut?");
                System.Console.WriteLine("Available Commands:");
                System.Console.WriteLine("-help -h : Figure it out, genius");
                System.Console.WriteLine("-backup_task : Runs the backup only task");
            } 
            else if (args.Contains("-backup_task")) {
                bool admin_status = SecurityHandler.amAdmin();
                if (!admin_status) {
                    SecurityHandler.elevation(Core.programs.backup, null);
                    return;
                }

                Core.all_users_mode = true;

                if (!EventLog.SourceExists("MASGAU")) {
                    EventLog.CreateEventSource("MASGAU", "Application");
                }

                // Create an EventLog instance and assign its source.
                myLog = new EventLog();
                myLog.Source = "MASGAU";

                task = new ABackupProgramHandler<L>(Interface.Console);
                task.RunWorkerCompleted += new RunWorkerCompletedEventHandler(checkExceptions);
                task.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workCompleted);

                System.Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

                task.RunWorkerAsync();

                while (task.IsBusy)
                    System.Threading.Thread.Sleep(100);

                System.Console.WriteLine("Process Completed");
                if (!non_interactive)
                    System.Console.ReadLine();
            }
            else {
                System.Console.Clear();
                drawLogo();
                program = new AConsoleProgramHandler<L>();
                program.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(program_RunWorkerCompleted);
                program.RunWorkerAsync();
                while (!exit)
                    System.Threading.Thread.Sleep(100);

            }
        }

        void program_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
			
			if(e.Cancelled||e.Error!=null) {
				System.Console.ReadLine();
				return;
			}
            // Default program loop
            while(!exit) {
                this.showMainMenu();
            }
        }


        private bool exit = false;


        public void enableInterface() { }
        public void disableInterface() { }
        public void showInterface() { }
        public void hideInterface() { }
        public void closeInterface() { }
        // ─│┌┐└┘├┤┬┴┼


        private void printXTimes(String print, int times) {
            for(int j=0;j<times;j++)
                System.Console.Write(print);
        }

        private void writeCentered(String write) {
            printXTimes(" ",(width-write.Length)/2);
            System.Console.Write(write);
            printXTimes(" ",(width-write.Length)/2);
            System.Console.WriteLine();
        }

        int width = System.Console.WindowWidth;
        int height = System.Console.WindowHeight;

        private int createMenu(String title, List<String> options) {
            System.Console.CursorVisible = false;
            int current_selection = 0;
            int max_visible = height - logo.Length - 1 - 4;
            int scroll_position = 0;
            while(true) {
                System.Console.SetCursorPosition(0,8);

                printXTimes(" ",(width-title.Length)/2);
                System.Console.WriteLine(title);
                
                int longest_option = 0;
                for(int i = 0;i<options.Count;i++) {
                    if(options[i].Length>longest_option)
                        longest_option = options[i].Length;
                }

                printXTimes(" ",(width-longest_option)/2);
                printXTimes("─",longest_option);
                System.Console.WriteLine();

                for(int i = scroll_position;i<options.Count&&i<max_visible;i++) {
                    printXTimes(" ",(width-longest_option)/2);
                    if(current_selection==i) {
                        System.Console.ForegroundColor = ConsoleColor.Black;
                        System.Console.BackgroundColor = ConsoleColor.Red;
                    }
                    System.Console.Write(options[i]);
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.BackgroundColor = ConsoleColor.Black; ;
                    printXTimes(" ", width - ((width - longest_option) / 2) - options[i].Length-1);
                    System.Console.WriteLine();
                }

                ConsoleKeyInfo key =  System.Console.ReadKey();
                switch(key.Key) {
                    case ConsoleKey.UpArrow:
                        current_selection--;
                        if(current_selection<0)
                            current_selection = options.Count-1;

                        if(current_selection<scroll_position)
                            scroll_position = current_selection;
                        break;
                    case ConsoleKey.DownArrow:
                        current_selection++;
                        if(current_selection>=options.Count)
                            current_selection = 0;
                        if(current_selection>scroll_position+max_visible-1)
                            scroll_position++;
                        break;
                    case ConsoleKey.Enter:
                        return current_selection;
                    case ConsoleKey.Escape:
                        return -1;
                }
            }
        }

        private void showMainMenu() {

            List<String> options = new List<string>();
            options.Add("Backup All " + Core.games.enabled_games_count + " Enabled Games");
            options.Add("Backup Selected Game");
            options.Add("Restore Save Archive");
            options.Add("Exit");
            switch(createMenu("Main Menu",options)) {
                case 1:
                    showGamesMenu();
                    break;
                case 3:
                case -1:
                    exit = true;
                    break;

                default:
                    break;
            }
        }

        private void showGamesMenu() {
            List<String> option = new List<String>();
            foreach(Game.GameHandler game in Core.games) 
                option.Add(game.title);

            createMenu("Select Game(s) To Backup" ,option);
        }

        private string[] logo = new string[] {
        @"___  ___ ___  ___________  ___ ___ ___",
        @"|  \/  |/ _ \/  ___|  __ \/ _ \| | | |",
        @"| .  . / /_\ \ `--.| |  \/ /_\ \ | | |",
        @"| |\/| |  _  |`--. \ | __|  _  | | | |",
        @"| |  | | | | /\__/ / |_\ \ | | | |_| |",
        @"\_|  |_|_| |_|____/ \____|_| |_/\___/ "
        };


        private void clearScreen() {
            System.Console.Clear();
            drawLogo();
        }
        //─│┌┐└┘├┤┬┴┼▌▐▄▀█
        private void drawLogo() {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine();
            foreach(string line in logo ){
                printXTimes(" ", (width - line.Length) / 2);
                System.Console.WriteLine(line);
            }
            System.Console.WriteLine();

            System.Console.ForegroundColor = ConsoleColor.White;
        }

        void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            task.CancelAsync();
            string message = "Cancelling...";
            myLog.WriteEntry(message, EventLogEntryType.Information);
            last_message = message;
            System.Console.WriteLine(message);
            while (task.IsBusy)
                System.Threading.Thread.Sleep(100);

        }


        private void currentProgressChanged(object sender, MASGAU.Communication.Progress.ProgressUpdatedEventArgs e) {
            myLog.WriteEntry("File " + e.value + " of " + e.max, EventLogEntryType.Information);
            System.Console.WriteLine("File " + e.value + " of " + e.max);
        }

        private string last_message = null;


        List<String> progress_messages = new List<string>();
        public void updateProgress(MASGAU.Communication.Progress.ProgressUpdatedEventArgs e) {
            if (e.message != null) {
                string message = e.message;
                if (message.StartsWith("Detecting Games"))
                    message = "Detecting Games...";
                if (message != last_message) {
                    progress_messages.Insert(0,message);
                    if(myLog!=null) 
                        myLog.WriteEntry(message, EventLogEntryType.Information);

                }
                last_message = message;
            }
            System.Console.SetCursorPosition(0,height-4);

            if(progress_messages.Count==0)
                return;


            if(progress_messages.Count>1)
                writeCentered(progress_messages[1]);
            writeCentered(progress_messages[0]);


            System.Console.ForegroundColor = ConsoleColor.White;

            if(e.max==0)
                return;

            System.Console.SetCursorPosition(0,height-2);
            float progress = e.progress_percentage;
            int temp = (int)(progress * width);

            if(temp>0)
                System.Console.BackgroundColor = ConsoleColor.Red;


            System.Console.BackgroundColor = ConsoleColor.Red;
            printXTimes(" ",temp);
            System.Console.BackgroundColor = ConsoleColor.Black;
            printXTimes(" ",width-temp);
        }

        private void workCompleted(object sender, RunWorkerCompletedEventArgs e) {
        }

        private void checkExceptions(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Error != null) {
                MException ex = e.Error as MException;
                string message = ex.Message;
                if (debug) {
                    message = message + Environment.NewLine + Environment.NewLine + e.Error.StackTrace;
                    if (e.Error.InnerException != null) {
                        message = message + Environment.NewLine + Environment.NewLine + e.Error.InnerException.Message;
                    }
                }

                myLog.WriteEntry("Error: " + message, EventLogEntryType.Error);
                System.Console.WriteLine("Error: " + message);
            }
        }

        public void sendMessage(MessageEventArgs e) {
            string message = "";
            EventLogEntryType type = EventLogEntryType.Information;
            switch (e.type) {
                case MessageTypes.Error:
                    message = "Error: " + e.message;
                    type = EventLogEntryType.Error;
                    break;
                case MessageTypes.Info:
                    message = "Info: " + e.message;
                    type = EventLogEntryType.Information;
                    break;
                case MessageTypes.Warning:
                    message = "Warning: " + e.message;
                    type = EventLogEntryType.Warning;
                    break;
            }
			if(myLog!=null)
            	myLog.WriteEntry(message, type);
            System.Console.WriteLine(message);
			if(!non_interactive)
				System.Console.ReadLine();
            e.response = ResponseType.OK;
        }

        public void requestInformation(RequestEventArgs e) {
            if (non_interactive) {
                System.Console.WriteLine("Task needed to know: \n" + e.message + "\nBut since we're non-interactive, we can't help :(");
                myLog.WriteEntry("Task needed to know: \n" + e.message + "\nBut since we're non-interactive, we can't help :(", EventLogEntryType.Error);
                e.response = ResponseType.No;
                e.result.cancelled = true;
            }
            else {
                string response;
                switch (e.info_type) {
                    case RequestType.Choice:
                        System.Console.WriteLine(e.message);
                        int i = 1;
                        foreach (string option in e.options) {
                            System.Console.WriteLine(i + ". " + option);
                            i++;
                        }
                        System.Console.Write("0. Cancel");
                        System.Console.Write("Type Your Selection (0-" + i + "):");
                        response = System.Console.ReadLine();
                        while (true) {
                            try {
                                int choice = Int32.Parse(response);
                                if (choice == 0) {
                                    e.result.cancelled = false;
                                    break;
                                }
                                else if (choice <= i) {
                                    e.result.selected_index = choice - 1;
                                    e.result.selected_option = e.options[choice - 1];
                                }
                            }
                            catch { }
                            System.Console.Write("Invalid Response, Please Enter A Number, 0 Through " + i + ":");
                            response = System.Console.ReadLine();
                        }


                        break;
                    case RequestType.Question:
                        System.Console.Write(e.message + " (Y/N)");
                        response = System.Console.ReadLine();
                        while (true) {
                            if (response == "Y") {
                                e.result.selected_option = "Yes";
                                e.result.selected_index = 1;
                                break;
                            }
                            else if (response == "N") {
                                e.response = ResponseType.No;
                                e.result.cancelled = true;
                                break;
                            }
                            System.Console.Write("Invalid Response, Please Enter Y Or N:");
                            response = System.Console.ReadLine();
                        }
                        break;
                }
            }
        }


    }
}
