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
				SendTCPData(_toClient, packet);
			}
		}

        public static void UDPTest(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.udpTest))
            {
                _packet.Write("UDP Test packet wowowowoow");
                SendUDPData(_toClient, _packet);
            }
        }
	}
}

