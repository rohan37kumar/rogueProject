using RLNET;
namespace rogueProject
{
    public class Player : Actor
    {
        public Player()
        {
            Awareness = 15;
            Name = "kc";
            Color = Colors.Player;
            Symbol = '@';
            X = 10;
            Y = 10;
            //we can add more properties to make our player special
            Attack = 2;
            AttackChance = 70;
            Defense = 2;
            DefenseChance = 40;
            Gold = 0;
            Health = 100;
            MaxHealth = 100;
            Speed = 8;
        }

        public void DrawStats(RLConsole statConsole)
        {
            //stats Printing
            statConsole.Print(1, 1, $"Name: {Name}", Colors.Text);
            statConsole.Print(1, 3, $"Health: {Health}/{MaxHealth}", Colors.Text);
            statConsole.Print(1, 5, $"Attack: {Attack} ({AttackChance}%)", Colors.Text);
            statConsole.Print(1, 7, $"Defense: {Defense} ({DefenseChance}%)", Colors.Text);
            statConsole.Print(1, 9, $"Gold: {Gold}", Colors.Gold);
        }
    }
}
