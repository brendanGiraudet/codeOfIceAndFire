using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class CodeIceAndFire
{
    private const int ME = 0;
    private const int OPPONENT = 1;
    private const int NEUTRAL = -1;

    private const int TRAIN_COST_LEVEL_1 = 10;
    private const int TRAIN_COST_LEVEL_2 = 20;
    private const int TRAIN_COST_LEVEL_3 = 30;
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
                buildings.Add(new Building 
                {
                    Owner = int.Parse(inputs[0]),
                    Type = int.Parse(inputs[1]),
                    Location = new Location
                    {
                        X = int.Parse(inputs[2]),
                        Y = int.Parse(inputs[3])
                    }
                }); 
            }
            me.Buildings = buildings.Where(b => b.Owner.Equals(ME)).ToList();
            opponent.Buildings = buildings.Where(b => b.Owner.Equals(OPPONENT)).ToList();
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
                        Location = new Location
                        {
                            X = int.Parse(inputs[3]),
                            Y = int.Parse(inputs[4])
                        }
                    }
                );
            }
            me.Units = units.Where(u => u.Owner.Equals(ME)).ToList();
            opponent.Units = units.Where(u => u.Owner.Equals(OPPONENT)).ToList();

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
            Console.Error.WriteLine(me);
            Console.Error.WriteLine(me.Units.Count);
            me.Units.ForEach(u => 
            {
                System.Console.Error.WriteLine(u);
            });
            // Première unité
            if(me.Units.Count.Equals(0))
            {
                Console.WriteLine("WAIT;TRAIN 1 " + me.Buildings.FirstOrDefault().Location.X+1 + " " + me.Buildings.FirstOrDefault().Location.Y );
            }
            else if(me.Units.FirstOrDefault() != null)
            {
                var selectedUnit = me.Units.FirstOrDefault();
                System.Console.Error.WriteLine(selectedUnit);
                Console.WriteLine("WAIT;MOVE " + selectedUnit.ID + " " + selectedUnit.Location.X+1 + " " + selectedUnit.Location.Y );
            }
            else
            {
                Console.WriteLine("WAIT");    
            }
        }
    }
}
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
class Game
{
    public List<string> Map { get; set; }  = new List<string>();
    public bool IsVoid(Location loc)
    {
        return Map[loc.Y][loc.X].Equals('#');
    }
    public bool IsNeutral(Location loc)
    {
        return Map[loc.Y][loc.X].Equals('.');
    }
    public bool IsActiveOwned(Location loc)
    {
        return Map[loc.Y][loc.X].Equals('O');
    }
    public bool IsInactiveOwned(Location loc)
    {
        return Map[loc.Y][loc.X].Equals('o');
    }
    public bool IsActiveOpponent(Location loc)
    {
        return Map[loc.Y][loc.X].Equals('X');
    }
    public bool IsInactiveOpponent(Location loc)
    {
        return Map[loc.Y][loc.X].Equals('x');
    }

}
class Player
{
    public int Gold { get; set; }
    public int Income { get; set; }
    public List<Unit> Units { get; set; } = new List<Unit>();
    public List<Building> Buildings { get; set; } = new List<Building>();

    public override string ToString()
    {
        return "Gold : " + Gold + " Income : " + Income;
    }
}
class Location
{
    public int X { get; set; }
    public int Y { get; set; }

    public override string  ToString()
    {
        return "X : " + X + " Y : " + Y;
    }
}
class Unit
{
    public int Owner { get; set; }
    public int ID { get; set; }
    public int Level { get; set; }
    public Location Location { get; set; } = new Location();

    public override string ToString()
    {
        return "Owner : " + Owner + " ID : " + ID + " Level : " + Level + " " + Location;
    }
}
class Building
{
    public int Owner { get; set; }
    public int Type { get; set; }
    public Location Location { get; set; } = new Location();

    public override string ToString()
    {
        return "Owner : " + Owner + " Type : " + Type + " " + Location;
    }
}