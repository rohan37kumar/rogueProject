using RLNET;
using rogueProject;
using RogueSharp;

public class Actor : IActor, IDrawable, IScheduleable
{

    private int _attack;
    private int _attackChance;
    private int _awareness;
    private int _defense;
    private int _defenseChance;
    private int _gold;
    private int _health;
    private int _maxHealth;
    private string _name;
    private int _speed;


    // getter & setter functions for both interfaces

    public int Attack
    {
        get
        {
            return _attack;
        }
        set
        {
            _attack = value;
        }
    }

    public int AttackChance
    {
        get
        {
            return _attackChance;
        }
        set
        {
            _attackChance = value;
        }
    }

    public int Awareness
    {
        get
        {
            return _awareness;
        }
        set
        {
            _awareness = value;
        }
    }

    public int Defense
    {
        get
        {
            return _defense;
        }
        set
        {
            _defense = value;
        }
    }

    public int DefenseChance
    {
        get
        {
            return _defenseChance;
        }
        set
        {
            _defenseChance = value;
        }
    }

    public int Gold
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
        }
    }

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }

    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    public int Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
        }
    }

    public RLColor Color { get; set; }
    public char Symbol { get; set; }
    public int X { get; set; }
    public int Y { get; set; }


    //inspect IMap for explored and inFOV cells
    public void Draw(RLConsole console, IMap map)
    {
        // unexplored
        if (!map.GetCell(X, Y).IsExplored)
        {
            return;
        }

        // in FOV
        if (map.IsInFov(X, Y))
        {
            console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
        }
        else
        {
            //not in FOV
            console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
        }
    }

    public int Time
    {
        get { return Speed; }
    }

}
    
