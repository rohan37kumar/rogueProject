using rogueProject;
using RogueSharp;
using RogueSharp.DiceNotation;

public class MapGenerator
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _maxRooms;
    private readonly int _roomMaxSize;
    private readonly int _roomMinSize;

    private readonly DungeonMap _map;

    public MapGenerator(int width, int height,
    int maxRooms, int roomMaxSize, int roomMinSize)
    {
        _width = width;
        _height = height;
        _maxRooms = maxRooms;
        _roomMaxSize = roomMaxSize;
        _roomMinSize = roomMinSize;
        _map = new DungeonMap();
    }

    public DungeonMap CreateMap()
    {
        _map.Initialize(_width, _height);

        for (int r = _maxRooms; r > 0; r--)
        {
            // random declaration of sizes and position
            int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
            int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
            int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
            int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

            var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

            // checking for intersections and add to the list if no intersection
            bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));
            if (!newRoomIntersects)
            {
                _map.Rooms.Add(newRoom);
            }
        }
        foreach (Rectangle room in _map.Rooms)
        {
            CreateRoom(room);
        }
        int _noOfRoomGen = _map.Rooms.Count;
        //we have the list of rooms, wel'll start with r=1 index to last index and use helper methods to connect the hallways
        for(int r=1; r < _noOfRoomGen; r++)
        {
            int prevRoomCenterX, prevRoomCenterY;
            int currRoomCenterX, currRoomCenterY;

            prevRoomCenterX = _map.Rooms[r - 1].Center.X;
            prevRoomCenterY = _map.Rooms[r - 1].Center.Y;

            currRoomCenterX = _map.Rooms[r].Center.X;
            currRoomCenterY = _map.Rooms[r].Center.Y;

            if (Game.Random.Next(1, 2) == 1)
            {
                CreateHorizontalTunnel(prevRoomCenterX, currRoomCenterX, prevRoomCenterY);
                CreateVerticalTunnel(prevRoomCenterY, currRoomCenterY, currRoomCenterX);
            }
            else
            {
                CreateVerticalTunnel(prevRoomCenterY, currRoomCenterY, prevRoomCenterX);
                CreateHorizontalTunnel(prevRoomCenterX, currRoomCenterX, currRoomCenterY);
            }
        }

        PlacePlayer();
        PlaceMonsters();
        return _map;
    }
    private void CreateRoom(Rectangle room)
    {
        for (int x = room.Left + 1; x < room.Right; x++)
        {
            for (int y = room.Top + 1; y < room.Bottom; y++)
            {
                _map.SetCellProperties(x, y, true, true, false);
            }
        }
    }

    private void PlacePlayer()
    {
        Player player = Game.Player;

        if(player == null)
        {
            player = new Player();
        }

        player.X = _map.Rooms[0].Center.X;
        player.Y = _map.Rooms[0].Center.Y;

        _map.AddPlayer(player);
    }

    private void PlaceMonsters()
    {
        foreach (var room in _map.Rooms)
        {
            if (Dice.Roll("1D10") < 7) //60% chance monsters will be therein the room...
            {
                // now no. of monsters between 1-4...
                var numberOfMonsters = Dice.Roll("1D4");
                for (int i = 0; i < numberOfMonsters; i++)
                {
                    Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(room);
                    if (randomRoomLocation != default(Point)) //if not null then we'll add the monster
                    {
                        var monster = Kobold.Create(1); //temporary create it at level 1...
                        monster.X = randomRoomLocation.X;
                        monster.Y = randomRoomLocation.Y;
                        _map.AddMonster(monster);
                    }
                }
            }
        }
    }

    //tunnel creation methods
    //simple startegy to make cells walkable from one end to another and call them while creation of map
    //from one center to another...
    private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
    {
        for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
        {
            _map.SetCellProperties(x, yPosition, true, true);
        }
    }
    private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
    {
        for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
        {
            _map.SetCellProperties(xPosition, y, true, true);
        }
    }

}