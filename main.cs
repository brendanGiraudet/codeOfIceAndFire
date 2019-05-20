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
                    if(location.Value.Equals('O') || location.Value.Equals('o'))
                    {
                        me.Locations.Add(location);
                    }
                    if(location.Value.Equals('X') || location.Value.Equals('x'))
                    {
                        opponent.Locations.Add(location);
                    }
                    game.Map.Add(location);
                }
            }
            // Buildings
            int buildingCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < buildingCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                game.Buildings.Add(new Building 
                {
                    Owner = int.Parse(inputs[0]),
                    Type = (BuildingType) Enum.Parse(typeof(BuildingType), inputs[1]),
                    Location = new Location
                    {
                        X = int.Parse(inputs[2]),
                        Y = int.Parse(inputs[3])
                    }
                }); 
            }
            me.Buildings = game.Buildings.Where(b => b.Owner.Equals(ME)).ToList();
            opponent.Buildings = game.Buildings.Where(b => b.Owner.Equals(OPPONENT)).ToList();
            // Units
            int unitCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < unitCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                game.Units.Add(
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
            me.Units = game.Units.Where(u => u.Owner.Equals(ME)).ToList();
            opponent.Units = game.Units.Where(u => u.Owner.Equals(OPPONENT)).ToList();

            Console.Error.WriteLine(me);
            System.Console.Error.WriteLine("Unités :");
            me.Units.ForEach(u => 
            {
                System.Console.Error.WriteLine(u);
            });
            System.Console.Error.WriteLine("Locations :");
            me.Locations.ForEach(l => 
            {
                System.Console.Error.WriteLine(l);
            });
            System.Console.Error.WriteLine("Buildings :");
            me.Buildings.ForEach(b => 
            {
                System.Console.Error.WriteLine(b);
            });
            
            var rep = "";            
            // Partie entrainement
            var trainLocation = me.GetTrainLocation();
            do
            {
                // Récuperation emplacement pour entrainer
                trainLocation = me.GetTrainLocation();
                System.Console.Error.WriteLine("train " + trainLocation);
                if(trainLocation == null)
                {
                    trainLocation = me.GetLocationToTrainUnitAroundHQ();
                    System.Console.Error.WriteLine("train hq " + trainLocation);
                }
                if(me.HaveEnoughGoldToUnitLvl3() && trainLocation != null)
                {
                    rep += "TRAIN 3 " + trainLocation.X + " " + trainLocation.Y + ";";
                    me.Gold -= TRAIN_COST_LEVEL_3;
                    me.Units.Add(new Unit{ Location = new Location{ X = trainLocation.X, Y= trainLocation.Y, Value = '0'}, Level = 3});
                }
                else if(me.HaveEnoughGoldToUnitLvl2() && trainLocation != null && me.Units.Where(u => u.Level == 2).ToList().Count() < 4)
                {
                    rep += "TRAIN 2 " + trainLocation.X + " " + trainLocation.Y + ";";
                    me.Gold -= TRAIN_COST_LEVEL_2;
                    me.Units.Add(new Unit{ Location = new Location{ X = trainLocation.X, Y= trainLocation.Y, Value = '0'}, Level = 2});
                }
                else if(me.HaveEnoughGoldToUnitLvl1() && trainLocation != null && me.Units.Where(u => u.Level == 1).ToList().Count() < 9)
                {
                    rep += "TRAIN 1 " + trainLocation.X + " " + trainLocation.Y + ";";
                    me.Gold -= TRAIN_COST_LEVEL_1;
                    me.Units.Add(new Unit{ Location = new Location{ X = trainLocation.X, Y= trainLocation.Y, Value = '0'}, Level = 1});
                }
            }while(
                ((me.HaveEnoughGoldToUnitLvl1() && me.Units.Where(u => u.Level == 1).ToList().Count() < 9 )
                ||(me.HaveEnoughGoldToUnitLvl2() && me.Units.Where(u => u.Level == 2).ToList().Count() < 4)
                || me.HaveEnoughGoldToUnitLvl3()) 
                && trainLocation != null);
            
            // partie mouvement des unités
            foreach(var unit in me.Units.Where(u => u.ID != 0 ))
            {
                System.Console.Error.WriteLine("Unit : " + unit);
                Location bestLocation = null;
                if(unit.Level > 1 && opponent.Units.Count() != 0)
                {
                    bestLocation = game.MoveTo(opponent.Units.FirstOrDefault().Location, unit);
                }
                else
                {
                    bestLocation = game.MoveTo(opponent.GetHQLocation(), unit);
                }
                if(bestLocation != null)
                {
                    unit.Location.X = bestLocation.X;
                    unit.Location.Y = bestLocation.Y;
                    rep += "MOVE " + unit.ID + " " + unit.Location.X + " " + unit.Location.Y + ";";
                    me.Locations.Add(new Location 
                    {
                        X = unit.Location.X,
                        Y = unit.Location.Y,
                        Value = 'O'
                    });
                    game.Map.FirstOrDefault(m => m.X == unit.Location.X && m.Y == unit.Location.Y).Value = 'O';
                }
                else
                {
                    bestLocation = me.GetBestLocationAroundUnit(unit.Location);
                    if(bestLocation != null)
                    {
                        unit.Location.X = bestLocation.X;
                        unit.Location.Y = bestLocation.Y;
                        rep += "MOVE " + unit.ID + " " + unit.Location.X + " " + unit.Location.Y + ";";
                    }
                }
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

    public List<Building> Buildings { get; set; } = new List<Building>();
    public List<Unit> Units { get; set; } = new List<Unit>();

    public List<Location> GetOwnedLocation()
    {
        return Map.Where(l => l.Value.Equals('O')).ToList();
    }
    public bool CanMoveToHere(Location loc)
    {
        return  loc.X >=0 && loc.X < 12 && loc.Y >=0 && loc.Y < 12 && !loc.IsVoid();
    }
    public Location GetDiscoveredLocation(Location loc)
    {
        var rightLoc = Map.FirstOrDefault(m => m.X == loc.X+1 && m.Y == loc.Y);
        if(rightLoc != null && !rightLoc.IsVoid() && (rightLoc.IsNeutral() || rightLoc.IsActiveOpponent()))
        {
            return rightLoc;
        }
        
        var leftLoc = Map.FirstOrDefault(m => m.X == loc.X-1 && m.Y == loc.Y);
        if(leftLoc != null && !leftLoc.IsVoid() && (leftLoc.IsNeutral() || leftLoc.IsActiveOpponent()))
        {
            return leftLoc;
        }

        var upLoc = Map.FirstOrDefault(m => m.X == loc.X && m.Y == loc.Y-1);
        if(upLoc != null && !upLoc.IsVoid() && (upLoc.IsNeutral() || upLoc.IsActiveOpponent()))
        {
            return upLoc;
        }
        
        var bottomLoc = Map.FirstOrDefault(m => m.X == loc.X && m.Y == loc.Y+1);
        if(bottomLoc != null && !bottomLoc.IsVoid() && (bottomLoc.IsNeutral() || bottomLoc.IsActiveOpponent()))
        {
            return bottomLoc;
        }

        return null;
    }
     public Location MoveTo(Location loc, Unit unit)
    {
        System.Console.Error.WriteLine("MOVE TO : ");
        // si je dois aller a droite
        var rightLoc = Map.FirstOrDefault(m => m.X == unit.Location.X+1 && m.Y == unit.Location.Y);
        if(unit.Location.X < loc.X && rightLoc != null && !rightLoc.IsVoid() && (rightLoc.IsNeutral() || rightLoc.IsActiveOpponent()))
        {
            System.Console.Error.WriteLine("right : " + rightLoc);
            return rightLoc;
        }
        // si je dois aller en bas
        var bottomLoc = Map.FirstOrDefault(m => m.X == unit.Location.X && m.Y == unit.Location.Y+1);
        if(unit.Location.Y < loc.Y && bottomLoc != null && !bottomLoc.IsVoid() && (bottomLoc.IsNeutral() || bottomLoc.IsActiveOpponent()))
        {
            System.Console.Error.WriteLine("down : " + bottomLoc);
            return bottomLoc;
        }
        // si je dois aller a gauche
        var leftLoc = Map.FirstOrDefault(m => m.X == unit.Location.X-1 && m.Y == unit.Location.Y);
        if(unit.Location.X > loc.X && leftLoc != null &&!leftLoc.IsVoid() && (leftLoc.IsNeutral() || leftLoc.IsActiveOpponent()))
        {
            System.Console.Error.WriteLine("left : " + leftLoc);
            return leftLoc;
        }
        // si je dois aller en haut
        var upLoc = Map.FirstOrDefault(m => m.X == unit.Location.X && m.Y == unit.Location.Y-1);
        if(unit.Location.Y > loc.Y && upLoc != null && !upLoc.IsVoid()  && (upLoc.IsNeutral() || upLoc.IsActiveOpponent()))
        {
            System.Console.Error.WriteLine("up : " + upLoc);
            return upLoc;
        }
        
        return null;
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
    public bool HaveEnoughGoldToUnitLvl2()
    {
        return Gold >= 20 && Income > 4;
    }
    public bool HaveEnoughGoldToUnitLvl3()
    {
        return Gold >= 30 && Income > 20;
    }
    public Location GetTrainLocation()
    {
        return Locations.LastOrDefault( l => !Units.Any(u => u.Location.X == l.X && u.Location.Y == l.Y) && !Buildings.Any(b => b.Location.X == l.X && b.Location.Y == l.Y));
    }

    public Location GetHQLocation()
    {
        return Buildings.FirstOrDefault(b => b.Type.Equals(BuildingType.Hq)).Location;
    }
    public Location GetLocationToTrainUnitAroundHQ()
    {
        var hqLocation = GetHQLocation();
        // si je n'ai pas d'unité à droite du HQ
        var rightLoc = new Location { X = hqLocation.X+1, Y = hqLocation.Y };
        if(Units.FirstOrDefault(u => u.Location.X.Equals(rightLoc.X) && u.Location.Y.Equals(rightLoc.Y)) == null)
        {
            return rightLoc;
        }
        // si je n'ai pas d'unité en bas du HQ
        var bottomLoc = new Location { X = hqLocation.X, Y = hqLocation.Y +1 };
        if(Units.FirstOrDefault(u => u.Location.X.Equals(bottomLoc.X) && u.Location.Y.Equals(bottomLoc.Y)) == null)
        {
            return bottomLoc;
        }
        // si je n'ai pas d'unité en bas a droite du HQ
        var rightBottomLoc = new Location { X = hqLocation.X+1, Y = hqLocation.Y +1 };
        if(Units.FirstOrDefault(u => u.Location.X.Equals(rightBottomLoc.X) && u.Location.Y.Equals(rightBottomLoc.Y)) == null)
        {
            return rightBottomLoc;
        }
        
        return null;
    }

    public Location GetBestLocationAroundUnit(Location unitLocation)
    {
        var rightLoc = Locations.FirstOrDefault(l => l.X == unitLocation.X+1 && l.Y == unitLocation.Y);
        if(rightLoc != null && Units.FirstOrDefault(u => u.Location.X == rightLoc.X && u.Location.Y == rightLoc.Y) == null)
        {
            return rightLoc;
        }
        var leftLoc = Locations.FirstOrDefault(l => l.X == unitLocation.X-1 && l.Y == unitLocation.Y);
        if(leftLoc != null && Units.FirstOrDefault(u => u.Location.X == leftLoc.X && u.Location.Y == leftLoc.Y) == null)
        {
            return leftLoc;
        }
        var upLoc = Locations.FirstOrDefault(l => l.X == unitLocation.X && l.Y == unitLocation.Y-1);
        if(upLoc != null && Units.FirstOrDefault(u => u.Location.X == upLoc.X && u.Location.Y == upLoc.Y) == null)
        {
            return upLoc;
        }
        var bottomLoc = Locations.FirstOrDefault(l => l.X == unitLocation.X && l.Y == unitLocation.Y+1);
        if(bottomLoc != null && Units.FirstOrDefault(u => u.Location.X == bottomLoc.X && u.Location.Y == bottomLoc.Y) == null)
        {
            return bottomLoc;
        }
        return null;
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
    public BuildingType Type { get; set; }
    public Location Location { get; set; } = new Location();

    public override string ToString()
    {
        return "Owner : " + Owner + " Type : " + Type + " " + Location;
    }
}