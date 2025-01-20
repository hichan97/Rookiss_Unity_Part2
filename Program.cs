using System;

namespace Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();
            Player player = new Player();
            board.Initialize(25, player);
            player.Initialize(1, 1, board);

            Console.CursorVisible = false;

            const int WAIT_TICK = 1000 / 30;
            

            int lastTick = 0;
            while (true)
            {
                #region  프레임 관리
                int CurrentTick = System.Environment.TickCount;
                //만약 경과한 시간이 1/30초 보다 작다면
                if (CurrentTick - lastTick < WAIT_TICK) //1000ns = 1sec
                    continue;
                int deltaTick = CurrentTick - lastTick;
                lastTick = CurrentTick;
                #endregion

                //입력

                //로직
                player.Update(deltaTick);

                //렌더링
                Console.SetCursorPosition(0, 0);

                board.Render();

            }
        }
    }
}
