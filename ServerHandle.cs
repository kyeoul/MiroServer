using System;
namespace GameServer
{
	public class ServerHandle
	{
		public static void WelcomeReceived(int fromClient, Packet packet)
		{
			int clientIdCheck = packet.ReadInt();
			string username = packet.ReadString();

			Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected and is now player {fromClient}");

			if(fromClient != clientIdCheck)
			{
				Console.WriteLine($"oopsy poopsy something went wrong; Player \"{username}\" (ID: {fromClient}) assumed the wrong clientID ({clientIdCheck})");
			}

			//Send Player into game
		}

		public static void UDPTestReceived(int _fromClient, Packet _packet)
		{
			string _msg = _packet.ReadString();
			Console.WriteLine($"Received packet via UDP, containing message: {_msg}");
		}
	}
}

