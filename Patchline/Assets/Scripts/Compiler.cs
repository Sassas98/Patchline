
using System;
using System.Collections.Generic;
using System.Linq;
public class Compiler : ConditionReader
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
            memory.Let(line.Args[0], HandleExpression(line.Args));
            this.row++;
        }
        else if(line.Command == CMD.Set)
        {
            memory.Set(line.Args[0], HandleExpression(line.Args));
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
        if(LoopGates.ContainsKey(this.row))
        {
            var end = this.row;
            this.row = LoopGates[end];
            LoopGates.Remove(end);
        }
    }

    private bool HandleCondition(string[] args){
        if((args.Length+1)%4!=0) 
        {
            memory.SetOnError("Condizione non corretta a riga " + this.row);
            return false;
        }
        bool result = true;
        for(int i = 0; i+2 < args.Length && !memory.InError; i+=3){
            var condition = GetCondition(args[1+i]);
            result = memory.CheckCondition(args[0+i], args[2+i], condition);
            if(i+3 == args.Length) break;
            if(args[i+3].ToLower() == "and" && !result) return false;
            if(args[i+3].ToLower() == "or" && result) return true;
        }
        return result;
    }

    private int HandleExpression(string[] args){
        if((args.Length+1)%2!=0 || args[1] != "=") 
        {
            memory.SetOnError("Espressione non corretta a riga " + this.row);
            return -1;
        }
        int result = memory.Get(args[2]);
        for(int i = 3; i+1 < args.Length && !memory.InError; i+=2){
            var v2 = memory.Get(args[1 + i]);
            result = ApplyOperator(args[i], result, v2);
        }
        return result;
    }

    private int ApplyOperator(string op, int v1, int v2){
        if(op == "+")
            return v1 + v2;
        else if(op == "-")
            return v1 - v2;
        else if(op == "*")
            return v1 * v2;
        else if(op == "/")
            return v1 / v2;
        else if(op == "%")
            return v1 % v2;
        else {
            memory.SetOnError("Operatore non riconosciuto a riga " + this.row);
            return -1;
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
