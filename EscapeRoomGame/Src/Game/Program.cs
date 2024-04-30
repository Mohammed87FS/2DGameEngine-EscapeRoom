using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Game game = new Game();
        game.Start();
    }
}

class Game
{
    private Room currentRoom;
    private bool isRunning;

    public Game()
    {
        Room room1 = new Room("Entrance Hall", "You are in a large, dimly lit hall.");
        Room room2 = new Room("Library", "You see walls covered with books.");
        room1.AddExit("north", room2);
        room2.AddExit("south", room1);

        // Start the game in the entrance hall.
        currentRoom = room1;
    }

    public void Start()
    {
        isRunning = true;
        Console.WriteLine("Welcome to the Escape Room Game!");

        while (isRunning)
        {
            Console.WriteLine(currentRoom.Description);
            Console.Write("Command (look/move/quit): ");
            HandleCommand(Console.ReadLine().Trim().ToLower());
        }
    }

    private void HandleCommand(string command)
    {
        if (command == "quit")
        {
            isRunning = false;
            Console.WriteLine("Thanks for playing!");
        }
        else if (command.StartsWith("move "))
        {
            Move(command.Substring(5));
        }
        else if (command == "look")
        {
            Console.WriteLine(currentRoom.Description);
        }
        else
        {
            Console.WriteLine("Unknown command.");
        }
    }

    private void Move(string direction)
    {
        if (currentRoom.Exits.ContainsKey(direction))
        {
            currentRoom = currentRoom.Exits[direction];
            Console.WriteLine($"You move {direction}.");
        }
        else
        {
            Console.WriteLine("You can't go that way.");
        }
    }
}

class Room
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Dictionary<string, Room> Exits { get; private set; }

    public Room(string name, string description)
    {
        Name = name;
        Description = description;
        Exits = new Dictionary<string, Room>();
    }

    public void AddExit(string direction, Room room)
    {
        Exits[direction] = room;
    }
}
