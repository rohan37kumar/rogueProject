using rogueProject;
using RogueSharp.DiceNotation;
using System.Text;

public class CommandSystem
{
    // Return value is true if the player was able to move
    // false when the player couldn't move, such as trying to move into a wall
    public bool MovePlayer(Direction direction) 
    {
        int x = Game.Player.X;
        int y = Game.Player.Y;

        switch (direction)
        {
            case Direction.Up:
                {
                    y = Game.Player.Y - 1;
                    break;
                }
            case Direction.Down:
                {
                    y = Game.Player.Y + 1;
                    break;
                }
            case Direction.Left:
                {
                    x = Game.Player.X - 1;
                    break;
                }
            case Direction.Right:
                {
                    x = Game.Player.X + 1;
                    break;
                }
            default:
                {
                    return false;
                }
        }

        if (Game.DungeonMap.SetActorPosition(Game.Player, x, y))
        {
            return true;
        }

        //checking the bump attack here
        Monster monster = Game.DungeonMap.GetMonsterAt(x, y);
        if (monster != null)
        {
            Attack(Game.Player, monster);
            return true;
        }

        return false;
    }

    public void Attack(Actor attacker, Actor defender)
    {
        StringBuilder atkMsg = new StringBuilder();
        StringBuilder defMsg = new StringBuilder();

        //calculate hits and blocks from two helper methods
        int hits = calcAtk(attacker, defender, atkMsg);
        int blocks = calcDef(defender, hits, atkMsg, defMsg);

        //adding messages to the console
        Game.MessageLog.Add(atkMsg.ToString());
        if (!string.IsNullOrWhiteSpace(defMsg.ToString()))
        {
            Game.MessageLog.Add(defMsg.ToString());
        }

        int damage = hits - blocks;
        calcDamage(defender, damage);

    }
    //helper methods in Attack()
    private static int calcAtk(Actor attacker, Actor defender, StringBuilder atkMsg) //calculating no of attacks based on Dice rolls
    {
        int hits = 0;
        atkMsg.AppendFormat("{0} attacks {1} and rolls: ", attacker.Name, defender.Name);

        //roll a 100 sided die equal to number of attack points of the attacker
        DiceResult atkResult = new DiceExpression().Dice(attacker.Attack, 100).Roll();

        foreach(TermResult termresult in atkResult.Results)
        {
            atkMsg.Append(termresult.Value + ", ");

            //this is calculating chance of hit
            if (termresult.Value >= 100 - attacker.AttackChance)
            {
                hits++;
            }
        }
        return hits;
    }
    private static int calcDef(Actor defender, int hits, StringBuilder atkMSg, StringBuilder defMsg)
    {
        int blocks = 0;

        if (hits > 0)
        {
            atkMSg.AppendFormat("scoring {0} hits.", hits);
            defMsg.AppendFormat("  {0} defends and rolls: ", defender.Name);

            DiceResult defResult = new DiceExpression().Dice(defender.Defense, 100).Roll();

            foreach(TermResult termresult in defResult.Results)
            {
                defMsg.Append(termresult.Value + ", ");

                if(termresult.Value >= 100 - defender.DefenseChance)
                {
                    blocks++;
                }
            }
            defMsg.AppendFormat("resulting in {0} blocks.", blocks);
        }
        else
        {
            atkMSg.Append("and misses completely. ");
        }
        return blocks;
    }
    private static void calcDamage(Actor defender, int damage)
    {
        //simple reducing health in response to the damage dealt
        if (damage > 0)
        {
            defender.Health -= damage;
            Game.MessageLog.Add($"  {defender.Name} was hit for {damage} damage");

            if (defender.Health <= 0)
            {
                resolveDeath(defender); //the actor will die
            }
        }
        else
        {
            Game.MessageLog.Add($" {defender.Name} blocked all damage");
        }
    }
    private static void resolveDeath(Actor defender)
    {
        if (defender is Player)
        {
            Game.MessageLog.Add($"  {defender.Name} is now with millions who are no more!");
        }
        else if (defender is Monster)
        {
            Game.DungeonMap.RemoveMonster((Monster)defender);

            Game.MessageLog.Add($"  {defender.Name} died and dropped {defender.Gold} gold");
        }
    }

}

