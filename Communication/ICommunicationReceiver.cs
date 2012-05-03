using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Communication.Message;
using Communication.Progress;
using Communication.Request;

namespace Communication
{
    public interface ICommunicationReceiver
    {
        SynchronizationContext context {get;}
        bool available { get; }
        void sendMessage(MessageEventArgs e);
        void requestInformation(RequestEventArgs e);
        void updateProgress(ProgressUpdatedEventArgs e);
        void disableInterface();
        void enableInterface();
        void hideInterface();
        void showInterface();
        void closeInterface();
    }
}
