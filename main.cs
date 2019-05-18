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
            var game = new Game();
            // MAP
            for (int i = 0; i < 12; i++)
            {
                var line = Console.ReadLine();
                for (int j = 0; j < line.Length; j++)
                {
                    var location = new Location
                    {
                        X = j,
                        Y = i,
                        Value = line[j]
                    };
                    if(location.Equals('O') || location.Equals('o'))
                    {
                        me.Locations.Add(location);
                    }
                    if(location.Equals('X') || location.Equals('x'))
                    {
                        opponent.Locations.Add(location);
                    }
                    game.Map.Add(location);
                }
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

            Console.Error.WriteLine(me);
            me.Units.ForEach(u => 
            {
                System.Console.Error.WriteLine(u);
            });
            me.Locations.ForEach(l => 
            {
                System.Console.Error.WriteLine(l);
            });
            
            var rep = "";            
            var trainLocation = me.GetTrainLocation();
            System.Console.Error.WriteLine("train " + trainLocation);
            var selectedUnit = me.Units.FirstOrDefault();
            if(trainLocation == null)
            {
                trainLocation = game.GetLocationToTrainUnitAroundHQ();
            }
            System.Console.Error.WriteLine("train " + trainLocation);
            if(me.HaveEnoughGoldToUnitLvl1() && trainLocation != null)
            {
                rep += "TRAIN 1 " + trainLocation.X + " " + trainLocation.Y + ";";
            }
            else if(selectedUnit != null)
            {
                var bestLocation = game.GetBestMove(selectedUnit.Location);
                rep += "MOVE " + selectedUnit.ID + " " + bestLocation.X + " " + bestLocation.Y + ";";
            }
            else
            {
                rep += "WAIT;";    
            }
            System.Console.WriteLine(rep);
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
    public List<Location> Map { get; set; }  = new List<Location>();

    public List<Location> GetOwnedLocation()
    {
        return Map.Where(l => l.Value.Equals('O')).ToList();
    }
    public bool CanMoveToHere(Location loc)
    {
        return  loc.X >=0 && loc.X < 12 && loc.Y >=0 && loc.Y < 12 && !loc.IsVoid();
    }
   
    public Location GetBestMove(Location loc)
    {
        var rightLoc = new Location
        {
            X = loc.X+1,
            Y = loc.Y
        };
        var leftLoc = new Location
        {
            X = loc.X-1,
            Y = loc.Y
        };
        var upLoc = new Location
        {
            X = loc.X,
            Y = loc.Y-1
        };
        var bottomLoc = new Location
        {
            X = loc.X,
            Y = loc.Y+1
        };
        if(CanMoveToHere(rightLoc) && rightLoc.IsNeutral())
        {
            return rightLoc;
        }
        else if(CanMoveToHere(leftLoc) && leftLoc.IsNeutral())
        {
            return leftLoc;
        }
        else if(CanMoveToHere(upLoc) && upLoc.IsNeutral())
        {
            return upLoc;
        }
        else if(CanMoveToHere(bottomLoc) && bottomLoc.IsNeutral())
        {
            return bottomLoc;
        }
        else
        {
            return loc;
        }
    }
    public Location GetHQLocation()
    {
        return new Location { X = 0, Y = 0};
    }

    public Location GetLocationToTrainUnitAroundHQ()
    {
        var hqLocation = GetHQLocation();
        var rightLoc = Map.FirstOrDefault(m => m.X == hqLocation.X+1 && m.Y == hqLocation.Y);
        var bottomLoc = Map.FirstOrDefault(m => m.X == hqLocation.X && m.Y == hqLocation.Y+1);

        if(rightLoc.IsActiveOwned() || rightLoc.IsNeutral())
        {
            return rightLoc;
        }
        
        return bottomLoc;
    }
}
class Player
{
    public int Gold { get; set; }
    public int Income { get; set; }
    public List<Unit> Units { get; set; } = new List<Unit>();
    public List<Building> Buildings { get; set; } = new List<Building>();

    public List<Location> Locations { get; set; } = new List<Location>();

    public override string ToString()
    {
        return "Gold : " + Gold + " Income : " + Income;
    }

    public bool HaveEnoughGoldToUnitLvl1()
    {
        return Gold >= 10;
    }

    public Location GetTrainLocation()
    {
        return Locations.FirstOrDefault( l => l.Equals('O') && Units.Any(u => u.Location.X.Equals(l.X) && u.Location.Y.Equals(l.Y)) == null);
    }
}
class Location
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Value { get; set; }

    public override string  ToString()
    {
        return "X : " + X + " Y : " + Y + " Value : " + Value;
    }
    public bool IsVoid()
    {
        return Value.Equals('#');
    }
    public bool IsNeutral()
    {
        return Value.Equals('.');
    }
    public bool IsActiveOwned()
    {
        return Value.Equals('O');
    }
    public bool IsInactiveOwned()
    {
        return Value.Equals('o');
    }
    public bool IsActiveOpponent()
    {
        return Value.Equals('X');
    }
    public bool IsInactiveOpponent()
    {
        return Value.Equals('x');
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