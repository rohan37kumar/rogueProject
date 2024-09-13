using rogueProject;
using RogueSharp;

public class StandardMoveAndAttack : IBehaviour
{
    public bool Act(Monster monster, CommandSystem commandSystem)
    {
        DungeonMap dungeonMap = Game.DungeonMap;
        Player player = Game.Player;
        FieldOfView monsterFov = new FieldOfView(dungeonMap);

        //check if monster is alert
        //if player in fov then alert the monster

        if (!monster.TurnsAlerted.HasValue)
        {
            monsterFov.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
            if (monsterFov.IsInFov(player.X, player.Y))
            {
                Game.MessageLog.Add($"{monster.Name} wants to hug {player.Name}");
                monster.TurnsAlerted = 1;
            }
        }

        if (monster.TurnsAlerted.HasValue)
        {
            //first we'll set source & destination cells walkable
            //then compute the shortest path
            //then make them non-walkable again and the move the monster
            dungeonMap.SetIsWalkable(monster.X, monster.Y, true);
            dungeonMap.SetIsWalkable(player.X, player.Y, true);

            PathFinder pathFinder = new PathFinder(dungeonMap);
            RogueSharp.Path path = null;

            try
            {
                path = pathFinder.ShortestPath(dungeonMap.GetCell(monster.X, monster.Y), dungeonMap.GetCell(player.X, player.Y));
            }
            catch (PathNotFoundException)
            {
                //when the monster can't reach the player
                Game.MessageLog.Add($"{monster.Name} waits for it's turn");
            }

            
            dungeonMap.SetIsWalkable(monster.X, monster.Y, false);
            dungeonMap.SetIsWalkable(player.X, player.Y, false);

            //now move the moster if path found
            if (path != null)
            {
                try
                {
                    commandSystem.MoveMonster(monster, (Cell)path.StepForward());
                }
                catch (NoMoreStepsException)
                {
                    Game.MessageLog.Add($"{monster.Name} taunts from distance ");
                }
            }

            monster.TurnsAlerted++;

            //if for 15 turns the monster doesn't finds the player then it will stop being alert
            if (monster.TurnsAlerted > 15)
            {
                monster.TurnsAlerted = null;
            }
        }
        return true;
    }
}