using RLNET;

public class MessageLog
{
    //maxlines & the Queue
    private static readonly int _maxLines = 9;
    private readonly Queue<string> _lines;

    public MessageLog()
    {
        _lines = new Queue<string>(); //initialise when newGame
    }


    //add function
    public void Add(string message)
    {
        _lines.Enqueue(message);

        //to prevent overflow
        if (_lines.Count > _maxLines)
        {
            _lines.Dequeue();
        }
    }



    public void Draw(RLConsole console)
    {
        console.Clear();
        //simple Queue to array and print each line
        string[] lines = _lines.ToArray();
        for (int i = 0; i < lines.Length; i++)
        {
            console.Print(1, i + 1, lines[i], RLColor.White);
        }
    }
}