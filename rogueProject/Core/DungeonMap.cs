using RLNET;
using rogueProject;
using RogueSharp;
using System.Numerics;

public class DungeonMap : Map
{
    // The Draw method will be called each time the map is updated
    // It will render all of the symbols/colors for each cell to the map sub console

    public List<Rectangle> Rooms;
    private readonly List<Monster> _monsters;

    public DungeonMap()
    {
        Rooms = new List<Rectangle>();
        _monsters = new List<Monster>();
    }

    public void Draw(RLConsole mapConsole, RLConsole statConsole)
    {
        //mapConsole.Clear();
        foreach (Cell cell in GetAllCells())
        {
            SetConsoleSymbolForCell(mapConsole, cell);
        }

        int i = 0; //index which will help in monnster stat counting...

        foreach(Monster monster in _monsters)
        {
            monster.Draw(mapConsole, this);
            //if the monster is in fov then DrawStats()
            if(IsInFov(monster.X, monster.Y))
            {
                monster.DrawStats(statConsole, i);
                ++i;
            }
        }

    }

    private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
    {

        if (!cell.IsExplored)
        {
            return;
        }

        // checking for cells in FOV
        if (IsInFov(cell.X, cell.Y))
        {
            // '.' for floor and '#' for walls
            if (cell.IsWalkable)
            {
                console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
            }
            else
            {
                console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
            }
        }
        // outside FOV
        else
        {
            if (cell.IsWalkable)
            {
                console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
            }
            else
            {
                console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
            }
        }
    }

    public void UpdatePlayerFieldOfView()
    {
        Player player = Game.Player;
        ComputeFov(player.X, player.Y, player.Awareness, true);

        //marking cells
        foreach(Cell cell in GetAllCells())
        {
            if ((IsInFov(cell.X, cell.Y)))
            {
                SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            }
        }
    }

    public void SetIsWalkable(int x, int y, bool isWalkable) //helper function to set cell walkable properties
    {
        Cell cell = (Cell)GetCell(x, y);
        SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);

    }

    public bool SetActorPosition(Actor actor, int x, int y) //main function responsible for setting position of actors position...
    {
        if(GetCell(x, y).IsWalkable)
        {
            //prev cell now walkable
            SetIsWalkable(actor.X, actor.Y, true);

            //now update actor position
            actor.X = x;
            actor.Y = y;

            SetIsWalkable(actor.X, actor.Y, false);

            //and we'll update the FOV in case the actor is Player
            if(actor is Player)
            {
                UpdatePlayerFieldOfView();
            }
            return true;
        }
        return false;
    }

    public void AddPlayer(Player player)
    {
        Game.Player = player;
        SetIsWalkable(player.X, player.Y, false);
        Game.SchedulingSystem.Add(player);
        UpdatePlayerFieldOfView();
    }

    public void AddMonster(Monster monster)
    {
        _monsters.Add(monster);
        SetIsWalkable(monster.X, monster.Y, false); //false where monster is placed...
        Game.SchedulingSystem.Add(monster);
    }

    public void RemoveMonster(Monster monster)
    {
        _monsters.Remove(monster);
        SetIsWalkable(monster.X, monster.Y, true);
        Game.SchedulingSystem.Remove(monster);
    }
    public Monster GetMonsterAt(int x, int y)
    {
        //find the firsl monster which is found at these coords...
        return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
    } 

    public Point GetRandomWalkableLocationInRoom(Rectangle room)
    {
        if (DoesRoomHaveWalkableSpace(room))
        {
            for (int i = 1; i <= 100; i++)
            {
                int x = Game.Random.Next(1, room.Width - 2) + room.X;
                int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                if (IsWalkable(x, y))
                {
                    return new Point(x, y);
                }
            }
        }

        // we'll return null if no point was found, default value is null...
        return default(Point);
    }

    public bool DoesRoomHaveWalkableSpace(Rectangle room)
    {
        for (int x = 1; x <= room.Width - 2; x++)
        {
            for (int y = 1; y <= room.Height - 2; y++)
            {
                if (IsWalkable(x + room.X, y + room.Y))
                {
                    return true;
                }
            }
        }
        return false;
    }

}
