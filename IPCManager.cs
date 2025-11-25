using System;
using System.Collections.Generic;

namespace CosmosKernel1
{
    public static class IPCManager
    {
        private static Queue<string> messageQueue = new Queue<string>();

        public static void SendMessage(string message)
        {
            messageQueue.Enqueue(message);
            Console.WriteLine($"[IPC] Message sent: {message}");
        }

        public static string ReceiveMessage()
        {
            if (messageQueue.Count > 0)
            {
                string message = messageQueue.Dequeue();
                Console.WriteLine($"[IPC] Message received: {message}");
                return message;
            }
            return "[IPC] No messages in queue.";
        }
    }
}