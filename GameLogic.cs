using System;
namespace GameServer
{
	public class GameLogic
	{
		public static void Update()
		{

			if(Server.avatar != null)
			{
                Server.avatar.Update();
            }
			ThreadManager.UpdateMain();
		}
	}
}

