using System;
namespace GameServer
{
	public class ServerSend
	{
		private static void SendTCPData(int _toClient, Packet packet)
		{
            packet.WriteLength();
			Server.clients[_toClient].tcp.SendData(packet);

		}

		private static void SendUDPData(int _toClient, Packet packet)
		{
			packet.WriteLength();
			Server.clients[_toClient].udp.SendData(packet);
		}

		private static void SendTCPDataToAll(Packet packet)
		{
			packet.WriteLength();
			for(int i = 1; i <= Server.MaxPlayers; i++)
			{
				Server.clients[i].tcp.SendData(packet);
			}
		}

        private static void SendTCPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if(i != exceptClient)
				{
                    Server.clients[i].tcp.SendData(packet);
                }
            }
        }

        private static void SendUDPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(packet);
            }
        }

        private static void SendUDPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClient)
                {
                    Server.clients[i].udp.SendData(packet);
                }
            }
        }

        public static void Welcome(int _toClient, string _msg)
		{
			using (Packet packet = new Packet((int)ServerPackets.welcome))
			{
				packet.Write(_msg);
				packet.Write(_toClient);
                //packet.Write(maze);
                //packet.Write(Server.avatar.pos);
				SendTCPData(_toClient, packet);
			}
		}

        public static void SpawnPlayer(int _toClient, int[][] maze)
        {
            //using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            //{
            //    _packet.Write(_player.id);
            //    _packet.Write(_player.username);
            //    _packet.Write(_player.pos);
            //    _packet.Write(_player.rotation);

            //    SendTCPData(_toClient, _packet);
            //}
        }

        public static void BeginGame(int[][] maze)
        {
            using (Packet packet = new Packet((int)ServerPackets.beginGame))
            {
                packet.Write(maze);
                packet.Write(Server.avatar.pos);
                SendTCPDataToAll(packet);
            }
        }

        public static void GetTurn()
        {
            using (Packet packet = new Packet((int)ServerPackets.getTurn))
            {
                packet.Write(Server.getTurn());
                SendTCPDataToAll(packet);
            }
        }

        public static void PlayerPosition(int _clientId)
        {
            using(Packet _packet = new Packet((int)ServerPackets.playerPos))
            {
                _packet.Write(Server.avatar.pos);
                //Console.WriteLine("Current Pos: " + _player.pos);
                SendUDPData(_clientId, _packet);
            }
        }

        public static void PlayerRotation(int _clientId)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
            {
                _packet.Write(Server.avatar.rotation);
                // SendUDPDataToAll(_player.id, _packet);
                SendUDPData(_clientId, _packet);
            }
        }

        public static void SendWin()
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendWin))
            {
                _packet.Write(true);
                SendUDPDataToAll(_packet);
            }
        }

        // Add function that sends maze
    }
}

