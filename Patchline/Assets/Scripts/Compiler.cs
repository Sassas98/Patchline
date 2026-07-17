
using System;
using System.Collections.Generic;
using System.Linq;
public class Compiler : ExpressionHandler
{
    private List<Line> lines;
    private int row = 0;
    private bool ended = false;
    private Dictionary<int, bool> OptionHandled = new Dictionary<int, bool>();
    private Dictionary<int, int> LoopGates = new Dictionary<int, int>();

    public Compiler(Memory memory, List<Line> lines)
        : base(memory)
    {
        this.lines = lines;
    }

    public bool IsEnded()
    {
        return ended;
    }
    public int GetRow()
    {
        return row;
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
            memory.Let(line.Args[0], HandleExpression(line.Args.Skip(2).ToArray()));
            this.row++;
        }
        else if(line.Command == CMD.Set)
        {
            memory.Set(line.Args[0], HandleExpression(line.Args.Skip(2).ToArray()));
            this.row++;
        }
        else if(line.Command == CMD.Push)
        {
            memory.Push(line.Args[0], HandleExpression(line.Args.Skip(2).ToArray()));
            this.row++;
        }
        else if(line.Command == CMD.Inject)
        {
            memory.Inject(line.Args[0], HandleExpression(line.Args.Skip(2).ToArray()));
            this.row++;
        }
        else if (line.Command == CMD.If)
        {
            if (HandleCondition(line.Args))
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
                MakesOneStep();
                return;
            }
            if (HandleCondition(line.Args))
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
                MakesOneStep();
                return;
            }
            this.row++;
        }
        else if (line.Command == CMD.Loop)
        {
            int now = this.row;
            SkipToNextRelevantLine();
            if (HandleCondition(line.Args))
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
        else if (line.Command == CMD.List)
        {
            memory.List(line.Args[0]);
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
