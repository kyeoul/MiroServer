﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace GameServer
{
    internal class Server
    {
        public static Random rand = new Random();

        public static int MaxPlayers { get; private set; }

        public static int Port { get; private set; }

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static Player avatar;

        bool started = false;

        public static int[][] maze;

        private static int turn = -1;

        public static int getTurn()
        {
            return turn;
        }

        public static void setTurn(int newTurn)
        {
            turn = newTurn;
        }

        // Add server representation of maze

        // Change / send maze here
        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            initializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on {Port}");

            for(int i = 0; i < maze.Length; i++)
            {
                string line = "";
                for(int j = 0; j < maze[i].Length; j++)
                {
                    line += maze[i][j] + " ";
                }
                Console.WriteLine(line);
            }
            
        }

        // Maybe change
        private static void TCPConnectCallback(IAsyncResult ar)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(ar);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");
            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client, maze);
                    return;
                }
            }
            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full");
        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);
                if(_data.Length < 4)
                {
                    return;
                }
                using(Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();
                    if(_clientId == 0)
                    {
                        return;
                    }
                    if (clients[_clientId].udp.endPoint == null)
                    {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }
                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error receiving UDP data: {ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if(_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {ex}");
            }
        }

        public static void BeginGame()
        {
            // check to see if all players are "ready"

            for(int i = 1; i < MaxPlayers; i++)
            {
                if (clients[i].tcp.socket != null)
                {
                    if (!clients[i].ready)
                    {
                        return;
                    }
                }
            }

            assignPlayers();
            ServerSend.BeginGame(maze);
        }

        private static void assignPlayers()
        {
            int m = 2; // For now too lmao

            int count = 0;
            for(int i = 1; i < MaxPlayers; i++)
            {
                if (clients[i].tcp.socket != null)
                {
                    count++;
                }
            }

            for(int z = 0; z < maze.Length; z++)
            {
                for (int x = 0; x < maze[z].Length; x++)
                {
                    if (maze[z][x] == -1)
                    {
                        // Calculate initial position, then set as position of player
                        Console.WriteLine("Starting Coords: " + z.ToString() + ", " + x.ToString());
                        //maze[z][x] = 101;
                        avatar = new Player(new Vector3((float)(m * x + 0.5 * m), 0, (float)-(m * z + 0.5 * m)));
                        Console.WriteLine("Coords in unity space: " + avatar.pos);
                    }
                    else if (maze[z][x] == 0)
                    {
                        maze[z][x] = rand.Next(101, 101 + count);
                    }
                }
            }

            turn = 1;
        }

        // Change (What kinds of packets will the server handle?)
        private static void initializeServerData()
        {
            for(int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            MazeGenerator gen = new MazeGenerator(10, 10, 6);
            maze = gen.GenerateMaze();

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                {(int)ClientPackets.playerPosition, ServerHandle.PlayerPosition},
                {(int)ClientPackets.beginGame, ServerHandle.BeginGame}
            };
            Console.WriteLine("Initialized packets");
        }
    }
}
