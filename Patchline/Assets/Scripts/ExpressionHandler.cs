using System.Collections.Generic;

public class ExpressionHandler : ConditionReader
{
    protected Memory memory;
    public ExpressionHandler(Memory mem)
    {
        this.memory = mem;
    }
    public bool HandleCondition(string[] args)
    {
        bool result = true;
        var list1 = new List<string>();
        var list2 = new List<string>();
        foreach (string arg in args)
        {
            if (IsCondition(arg) || arg.ToLower() == "and" || arg.ToLower() == "or")
            {
                list2.Add(HandleExpression(list1.ToArray()).ToString());
                list1.Clear();
                list2.Add(arg);
            }
            else
            {
                list1.Add(arg);
            }
        }
        list2.Add(HandleExpression(list1.ToArray()).ToString());
        args = list2.ToArray();
        for (int i = 0; i + 2 < args.Length && !memory.InError; i += 3)
        {
            var condition = GetCondition(args[1 + i]);
            result = memory.CheckCondition(args[0 + i], args[2 + i], condition);
            if (i + 3 == args.Length) break;
            if (args[i + 3].ToLower() == "and" && !result) return false;
            if (args[i + 3].ToLower() == "or" && result) return true;
        }
        return result && !memory.InError;
    }

    public int HandleExpression(string[] args)
    {
        int result = memory.Get(args[0]);
        for (int i = 1; i + 1 < args.Length && !memory.InError; i += 2)
        {
            var v2 = memory.Get(args[1 + i]);
            result = ApplyOperator(args[i], result, v2);
        }
        return result;
    }

    private int ApplyOperator(string op, int v1, int v2)
    {
        if (op == "+")
            return v1 + v2;
        else if (op == "-")
            return v1 - v2;
        else if (op == "*")
            return v1 * v2;
        else if (op == "/")
            return v1 / v2;
        else if (op == "%")
            return v1 % v2;
        else
        {
            memory.SetOnError($"Operatore {op} non riconosciuto");
            return -1;
        }
    }
}