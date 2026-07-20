using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class LevelData
    {
        public string Code { get; set; }
        public string Goals { get; set; }
        public int Energy { get; set; }
    }

    public class LevelMaster
    {
        public const int LevelCount = 31;
        public const int StepsPerLevel = 5;

        public LevelData GetLevel(int level, int step)
        {
            if (level < 0 || level >= LevelCount)
                throw new ArgumentOutOfRangeException(nameof(level));
            if (step < 0 || step >= StepsPerLevel)
                throw new ArgumentOutOfRangeException(nameof(step));

            return datas[(level * StepsPerLevel) + step];
        }

        private static LevelData L(string code, string goals, int energy) => new()
        {
            Code = code,
            Goals = goals,
            Energy = energy
        };

        private readonly List<LevelData> datas = new()
        {
            // Level 0 - SET / WAIT
            L(@"LET A = 10
LET B = 15", @"A == 20", 7),
            L(@"WAIT
LET A = 2
SET A = A + 1", @"A == 9", 3),
            L(@"LET B = 3
LET C = 6
WAIT
SET B = B + 1
WAIT", @"B == 10
C != 7", 5),
            L(@"LET C = 4
LET D = 7
WAIT
SET C = C + 1
WAIT
SET C = C + 1
WAIT", @"C == 11
D < C", 7),
            L(@"LET CORE = 6
LET SIDE = 9
WAIT
SET CORE = CORE + 1
WAIT
SET CORE = CORE + 1
WAIT
SET CORE = CORE + 1
WAIT
SET CORE = CORE + 1
WAIT
SET CORE = CORE + 1", @"CORE == 13
SIDE <= 10", 12),

            // Level 1 - LET snapshots
            L(@"LET BASE = 5
SET BASE = BASE + 1
WAIT", @"COPY == 5
BASE >= 5", 2),
            L(@"LET SEED = 6
SET SEED = SEED + 1
WAIT
LET D0 = 0", @"SNAP == 6
SEED != 8
D0 < SEED", 2),
            L(@"LET RATE = 7
SET RATE = RATE + 1
WAIT
LET D0 = 1
SET D0 = D0 + 1
LET D1 = 2
LET D2 = 3", @"GAIN == 7
RATE > D0
D0 <= 3
D1 >= 1
D2 != 4", 2),
            L(@"LET LIM = 8
SET LIM = LIM + 1
WAIT
LET D0 = 2
LET D1 = 3
SET D1 = D1 + 1
LET D2 = 4
LET D3 = 5
SET D3 = D3 + 1
LET D4 = 6", @"SAVE == 8
LIM > SAVE
D0 >= 1
D1 != 5
D2 < D3
D3 == D4", 2),
            L(@"LET MODE = 9
SET MODE = MODE + 1
WAIT
LET D0 = 3
SET D0 = D0 + 1
LET D1 = 4
LET D2 = 5
SET D2 = D2 + 1
LET D3 = 6
LET D4 = 7
SET D4 = D4 + 1
SET D0 = D0 + 2", @"MARK == 9
MODE >= 9
D0 != 7
D1 < D2
D2 == D3
D3 <= 7
D4 >= 7", 2),

            // Level 2 - LET expressions
            L(@"LET A = 6
LET B = 8
SET A = A + 1
WAIT", @"SUM == 14
A != 8
B < SUM", 3),
            L(@"LET X = 7
LET Y = 9
SET X = X + 1
WAIT
LET D0 = 7", @"DIFF == -2
X < Y
Y > D0
D0 <= 8", 3),
            L(@"LET BASE = 8
LET RATE = 10
SET BASE = BASE + 1
WAIT
LET D0 = 8
LET D1 = 0
SET D1 = D1 + 1
LET D2 = 1", @"PROD == 80
BASE < PROD
RATE >= 9
D0 != 9
D1 == D2", 3),
            L(@"LET WIDE = 9
LET TALL = 11
SET WIDE = WIDE + 1
WAIT
LET D0 = 0
SET D0 = D0 + 1
LET D1 = 1
LET D2 = 2
SET D2 = D2 + 1
LET D3 = 3
LET D4 = 4", @"AREA == 99
TALL >= 10
WIDE != 11
D0 == D1
D1 < D2
D2 <= 4", 3),
            L(@"LET LEFT = 10
LET RITE = 12
SET LEFT = LEFT + 1
WAIT
LET D0 = 1
LET D1 = 2
SET D1 = D1 + 1
LET D2 = 3
LET D3 = 4
SET D3 = D3 + 1
LET D4 = 5
SET D0 = D0 + 1
SET D1 = D1 + 2", @"SPAN == -2
LEFT >= 10
RITE != 13
D0 < D1
D1 <= 6
D2 >= 2
D3 != 6", 3),

            // Level 3 - LOOP basics
            L(@"LET LIM = 3
LET STEP = 1
WAIT
LET D0 = 4", @"I == 3
SUM == 3
LIM > STEP", 9),
            L(@"LET LIM = 4
LET STEP = 2
WAIT
LET D0 = 5
LET D1 = 6
SET D1 = D1 + 1", @"I == 4
SUM == 8
LIM <= 5
STEP >= 1", 9),
            L(@"LET LIM = 5
LET STEP = 1
WAIT
LET D0 = 6
SET D0 = D0 + 1
LET D1 = 7
LET D2 = 8
SET D2 = D2 + 1
LET D3 = 0", @"I == 5
SUM == 5
LIM >= 4
STEP != 2
D0 == D1", 9),
            L(@"LET LIM = 3
LET STEP = 2
WAIT
LET D0 = 7
LET D1 = 8
SET D1 = D1 + 1
LET D2 = 0
LET D3 = 1
SET D3 = D3 + 1
LET D4 = 2
SET D0 = D0 + 1
SET D1 = D1 + 2", @"I == 3
SUM == 6
LIM != 4
STEP < SUM
D0 <= 9
D1 >= 10", 9),
            L(@"LET LIM = 4
LET STEP = 1
WAIT
LET D0 = 8
SET D0 = D0 + 1
LET D1 = 0
LET D2 = 1
SET D2 = D2 + 1
LET D3 = 2
LET D4 = 3
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"I == 4
SUM == 4
LIM > STEP
STEP < SUM
D0 >= 10
D1 != 2
D2 > D3", 9),

            // Level 4 - LOOP accumulators
            L(@"LET LIM = 2
LET BASE = 2
LET RATE = 1
WAIT
LET D0 = 2", @"I == 2
SUM == 4
BASE == I", 9),
            L(@"LET LIM = 3
LET BASE = 3
LET RATE = 2
WAIT
LET D0 = 3
SET D0 = D0 + 1
LET D1 = 4", @"I == 3
SUM == 9
BASE <= 4
LIM != 4", 9),
            L(@"LET LIM = 4
LET BASE = 4
LET RATE = 1
WAIT
LET D0 = 4
LET D1 = 5
SET D1 = D1 + 1
LET D2 = 6
LET D3 = 7
SET D3 = D3 + 1", @"I == 4
SUM == 8
BASE >= 3
LIM > RATE
RATE < SUM", 9),
            L(@"LET LIM = 2
LET BASE = 5
LET RATE = 2
WAIT
LET D0 = 5
SET D0 = D0 + 1
LET D1 = 6
LET D2 = 7
SET D2 = D2 + 1
LET D3 = 8
LET D4 = 0
SET D4 = D4 + 1
SET D0 = D0 + 2", @"I == 2
SUM == 20
BASE != 6
J == LIM
LIM <= 3
RATE >= 1", 16),
            L(@"LET LIM = 3
LET BASE = 6
LET RATE = 1
WAIT
LET D0 = 6
LET D1 = 7
SET D1 = D1 + 1
LET D2 = 8
LET D3 = 0
SET D3 = D3 + 1
LET D4 = 1
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"I == 3
SUM == 18
BASE > I
J <= 2
LIM >= 2
RATE != 2
D0 < D1", 16),

            // Level 5 - STOP basics
            L(@"LET BASE = 2
LET STEP = 2
WAIT
LET D0 = 0
SET D0 = D0 + 1
LET D1 = 1", @"SUM == 4
BASE <= 3
STEP >= 1", 7),
            L(@"LET BASE = 3
LET STEP = 3
WAIT
LET D0 = 1
LET D1 = 2
SET D1 = D1 + 1
LET D2 = 3
LET D3 = 4", @"SUM == 6
BASE >= 2
STEP != 4
D0 < D1", 7),
            L(@"LET BASE = 4
LET STEP = 2
WAIT
LET D0 = 2
SET D0 = D0 + 1
LET D1 = 3
LET D2 = 4
SET D2 = D2 + 1
LET D3 = 5
LET D4 = 6
SET D4 = D4 + 1", @"SUM == 6
BASE != 5
STEP < SUM
D0 <= 4
D1 >= 2", 7),
            L(@"LET BASE = 5
LET STEP = 3
WAIT
LET D0 = 3
LET D1 = 4
SET D1 = D1 + 1
LET D2 = 5
LET D3 = 6
SET D3 = D3 + 1
LET D4 = 7
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"SUM == 8
BASE > STEP
STEP < SUM
D0 >= 3
D1 != 8
D2 < D3", 7),
            L(@"LET BASE = 6
LET STEP = 2
WAIT
LET D0 = 4
SET D0 = D0 + 1
LET D1 = 5
LET D2 = 6
SET D2 = D2 + 1
LET D3 = 7
LET D4 = 8
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"SUM == 8
BASE > STEP
STEP <= 3
D0 != 8
D1 < D2
D2 > D3
D3 <= 9", 7),

            // Level 6 - STOP reinforcement
            L(@"LET RATE = 1
LET BIAS = 0
LET LIM = 3
WAIT
LET D0 = 7
LET D1 = 8
SET D1 = D1 + 1", @"CNT == 1
SUM == 1
BIAS >= -1", 11),
            L(@"LET RATE = 2
LET BIAS = 1
LET LIM = 4
WAIT
LET D0 = 8
SET D0 = D0 + 1
LET D1 = 0
LET D2 = 1
SET D2 = D2 + 1", @"CNT == 1
SUM == 3
BIAS != 2
LIM > RATE", 11),
            L(@"LET RATE = 3
LET BIAS = 2
LET LIM = 3
WAIT
LET D0 = 0
LET D1 = 1
SET D1 = D1 + 1
LET D2 = 2
LET D3 = 3
SET D3 = D3 + 1
LET D4 = 4
SET D0 = D0 + 1", @"CNT == 1
SUM == 5
BIAS > CNT
LIM <= 4
RATE >= 2", 11),
            L(@"LET RATE = 1
LET BIAS = 3
LET LIM = 4
WAIT
LET D0 = 1
SET D0 = D0 + 1
LET D1 = 2
LET D2 = 3
SET D2 = D2 + 1
LET D3 = 4
LET D4 = 5
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"CNT == 1
SUM == 4
BIAS > CNT
LIM >= 3
RATE != 2
D0 > D1", 11),
            L(@"LET RATE = 2
LET BIAS = 4
LET LIM = 3
WAIT
LET D0 = 2
LET D1 = 3
SET D1 = D1 + 1
LET D2 = 4
LET D3 = 5
SET D3 = D3 + 1
LET D4 = 6
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"CNT == 1
SUM == 6
BIAS <= 5
LIM != 4
RATE < SUM
D0 <= 6
D1 >= 5", 11),

            // Level 7 - SKIP reading
            L(@"LET LIM = 3
LET CNT = 0
LET SUM = 0
LOOP CNT < LIM
   SET CNT = CNT + 1
   SET SUM = SUM + CNT
   SKIP
   SET SUM = 99
WAIT", @"SAVE == 3
CNT != 4
LIM == SAVE
SUM <= 7", 9),
            L(@"LET LIM = 4
LET CNT = 0
LET SUM = 0
LOOP CNT < LIM
   SET CNT = CNT + 1
   SET SUM = SUM + CNT
   SKIP
   SET SUM = 99
WAIT
LET D0 = 6", @"SAVE == 4
CNT == LIM
LIM == SAVE
SUM >= 9
D0 != 7", 9),
            L(@"LET LIM = 5
LET CNT = 0
LET SUM = 0
LOOP CNT < LIM
   SET CNT = CNT + 1
   SET SUM = SUM + CNT
   SKIP
   SET SUM = 99
WAIT
LET D0 = 7
SET D0 = D0 + 1
LET D1 = 8
LET D2 = 0", @"SAVE == 5
CNT == LIM
LIM <= 6
SUM != 16
D0 == D1
D1 > D2", 9),
            L(@"LET LIM = 3
LET CNT = 0
LET SUM = 0
LOOP CNT < LIM
   SET CNT = CNT + 1
   SET SUM = SUM + CNT
   SKIP
   SET SUM = 99
WAIT
LET D0 = 8
LET D1 = 0
SET D1 = D1 + 1
LET D2 = 1
LET D3 = 2
SET D3 = D3 + 1
LET D4 = 3", @"SAVE == 3
CNT <= 4
LIM >= 2
SUM < D0
D0 > D1
D1 <= 2
D2 >= 0", 9),
            L(@"LET LIM = 4
LET CNT = 0
LET SUM = 0
LOOP CNT < LIM
   SET CNT = CNT + 1
   SET SUM = SUM + CNT
   SKIP
   SET SUM = 99
WAIT
LET D0 = 0
SET D0 = D0 + 1
LET D1 = 1
LET D2 = 2
SET D2 = D2 + 1
LET D3 = 3
LET D4 = 4
SET D4 = D4 + 1
SET D0 = D0 + 2", @"SAVE == 4
CNT >= 3
LIM != 5
SUM > D0
D0 <= 4
D1 >= 0
D2 != 4
D3 < D4", 9),

            // Level 8 - IF / ELSE
            L(@"LET TEMP = -2
LET FLAG = 0
SET TEMP = TEMP + 1
WAIT
LET D0 = 3
LET D1 = 4
SET D1 = D1 + 1
LET D2 = 5
LET D3 = 6", @"FLAG == 1
TEMP < D0
D0 <= 4
D1 >= 4", 5),
            L(@"LET SCORE = 55
LET FLAG = 0
SET SCORE = SCORE + 2
WAIT
LET D0 = 4
SET D0 = D0 + 1
LET D1 = 5
LET D2 = 6
SET D2 = D2 + 1
LET D3 = 7
LET D4 = 8", @"FLAG == -1
SCORE <= 58
D0 >= 4
D1 != 6
D2 == D3", 5),
            L(@"LET MODE = 2
LET FLAG = 0
SET MODE = MODE + 1
WAIT
LET D0 = 5
LET D1 = 6
SET D1 = D1 + 1
LET D2 = 7
LET D3 = 8
SET D3 = D3 + 1
LET D4 = 0
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1", @"FLAG == 1
MODE >= 2
D0 != 7
D1 > D2
D2 < D3
D3 <= 10", 5),
            L(@"LET LOAD = 8
LET FLAG = 0
SET LOAD = LOAD + 2
WAIT
LET D0 = 6
SET D0 = D0 + 1
LET D1 = 7
LET D2 = 8
SET D2 = D2 + 1
LET D3 = 0
LET D4 = 1
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"FLAG == 1
LOAD != 11
D0 > D1
D1 < D2
D2 <= 12
D3 >= 0
D4 != 3", 5),
            L(@"LET VALUE = 13
LET FLAG = 0
SET VALUE = VALUE + 1
WAIT
LET D0 = 7
LET D1 = 8
SET D1 = D1 + 1
LET D2 = 0
LET D3 = 1
SET D3 = D3 + 1
LET D4 = 2
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"FLAG == 1
VALUE > D0
D0 < D1
D1 <= 13
D2 >= 2
D3 != 5
D4 > FLAG", 5),

            // Level 9 - conditions in LOOP
            L(@"LET LIM = 3
LET I = 0
LET SUM = 0
LET CUT = 1
WAIT
LET D0 = 1
SET D0 = D0 + 1
LET D1 = 2
LET D2 = 3
SET D2 = D2 + 1", @"I == 3
SUM == 6
CUT < I
LIM >= 2", 10),
            L(@"LET LIM = 4
LET I = 0
LET SUM = 0
LET CUT = 2
WAIT
LET D0 = 2
LET D1 = 3
SET D1 = D1 + 1
LET D2 = 4
LET D3 = 5
SET D3 = D3 + 1
LET D4 = 6", @"I == 4
SUM == 11
CUT <= 3
LIM != 5
D0 < D1", 10),
            L(@"LET LIM = 5
LET I = 0
LET SUM = 0
LET CUT = 1
WAIT
LET D0 = 3
SET D0 = D0 + 1
LET D1 = 4
LET D2 = 5
SET D2 = D2 + 1
LET D3 = 6
LET D4 = 7
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1", @"I == 5
SUM == 15
CUT >= 0
LIM < SUM
D0 <= 7
D1 >= 4", 10),
            L(@"LET LIM = 3
LET I = 0
LET SUM = 0
LET CUT = 2
WAIT
LET D0 = 4
LET D1 = 5
SET D1 = D1 + 1
LET D2 = 6
LET D3 = 7
SET D3 = D3 + 1
LET D4 = 8
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"I == 3
SUM == 7
CUT != 3
LIM < SUM
D0 >= 6
D1 != 9
D2 < D3", 10),
            L(@"LET LIM = 4
LET I = 0
LET SUM = 0
LET CUT = 1
WAIT
LET D0 = 5
SET D0 = D0 + 1
LET D1 = 6
LET D2 = 7
SET D2 = D2 + 1
LET D3 = 8
LET D4 = 0
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"I == 4
SUM == 10
CUT < I
LIM <= 5
D0 != 10
D1 < D2
D2 > D3
D3 <= 10", 10),

            // Level 10 - compound conditions
            L(@"LET LIM = 4
LET I = 0
LET HIT = 0
LET SUM = 0
LET MOD = 2
WAIT
LET D0 = 8
LET D1 = 0
SET D1 = D1 + 1
LET D2 = 1
LET D3 = 2", @"HIT == 2
SUM == 9
I >= 3
LIM != 5", 12),
            L(@"LET LIM = 5
LET I = 0
LET HIT = 0
LET SUM = 0
LET MOD = 3
WAIT
LET D0 = 0
SET D0 = D0 + 1
LET D1 = 1
LET D2 = 2
SET D2 = D2 + 1
LET D3 = 3
LET D4 = 4", @"HIT == 2
SUM == 16
I != 6
LIM > MOD
MOD < SUM", 12),
            L(@"LET LIM = 4
LET I = 0
LET HIT = 0
LET SUM = 0
LET MOD = 2
WAIT
LET D0 = 1
LET D1 = 2
SET D1 = D1 + 1
LET D2 = 3
LET D3 = 4
SET D3 = D3 + 1
LET D4 = 5
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1", @"HIT == 2
SUM == 9
I == LIM
LIM > MOD
MOD <= 3
D0 != 3", 12),
            L(@"LET LIM = 5
LET I = 0
LET HIT = 0
LET SUM = 0
LET MOD = 3
WAIT
LET D0 = 2
SET D0 = D0 + 1
LET D1 = 3
LET D2 = 4
SET D2 = D2 + 1
LET D3 = 5
LET D4 = 6
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"HIT == 2
SUM == 16
I == LIM
LIM <= 6
MOD >= 2
D0 > D1
D1 < D2", 12),
            L(@"LET LIM = 4
LET I = 0
LET HIT = 0
LET SUM = 0
LET MOD = 2
WAIT
LET D0 = 3
LET D1 = 4
SET D1 = D1 + 1
LET D2 = 5
LET D3 = 6
SET D3 = D3 + 1
LET D4 = 7
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"HIT == 2
SUM == 9
I <= 5
LIM >= 3
MOD != 3
D0 < D1
D1 <= 9
D2 >= 7", 12),

            // Level 11 - ELIF / LIST
            L(@"LET LIM = 3
LET I = 0
LET LOW = 0
LET MID = 0
LET HIGH = 0
WAIT
LET D0 = 6
SET D0 = D0 + 1
LET D1 = 7
LET D2 = 8
SET D2 = D2 + 1
LET D3 = 0", @"LOW == 1
MID == 1
HIGH == 1
LENGTH:BUF == 0", 13),
            L(@"LET LIM = 4
LET I = 0
LET LOW = 0
LET MID = 0
LET HIGH = 0
WAIT
LET D0 = 7
LET D1 = 8
SET D1 = D1 + 1
LET D2 = 0
LET D3 = 1
SET D3 = D3 + 1
LET D4 = 2
SET D0 = D0 + 1", @"LOW == 1
MID == 2
HIGH == 1
LENGTH:BUF == 0
I == LIM", 13),
            L(@"LET LIM = 5
LET I = 0
LET LOW = 0
LET MID = 0
LET HIGH = 0
WAIT
LET D0 = 8
SET D0 = D0 + 1
LET D1 = 0
LET D2 = 1
SET D2 = D2 + 1
LET D3 = 2
LET D4 = 3
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"LOW == 1
MID == 3
HIGH == 1
LENGTH:BUF == 0
I == LIM
LIM <= 6", 13),
            L(@"LET LIM = 3
LET I = 0
LET LOW = 0
LET MID = 0
LET HIGH = 0
WAIT
LET D0 = 0
LET D1 = 1
SET D1 = D1 + 1
LET D2 = 2
LET D3 = 3
SET D3 = D3 + 1
LET D4 = 4
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1", @"LOW == 1
MID == 1
HIGH == 1
LENGTH:BUF == 0
I <= 4
LIM >= 2
D0 < D1", 13),
            L(@"LET LIM = 4
LET I = 0
LET LOW = 0
LET MID = 0
LET HIGH = 0
WAIT
LET D0 = 1
SET D0 = D0 + 1
LET D1 = 2
LET D2 = 3
SET D2 = D2 + 1
LET D3 = 4
LET D4 = 5
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1", @"LOW == 1
MID == 2
HIGH == 1
LENGTH:BUF == 0
I >= 3
LIM != 5
D0 <= 6
D1 >= 4", 13),

            // Level 12 - FIRST / LAST
            L(@"LIST DATA
PUSH DATA = 1
PUSH DATA = 3
PUSH DATA = 5
LET LIM = 2
LET I = 0
LET SUM = 0
WAIT
LET D0 = 4
LET D1 = 5
SET D1 = D1 + 1
LET D2 = 6
LET D3 = 7", @"HEAD == 1
TAIL == 5
SUM == 2
DATA == [1,3,5]
LENGTH:DATA == 3", 12),
            L(@"LIST DATA
PUSH DATA = 2
PUSH DATA = 4
PUSH DATA = 6
LET LIM = 2
LET I = 0
LET SUM = 0
WAIT
LET D0 = 5
SET D0 = D0 + 1
LET D1 = 6
LET D2 = 7
SET D2 = D2 + 1
LET D3 = 8
LET D4 = 0", @"HEAD == 2
TAIL == 6
SUM == 4
DATA == [2,4,6]
LENGTH:DATA == 3", 12),
            L(@"LIST DATA
PUSH DATA = 3
PUSH DATA = 5
PUSH DATA = 7
LET LIM = 2
LET I = 0
LET SUM = 0
WAIT
LET D0 = 6
LET D1 = 7
SET D1 = D1 + 1
LET D2 = 8
LET D3 = 0
SET D3 = D3 + 1
LET D4 = 1
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1", @"HEAD == 3
TAIL == 7
SUM == 6
DATA == [3,5,7]
LENGTH:DATA == 3
I <= 3", 12),
            L(@"LIST DATA
PUSH DATA = 4
PUSH DATA = 6
PUSH DATA = 8
LET LIM = 2
LET I = 0
LET SUM = 0
WAIT
LET D0 = 7
SET D0 = D0 + 1
LET D1 = 8
LET D2 = 0
SET D2 = D2 + 1
LET D3 = 1
LET D4 = 2
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"HEAD == 4
TAIL == 8
SUM == 8
DATA == [4,6,8]
LENGTH:DATA == 3
I >= 1
LIM != 3", 12),
            L(@"LIST DATA
PUSH DATA = 5
PUSH DATA = 7
PUSH DATA = 9
LET LIM = 2
LET I = 0
LET SUM = 0
WAIT
LET D0 = 8
LET D1 = 0
SET D1 = D1 + 1
LET D2 = 1
LET D3 = 2
SET D3 = D3 + 1
LET D4 = 3
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"HEAD == 5
TAIL == 9
SUM == 10
DATA == [5,7,9]
LENGTH:DATA == 3
I != 3
LIM < SUM
D0 >= 10", 12),

            // Level 13 - list reading
            L(@"LIST DATA
PUSH DATA = 2
PUSH DATA = 4
PUSH DATA = 7
LET I = 0
LET LIM = 3
LET BAND = 0
LET SUM = 0
WAIT
LET D0 = 2
SET D0 = D0 + 1
LET D1 = 3
LET D2 = 4
SET D2 = D2 + 1", @"BAND == 1
SUM == 3
DATA == [2,4,7]
LENGTH:DATA == 3
LENGTH:KEEP == 0", 19),
            L(@"LIST DATA
PUSH DATA = 3
PUSH DATA = 5
PUSH DATA = 8
LET I = 0
LET LIM = 3
LET BAND = 0
LET SUM = 0
WAIT
LET D0 = 3
LET D1 = 4
SET D1 = D1 + 1
LET D2 = 5
LET D3 = 6
SET D3 = D3 + 1
LET D4 = 7", @"BAND == 1
SUM == 3
DATA == [3,5,8]
LENGTH:DATA == 3
LENGTH:KEEP == 0", 19),
            L(@"LIST DATA
PUSH DATA = 4
PUSH DATA = 6
PUSH DATA = 9
LET I = 0
LET LIM = 3
LET BAND = 0
LET SUM = 0
WAIT
LET D0 = 4
SET D0 = D0 + 1
LET D1 = 5
LET D2 = 6
SET D2 = D2 + 1
LET D3 = 7
LET D4 = 8
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1", @"BAND == 2
SUM == 6
DATA == [4,6,9]
LENGTH:DATA == 3
LENGTH:KEEP == 0
I >= 2", 19),
            L(@"LIST DATA
PUSH DATA = 5
PUSH DATA = 7
PUSH DATA = 10
LET I = 0
LET LIM = 3
LET BAND = 0
LET SUM = 0
WAIT
LET D0 = 5
LET D1 = 6
SET D1 = D1 + 1
LET D2 = 7
LET D3 = 8
SET D3 = D3 + 1
LET D4 = 0
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"BAND == 2
SUM == 6
DATA == [5,7,10]
LENGTH:DATA == 3
LENGTH:KEEP == 0
I != 4
LIM < SUM", 19),
            L(@"LIST DATA
PUSH DATA = 6
PUSH DATA = 8
PUSH DATA = 11
LET I = 0
LET LIM = 3
LET BAND = 0
LET SUM = 0
WAIT
LET D0 = 6
SET D0 = D0 + 1
LET D1 = 7
LET D2 = 8
SET D2 = D2 + 1
LET D3 = 0
LET D4 = 1
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"BAND == 2
SUM == 6
DATA == [6,8,11]
LENGTH:DATA == 3
LENGTH:KEEP == 0
I == LIM
LIM < SUM
VAL >= 5", 19),

            // Level 14 - PUSH basics
            L(@"LET LIM = 3
LET I = 0
LET BASE = 1
WAIT
LET D0 = 0
LET D1 = 1
SET D1 = D1 + 1
LET D2 = 2
LET D3 = 3
SET D3 = D3 + 1
LET D4 = 4
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"I == 3
OUT == [1,2,3]
LENGTH:OUT == 3
BASE < I
LIM >= 2", 7),
            L(@"LET LIM = 4
LET I = 0
LET BASE = 1
WAIT
LET D0 = 1
SET D0 = D0 + 1
LET D1 = 2
LET D2 = 3
SET D2 = D2 + 1
LET D3 = 4
LET D4 = 5
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"I == 4
OUT == [1,2,3,4]
LENGTH:OUT == 4
BASE <= 2
LIM != 5
D0 > D1", 7),
            L(@"LET LIM = 5
LET I = 0
LET BASE = 1
WAIT
LET D0 = 2
LET D1 = 3
SET D1 = D1 + 1
LET D2 = 4
LET D3 = 5
SET D3 = D3 + 1
LET D4 = 6
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"I == 5
OUT == [1,2,3,4,5]
LENGTH:OUT == 5
BASE >= 0
LIM == D0
D0 < D1
D1 <= 8", 7),
            L(@"LET LIM = 3
LET I = 0
LET BASE = 1
WAIT
LET D0 = 3
SET D0 = D0 + 1
LET D1 = 4
LET D2 = 5
SET D2 = D2 + 1
LET D3 = 6
LET D4 = 7
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"I == 3
OUT == [1,2,3]
LENGTH:OUT == 3
BASE != 2
LIM < D0
D0 <= 10
D1 >= 6
D2 != 10", 7),
            L(@"LET LIM = 4
LET I = 0
LET BASE = 1
WAIT
LET D0 = 4
LET D1 = 5
SET D1 = D1 + 1
LET D2 = 6
LET D3 = 7
SET D3 = D3 + 1
LET D4 = 8
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"I == 4
OUT == [1,2,3,4]
LENGTH:OUT == 4
BASE < I
LIM <= 5
D0 >= 7
D1 != 12
D2 < D3
D3 > D4", 7),

            // Level 15 - PUSH filtering
            L(@"LET LIM = 5
LET I = 0
LET MOD = 2
LET SUM = 0
WAIT
LET D0 = 7
SET D0 = D0 + 1
LET D1 = 8
LET D2 = 0
SET D2 = D2 + 1
LET D3 = 1
LET D4 = 2
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"SUM == 6
OUT == [2,4]
LENGTH:OUT == 2
I <= 6
LIM >= 4", 15),
            L(@"LET LIM = 6
LET I = 0
LET MOD = 3
LET SUM = 0
WAIT
LET D0 = 8
LET D1 = 0
SET D1 = D1 + 1
LET D2 = 1
LET D3 = 2
SET D3 = D3 + 1
LET D4 = 3
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"SUM == 9
OUT == [3,6]
LENGTH:OUT == 2
I >= 5
LIM != 7
MOD > REM", 15),
            L(@"LET LIM = 5
LET I = 0
LET MOD = 2
LET SUM = 0
WAIT
LET D0 = 0
SET D0 = D0 + 1
LET D1 = 1
LET D2 = 2
SET D2 = D2 + 1
LET D3 = 3
LET D4 = 4
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1", @"SUM == 6
OUT == [2,4]
LENGTH:OUT == 2
I != 6
LIM > MOD
MOD > REM
REM <= 2", 15),
            L(@"LET LIM = 6
LET I = 0
LET MOD = 3
LET SUM = 0
WAIT
LET D0 = 1
LET D1 = 2
SET D1 = D1 + 1
LET D2 = 3
LET D3 = 4
SET D3 = D3 + 1
LET D4 = 5
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"SUM == 9
OUT == [3,6]
LENGTH:OUT == 2
I == LIM
LIM > MOD
MOD <= 4
REM >= -1
D0 < D1", 15),
            L(@"LET LIM = 5
LET I = 0
LET MOD = 2
LET SUM = 0
WAIT
LET D0 = 2
SET D0 = D0 + 1
LET D1 = 3
LET D2 = 4
SET D2 = D2 + 1
LET D3 = 5
LET D4 = 6
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"SUM == 6
OUT == [2,4]
LENGTH:OUT == 2
I == LIM
LIM <= 6
MOD >= 1
REM != 2
D0 > D1
D1 <= 8", 15),

            // Level 16 - PUSH transforms
            L(@"LET LIM = 3
LET I = 0
LET BASE = 2
LET SUM = 0
WAIT
LET D0 = 5
LET D1 = 6
SET D1 = D1 + 1
LET D2 = 7
LET D3 = 8
SET D3 = D3 + 1
LET D4 = 0
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"SUM == 12
VAL == 5
OUT == [3,4,5]
LENGTH:OUT == 3
BASE >= 1", 11),
            L(@"LET LIM = 4
LET I = 0
LET BASE = 3
LET SUM = 0
WAIT
LET D0 = 6
SET D0 = D0 + 1
LET D1 = 7
LET D2 = 8
SET D2 = D2 + 1
LET D3 = 0
LET D4 = 1
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"SUM == 22
VAL == 7
OUT == [4,5,6,7]
LENGTH:OUT == 4
BASE != 4
I == LIM", 11),
            L(@"LET LIM = 5
LET I = 0
LET BASE = 4
LET SUM = 0
WAIT
LET D0 = 7
LET D1 = 8
SET D1 = D1 + 1
LET D2 = 0
LET D3 = 1
SET D3 = D3 + 1
LET D4 = 2
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"SUM == 35
VAL == 9
OUT == [5,6,7,8,9]
LENGTH:OUT == 5
BASE < I
I == LIM
LIM <= 6", 11),
            L(@"LET LIM = 3
LET I = 0
LET BASE = 5
LET SUM = 0
WAIT
LET D0 = 8
SET D0 = D0 + 1
LET D1 = 0
LET D2 = 1
SET D2 = D2 + 1
LET D3 = 2
LET D4 = 3
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1", @"SUM == 21
VAL == 8
OUT == [6,7,8]
LENGTH:OUT == 3
BASE > I
I <= 4
LIM >= 2
D0 > D1", 11),
            L(@"LET LIM = 4
LET I = 0
LET BASE = 6
LET SUM = 0
WAIT
LET D0 = 0
LET D1 = 1
SET D1 = D1 + 1
LET D2 = 2
LET D3 = 3
SET D3 = D3 + 1
LET D4 = 4
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"SUM == 34
VAL == 10
OUT == [7,8,9,10]
LENGTH:OUT == 4
BASE <= 7
I >= 3
LIM != 5
D0 <= 5
D1 >= 6", 11),

            // Level 17 - INJECT basics
            L(@"LET LIM = 3
LET I = 0
WAIT
LET D0 = 3
SET D0 = D0 + 1
LET D1 = 4
LET D2 = 5
SET D2 = D2 + 1
LET D3 = 6
LET D4 = 7
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"I == 3
OUT == [3,2,1]
LENGTH:OUT == 3
LIM < D0
D0 == D1", 7),
            L(@"LET LIM = 4
LET I = 0
WAIT
LET D0 = 4
LET D1 = 5
SET D1 = D1 + 1
LET D2 = 6
LET D3 = 7
SET D3 = D3 + 1
LET D4 = 8
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"I == 4
OUT == [4,3,2,1]
LENGTH:OUT == 4
LIM < D0
D0 <= 8
D1 >= 8", 7),
            L(@"LET LIM = 5
LET I = 0
WAIT
LET D0 = 5
SET D0 = D0 + 1
LET D1 = 6
LET D2 = 7
SET D2 = D2 + 1
LET D3 = 8
LET D4 = 0
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1", @"I == 5
OUT == [5,4,3,2,1]
LENGTH:OUT == 5
LIM <= 6
D0 >= 10
D1 != 11
D2 == D3", 7),
            L(@"LET LIM = 3
LET I = 0
WAIT
LET D0 = 6
LET D1 = 7
SET D1 = D1 + 1
LET D2 = 8
LET D3 = 0
SET D3 = D3 + 1
LET D4 = 1
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"I == 3
OUT == [3,2,1]
LENGTH:OUT == 3
LIM >= 2
D0 != 13
D1 > D2
D2 > D3
D3 <= 7", 7),
            L(@"LET LIM = 4
LET I = 0
WAIT
LET D0 = 7
SET D0 = D0 + 1
LET D1 = 8
LET D2 = 0
SET D2 = D2 + 1
LET D3 = 1
LET D4 = 2
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"I == 4
OUT == [4,3,2,1]
LENGTH:OUT == 4
LIM != 5
D0 == D1
D1 > D2
D2 <= 7
D3 >= 4
D4 != 4", 7),

            // Level 18 - deque operations
            L(@"LIST SRC
PUSH SRC = 1
PUSH SRC = 3
PUSH SRC = 5
LET LIM = 3
LET I = 0
LET VAL = 0
WAIT
LET D0 = 1
LET D1 = 2
SET D1 = D1 + 1
LET D2 = 3
LET D3 = 4
SET D3 = D3 + 1
LET D4 = 5
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"I == 3
VAL == 5
LENGTH:SRC == 0
OUT == [5,3,1]
LENGTH:OUT == 3", 12),
            L(@"LIST SRC
PUSH SRC = 2
PUSH SRC = 4
PUSH SRC = 6
LET LIM = 3
LET I = 0
LET VAL = 0
WAIT
LET D0 = 2
SET D0 = D0 + 1
LET D1 = 3
LET D2 = 4
SET D2 = D2 + 1
LET D3 = 5
LET D4 = 6
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"I == 3
VAL == 6
LENGTH:SRC == 0
OUT == [6,4,2]
LENGTH:OUT == 3
LIM <= 4", 12),
            L(@"LIST SRC
PUSH SRC = 3
PUSH SRC = 5
PUSH SRC = 7
LET LIM = 3
LET I = 0
LET VAL = 0
WAIT
LET D0 = 3
LET D1 = 4
SET D1 = D1 + 1
LET D2 = 5
LET D3 = 6
SET D3 = D3 + 1
LET D4 = 7
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"I == 3
VAL == 7
LENGTH:SRC == 0
OUT == [7,5,3]
LENGTH:OUT == 3
LIM >= 2
D0 < D1", 12),
            L(@"LIST SRC
PUSH SRC = 4
PUSH SRC = 6
PUSH SRC = 8
LET LIM = 3
LET I = 0
LET VAL = 0
WAIT
LET D0 = 4
SET D0 = D0 + 1
LET D1 = 5
LET D2 = 6
SET D2 = D2 + 1
LET D3 = 7
LET D4 = 8
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"I == 3
VAL == 8
LENGTH:SRC == 0
OUT == [8,6,4]
LENGTH:OUT == 3
LIM != 4
D0 > D1
D1 <= 9", 12),
            L(@"LIST SRC
PUSH SRC = 5
PUSH SRC = 7
PUSH SRC = 9
LET LIM = 3
LET I = 0
LET VAL = 0
WAIT
LET D0 = 5
LET D1 = 6
SET D1 = D1 + 1
LET D2 = 7
LET D3 = 8
SET D3 = D3 + 1
LET D4 = 0
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"I == 3
VAL == 9
LENGTH:SRC == 0
OUT == [9,7,5]
LENGTH:OUT == 3
LIM < VAL
D0 <= 10
D1 >= 11
D2 != 12", 12),

            // Level 19 - list filtering
            L(@"LIST SRC
PUSH SRC = 1
PUSH SRC = 4
PUSH SRC = 7
PUSH SRC = 2
LET CUT = 4
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 8
SET D0 = D0 + 1
LET D1 = 0
LET D2 = 1
SET D2 = D2 + 1
LET D3 = 2
LET D4 = 3
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1", @"SUM == 11
CNT == 2
VAL == 2
LENGTH:SRC == 0
OUT == [4,7]
LENGTH:OUT == 2", 16),
            L(@"LIST SRC
PUSH SRC = 2
PUSH SRC = 5
PUSH SRC = 8
PUSH SRC = 3
LET CUT = 5
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 0
LET D1 = 1
SET D1 = D1 + 1
LET D2 = 2
LET D3 = 3
SET D3 = D3 + 1
LET D4 = 4
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"SUM == 13
CNT == 2
VAL == 3
LENGTH:SRC == 0
OUT == [5,8]
LENGTH:OUT == 2", 16),
            L(@"LIST SRC
PUSH SRC = 1
PUSH SRC = 4
PUSH SRC = 9
PUSH SRC = 2
LET CUT = 4
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 1
SET D0 = D0 + 1
LET D1 = 2
LET D2 = 3
SET D2 = D2 + 1
LET D3 = 4
LET D4 = 5
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"SUM == 13
CNT == 2
VAL == 2
LENGTH:SRC == 0
OUT == [4,9]
LENGTH:OUT == 2
CUT != 5", 16),
            L(@"LIST SRC
PUSH SRC = 2
PUSH SRC = 5
PUSH SRC = 10
PUSH SRC = 3
LET CUT = 5
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 2
LET D1 = 3
SET D1 = D1 + 1
LET D2 = 4
LET D3 = 5
SET D3 = D3 + 1
LET D4 = 6
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"SUM == 15
CNT == 2
VAL == 3
LENGTH:SRC == 0
OUT == [5,10]
LENGTH:OUT == 2
CUT < SUM
D0 >= 5", 16),
            L(@"LIST SRC
PUSH SRC = 1
PUSH SRC = 4
PUSH SRC = 11
PUSH SRC = 2
LET CUT = 4
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 3
SET D0 = D0 + 1
LET D1 = 4
LET D2 = 5
SET D2 = D2 + 1
LET D3 = 6
LET D4 = 7
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1", @"SUM == 15
CNT == 2
VAL == 2
LENGTH:SRC == 0
OUT == [4,11]
LENGTH:OUT == 2
CUT < SUM
D0 != 10
D1 < D2", 16),

            // Level 20 - map / reduce
            L(@"LIST SRC
PUSH SRC = 1
PUSH SRC = 2
PUSH SRC = 3
LET RATE = 2
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 6
LET D1 = 7
SET D1 = D1 + 1
LET D2 = 8
LET D3 = 0
SET D3 = D3 + 1
LET D4 = 1
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"SUM == 12
VAL == 6
LENGTH:SRC == 0
OUT == [2,4,6]
LENGTH:OUT == 3", 12),
            L(@"LIST SRC
PUSH SRC = 2
PUSH SRC = 3
PUSH SRC = 4
LET RATE = 3
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 7
SET D0 = D0 + 1
LET D1 = 8
LET D2 = 0
SET D2 = D2 + 1
LET D3 = 1
LET D4 = 2
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"SUM == 27
VAL == 12
LENGTH:SRC == 0
OUT == [6,9,12]
LENGTH:OUT == 3
RATE >= 2", 12),
            L(@"LIST SRC
PUSH SRC = 3
PUSH SRC = 4
PUSH SRC = 5
LET RATE = 2
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 8
LET D1 = 0
SET D1 = D1 + 1
LET D2 = 1
LET D3 = 2
SET D3 = D3 + 1
LET D4 = 3
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"SUM == 24
VAL == 10
LENGTH:SRC == 0
OUT == [6,8,10]
LENGTH:OUT == 3
RATE != 3
D0 <= 13", 12),
            L(@"LIST SRC
PUSH SRC = 4
PUSH SRC = 5
PUSH SRC = 6
LET RATE = 3
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 0
SET D0 = D0 + 1
LET D1 = 1
LET D2 = 2
SET D2 = D2 + 1
LET D3 = 3
LET D4 = 4
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"SUM == 45
VAL == 18
LENGTH:SRC == 0
OUT == [12,15,18]
LENGTH:OUT == 3
RATE < SUM
D0 >= 5
D1 != 6", 12),
            L(@"LIST SRC
PUSH SRC = 5
PUSH SRC = 6
PUSH SRC = 7
LET RATE = 2
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 1
LET D1 = 2
SET D1 = D1 + 1
LET D2 = 3
LET D3 = 4
SET D3 = D3 + 1
LET D4 = 5
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"SUM == 36
VAL == 14
LENGTH:SRC == 0
OUT == [10,12,14]
LENGTH:OUT == 3
RATE < SUM
D0 != 8
D1 > D2
D2 < D3", 12),

            // Level 21 - retry queues
            L(@"LIST QUE
PUSH QUE = 1
PUSH QUE = 6
PUSH QUE = 2
LET CUT = 5
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 4
SET D0 = D0 + 1
LET D1 = 5
LET D2 = 6
SET D2 = D2 + 1
LET D3 = 7
LET D4 = 8
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"RET == 2
VAL == 7
LENGTH:QUE == 0
DONE == [6,6,7]
LENGTH:DONE == 3
CUT >= 4", 15),
            L(@"LIST QUE
PUSH QUE = 2
PUSH QUE = 7
PUSH QUE = 3
LET CUT = 6
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 5
LET D1 = 6
SET D1 = D1 + 1
LET D2 = 7
LET D3 = 8
SET D3 = D3 + 1
LET D4 = 0
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"RET == 2
VAL == 9
LENGTH:QUE == 0
DONE == [8,7,9]
LENGTH:DONE == 3
CUT != 7
D0 <= 9", 15),
            L(@"LIST QUE
PUSH QUE = 1
PUSH QUE = 8
PUSH QUE = 2
LET CUT = 5
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 6
SET D0 = D0 + 1
LET D1 = 7
LET D2 = 8
SET D2 = D2 + 1
LET D3 = 0
LET D4 = 1
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"RET == 2
VAL == 7
LENGTH:QUE == 0
DONE == [6,8,7]
LENGTH:DONE == 3
CUT > RET
D0 >= 11
D1 != 11", 15),
            L(@"LIST QUE
PUSH QUE = 2
PUSH QUE = 9
PUSH QUE = 3
LET CUT = 6
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 7
LET D1 = 8
SET D1 = D1 + 1
LET D2 = 0
LET D3 = 1
SET D3 = D3 + 1
LET D4 = 2
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"RET == 2
VAL == 9
LENGTH:QUE == 0
DONE == [8,9,9]
LENGTH:DONE == 3
CUT > RET
D0 != 12
D1 > D2
D2 < D3", 15),
            L(@"LIST QUE
PUSH QUE = 1
PUSH QUE = 10
PUSH QUE = 2
LET CUT = 5
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 8
SET D0 = D0 + 1
LET D1 = 0
LET D2 = 1
SET D2 = D2 + 1
LET D3 = 2
LET D4 = 3
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"RET == 2
VAL == 7
LENGTH:QUE == 0
DONE == [6,10,7]
LENGTH:DONE == 3
CUT <= 6
D0 > D1
D1 < D2
D2 <= 8
D3 >= 5", 15),

            // Level 22 - stack transforms
            L(@"LIST STK
PUSH STK = 1
PUSH STK = 3
PUSH STK = 5
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 2
LET D1 = 3
SET D1 = D1 + 1
LET D2 = 4
LET D3 = 5
SET D3 = D3 + 1
LET D4 = 6
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"SUM == 9
VAL == 1
LENGTH:STK == 0
OUT == [5,3,1]
LENGTH:OUT == 3
D0 < D1", 10),
            L(@"LIST STK
PUSH STK = 2
PUSH STK = 4
PUSH STK = 6
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 3
SET D0 = D0 + 1
LET D1 = 4
LET D2 = 5
SET D2 = D2 + 1
LET D3 = 6
LET D4 = 7
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"SUM == 12
VAL == 2
LENGTH:STK == 0
OUT == [6,4,2]
LENGTH:OUT == 3
D0 <= 8
D1 >= 6", 10),
            L(@"LIST STK
PUSH STK = 3
PUSH STK = 5
PUSH STK = 7
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 4
LET D1 = 5
SET D1 = D1 + 1
LET D2 = 6
LET D3 = 7
SET D3 = D3 + 1
LET D4 = 8
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"SUM == 15
VAL == 3
LENGTH:STK == 0
OUT == [7,5,3]
LENGTH:OUT == 3
D0 >= 7
D1 != 12
D2 < D3", 10),
            L(@"LIST STK
PUSH STK = 4
PUSH STK = 6
PUSH STK = 8
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 5
SET D0 = D0 + 1
LET D1 = 6
LET D2 = 7
SET D2 = D2 + 1
LET D3 = 8
LET D4 = 0
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"SUM == 18
VAL == 4
LENGTH:STK == 0
OUT == [8,6,4]
LENGTH:OUT == 3
D0 != 13
D1 < D2
D2 > D3
D3 <= 13", 10),
            L(@"LIST STK
PUSH STK = 5
PUSH STK = 7
PUSH STK = 9
LET VAL = 0
LET SUM = 0
WAIT
LET D0 = 6
LET D1 = 7
SET D1 = D1 + 1
LET D2 = 8
LET D3 = 0
SET D3 = D3 + 1
LET D4 = 1
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"SUM == 21
VAL == 5
LENGTH:STK == 0
OUT == [9,7,5]
LENGTH:OUT == 3
D0 < D1
D1 == D2
D2 <= 15
D3 >= 6
D4 != 2", 10),

            // Level 23 - dual buffers
            L(@"LIST A
PUSH A = 1
PUSH A = 4
PUSH A = 7
LIST B
PUSH B = 2
PUSH B = 5
LET VAL = 0
LET CNT = 0
WAIT
LET D0 = 0
SET D0 = D0 + 1
LET D1 = 1
LET D2 = 2
SET D2 = D2 + 1
LET D3 = 3
LET D4 = 4
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"CNT == 1
LENGTH:A == 0
LENGTH:B == 0
OUT == [1,2,4,5,7]
LENGTH:OUT == 5
VAL < D0", 17),
            L(@"LIST A
PUSH A = 2
PUSH A = 5
PUSH A = 8
LIST B
PUSH B = 3
PUSH B = 6
LET VAL = 0
LET CNT = 0
WAIT
LET D0 = 1
LET D1 = 2
SET D1 = D1 + 1
LET D2 = 3
LET D3 = 4
SET D3 = D3 + 1
LET D4 = 5
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"CNT == 1
LENGTH:A == 0
LENGTH:B == 0
OUT == [2,3,5,6,8]
LENGTH:OUT == 5
VAL <= 1
D0 >= 3", 17),
            L(@"LIST A
PUSH A = 3
PUSH A = 6
PUSH A = 9
LIST B
PUSH B = 4
PUSH B = 7
LET VAL = 0
LET CNT = 0
WAIT
LET D0 = 2
SET D0 = D0 + 1
LET D1 = 3
LET D2 = 4
SET D2 = D2 + 1
LET D3 = 5
LET D4 = 6
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"CNT == 1
LENGTH:A == 0
LENGTH:B == 0
OUT == [3,4,6,7,9]
LENGTH:OUT == 5
VAL >= -1
D0 != 9
D1 < D2", 17),
            L(@"LIST A
PUSH A = 4
PUSH A = 7
PUSH A = 10
LIST B
PUSH B = 5
PUSH B = 8
LET VAL = 0
LET CNT = 0
WAIT
LET D0 = 3
LET D1 = 4
SET D1 = D1 + 1
LET D2 = 5
LET D3 = 6
SET D3 = D3 + 1
LET D4 = 7
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"CNT == 1
LENGTH:A == 0
LENGTH:B == 0
OUT == [4,5,7,8,10]
LENGTH:OUT == 5
VAL != 1
D0 < D1
D1 > D2
D2 <= 10", 17),
            L(@"LIST A
PUSH A = 5
PUSH A = 8
PUSH A = 11
LIST B
PUSH B = 6
PUSH B = 9
LET VAL = 0
LET CNT = 0
WAIT
LET D0 = 4
SET D0 = D0 + 1
LET D1 = 5
LET D2 = 6
SET D2 = D2 + 1
LET D3 = 7
LET D4 = 8
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"CNT == 1
LENGTH:A == 0
LENGTH:B == 0
OUT == [5,6,8,9,11]
LENGTH:OUT == 5
VAL < D0
D0 > D1
D1 <= 10
D2 >= 11
D3 != 12", 17),

            // Level 24 - threshold routing
            L(@"LIST SRC
PUSH SRC = 2
PUSH SRC = 6
PUSH SRC = 10
PUSH SRC = 4
LET LOW = 5
LET HIGH = 9
LET VAL = 0
WAIT
LET D0 = 7
LET D1 = 8
SET D1 = D1 + 1
LET D2 = 0
LET D3 = 1
SET D3 = D3 + 1
LET D4 = 2
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"VAL == 4
LENGTH:SRC == 0
A == [2,4]
LENGTH:A == 2
B == [6]
LENGTH:B == 1
C == [10]
LENGTH:C == 1", 20),
            L(@"LIST SRC
PUSH SRC = 3
PUSH SRC = 7
PUSH SRC = 11
PUSH SRC = 5
LET LOW = 6
LET HIGH = 10
LET VAL = 0
WAIT
LET D0 = 8
SET D0 = D0 + 1
LET D1 = 0
LET D2 = 1
SET D2 = D2 + 1
LET D3 = 2
LET D4 = 3
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"VAL == 5
LENGTH:SRC == 0
A == [3,5]
LENGTH:A == 2
B == [7]
LENGTH:B == 1
C == [11]
LENGTH:C == 1", 20),
            L(@"LIST SRC
PUSH SRC = 2
PUSH SRC = 6
PUSH SRC = 12
PUSH SRC = 4
LET LOW = 5
LET HIGH = 9
LET VAL = 0
WAIT
LET D0 = 0
LET D1 = 1
SET D1 = D1 + 1
LET D2 = 2
LET D3 = 3
SET D3 = D3 + 1
LET D4 = 4
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"VAL == 4
LENGTH:SRC == 0
A == [2,4]
LENGTH:A == 2
B == [6]
LENGTH:B == 1
C == [12]
LENGTH:C == 1", 20),
            L(@"LIST SRC
PUSH SRC = 3
PUSH SRC = 7
PUSH SRC = 13
PUSH SRC = 5
LET LOW = 6
LET HIGH = 10
LET VAL = 0
WAIT
LET D0 = 1
SET D0 = D0 + 1
LET D1 = 2
LET D2 = 3
SET D2 = D2 + 1
LET D3 = 4
LET D4 = 5
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"VAL == 5
LENGTH:SRC == 0
A == [3,5]
LENGTH:A == 2
B == [7]
LENGTH:B == 1
C == [13]
LENGTH:C == 1
HIGH != 11", 20),
            L(@"LIST SRC
PUSH SRC = 2
PUSH SRC = 6
PUSH SRC = 14
PUSH SRC = 4
LET LOW = 5
LET HIGH = 9
LET VAL = 0
WAIT
LET D0 = 2
LET D1 = 3
SET D1 = D1 + 1
LET D2 = 4
LET D3 = 5
SET D3 = D3 + 1
LET D4 = 6
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"VAL == 4
LENGTH:SRC == 0
A == [2,4]
LENGTH:A == 2
B == [6]
LENGTH:B == 1
C == [14]
LENGTH:C == 1
HIGH > LOW
LOW > VAL", 20),

            // Level 25 - bounded retries
            L(@"LIST QUE
PUSH QUE = 1
PUSH QUE = 6
PUSH QUE = 2
LET CUT = 5
LET LIM = 2
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 5
SET D0 = D0 + 1
LET D1 = 6
LET D2 = 7
SET D2 = D2 + 1
LET D3 = 8
LET D4 = 0
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"RET == 2
VAL == 2
QUE == [7]
LENGTH:QUE == 1
DONE == [6,6]
LENGTH:DONE == 2", 21),
            L(@"LIST QUE
PUSH QUE = 2
PUSH QUE = 7
PUSH QUE = 3
LET CUT = 6
LET LIM = 3
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 6
LET D1 = 7
SET D1 = D1 + 1
LET D2 = 8
LET D3 = 0
SET D3 = D3 + 1
LET D4 = 1
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"RET == 2
VAL == 9
LENGTH:QUE == 0
DONE == [8,7,9]
LENGTH:DONE == 3
CUT >= 5
LIM != 4", 21),
            L(@"LIST QUE
PUSH QUE = 1
PUSH QUE = 8
PUSH QUE = 2
LET CUT = 5
LET LIM = 2
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 7
SET D0 = D0 + 1
LET D1 = 8
LET D2 = 0
SET D2 = D2 + 1
LET D3 = 1
LET D4 = 2
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"RET == 2
VAL == 2
QUE == [7]
LENGTH:QUE == 1
DONE == [6,8]
LENGTH:DONE == 2
CUT != 6
LIM == RET", 21),
            L(@"LIST QUE
PUSH QUE = 2
PUSH QUE = 9
PUSH QUE = 3
LET CUT = 6
LET LIM = 3
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 8
LET D1 = 0
SET D1 = D1 + 1
LET D2 = 1
LET D3 = 2
SET D3 = D3 + 1
LET D4 = 3
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"RET == 2
VAL == 9
LENGTH:QUE == 0
DONE == [8,9,9]
LENGTH:DONE == 3
CUT > LIM
LIM > RET
D0 != 15
D1 == D2", 21),
            L(@"LIST QUE
PUSH QUE = 1
PUSH QUE = 10
PUSH QUE = 2
LET CUT = 5
LET LIM = 2
LET VAL = 0
LET RET = 0
WAIT
LET D0 = 0
SET D0 = D0 + 1
LET D1 = 1
LET D2 = 2
SET D2 = D2 + 1
LET D3 = 3
LET D4 = 4
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"RET == 2
VAL == 2
QUE == [7]
LENGTH:QUE == 1
DONE == [6,10]
LENGTH:DONE == 2
CUT > LIM
LIM <= 3
D0 == D1
D1 < D2", 21),

            // Level 26 - aggregation
            L(@"LIST DATA
PUSH DATA = 2
PUSH DATA = 5
PUSH DATA = 8
PUSH DATA = 3
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
WAIT
LET D0 = 3
LET D1 = 4
SET D1 = D1 + 1
LET D2 = 5
LET D3 = 6
SET D3 = D3 + 1
LET D4 = 7
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"SUM == 18
CNT == 4
MAX == 8
VAL == 3
LENGTH:DATA == 0
D0 <= 7", 15),
            L(@"LIST DATA
PUSH DATA = 3
PUSH DATA = 6
PUSH DATA = 9
PUSH DATA = 4
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
WAIT
LET D0 = 4
SET D0 = D0 + 1
LET D1 = 5
LET D2 = 6
SET D2 = D2 + 1
LET D3 = 7
LET D4 = 8
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"SUM == 22
CNT == 4
MAX == 9
VAL == 4
LENGTH:DATA == 0
D0 >= 9
D1 != 9", 15),
            L(@"LIST DATA
PUSH DATA = 4
PUSH DATA = 7
PUSH DATA = 10
PUSH DATA = 5
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
WAIT
LET D0 = 5
LET D1 = 6
SET D1 = D1 + 1
LET D2 = 7
LET D3 = 8
SET D3 = D3 + 1
LET D4 = 0
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"SUM == 26
CNT == 4
MAX == 10
VAL == 5
LENGTH:DATA == 0
D0 != 10
D1 > D2
D2 < D3", 15),
            L(@"LIST DATA
PUSH DATA = 5
PUSH DATA = 8
PUSH DATA = 11
PUSH DATA = 6
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
WAIT
LET D0 = 6
SET D0 = D0 + 1
LET D1 = 7
LET D2 = 8
SET D2 = D2 + 1
LET D3 = 0
LET D4 = 1
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"SUM == 30
CNT == 4
MAX == 11
VAL == 6
LENGTH:DATA == 0
D0 == D1
D1 < D2
D2 <= 15
D3 >= 3", 15),
            L(@"LIST DATA
PUSH DATA = 6
PUSH DATA = 9
PUSH DATA = 12
PUSH DATA = 7
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
WAIT
LET D0 = 7
LET D1 = 8
SET D1 = D1 + 1
LET D2 = 0
LET D3 = 1
SET D3 = D3 + 1
LET D4 = 2
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"SUM == 34
CNT == 4
MAX == 12
VAL == 7
LENGTH:DATA == 0
D0 < D1
D1 <= 16
D2 >= 5
D3 != 9
D4 < CNT", 15),

            // Level 27 - ordered merging
            L(@"LIST A
PUSH A = 1
PUSH A = 4
PUSH A = 7
LIST B
PUSH B = 2
PUSH B = 5
PUSH B = 8
LET CNT = 0
WAIT
LET D0 = 1
SET D0 = D0 + 1
LET D1 = 2
LET D2 = 3
SET D2 = D2 + 1
LET D3 = 4
LET D4 = 5
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"CNT == 0
LENGTH:A == 0
LENGTH:B == 0
OUT == [1,2,4,5,7,8]
LENGTH:OUT == 6
D0 == D1", 15),
            L(@"LIST A
PUSH A = 2
PUSH A = 5
PUSH A = 8
LIST B
PUSH B = 3
PUSH B = 6
PUSH B = 9
LET CNT = 0
WAIT
LET D0 = 2
LET D1 = 3
SET D1 = D1 + 1
LET D2 = 4
LET D3 = 5
SET D3 = D3 + 1
LET D4 = 6
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1", @"CNT == 0
LENGTH:A == 0
LENGTH:B == 0
OUT == [2,3,5,6,8,9]
LENGTH:OUT == 6
D0 < D1
D1 <= 10", 15),
            L(@"LIST A
PUSH A = 3
PUSH A = 6
PUSH A = 9
LIST B
PUSH B = 4
PUSH B = 7
PUSH B = 10
LET CNT = 0
WAIT
LET D0 = 3
SET D0 = D0 + 1
LET D1 = 4
LET D2 = 5
SET D2 = D2 + 1
LET D3 = 6
LET D4 = 7
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT", @"CNT == 0
LENGTH:A == 0
LENGTH:B == 0
OUT == [3,4,6,7,9,10]
LENGTH:OUT == 6
D0 <= 10
D1 >= 7
D2 != 12", 15),
            L(@"LIST A
PUSH A = 4
PUSH A = 7
PUSH A = 10
LIST B
PUSH B = 5
PUSH B = 8
PUSH B = 11
LET CNT = 0
WAIT
LET D0 = 4
LET D1 = 5
SET D1 = D1 + 1
LET D2 = 6
LET D3 = 7
SET D3 = D3 + 1
LET D4 = 8
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1", @"CNT == 0
LENGTH:A == 0
LENGTH:B == 0
OUT == [4,5,7,8,10,11]
LENGTH:OUT == 6
D0 >= 9
D1 != 13
D2 < D3
D3 > D4", 15),
            L(@"LIST A
PUSH A = 5
PUSH A = 8
PUSH A = 11
LIST B
PUSH B = 6
PUSH B = 9
PUSH B = 12
LET CNT = 0
WAIT
LET D0 = 5
SET D0 = D0 + 1
LET D1 = 6
LET D2 = 7
SET D2 = D2 + 1
LET D3 = 8
LET D4 = 0
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"CNT == 0
LENGTH:A == 0
LENGTH:B == 0
OUT == [5,6,8,9,11,12]
LENGTH:OUT == 6
D0 != 13
D1 < D2
D2 == D3
D3 <= 15
D4 >= 0", 15),

            // Level 28 - priority scheduler
            L(@"LIST NORM
PUSH NORM = 3
PUSH NORM = 6
PUSH NORM = 9
LIST PRIO
INJECT PRIO = 1
INJECT PRIO = 2
LET VAL = 0
LET PCNT = 0
WAIT
LET D0 = 8
LET D1 = 0
SET D1 = D1 + 1
LET D2 = 1
LET D3 = 2
SET D3 = D3 + 1
LET D4 = 3
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"VAL == 9
PCNT == 2
LENGTH:NORM == 0
LENGTH:PRIO == 0
DONE == [2,1,3,6,9]
LENGTH:DONE == 5
D0 <= 13", 16),
            L(@"LIST NORM
PUSH NORM = 4
PUSH NORM = 7
PUSH NORM = 10
LIST PRIO
INJECT PRIO = 2
INJECT PRIO = 3
LET VAL = 0
LET PCNT = 0
WAIT
LET D0 = 0
SET D0 = D0 + 1
LET D1 = 1
LET D2 = 2
SET D2 = D2 + 1
LET D3 = 3
LET D4 = 4
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2", @"VAL == 10
PCNT == 2
LENGTH:NORM == 0
LENGTH:PRIO == 0
DONE == [3,2,4,7,10]
LENGTH:DONE == 5
D0 >= 5
D1 != 6", 16),
            L(@"LIST NORM
PUSH NORM = 5
PUSH NORM = 8
PUSH NORM = 11
LIST PRIO
INJECT PRIO = 3
INJECT PRIO = 4
LET VAL = 0
LET PCNT = 0
WAIT
LET D0 = 1
LET D1 = 2
SET D1 = D1 + 1
LET D2 = 3
LET D3 = 4
SET D3 = D3 + 1
LET D4 = 5
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1", @"VAL == 11
PCNT == 2
LENGTH:NORM == 0
LENGTH:PRIO == 0
DONE == [4,3,5,8,11]
LENGTH:DONE == 5
D0 != 8
D1 > D2
D2 < D3", 16),
            L(@"LIST NORM
PUSH NORM = 6
PUSH NORM = 9
PUSH NORM = 12
LIST PRIO
INJECT PRIO = 4
INJECT PRIO = 5
LET VAL = 0
LET PCNT = 0
WAIT
LET D0 = 2
SET D0 = D0 + 1
LET D1 = 3
LET D2 = 4
SET D2 = D2 + 1
LET D3 = 5
LET D4 = 6
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2", @"VAL == 12
PCNT == 2
LENGTH:NORM == 0
LENGTH:PRIO == 0
DONE == [5,4,6,9,12]
LENGTH:DONE == 5
D0 == D1
D1 < D2
D2 <= 12
D3 >= 10", 16),
            L(@"LIST NORM
PUSH NORM = 7
PUSH NORM = 10
PUSH NORM = 13
LIST PRIO
INJECT PRIO = 5
INJECT PRIO = 6
LET VAL = 0
LET PCNT = 0
WAIT
LET D0 = 3
LET D1 = 4
SET D1 = D1 + 1
LET D2 = 5
LET D3 = 6
SET D3 = D3 + 1
LET D4 = 7
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"VAL == 13
PCNT == 2
LENGTH:NORM == 0
LENGTH:PRIO == 0
DONE == [6,5,7,10,13]
LENGTH:DONE == 5
D0 < D1
D1 <= 14
D2 >= 10
D3 != 14", 16),

            // Level 29 - sentinels
            L(@"LIST IN
PUSH IN = 3
PUSH IN = 5
PUSH IN = 0
PUSH IN = 8
LIST DONE
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 6
SET D0 = D0 + 1
LET D1 = 7
LET D2 = 8
SET D2 = D2 + 1
LET D3 = 0
LET D4 = 1
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1", @"VAL == 0
SUM == 8
CNT == 2
IN == [8]
LENGTH:IN == 1
DONE == [3,5]
LENGTH:DONE == 2", 16),
            L(@"LIST IN
PUSH IN = 4
PUSH IN = 6
PUSH IN = 0
PUSH IN = 9
LIST DONE
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 7
LET D1 = 8
SET D1 = D1 + 1
LET D2 = 0
LET D3 = 1
SET D3 = D3 + 1
LET D4 = 2
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT", @"VAL == 0
SUM == 10
CNT == 2
IN == [9]
LENGTH:IN == 1
DONE == [4,6]
LENGTH:DONE == 2
D0 < D1", 16),
            L(@"LIST IN
PUSH IN = 5
PUSH IN = 7
PUSH IN = 0
PUSH IN = 10
LIST DONE
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 8
SET D0 = D0 + 1
LET D1 = 0
LET D2 = 1
SET D2 = D2 + 1
LET D3 = 2
LET D4 = 3
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2", @"VAL == 0
SUM == 12
CNT == 2
IN == [10]
LENGTH:IN == 1
DONE == [5,7]
LENGTH:DONE == 2
D0 > D1
D1 <= 7", 16),
            L(@"LIST IN
PUSH IN = 6
PUSH IN = 8
PUSH IN = 0
PUSH IN = 11
LIST DONE
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 0
LET D1 = 1
SET D1 = D1 + 1
LET D2 = 2
LET D3 = 3
SET D3 = D3 + 1
LET D4 = 4
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"VAL == 0
SUM == 14
CNT == 2
IN == [11]
LENGTH:IN == 1
DONE == [6,8]
LENGTH:DONE == 2
D0 <= 8
D1 >= 7
D2 != 9", 16),
            L(@"LIST IN
PUSH IN = 7
PUSH IN = 9
PUSH IN = 0
PUSH IN = 12
LIST DONE
LET VAL = 0
LET SUM = 0
LET CNT = 0
WAIT
LET D0 = 1
SET D0 = D0 + 1
LET D1 = 2
LET D2 = 3
SET D2 = D2 + 1
LET D3 = 4
LET D4 = 5
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1", @"VAL == 0
SUM == 16
CNT == 2
IN == [12]
LENGTH:IN == 1
DONE == [7,9]
LENGTH:DONE == 2
D0 >= 9
D1 != 10
D2 == D3", 16),

            // Level 30 - final dispatch
            L(@"LIST IN
PUSH IN = 3
PUSH IN = 8
PUSH IN = 1
PUSH IN = 0
PUSH IN = 6
LIST GOOD
LIST RET
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET CUT = 5
WAIT
LET D0 = 4
LET D1 = 5
SET D1 = D1 + 1
LET D2 = 6
LET D3 = 7
SET D3 = D3 + 1
LET D4 = 8
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"VAL == 0
SUM == 8
CNT == 2
IN == [6]
LENGTH:IN == 1
GOOD == [8]
LENGTH:GOOD == 1
RET == [6,8]
LENGTH:RET == 2", 21),
            L(@"LIST IN
PUSH IN = 4
PUSH IN = 9
PUSH IN = 2
PUSH IN = 0
PUSH IN = 7
LIST GOOD
LIST RET
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET CUT = 6
WAIT
LET D0 = 5
SET D0 = D0 + 1
LET D1 = 6
LET D2 = 7
SET D2 = D2 + 1
LET D3 = 8
LET D4 = 0
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1", @"VAL == 0
SUM == 9
CNT == 2
IN == [7]
LENGTH:IN == 1
GOOD == [9]
LENGTH:GOOD == 1
RET == [8,10]
LENGTH:RET == 2", 21),
            L(@"LIST IN
PUSH IN = 3
PUSH IN = 10
PUSH IN = 3
PUSH IN = 0
PUSH IN = 8
LIST GOOD
LIST RET
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET CUT = 5
WAIT
LET D0 = 6
LET D1 = 7
SET D1 = D1 + 1
LET D2 = 8
LET D3 = 0
SET D3 = D3 + 1
LET D4 = 1
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2", @"VAL == 0
SUM == 10
CNT == 2
IN == [8]
LENGTH:IN == 1
GOOD == [10]
LENGTH:GOOD == 1
RET == [8,8]
LENGTH:RET == 2", 21),
            L(@"LIST IN
PUSH IN = 4
PUSH IN = 11
PUSH IN = 1
PUSH IN = 0
PUSH IN = 9
LIST GOOD
LIST RET
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET CUT = 6
WAIT
LET D0 = 7
SET D0 = D0 + 1
LET D1 = 8
LET D2 = 0
SET D2 = D2 + 1
LET D3 = 1
LET D4 = 2
SET D4 = D4 + 1
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1", @"VAL == 0
SUM == 11
CNT == 2
IN == [9]
LENGTH:IN == 1
GOOD == [11]
LENGTH:GOOD == 1
RET == [7,10]
LENGTH:RET == 2
CUT < SUM", 21),
            L(@"LIST IN
PUSH IN = 3
PUSH IN = 12
PUSH IN = 2
PUSH IN = 0
PUSH IN = 10
LIST GOOD
LIST RET
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET CUT = 5
WAIT
LET D0 = 8
LET D1 = 0
SET D1 = D1 + 1
LET D2 = 1
LET D3 = 2
SET D3 = D3 + 1
LET D4 = 3
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1
SET D1 = D1 + 2
SET D2 = D2 + 1
SET D3 = D3 + 2
WAIT
SET D0 = D0 + 2
SET D1 = D1 + 1
SET D2 = D2 + 2
SET D3 = D3 + 1
WAIT
SET D0 = D0 + 1", @"VAL == 0
SUM == 12
CNT == 2
IN == [10]
LENGTH:IN == 1
GOOD == [12]
LENGTH:GOOD == 1
RET == [7,8]
LENGTH:RET == 2
CUT <= 6", 21),

        };
    }
}
