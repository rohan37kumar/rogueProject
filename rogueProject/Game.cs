using RLNET;
using RogueSharp.Random;

namespace rogueProject
{
    public static class Game
    {
        private static bool _renderReq = true;
        public static CommandSystem CommandSystem { get; private set; }

        public static IRandom Random { get; private set; }

        //screen height and width are in number of tiles
        //root console
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static RLRootConsole _rootConsole;

        //map console
        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static RLConsole _map;

        //msg console
        private static readonly int _msgWidth = 80;
        private static readonly int _msgHeight = 11;
        private static RLConsole _msg;

        //stat console
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole _stat;

        //inventory cosole
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole _inventory;

        //getting DungeonMap
        public static DungeonMap DungeonMap { get; private set; }
        public static Player Player { get; set; }
        public static MessageLog MessageLog { get; private set; }


        public static void Main() //this is kinda similiar to Start() in Unity...
        {
            //string fontFileName = Convert.ToBase64String(System.IO.File.ReadAllBytes("terminal8x8.png"));
            int seed = (int) DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);
            string fontFileName = "terminal8x8.bmp";
            //string fontFileName = "testFont.bmp";
            string consoleTitle = $"rogueProject - v1.01 - Seed {seed}";

            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1f, consoleTitle);
            //Microsoft.Win32.SystemEvents
            //init subconsoles
            _map = new RLConsole(_mapWidth, _mapHeight);
            _msg = new RLConsole(_msgWidth, _msgHeight);
            _stat = new RLConsole(_statWidth, _statHeight);
            _inventory = new RLConsole(_inventoryWidth, _inventoryHeight);


            CommandSystem = new CommandSystem();
            //Player = new Player();
            MapGenerator mapGen = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7);
            DungeonMap = mapGen.CreateMap();
            DungeonMap.UpdatePlayerFieldOfView();

            MessageLog = new MessageLog();
            MessageLog.Add("KC arrives on level_0");
            MessageLog.Add($"Level created with the seed: '{seed}'");


            //setting color & text for each subconsoles...
            //_map.SetBackColor(0, 0, _mapWidth, _mapHeight, Colors.FloorBackground);
            _map.Print(1, 1, "Map", Colors.TextHeading);

            //_msg.SetBackColor(0, 0, _msgWidth, _msgHeight, Swatch.DbDeepWater);
            //_msg.Print(1, 1, "Console Messages", Colors.TextHeading);

            //_stat.SetBackColor(0, 0, _statWidth, _statHeight, Swatch.DbOldStone);
            //_stat.Print(1, 1, "Stats", Colors.TextHeading);

            _inventory.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Swatch.DbWood);
            _inventory.Print(1, 1, "Inventory", Colors.TextHeading);

            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;

            _rootConsole.Run(); //final entry point of console application...
        }

        //temp var to check console messages...
        //private static int _steps = 0;
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e) //and this one's like Update() in Unity
        {
            //_rootConsole.Print(10, 10, "It worked!", RLColor.Green);

            bool didPlayerAct = false; //to check if player did something...
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            if(keyPress != null)
            {
                //moving player based on input direction
                if (keyPress.Key == RLKey.Up)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                }
                else if (keyPress.Key == RLKey.Down)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                }
                else if (keyPress.Key == RLKey.Left)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                }
                else if (keyPress.Key == RLKey.Right)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                }
                else if (keyPress.Key == RLKey.Escape)
                {
                    _rootConsole.Close();
                }
            }

            if(didPlayerAct)
            {
                //MessageLog.Add($"Step #{++_steps}"); //display no of steps on console...
                _renderReq = true;
            }

        }
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e) //for rendering elements, DRAWING...
        {
            if (_renderReq) //only redraw the map if something happened else no need for code optimisation...
            {
                //clear old consoles before drawing
                _map.Clear();
                _stat.Clear();
                _msg.Clear();

                //drawing what we setup
                DungeonMap.Draw(_map, _stat);
                Player.Draw(_map, DungeonMap);
                Player.DrawStats(_stat);
                MessageLog.Draw(_msg);

                //here we BLIT all subconsoles into the root console;
                RLConsole.Blit(_map, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _inventoryHeight);
                RLConsole.Blit(_stat, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0);
                RLConsole.Blit(_msg, 0, 0, _msgWidth, _msgHeight, _rootConsole, 0, (_screenHeight - _msgHeight));
                RLConsole.Blit(_inventory, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole, 0, 0);


                _rootConsole.Draw(); //root console should be drawn at last to prevent any last key pressed delays...
                _renderReq = false;
            }
        }
    }
}
