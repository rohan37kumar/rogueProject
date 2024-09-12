using System.Xml.Linq;

public class SchedulingSystem
{
    private int _time;
    private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;

    public SchedulingSystem()
    {
        _time = 0;
        _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
    }

    //now we'll add new scheduleables to the list

    public void Add(IScheduleable scheduleable)
    {
        int key = _time + scheduleable.Time;
        //if key already present then add and if not we add a new Ischeduleable list
        if(!_scheduleables.ContainsKey(key))
        {
            _scheduleables.Add(key, new List<IScheduleable>());
        }
        _scheduleables[key].Add(scheduleable);
    }
    //now we'll need a Remove() to remove an entity from the Dictionary, kinda when a monster dies etc...
    public void Remove(IScheduleable scheduleable) //finds a pair, removes the schduleable from list, after that if list empty remove pair from dictionary
    {
        KeyValuePair<int, List<IScheduleable>> scheduleableListFound = new KeyValuePair<int, List<IScheduleable>>(-1, null);

        foreach (var scheduleablesList in _scheduleables)
        {
            if (scheduleablesList.Value.Contains(scheduleable))
            {
                scheduleableListFound = scheduleablesList;
                break;
            }
        }
        if (scheduleableListFound.Value != null)
        {
            scheduleableListFound.Value.Remove(scheduleable);
            if (scheduleableListFound.Value.Count <= 0)
            {
                _scheduleables.Remove(scheduleableListFound.Key);
            }
        }
    }

    //Get(), GetTime(), Clear()  :: 3 more helper methods
    public IScheduleable Get()
    {
        var firstSchGroup = _scheduleables.First();
        var firstSch = firstSchGroup.Value.First();
        Remove(firstSch);
        _time = firstSchGroup.Key;
        return firstSch;
    }
    public int GetTime()
    {
        return _time;
    }
    public void Clear()
    {
        _time = 0;
        _scheduleables.Clear();
    }

}
