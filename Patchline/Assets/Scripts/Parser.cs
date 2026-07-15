
using System;
using System.Collections.Generic;

public class Parser : ConditionReader
{
    public List<Line> Parse(string[] code)
    {
        var list =  new List<Line>();
        foreach(var line in code)
        {
            int ls = (line.Length - line.TrimStart().Length) / 3;
            var parts = line.Trim().ToLower().Split(' ');
            var cmd = (CMD)Enum.Parse(typeof(CMD), parts[0], true);
            var args = parts.Length > 1 ? parts[1..] : new string[0];
            if (ArgomentiNonCorretti(cmd, args.Length))
            {
                throw new Exception($"Invalid command: {line}");
            }
            list.Add(new Line { Command = cmd, Args = args, LeftSpace = ls });
        }
        return list;
    }

    public Goal[] GetGoals(string[] code)
    {
        var goals = new List<Goal>();
        foreach (var line in code)
        {
            var parts = line.Trim().ToLower().Split(' ');
            goals.Add(new Goal { 
                Label = line.Trim().ToLower(),
                Arg1 = parts[0], 
                Arg2 = parts[2], 
                Condition = this.GetCondition(parts[1]) 
            });
        }
        return goals.ToArray();
    }

    private bool ArgomentiNonCorretti(CMD cmd, int length)
    {
        return cmd switch
        {
            CMD.Let => length < 3 || length % 2 == 0,
            CMD.Set => length < 3 || length % 2 == 0,
            CMD.If => (length + 1) % 4 != 0,
            CMD.Elif => (length + 1) % 4 != 0,
            CMD.Else => length != 0,
            CMD.Loop => (length + 1) % 4 != 0,
            CMD.Stop => length != 0,
            CMD.Skip => length != 0,
            CMD.Wait => length != 0,
            CMD.List => length != 1,
            CMD.Push => length < 3 || length % 2 == 0,
            CMD.Inject => length < 3 || length % 2 == 0,
            _ => true
        };
    }
}
