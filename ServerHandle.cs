using System;
using System.Numerics;
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
			Server.clients[fromClient].SendIntoGame(username);
		}

		public static void PlayerMovement(int _fromClient, Packet _packet)
		{
			bool[] _inputs = new bool[_packet.ReadInt()];

			for(int i = 0; i < _inputs.Length; i++)
			{
				_inputs[i] = _packet.ReadBool();
			}
			Quaternion _rotation = _packet.ReadQuaternion();
			Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
		}
	}
}

