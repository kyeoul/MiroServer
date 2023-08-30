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

		// Change (filter out player data that isn't {turn})
		public static void PlayerMovement(int _fromClient, Packet _packet)
		{
			if (Server.getTurn() == _fromClient)
			{
				bool[] _inputs = new bool[_packet.ReadInt()];

				for (int i = 0; i < _inputs.Length; i++)
				{
					_inputs[i] = _packet.ReadBool();
				}
				Quaternion _rotation = _packet.ReadQuaternion();
				float magnitude = _packet.ReadFloat();
				//Console.WriteLine(_rotation);
				//Console.WriteLine(magnitude);
				Server.avatar.SetInput(_inputs, _rotation, magnitude);
			}

		}

		// Begin the game
		public static void BeginGame(int _fromClient, Packet _packet)
		{
			// fromclient and packet could be useful in filtering out any start game requests not from the owner of the lobby
			Console.WriteLine("Beginning Game...");
			Server.BeginGame();
		}

		// For testing purposes
		public static void PlayerPosition(int _fromClient, Packet _packet)
		{
			if(Server.getTurn() == _fromClient)
			{
				Vector3 pos = _packet.ReadVector3();
				Server.avatar.pos = pos;
			}
		}
	}
}

