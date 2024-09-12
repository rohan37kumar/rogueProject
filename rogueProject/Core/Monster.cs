using RLNET;
using rogueProject;

public class Monster : Actor
{
    //here we'll add more functionalities in the future
    public void DrawStats(RLConsole statConsole, int pos)
    {
        //we'll start at y=13, which is below player stats
        int yPos = 13 + (pos + 2);
        statConsole.Print(1, yPos, Symbol.ToString(), Color);

        int width = Convert.ToInt32(((double)Health / (double)MaxHealth) * 16.0);
        int remWidth = 16 - width;

        //setting the back color for healthbar effect
        statConsole.SetBackColor(3, yPos, width, 1, Swatch.Primary);
        statConsole.SetBackColor(3 + width, yPos, remWidth, 1, Swatch.PrimaryDarkest);

        //now print the name of the monster above health bar
        statConsole.Print(2, yPos, $": {Name}", Swatch.DbLight);

    }
}