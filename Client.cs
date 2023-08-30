using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GameServer
{
    internal class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public string username;
        public TCP tcp;
        public UDP udp;

        // Probably remove
        // public Player player;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private byte[] recieveBuffer;
            private Packet receivedPacket;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket, int[][] maze)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;
                stream = socket.GetStream();

                recieveBuffer = new byte[dataBufferSize];
                receivedPacket = new Packet();

                stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceieveCallback, null);

                // welcome packet
                // Maze sent here
                ServerSend.Welcome(id, "Hello Client");
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if(socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {ex}");
                }
            }

            private bool HandleData(byte[] data)
            {
                int _packetLength = 0;
                receivedPacket.SetBytes(data);
                if (receivedPacket.UnreadLength() >= 4)
                {
                    _packetLength = receivedPacket.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
                while (_packetLength > 0 && _packetLength <= receivedPacket.UnreadLength())
                {
                    byte[] _packetBytes = receivedPacket.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id, _packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedPacket.UnreadLength() >= 4)
                    {
                        _packetLength = receivedPacket.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

            private void ReceieveCallback(IAsyncResult asyncResult)
            {
                try
                {
                    int byteLength = stream.EndRead(asyncResult);
                    if (byteLength <= 0)
                    {
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(recieveBuffer, data, byteLength);

                    //Handle data
                    receivedPacket.Reset(HandleData(data));
                    stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceieveCallback, null);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Error receieving data: {e}");
                }
            }
        }

        public class UDP
        {
            public IPEndPoint endPoint;

            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
            }

            public void SendData(Packet _packet)
            {
                Server.SendUDPData(endPoint, _packet);
            }

            public void HandleData(Packet _packetData)
            {
                int packetLength = _packetData.ReadInt();
                byte[] packetBytes = _packetData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });
            }
        }

        // Change (we dont need to spawn a player)
        public void SendIntoGame(string _playerName)
        {
            //player = new Player(id, _playerName, new Vector3(0, 0, 0));

            //foreach(Client _client in Server.clients.Values)
            //{
            //    if(_client.player != null)
            //    {
            //        ServerSend.SpawnPlayer(id, _client.player);
            //    }
            //}

            //foreach(Client _client in Server.clients.Values)
            //{
            //    if(_client.player != null)
            //    {
            //        ServerSend.SpawnPlayer(_client.id, player);
            //    }
            //}
        }
    }
}
