using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public enum BuildingType
{
    Hq,
    Mine,
    Tower
}

public enum Team
{
    Fire,
    Ice
}
private const int ME = 0;
private const int OPPONENT = 1;
private const int NEUTRAL = -1;

private const int TRAIN_COST_LEVEL_1 = 10;
private const int TRAIN_COST_LEVEL_2 = 20;
private const int TRAIN_COST_LEVEL_3 = 30;
/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Main
{
    static void Main(string[] args)
    {
        var game = new Game();
        string[] inputs;
        int numberMineSpots = int.Parse(Console.ReadLine());
        for (int i = 0; i < numberMineSpots; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int x = int.Parse(inputs[0]);
            int y = int.Parse(inputs[1]);
        }

        // game loop
        while (true)
        {
            // Players
            var me = new Player
            {
                Gold = int.Parse(Console.ReadLine()),
                Income = int.Parse(Console.ReadLine())
            };
            var opponent = new Player
            {
                Gold = int.Parse(Console.ReadLine()),
                Income = int.Parse(Console.ReadLine())
            };
            // MAP
            for (int i = 0; i < 12; i++)
            {
                game.Map.Add(Console.ReadLine());
            }
            // Buildings
            var buildings = new List<Building>();
            int buildingCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < buildingCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                building.Add(new Building 
                {
                    Owner = int.Parse(inputs[0]),
                    Type = int.Parse(inputs[1]),
                    Localisation = new Localisation
                    {
                        X = int.Parse(inputs[2]),
                        Y = int.Parse(inputs[3])
                    }
                }); 
            }
            me.Buildings = buildings.Where(b => b.Owner.Equals(ME));
            opponent.Buildings = buildings.Where(b => b.Owner.Equals(OPPONENT));
            // Units
            var units = new List<Unit>();
            int unitCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < unitCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                units.Add(
                    new Unit
                    {
                        Owner = int.Parse(inputs[0]),
                        ID = int.Parse(inputs[1]),
                        Level = int.Parse(inputs[2]),
                        Localisation = new Localisation
                        {
                            X = int.Parse(inputs[3]),
                            Y = int.Parse(inputs[4])
                        }
                    }
                );
            }
            me.Units = units.Where(u => u.Owner.Equals(ME));
            opponent.Units = units.Where(u => u.Owner.Equals(OPPONENT));

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
            Console.Error.WriteLine("Debug messages...");

            Console.WriteLine("WAIT");
        }
    }
}
class Game
{
    public List<string> Map { get; set; }  = new List<string>();
}
class Player
{
    public int Gold { get; set; }
    public int Income { get; set; }
    public List<Unit> Units { get; set; } = new List<Unit>();
    public List<Building> Buildings { get; set; } = new List<Building>();

    public override ToString()
    {
        return "Gold : " + Gold + " Income : " + Income;
    }
}
class Localisation
{
    public int X { get; set; }
    public int Y { get; set; }

    public override ToString()
    {
        return "X : " + X + " Y : " + Y;
    }
}
class Unit
{
    public int Owner { get; set; }
    public int ID { get; set; }
    public int Level { get; set; }
    public Localisation Localisation { get; set; } = new Localisation();

    public override ToString()
    {
        return "Owner : " + Owner + " ID : " + ID + " Level : " + Level + " " + Localisation;
    }
}
class Building
{
    public int Owner { get; set; }
    public int Type { get; set; }
    public Localisation Localisation { get; set; } = new Localisation();

    public override ToString()
    {
        return "Owner : " + Owner + " Type : " + Type + " " + Localisation;
    }
}