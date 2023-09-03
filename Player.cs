using System;
using System.Numerics;
namespace GameServer
{
	public class Player
	{
		public int id = 5;
		public Vector3 pos;
		public Quaternion rotation;

		private float moveSpeed = 5f;
		private bool[] inputs;

		public Player(Vector3 _spawnPosition)
		{
			pos = _spawnPosition;
			rotation = Quaternion.Identity;

			inputs = new bool[4];
		}

		public void Update()
		{
			Vector2 _inputDirection = Vector2.Zero;
			if (inputs[0])
			{
				_inputDirection.Y += 1;
			}
            if (inputs[1])
            {
                _inputDirection.Y -= 1;
            }
            if (inputs[2])
            {
                _inputDirection.X += 1;
            }
            if (inputs[3])
            {
                _inputDirection.X -= 1;
            }

			// Move(_inputDirection);
			//Console.WriteLine(pos);
            Vector2 griddy = ConvertPosToGrid(pos);
			int intAtGriddy = Server.maze[(int)griddy.X][(int)griddy.Y];

            if (intAtGriddy == 2)
			{
				ServerSend.SendWin();
				Console.WriteLine("Wow the game was won");
				Program.setRunning(false);
			}
			else if (intAtGriddy == -1)
			{
                Server.setTurn(1);
            }
			else
			{
				if(intAtGriddy - 100 != Server.getTurn())

				{
					ServerSend.PlayerPosition(intAtGriddy - 100);
					ServerSend.PlayerRotation(intAtGriddy - 100);
				}
                Server.setTurn(intAtGriddy - 100);
            }

			ServerSend.GetTurn();
        }

		private void Move(Vector2 _inputDirection)
		{
			Vector3 _forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
			Vector3 _right = Vector3.Normalize(Vector3.Cross(_forward, new Vector3(0, 1, 0)));

			Vector3 _moveDirection = _right * _inputDirection.X + _forward * _inputDirection.Y;
			pos += _moveDirection * moveSpeed;

			//ServerSend.PlayerPosition(this);
			//ServerSend.PlayerRotation(this);

        }

        private Vector2 ConvertPosToGrid(Vector3 pos)
        {
            int tempM = 2;

            return new Vector2((float) Math.Floor(-pos.Z / tempM), (float)Math.Floor(pos.X / tempM));
        }

        public void SetInput(bool[] _inputs, Quaternion _rotation, float speed)
		{
			inputs = _inputs;
			rotation = _rotation;
		}
	}
}

