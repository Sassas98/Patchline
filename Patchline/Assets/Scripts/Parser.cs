
using System;
using System.Collections.Generic;

public class Parser : ConditionReader
{
    public List<Line> Parse(string[] code)
    {
        var list =  new List<Line>();
        foreach(var line in code)
        {
            int ls = line.Length - line.TrimStart().Length;
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
            CMD.Let => length != 3,
            CMD.If => length != 3,
            CMD.Elif => length != 3,
            CMD.Else => length != 0,
            CMD.For => length != 5,
            CMD.Loop => length != 3,
            CMD.Stop => length != 0,
            CMD.Skip => length != 0,
            CMD.Wait => length != 0,
            _ => true
        };
    }
}
