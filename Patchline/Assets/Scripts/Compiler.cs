
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class Compiler
{
    private Memory memory;
    private List<Line> lines;
    private int row = 0;
    private bool ended = false;
    private Dictionary<int, bool> OptionHandled = new Dictionary<int, bool>();
    private Dictionary<int, int> LoopGates = new Dictionary<int, int>();

    public Compiler(Memory memory, List<Line> lines)
    {
        this.memory = memory;
        this.lines = lines;
    }

    public bool IsEnded()
    {
        return ended;
    }

    private Condition GetCondition(string condition)
    {
        return condition switch
        {
            "==" => Condition.Equal,
            "!=" => Condition.NotEqual,
            ">" => Condition.GreaterThan,
            "<" => Condition.LessThan,
            ">=" => Condition.GreaterThanOrEqual,
            "<=" => Condition.LessThanOrEqual,
            _ => throw new Exception($"Invalid condition: {condition}")
        };
    }
    public void MakesOneStep()
    {
        if(ended)
        {
            throw new Exception("Program has already ended");
        }
        if(this.row >= lines.Count)
        {
            ended = true;
            return;
        }
        var line = lines[this.row];
        if(line.Command == CMD.Let)
        {
            memory.Set(line.Args[0], line.Args[2]);
            this.row++;
        }
        else if (line.Command == CMD.If)
        {
            var condition = GetCondition(line.Args[1]);
            if (memory.CheckCondition(line.Args[0], line.Args[2], condition))
            {
                OptionHandled[line.LeftSpace] = true;
                this.row++;
            }
            else
            {
                OptionHandled[line.LeftSpace] = false;
                SkipToNextRelevantLine();
            }
        }
        else if (line.Command == CMD.Elif)
        {
            if (!OptionHandled.ContainsKey(line.LeftSpace))
            {
                throw new Exception("Elif without If");
            }
            if(OptionHandled[line.LeftSpace])
            {
                SkipToNextRelevantLine();
                return;
            }
            var condition = GetCondition(line.Args[1]);
            if (memory.CheckCondition(line.Args[0], line.Args[2], condition))
            {
                OptionHandled[line.LeftSpace] = true;
                this.row++;
            }
            else
            {
                SkipToNextRelevantLine();
            }
        }
        else if (line.Command == CMD.Else)
        {
            if (!OptionHandled.ContainsKey(line.LeftSpace))
            {
                throw new Exception("Else without If");
            }
            if (OptionHandled[line.LeftSpace])
            {
                SkipToNextRelevantLine();
                return;
            }
            this.row++;
        }
        else if (line.Command == CMD.For)
        {
            int now = this.row;
            SkipToNextRelevantLine();
            int from = memory.Get(line.Args[2]);
            int to = memory.Get(line.Args[4]);
            if(from > to)
            {
                return;
            }
            int value = memory.GetOrElse(line.Args[0], from - 1);
            memory.Set(line.Args[0], (++value).ToString());
            if (value <= to)
            {
                int end = this.row;
                this.row = now + 1;
                LoopGates[end] = now;
            }
        }
        else if (line.Command == CMD.Loop)
        {
            int now = this.row;
            SkipToNextRelevantLine();
            var condition = GetCondition(line.Args[1]);
            if (memory.CheckCondition(line.Args[0], line.Args[2], condition))
            {
                int end = this.row;
                this.row = now + 1;
                LoopGates[end] = now;
            }
        }
        else if (line.Command == CMD.Stop)
        {
            if(!LoopGates.Any()) 
                throw new Exception("Stop without Loop");
            this.row = LoopGates.Keys.Max();
            LoopGates.Remove(this.row);
        }
        else if (line.Command == CMD.Skip)
        {
            if (!LoopGates.Any())
                throw new Exception("Skip without Loop");
            var end = LoopGates.Keys.Max();
            this.row = LoopGates[end];
        }
        else if (line.Command == CMD.Wait)
        {
            this.row++;
        }
        if(LoopGates.ContainsKey(this.row))
        {
            var end = this.row;
            this.row = LoopGates[end];
            LoopGates.Remove(end);
        }
    }

    private void SkipToNextRelevantLine()
    {
        var line = lines[this.row];
        int currentIndent = line.LeftSpace;
        this.row++;
        while (this.row < lines.Count && lines[this.row].LeftSpace > currentIndent)
        {
            this.row++;
        }
    }
}
