using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace WebSocketsSample
{
    public class ClientsProcessor
    {
        private static ClientsProcessor instance;
        private Thread processor;
        private HashSet<WebSocket> clients;

        private ClientsProcessor() 
        {
            processor = new Thread(DoProcess);
            processor.Start();

            clients = new HashSet<WebSocket>();
        }

        public static ClientsProcessor GetInstance()
        {
            if (instance == null)
            {
                instance = new ClientsProcessor();
            }

            return instance;
        }

        private static async void Log(string log) 
        {
            var resStr = String.Concat(DateTime.Now.ToString("hh:mm:ss"), " - ", log);
            using StreamWriter file = new("WriteLines2.txt", true);
            await file.WriteLineAsync(resStr);
        }

        private void DoProcess()
        {
            while (true)
            {
                SendMessage("hello");
                Thread.Sleep(3000);

                var updatedClients = new HashSet<WebSocket>();
                foreach (var client in clients)
                {
                    var state = client.State;
                    if (state == WebSocketState.Open)
                    {
                        updatedClients.Add(client);
                    }
                }
                clients = updatedClients;
                Log(clients.Count + " clients");
            }
        }

        public void AddClient(WebSocket client)
        {
            clients.Add(client);
        }

        private async void SendMessage(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            foreach (var client in clients)
            {
                await client.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }


    }
}
