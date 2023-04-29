using PatternsUI.MVVM.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PatternsUI.MVVM
{
    /// <summary>
    /// This class is responsible for message passing. Objects register for message types, and other objects use this to send
    /// messages to objects that have signed up to hear those messages. This class could also be called "The Post Office"
    /// </summary>
    public class Messenger
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<Guid, MessageRecipient>> _listeners = new();
        
        /// <summary>
        /// Sends a message to every object that has registered to listen to this type of message
        /// </summary>
        /// <param name="message">The message to send</param>
        public static void Send(IMessage message)
        {
            string messageType = message.GetType().Name;
            ConcurrentDictionary<Guid, MessageRecipient>? listeners = null;

            // See if anyone is listening for this kind of message
            if (_listeners.TryGetValue(messageType, out listeners))
            {
                List<Guid> toRemove = new();
                Action<IMessage>? sendMessage;

                // For everyone who signed up for this message, send it to 'em
                foreach(MessageRecipient listener in listeners.Values) 
                {
                    sendMessage = null;
                    if (listener.Target.TryGetTarget(out sendMessage))
                    {
                        sendMessage(message);
                    }
                    else
                    {
                        toRemove.Add(listener.Id);
                    }
                }

                // If we ran into nulls, clean out our dictionary
                foreach (Guid id in toRemove)
                {
                    ConcurrentDictionary<Guid, MessageRecipient>? listenersToRemove = null;
                    if (_listeners.TryGetValue(messageType, out listenersToRemove))
                    {
                        listenersToRemove.Remove(id, out _);
                    }
                }
            }
        }

        /// <summary>
        /// Registers an object to get notified when messages of this type are sent.
        /// </summary>
        /// <typeparam name="T">The type of message the object is interested in listening to</typeparam>
        /// <param name="owner">The object that is listening (very typically "this")</param>
        /// <param name="onMessageRecieved">The action to perform when the message is sent</param>
        public static void Register<T>(object owner, Action<IMessage> onMessageRecieved) where T : IMessage
        {
            string messageType = typeof(T).Name;

            MessageRecipient recipient = new MessageRecipient(owner, onMessageRecieved);

            ConcurrentDictionary<Guid, MessageRecipient>? currentListeners;
            if (_listeners.TryGetValue(messageType, out currentListeners))
            {
                currentListeners.TryAdd(recipient.Id, recipient);
                _listeners.TryUpdate(messageType, currentListeners, new ConcurrentDictionary<Guid, MessageRecipient>());
            }
            else
            {
                currentListeners = new ConcurrentDictionary<Guid, MessageRecipient>();
                currentListeners.TryAdd(recipient.Id, recipient);
                _listeners.TryAdd(messageType, currentListeners);
            }
            
        }

        /// <summary>
        /// Unregisters the recipient from listening to this type of message
        /// </summary>
        /// <typeparam name="T">The type of message to stop listening to</typeparam>
        /// <param name="owner">The object to stop listening (very typically "this")</param>
        public static void Unregister<T>(object owner) where T : IMessage
        {
            string messageType = typeof(T).Name;
            Unregister(messageType, owner);
        }

        /// <summary>
        /// Unregisters an owner from a particular type of message
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="owner"></param>
        private static void Unregister(string messageType, object owner) 
        {
            ConcurrentDictionary<Guid, MessageRecipient>? currentListeners;
            if (_listeners.TryGetValue(messageType, out currentListeners))
            {
                foreach (MessageRecipient listener in currentListeners.Values)
                {
                    object? recipient;
                    if (listener.Recipient.TryGetTarget(out recipient))
                    {
                        if (ReferenceEquals(owner, recipient))
                        {
                            currentListeners.Remove(listener.Id, out _);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters the owner from any messages it registered for
        /// </summary>
        /// <param name="owner"></param>
        public static void UnregisterAll(object owner) 
        {
            foreach (string messageType in _listeners.Keys) 
            {
                Unregister(messageType, owner);
            }
        }
    }
}
