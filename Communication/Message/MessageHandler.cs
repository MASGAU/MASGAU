using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Communication.Progress;
namespace Communication.Message
{
    public delegate void MessageEventHandler(MessageEventArgs e);

    public class MessageHandler: CommunicationHandler
    {
        //protected static event MessageEventHandler MessageSent;   

        public static Boolean suppress_messages = false;

        public static ResponseType SendException(Exception e) {
            if(e.GetType()==typeof(CommunicatableException)) {
                CommunicatableException ex = e as CommunicatableException;
                string message = ex.Message;

                return SendMessage(ex.title, message, MessageTypes.Error, ex);
            } else {
                return SendMessage(e.GetType().ToString(), e.Message, MessageTypes.Error, e);
            }

        }

        protected static ResponseType SendError(string name, string title)
        {
            return SendError(name, title, null as Exception);
        }
        protected static ResponseType SendError(string name, string title, Exception e)
        {
            ProgressHandler.state = ProgressState.Error;
            return SendMessage(name, title, MessageTypes.Error, e);
        }
        protected static ResponseType SendWarning(string name, string title)
        {
            return SendWarning(name, title, null);
        }
        protected static ResponseType SendWarning(string name, string title, Exception e)
        {
            ProgressHandler.state = ProgressState.Error;
            return SendMessage(name, title, MessageTypes.Warning, e);
        }
        protected static ResponseType SendInfo(string name, string title)
        {
            ProgressHandler.state = ProgressState.Wait;
            return SendMessage(name, title, MessageTypes.Info, null);
        }

        protected static ResponseType SendMessage(string title, string message, MessageTypes type, Exception ex)
        {
            MessageEventArgs e = new MessageEventArgs();
            e.type = type;
            if(ex!=null)
                e.exception = ex;
            else
                e.exception = null;

            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return ResponseType.Cancel;

            if(receiver.context!=null) {
                receiver.context.Send(new SendOrPostCallback(delegate(object state) {
                    MessageEventHandler handler = receiver.sendMessage;
                    if(handler!=null) {
                        handler(e);
                    }
                }),null);
            } else {
                receiver.sendMessage(e);
            }

            waitForResponse(e);

            ProgressHandler.state = ProgressState.Normal;
            return e.response;

        }
    }
}
