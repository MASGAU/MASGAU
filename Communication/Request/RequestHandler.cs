using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace Communication.Request
{

    public delegate void RequestEventHandler(RequestEventArgs e);

    public class RequestHandler: CommunicationHandler
    {
        //protected static event RequestEventHandler RequestSent;

        protected static RequestReply Request(RequestType type) {
            return Request(type,null,null,null);
        }
        protected static RequestReply Request(RequestType type, string title, string message) {
            return Request(type, title, message, null, null);
        }
        protected static RequestReply Request(RequestType type, string title, string message, List<string> choices)
        {
            return Request(type, title, message, choices, null);
        }
        protected static RequestReply Request(RequestType type, string title, string message, List<string> choices, string default_choice)
        {
            RequestReply request = new RequestReply();
            
            if(type== RequestType.Choice&&choices==null)
                throw new CommunicatableException("Request Error", "A choice was requested, but no options provided");


            RequestEventArgs e = new RequestEventArgs(type,title,message,choices,default_choice,request);

            ICommunicationReceiver receiver = getReceiver();

            if(receiver==null) {
                request.cancelled =true;
                return request;
            }

            if(receiver.context!=null) {
                receiver.context.Post(new SendOrPostCallback(delegate(object state) {
                    RequestEventHandler handler = receiver.requestInformation;
                    if(handler!=null) {
                        handler(e);
                    }
                }),null);
            } else {
                receiver.requestInformation(e);
            }
            
            waitForResponse(e);

            if(e.response== ResponseType.Cancel||e.response== ResponseType.No)
                e.result.cancelled = true;

            return e.result;
        }


    }
}
