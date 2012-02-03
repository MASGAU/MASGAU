using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MASGAU.Communication.Progress;

namespace MASGAU.Communication.Message
{
    public delegate void MessageEventHandler(MessageEventArgs e);

    public class MessageHandler: CommunicationHandler
    {
        //protected static event MessageEventHandler MessageSent;   

        public static Boolean suppress_messages = false;

        public static ResponseType SendException(Exception e) {
            if(e.GetType()==typeof(MException)) {
                MException ex =e as MException;
                string message = ex.Message;

                return SendError(ex.title,message,ex);
            } else {
                return SendError(e.GetType().ToString(),e.Message, e);
            }

        }

        public static ResponseType SendError(string title, string message, string extended_message) {
            return SendError(title,message + Environment.NewLine + Environment.NewLine + extended_message);
        }
        public static ResponseType SendError(string title, string message) {
            return SendError(title,message,null as Exception);
        }
        public static ResponseType SendError(string title, string message, Exception e) {
            ProgressHandler.progress_state = ProgressState.Error;
            return SendMessage(title,message,MessageTypes.Error,e);
        }
        public static ResponseType SendWarning(string title, string message, string stack_trace) {
            if(stack_trace!=null)
                return SendWarning(title,message + Environment.NewLine + Environment.NewLine + stack_trace);
            else
                return SendWarning(title,message);
        }
        public static ResponseType SendWarning(string title, string message) {
            ProgressHandler.progress_state = ProgressState.Error;
            return SendMessage(title,message,MessageTypes.Warning,null);
        }
        public static ResponseType SendInfo(string title, string message) {
            ProgressHandler.progress_state = ProgressState.Wait;
            return SendMessage(title,message,MessageTypes.Info,null);
        }
        public static ResponseType SendMessage(string title, string message, MessageTypes type, Exception ex) {
            MessageEventArgs e = new MessageEventArgs();
            e.title = title;
            e.message = message;
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

            ProgressHandler.progress_state = ProgressState.Normal;
            return e.response;

        }
    }
}
