using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Communication.Progress;
using Translations;
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

        public static ResponseType SendError(string name, params string[] variables) {
            return SendError(name, null as Exception, variables);
        }
        public static ResponseType SendError(string name, Exception e, params string[] variables)
        {
            ProgressHandler.state = ProgressState.Error;
            return SendMessage(name, MessageTypes.Error, e);
        }
        public static ResponseType SendWarning(string name, params string[] variables)
        {
            ProgressHandler.state = ProgressState.Error;
            return SendMessage(name, MessageTypes.Warning, null);
        }
        public static ResponseType SendInfo(string name, params string[] variables)
        {
            ProgressHandler.state = ProgressState.Wait;
            return SendMessage(name,MessageTypes.Info,null);
        }

        // The translateable version of all this. OH YEAH!
        public static ResponseType SendMessage(string name, MessageTypes type, Exception ex)
        {
            StringCollection mes = Strings.getTitleMessagePair(name);
            return SendMessage(mes[StringType.Title], mes[StringType.Message], type, ex);
        }
        public static ResponseType SendMessage(string title, string message, MessageTypes type, Exception ex)
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
