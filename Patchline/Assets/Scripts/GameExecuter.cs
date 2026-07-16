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
        private string[] Goals;

        private static string[] GetLines(string s) => s.Replace("\r", "")
            .ToLower().Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();

        public GameExecuter(string lineStd, string goals, string linePlayer)
            : this(GetLines(lineStd), GetLines(goals), GetLines(linePlayer)) { }

        public GameExecuter(string[] lineStd, string[] goals, string[] linePlayer)
        {
            var parser = new Parser();
            Goals = goals;
            Memory = new Memory();
            CompilerStd = new Compiler(Memory, parser.Parse(lineStd));
            CompilerPlayer = new Compiler(Memory, parser.Parse(linePlayer));
        }
        
        public GameData GetData()
        {
            var handler = new ExpressionHandler(Memory);
            return new GameData
            {
                StepCount = StepCount,
                Memory = Memory.GetState(),
                StdRow = CompilerStd.GetRow(),
                PlayerRow = CompilerPlayer.GetRow(),
                IsEnded = IsEnded,
                Goals = Goals.Select(g => {
                    var error = Memory.InError;
                    var result = new GoalResult
                    {
                        Label = g,
                        Result = handler.HandleCondition(g.Split(" "))
                    };
                    if(Memory.InError && !error)
                    {
                        Memory.ResetError();
                    }
                    return result;
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
