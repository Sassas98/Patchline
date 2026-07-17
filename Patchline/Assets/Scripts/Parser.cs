
using System;
using System.Collections.Generic;

public class Parser : ConditionReader
{
    public List<Line> Parse(string[] code)
    {
        var list =  new List<Line>();
        foreach(var line in code)
        {
            if(string.IsNullOrWhiteSpace(line)) continue;
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

    private bool ArgomentiNonCorretti(CMD cmd, int length)
    {
        return cmd switch
        {
            CMD.Let => length < 3 || length % 2 == 0,
            CMD.Set => length < 3 || length % 2 == 0,
            CMD.Else => length != 0,
            CMD.Stop => length != 0,
            CMD.Skip => length != 0,
            CMD.Wait => length != 0,
            CMD.List => length != 1,
            CMD.Push => length < 3 || length % 2 == 0,
            CMD.Inject => length < 3 || length % 2 == 0,
            _ => false
        };
    }
}
