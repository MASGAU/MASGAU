using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Translations;
namespace MASGAU.Communication.Progress
{
    #region Progress Definitions
    public delegate void ProgressChangedEventHandler(ProgressUpdatedEventArgs e);
    #endregion

    public class ProgressHandler: CommunicationHandler
    {
        private static string _message = null;
        public static string message {
            get { 
                return _message;
            }
            protected set {
                _message = value;
                setProgress();
            }
        }

        public static void setTranslatedMessage(string name, params string[] variables)
        {
            message = Strings.get(name, variables);
        }


        private static int _value = 0;
        public static int value {
            set{
                _value = value;
                _state = ProgressState.Normal;
                setProgress();
            }
            get {
                return _value;
            }
        }
        private static int _max = 0; 
        public static int max {
            set{
                _max = value;
                setProgress();
            }
            get {
                return _max;
            }
        }
        private static ProgressState _state = ProgressState.Normal;
        public static ProgressState state {
            set{
                _state = value;
                setProgress();
            }
            get {
                return _state;
            }
        }
        private static int _sub_value = 0;
        public static int sub_value {
            set{
                _sub_value = value;
                setSubProgress();
            }
            get {
                return _sub_value;
            }
        }
        private static int _sub_max = 0;
        public static int sub_max {
            set{
                _sub_max = value;
                setSubProgress();
            }
            get {
                return _sub_max;
            }
        }
        private static ProgressState _sub_state = ProgressState.Normal;
        public static ProgressState sub_state {
            set{
                _sub_state = value;
                setSubProgress();
            }
            get {
                return _sub_state;
            }
        }


        //protected static event ProgressChangedEventHandler ProgressChanged;
        //protected static event ProgressChangedEventHandler SubProgressChanged;

        private static void setProgress() {
            setProgress(_value,_max,_message,_state);
        }

        private static void setProgress(int value, int max, string message, ProgressState progstate) {
            ProgressUpdatedEventArgs e = new ProgressUpdatedEventArgs();
            e.max = max;
            e.message = message;
            e.value = value;
            e.state = progstate;
            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return;

            if(receiver.context!=null) {
                receiver.context.Post(new SendOrPostCallback(delegate(object state) {
                    ProgressChangedEventHandler handler = receiver.updateProgress;
                    if(handler!=null) {
                        handler(e);
                    }
                }),null);
            } else {
                receiver.updateProgress(e);
            }
        }

        private static void setSubProgress() {
            setSubProgress(_sub_value,_sub_max,_sub_state);
        }

        private static void setSubProgress(int value, int max, ProgressState progstate) {
            ProgressUpdatedEventArgs e = new ProgressUpdatedEventArgs();
            e.max = max;
            e.message = null;
            e.value = value;
            e.state = progstate;

            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return;

            if(receiver.context!=null) {
                receiver.context.Post(new SendOrPostCallback(delegate(object state) {
                    ProgressChangedEventHandler handler = receiver.updateProgress;
                    if(handler!=null) {
                        handler(e);
                    }
                }),null);
            } else {
                receiver.updateProgress(e);
            }
        }
    }
}
