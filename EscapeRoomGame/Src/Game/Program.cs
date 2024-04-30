using System;
using System.Collections.Generic;

class Program
{

    
    static void Main()
    {
        Game game = new();
        game.Start();
    }
}

class Game
{
    private Room currentRoom;
    private bool isRunning;

    public Game()
    {
        Room room1 = new("Entrance Hall", "You are in a large, dimly lit hall.");
        Room room2 = new("Library", "You see walls covered with books.");
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
            Move(command[5..]);
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
        if (currentRoom.Exits.TryGetValue(direction, out Room? value))
        {
            currentRoom = value;
            Console.WriteLine($"You move {direction}.");
        }
        else
        {
            Console.WriteLine("You can't go that way.");
        }
    }
}

class Room(string name, string description)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public Dictionary<string, Room> Exits { get; private set; } = [];

    public void AddExit(string direction, Room room)
    {
        Exits[direction] = room;
    }
}
