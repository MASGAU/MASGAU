using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Communication.Interface
{
    delegate void GenericInterfaceEventHandler();

    public class InterfaceHandler: CommunicationHandler
    {
        public static void closeInterface() {
            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return;

            if(receiver.context!=null) {
                receiver.context.Send(new SendOrPostCallback(delegate(object state) {
                    GenericInterfaceEventHandler handler = receiver.closeInterface;
                    if(handler!=null) {
                        handler();
                    }
                }),null);
            } else {
                receiver.closeInterface();
            }
        }
        public static void disableInterface() {
            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return;

            if(receiver.context!=null) {
                receiver.context.Send(new SendOrPostCallback(delegate(object state) {
                    GenericInterfaceEventHandler handler = receiver.disableInterface;
                    if(handler!=null) {
                        handler();
                    }
                }),null);
            } else {
                receiver.disableInterface();
            }
        }
        public static void enableInterface() {
            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return;

            if(receiver.context!=null) {
                receiver.context.Send(new SendOrPostCallback(delegate(object state) {
                    GenericInterfaceEventHandler handler = receiver.enableInterface;
                    if(handler!=null) {
                        handler();
                    }
                }),null);
            } else {
                receiver.enableInterface();
            }

        }
        public static void hideInterface() {
            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return;

            if(receiver.context!=null) {
                receiver.context.Send(new SendOrPostCallback(delegate(object state) {
                    GenericInterfaceEventHandler handler = receiver.hideInterface;
                    if(handler!=null) {
                        handler();
                    }
                }),null);
            } else {
                receiver.hideInterface();
            }

        }
        public static void showInterface() {
            ICommunicationReceiver receiver = getReceiver();
            if(receiver==null)
                return;

            if(receiver.context!=null) {
                receiver.context.Send(new SendOrPostCallback(delegate(object state) {
                    GenericInterfaceEventHandler handler = receiver.showInterface;
                    if(handler!=null) {
                        handler();
                    }
                }),null);
            } else {
                receiver.showInterface();
            }

        }
    }
}
