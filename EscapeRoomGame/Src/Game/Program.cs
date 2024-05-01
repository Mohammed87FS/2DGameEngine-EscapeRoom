using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace EscapeRoomGame
{
  
    public abstract class GameObject
    {
        public enum GameObjectType { Player, Obstacle, Box, Empty }
        public GameObjectType Type { get; protected set; }
        public char CharRepresentation { get; protected set; }
        public ConsoleColor Color { get; protected set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public abstract void Move(int dx, int dy);
    }


    public interface IMovable
    {
        void Move(int dx, int dy);
    }

 
    public class Player : GameObject, IMovable
    {
        private static Player? instance;
        public static Player Instance => instance ??= new Player();

        private Player()
        {
            Type = GameObjectType.Player;
            CharRepresentation = '☻';
            Color = ConsoleColor.DarkYellow;
        }

        public override void Move(int dx, int dy)
        {
            GameEngine.Instance.MoveGameObject(this, dx, dy);
        }
    }

   
    public class Box : GameObject, IMovable
    {
        public Box()
        {
            Type = GameObjectType.Box;
            CharRepresentation = '□';
            Color = ConsoleColor.Red;
        }

        public override void Move(int dx, int dy)
        {
            GameEngine.Instance.MoveGameObject(this, dx, dy);
        }
    }

  
   public class GameEngine
{
    private static GameEngine _instance;
    public static GameEngine Instance => _instance ?? (_instance = new GameEngine());

    private Map map;
    public Map Map => map; 

    private GameEngine()
    {
        InitializeFromJson("C:/Users/moham/Desktop/2DGameEngine-EscapeRoom/EscapeRoomGame/Setup.json");
    }

    private void InitializeFromJson(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        GameData gameData = JsonConvert.DeserializeObject<GameData>(jsonData);

        map = new Map(gameData.map.width, gameData.map.height);

        foreach (var gameObject in gameData.gameObjects)
        {
            GameObject obj = gameObject.Type switch
            {
                1 => Player.Instance,
                2 => new Box(),
                _ => new Empty(),
            };

            obj.PosX = gameObject.PosX;
            obj.PosY = gameObject.PosY;
            map.PlaceGameObject(obj, gameObject.PosX, gameObject.PosY);
        }
    }

    public void MoveGameObject(GameObject obj, int dx, int dy)
    {
        int newX = obj.PosX + dx;
        int newY = obj.PosY + dy;

        if (map.CanMoveTo(newX, newY))
        {
            map.MoveGameObject(obj, obj.PosX, obj.PosY, newX, newY);
        }
    }
}



    public class Map
    {
        private GameObject[,] grid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            grid = new GameObject[height, width];
            InitializeMap();
        }

        private void InitializeMap()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (i == 0 || i == Height - 1 || j == 0 || j == Width - 1)
                    {
                        grid[i, j] = new Obstacle();
                    }
                    else
                    {
                        grid[i, j] = new Empty();
                    }
                }
            }
        }

        public bool CanMoveTo(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height || grid[y, x].Type == GameObject.GameObjectType.Obstacle)
            {
                return false;
            }
            return true;
        }

        public void PlaceGameObject(GameObject obj, int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                grid[y, x] = obj;
            }
        }

public GameObject GetGameObjectAt(int x, int y)
{
    return grid[y, x];
}
        public void MoveGameObject(GameObject obj, int oldX, int oldY, int newX, int newY)
        {
            if (CanMoveTo(newX, newY))
            {
                grid[oldY, oldX] = new Empty();
                grid[newY, newX] = obj;
            }
        }
    }


    public class Obstacle : GameObject
    {
        public Obstacle()
        {
            Type = GameObjectType.Obstacle;
            CharRepresentation = '#';
            Color = ConsoleColor.Gray;
        }

        public override void Move(int dx, int dy)
        {
            throw new NotImplementedException();
        }
    }

 public class Empty : GameObject
{
    public Empty()
    {
        Type = GameObjectType.Empty;
        CharRepresentation = '·';  
        Color = ConsoleColor.DarkGray; 
    }

    public override void Move(int dx, int dy)
    {
        throw new NotImplementedException();
    }
}

 
    public class GameData
    {
        public MapData map { get; set; }
        public List<GameObjectData> gameObjects { get; set; }
    }

    public class MapData
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class GameObjectData
    {
        public int Type { get; set; }
        public int Color { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
    }

  public class GameLoop
{
    public void Run()
    {
        bool isRunning = true;
        while (isRunning)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    Player.Instance.Move(0, -1);
                    break;
                case ConsoleKey.DownArrow:
                    Player.Instance.Move(0, 1);
                    break;
                case ConsoleKey.LeftArrow:
                    Player.Instance.Move(-1, 0);
                    break;
                case ConsoleKey.RightArrow:
                    Player.Instance.Move(1, 0);
                    break;
                case ConsoleKey.Escape:
                    isRunning = false;
                    break;
            }

           
            DrawMap(GameEngine.Instance.Map);
        }
    }

     private void DrawMap(Map map)
    {
        Console.Clear();
        for (int i = 0; i < map.Height; i++)
        {
            for (int j = 0; j < map.Width; j++)
            {
                GameObject obj = map.GetGameObjectAt(j, i);
                Console.SetCursorPosition(j, i);
                Console.ForegroundColor = obj.Color;
                Console.Write(obj.CharRepresentation);
            }
        }
        Console.SetCursorPosition(Player.Instance.PosX, Player.Instance.PosY);
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write(Player.Instance.CharRepresentation);
    }
}



    
    class Program
    {
        static void Main(string[] args)
        {
            GameLoop loop = new();
            loop.Run();
        }
    }
}
