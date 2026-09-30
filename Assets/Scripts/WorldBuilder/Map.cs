using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Map
{
    MapPath[,] board;
    int x;

    public Map()
    {
        board = new MapPath[17, 17];
        for (int i=0; i<board.GetLength(0); i++)
        {
            for (int j=0; j<board.GetLength(1); j++)
            {
                board[i, j] = new MapPath("xNon ("+ i + " , " + j + ")");
            }
        }
        x = 8;
        for (int i=0; i<board.GetLength(1)/2; i++)
        {
            board[i, x].SetTheme("ice");
            board[i, x].SetIcon("i");
        }
        for (int i = board.GetLength(1) / 2; i < board.GetLength(1); i++)
        {
            board[i, x].SetTheme("rock");
            board[i, x].SetIcon("r");
        }
        
    }
    public void Add(string name)
    {
        MapPath mPath = new MapPath(name);
        board[x + mPath.X(), x + mPath.Y()] = mPath;
        if (x + mPath.X() - 1 < 0 || x + mPath.Y() - 1 < 0 || x + mPath.X() + 1 >= board.GetLength(0) || x + mPath.Y() + 1 >= board.GetLength(1))
        {
            ExpandBoard();
        }
    }
    public string Print()
    {
        string ans = "\n";
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                ans += " " + board[i, j].Icon() + " ";
            }
            ans += "\n";
        }
        return ans;
    }
    public string GetIcon(Vector2 position)
    {
        return board[(int)(x + (-1 * position.y / 1000)), (int)(x + (position.x / 1000))].Icon();
    }
    public string GetNextIcon(Vector2 position)
    {
        int yCurrent = (int)(x + (position.x / 1000));
        int xCurrent = (int)(x + (-1 * position.y / 1000));
        string icon = board[xCurrent, yCurrent].Icon();
        int index = 1;
        while (icon.Equals("x"))
        {
            icon = Spirala(xCurrent, yCurrent, index);
            index++;
        }
        return icon.ToLower(); 
    }
    public string Spirala(int i, int j, int level)
    {
        if(i - level < 0 || j - level < 0 || i + level >= board.GetLength(0) || j + level >= board.GetLength(1))
        {
            ExpandBoard();
        }
        int current = UnityEngine.Random.Range(1, 9);
        int count = 1;
        int rand = UnityEngine.Random.Range(0, 2);
        if (rand == 0)
            rand = -1;
        while(count < 9)
        {
            switch (current)
            {
                case 1:
                    if (board[i, j - level].isNextIcon())
                    {
                        return board[i, j - level].Icon();
                    }
                    break;
                case 2:
                    if (board[i - level, j - level].isNextIcon())
                    {
                        return board[i - level, j - level].Icon();
                    }
                    break;
                case 3:
                    if (board[i - level, j].isNextIcon())
                    {
                        return board[i - level, j].Icon();
                    }
                    break;
                case 4:
                    if (board[i - level, j + level].isNextIcon())
                    {
                        return board[i - level, j + level].Icon();
                    }
                    break;
                case 5:
                    if (board[i, j + level].isNextIcon())
                    {
                        return board[i, j + level].Icon();
                    }
                    break;
                case 6:
                    if (board[i + level, j + level].isNextIcon())
                    {
                        return board[i + level, j + level].Icon();
                    }
                    break;
                case 7:
                    if (board[i + level, j].isNextIcon())
                    {
                        return board[i + level, j].Icon();
                    }
                    break;
                case 8:
                    if (board[i + level, j - level].isNextIcon())
                    {
                        return board[i + level, j - level].Icon();
                    }
                    break;
            }

            current = TickClock(current, rand);
            count++;
        }
        return "x";
    }
    public void ExpandBoard()
    {
        int n = board.GetLength(0);
        if (n % 2 == 1)
            n++;
        MapPath[,] boardNew = new MapPath[n + 17, n + 17];
        for (int i = 0; i < boardNew.GetLength(0); i++)
        {
            for (int j = 0; j < boardNew.GetLength(1); j++)
            {
                boardNew[i, j] = new MapPath("xNon (" + i + " , " + j + ")");
            }
        }
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                boardNew[x + i + 1, x + j + 1] = board[i, j];
            }
        }
        x = 2*x + 1;
        board = boardNew;
    }
    private int TickClock(int num, int adder)
    {
        if (num >= 8 && adder > 0)
            return 1;
        if (num <= 1 && adder < 0)
            return 8;
        return num + adder;

    }


}

