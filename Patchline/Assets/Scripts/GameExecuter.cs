using System;
using System.Linq;
using UnityEditor;

namespace Assets.Scripts
{
    public class GameExecuter
    {
        private bool IsEnded = false;
        private bool PlayerTurn = false;
        private int StepCount = 0;
        private Memory Memory;
        private Compiler CompilerStd;
        private Compiler CompilerPlayer;
        private Goal[] Goals;

        private static string[] GetLines(string s) => s.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();

        public GameExecuter(string lineStd, string goals, string linePlayer)
            : this(GetLines(lineStd), GetLines(goals), GetLines(linePlayer)) { }

        public GameExecuter(string[] lineStd, string[] goals, string[] linePlayer)
        {
            var parser = new Parser();
            Goals = parser.GetGoals(goals);
            Memory = new Memory();
            CompilerStd = new Compiler(Memory, parser.Parse(lineStd));
            CompilerPlayer = new Compiler(Memory, parser.Parse(linePlayer));
        }
        
        public GameData GetData()
        {
            return new GameData
            {
                StepCount = StepCount,
                Memory = Memory.GetState(),
                StdRow = CompilerStd.GetRow(),
                PlayerRow = CompilerPlayer.GetRow(),
                IsEnded = IsEnded,
                Goals = Goals.Select(g => new GoalResult {
                    Label = g.Label,
                    Result = Memory.CheckCondition(g.Arg1, g.Arg2, g.Condition) 
                }).ToArray()
            }; 
        }

        public void MakeOneStep()
        {
            if(IsEnded)
            {
                throw new Exception("Game has already ended");
            }
            StepCount++;
            if (PlayerTurn)
            {
                CompilerPlayer.MakesOneStep();
                if(!CompilerStd.IsEnded())
                {
                    PlayerTurn = false;
                }
            }
            else
            {
                CompilerStd.MakesOneStep();
                if (!CompilerPlayer.IsEnded())
                {
                    PlayerTurn = true;
                }
            }
            if(CompilerStd.IsEnded() && CompilerPlayer.IsEnded())
            {
                IsEnded = true;
            }
        }
    }
}
