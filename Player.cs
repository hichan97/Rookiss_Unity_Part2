﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Algorithm
{
    class Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;
    }

    class Player
    {
        public int PosY { get; private set; }
        public int PosX { get; private set; }
        Random _random = new Random();
        
        Board _board;

        enum Dir
        {
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3
        }
        int _dir = (int)Dir.Up;
        List<Pos> _points = new List<Pos>();

        public void Initialize(int posY, int posX, Board board)
        {
            PosY = posY;
            PosX = posX;

            _board = board;

            AStar();

        }

        struct PQNode : IComparable<PQNode>
        {
            public int F;
            public int G;
            public int Y;
            public int X;

            public int CompareTo(PQNode other)
            {
                if (F == other.F)
                    return 0;
                return F < other.F ? 1 : -1;
            }
        }

        void AStar()
        {
                        //U L D R UL DL DR UR
                                                    //대각선 이동
            int[] deltaY = new int[] {-1, 0, 1, 0, /*-1, 1, 1, -1*/};
            int[] deltaX = new int[] { 0, -1, 0, 1, /*-1, -1, 1, 1*/ };
            int[] cost = new int[] { 10, 10, 10, 10, /*14, 14, 14 ,14*/}

            //점수 매기기
            //F = G + H
            //F = 최종 점수 (작을 수록 종ㅎ음, 경로에 따라 달라짐)
            //G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
            //H = 목적지에서 얼마나 가까운지 (작을수록 좋음, 고정값)

            //(y, x)에 방문했는지 여부(방문 = closed 상태)
            bool[,] closed = new bool[_board.Size, _board.Size]; //Closed 사이즈

            //[y,x] 가는 길을 한 번이라도 발견했는지
            // 미발견 => MaxValue
            // 발견   => F = G + H

            int[,] open = new int[_board.Size, _board.Size]; //Open List
            for (int y = 0; y < _board.Size; y++)
            {
                for (int x = 0; x < _board.Size; x++)
                {
                    open[y, x] = Int32.MaxValue;
                }
            }

            Pos[,] parent = new Pos[_board.Size, _board.Size];

            // 오픈 리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            //시작점 발견
            open[PosY, PosX] = 10 * (Math.Abs(_board.DesY - PosY) + Math.Abs(_board.DesX - PosX));
            pq.Push(new PQNode() { F = 10 * (Math.Abs(_board.DesY - PosY) + Math.Abs(_board.DesX - PosX)), G = 0, Y = PosY, X = PosX });
            parent[PosY, PosX] = new Pos(PosY, PosX);

            while(pq.Count > 0)
            {
                //제일 좋은 후보를 찾는다
                PQNode node = pq.Pop();
                //동일한 좌표를 여러 경로를 찾아서, 더빠른 경로로인해서 이미 방문 (Closed)된 경우 스킵
                if (closed[node.Y, node.X])
                    continue;

                //방문한다
                closed[node.Y, node.X] = true;
                if (node.Y == _board.DesY && node.X == _board.DesX)
                    break;

                //상하좌우 등 이동할 수 있는 좌표인지 확인해서 에약(open)한다.
                for (int i = 0; i < deltaY.Length; i++)
                {
                    int nextY = node.Y + deltaY[i];
                    int nextX = node.X + deltaX[i];

                    //유효 범위를 벗어났으면 스킵
                    if(nextX < 0 || nextX >= _board.Size || nextY <0 ||nextY >= _board.Size)    
                        continue;
                    //벽으로 막히면 스킵
                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall)
                        continue;
                    //이미 방문한 곳이면 스킵
                    if (closed[nextY, nextX])
                        continue;

                    //비용 계산
                    int g = node.G + cost[i];
                    int h = 10 * (Math.Abs(_board.DesY - nextY) + Math.Abs(_board.DesX - nextX));
                    //다른 경로에서 더 빠른 길을 찾았으면 스킵
                    if (open[nextY, nextX] < g + h)
                        continue;

                    //예약 진행
                    open[nextY, nextX] = g + h;
                    pq.Push(new PQNode() { F = g + h, G = g, Y = nextY, X = nextX });
                    parent[nextY, nextX] = new Pos(node.Y, node.X);

                }
            }
            CalcPathFromParent(parent);
        }

        void CalcPathFromParent(Pos[,] parent)
        {
            int y = _board.DesY;
            int x = _board.DesX;

            while(parent[y, x].Y != y || parent[y, x].X != x)
            {
                _points.Add(new Pos(y, x));
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            _points.Add(new Pos(y, x));
            _points.Reverse();
        }

        void RightHand()
        { 
            
            //현재 바라보고 있는 방향을 기준으로, 좌표 변화를 나타낸다.
            int[] frontY = new int[] {-1, 0, 1, 0};
            int[] frontX = new int[] { 0, -1, 0, 1 };
            int[] rightY = new int[] { 0, -1, 0, 1 };
            int[] rightX = new int[] { 1, 0, -1, 0 };

            _points.Add(new Pos(PosY, PosX));

            while (PosY != _board.DesY || PosX != _board.DesX)
            {
                //1. 현재 바라보는 방향을 기준으로 오른쪽으로 갈 수 있는 지 확인
                if (_board.Tile[PosY + rightY[_dir], PosX + rightX[_dir]] == Board.TileType.Empty)
                {
                    //오른쪽 방향으로 90도 회전
                    _dir = (_dir - 1 + 4) % 4;

                    //앞으로 한 보 전진
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));
                }
                else if (_board.Tile[PosY + frontY[_dir], PosX + frontX[_dir]] == Board.TileType.Empty)
                {
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));
                }
                else
                {   
                    //왼쪽 방향으로 90도 회전
                    _dir = (_dir + 1 + 4) % 4;
                }
            }
        }

        const int MOVE_TICK = 30;
        int _sumTick = 0;
        int _lastIndex = 0;

        public void Update(int deltaTick)
        {
            if(_lastIndex >= _points.Count)
                { 
                _lastIndex = 0;
                _points.Clear();
                _board.Initialize(_board.Size, this);
                Initialize(1, 1, _board);
                }

            _sumTick += deltaTick;
            if (_sumTick >= MOVE_TICK) 
            { 
                _sumTick = 0;

                //여기에다가 0,1초마다 실행될 로직을 넣어준다

                PosY = _points[_lastIndex].Y;
                PosX = _points[_lastIndex].X;
                _lastIndex++;

            }
        }
    }
}
