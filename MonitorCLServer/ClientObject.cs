﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static MonitorCLServer.ServerObject;
using System.Diagnostics;

namespace MonitorCLServer
{
    public class ClientObject
    {
        public NetworkStream Stream { get; private set; }
        public TcpClient client;
        ServerObject server; // объект сервера
        ReceiveDelegate receiveOut;
        public string login = "";

        public string Id { get; protected set; }

        public void setId(string id)
        {
            Id = id;
        }

        public void setReceiveOut(ReceiveDelegate receive)
        {
            receiveOut = receive;
        }

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
            Stream = client.GetStream();
            serverObject.AddConnection(this);
        }

        // чтение входящего сообщения и преобразование в строку
        public string GetMessage()
        {
            byte[] data = new byte[256]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                if (builder.ToString() == "")
                {
                    continue;
                }
            }
            while (Stream.DataAvailable);

            
            return builder.ToString();
        }

        // закрытие подключения
        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }

        public void Process()
        {
            try
            {
                if (Id == "" || Id == "-1" || Id == null)
                {
                    Id = Guid.NewGuid().ToString();
                    server.BroadcastMessage("<id=" + Id + ">", this);
                }
                else
                {
                    server.BroadcastMessage("<id=OK>", this);
                }

                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        string message = GetMessage();
                        receiveOut(Id + ":" + message);
                    }
                    catch
                    {
                        Debug.WriteLine("Error process");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Debug.WriteLine("выход из Process()");
                Close();
            }
        }
    }
}