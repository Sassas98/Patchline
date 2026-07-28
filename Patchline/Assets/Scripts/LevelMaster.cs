using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class LevelData
    {
        public string Code { get; set; }
        public string Goals { get; set; }
        public int Energy { get; set; }
        public int Lives { get; set; }
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

            var result = datas[(level * StepsPerLevel) + step];
            return L(result.Code.Replace("\r", ""), result.Goals.Replace("\r", ""), result.Energy);
        }

        private static LevelData L(string code, string goals, int energy, int lives = 3) => new()
        {
            Code = code,
            Goals = goals,
            Energy = energy,
            Lives = lives
        };

        private readonly List<LevelData> datas = new()
        {
            // Level 0 - SET / WAIT
            L(@"LET A = 2
A = 3
A = 4", @"A == 4", 0),
            L(@"LET A = 2
WAIT", @"A == 9", 12),
            L(@"LET B = 3
LET C = 7
WAIT
WAIT", @"B == 10
C != 7", 4),
            L(@"LET C = 4
LET D = 7
WAIT
C = C + 1
WAIT
C = C + 1
WAIT", @"C == 11
D < C", 4),
            L(@"LET CORE = 6
LET SIDE = 11
WAIT
CORE = CORE + 1
WAIT
CORE = CORE + 1
WAIT
CORE = CORE + 1
WAIT
CORE = CORE + 1
WAIT
CORE = CORE + 1", @"CORE == 13
SIDE <= 10", 23, 1),

            // Level 1 - LET snapshots
            L(@"LET BASE = 5
BASE = BASE + 1
WAIT", @"COPY == 5
BASE >= 5", 23),
            L(@"LET SEED = 6
SEED = SEED + 1
WAIT
LET D0 = 0", @"SNAP == 6
SEED != 8
D0 < SNAP", 5),
            L(@"LET RATE = 7
RATE = RATE + 1
WAIT
LET D0 = 1
D0 = D0 + 1
LET D1 = 2
D0 = D0 + 2", @"GAIN == 7
RATE > D0
D1 <= 1
D0 != 4", 0),
            L(@"LET LIM = 8
LIM = LIM + 1
WAIT
WAIT
WAIT
LET D0 = 2
LET D3 = 5
D3 = D3 + 1", @"SAVE == 8
D0 == 5
D3 < D0", 10),
            L(@"LET MODE = 9
MODE = MODE + 1
WAIT
LET D0 = 3
D0 = D0 + 2
LET D1 = 4
D0 = D0 + 2", @"MARK == 9
MODE == 9
D0 != 7
D1 > MODE", 13, 1),

            // Level 2 - LET expressions
            L(@"LET A = 6
LET B = 8
A = A + 1
WAIT", @"SUM == 14
A != 8
B < SUM", 24),
            L(@"LET X = 7
LET Y = 9
X = X + 1
WAIT
LET D0 = 7", @"X > D0
Y > DIFF
D0 <= 8", 0),
            L(@"LET BASE = 8
LET RATE = 10
BASE = BASE + 1
WAIT
LET D0 = 8", @"PROD == 80
BASE > D0
RATE >= 9
D0 != 8", 10),
            L(@"LET WIDE = 9
LET TALL = 11
WIDE = WIDE + 1
WAIT
TEMP = TEMP - 1", @"AREA == 99
TALL < 11
WIDE >= 11", 2, 1),
            L(@"LET LEFT = 10
RITE = 12
LEFT = LEFT + 1
WAIT
D0 = 1
D1 = D0 + 1
D2 = D1 + 1
D3 = D2 + 1", @"SPAN == -2", 24),

            // Level 3 - LOOP basics
            L(@"LET CNT = 0
LOOP CNT < 10
   CNT = CNT + 1
WAIT", @"CNT == 10", 0, 3),
            L(@"LET CNT = 0
LOOP CNT < 10
   CNT = CNT + 1
WAIT", @"CNT == 9", 18, 4),
            L(@"LET RATE = 2
LIM = 10
LOOP CNT < LIM
   CNT = CNT + 1
   SUM = SUM + RATE
WAIT", @"RATE == 3
SUM == 30
CNT == 10", 10),
            L(@"LET CNT = 6
LET USED = 0
LOOP CNT > 0
   CNT = CNT - 1
   USED = USED + 1
WAIT", @"USED == 9
CNT == 9000", 5),
            L(@"LET WIDE = 4
LET HIGH = 3
LET X = 0
LET Y = 0
LOOP X < WIDE
   LOOP Y < HIGH
      Y = Y + 1
      X = X + 1
WAIT", @"X >= 3
Y != 4
HIGH <= WIDE", 15, 2),

            // Level 4 - LOOP accumulators
            L(@"LET LIM = 4
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   SUM = SUM + I
WAIT", @"LIM == 5
I <= 5
SUM != 16", 12, 4),
            L(@"LET LIM = 4
LET I = 0
LET PROD = 2
WAIT
WAIT
WAIT
WAIT
WAIT
LOOP I < LIM
   I = I + 1
   PROD = PROD * PROD", @"PROD == 16", 10),
            L(@"LET A = 8
LET B = 1
WAIT
WAIT
WAIT
LOOP A != B
   A = A + 1
   B = B - 1
WAIT
WAIT", @"A == B", 5),
            L(@"LET X = 10
LET Y = 0
LET Z = 0
LOOP X > Y
   Y = X - Y
   LOOP Y > Z
      Z = Y - Z
      Y = X - Z
   X = X + 1
WAIT", @"Y < 3
X != Y", 10),
            L(@"LET Y = 0
LET Z = 0
LOOP X > 0
   LOOP Y < X
      Y = Y + 1
      Z = Z - 1
   X = X - 1", @"X > 10
Y == 10
Z != 10
| 35 |", 13, 2),

            // Level 5 - STOP basics
            L(@"LET ADD = 2
LET CNT = 0
LET SUM = 0
LOOP CNT < 5
   CNT = CNT + 1
   SUM = SUM + ADD
   STOP
WAIT", @"ADD == 5
SUM != 6
CNT < ADD", 22),
            L(@"LET BASE = 4
LET CNT = 0
LET OUT = 0
LOOP CNT < 8
   CNT = CNT + 1
   OUT = BASE * CNT
   STOP
OUT = OUT + BASE
WAIT", @"BASE == 6
OUT > BASE
CNT >= 1", 10),
            L(@"LET STEP = 2
LET CNT = 9
LET USED = 0
LOOP CNT > 0
   CNT = CNT - STEP
   USED = USED + 1
   STOP
CNT = CNT - STEP
WAIT", @"STEP == 3
CNT >= 3
USED <= 1", 10),
            L(@"LET WIDE = 3
LET HIGH = 4
LET X = 0
LET Y = 0
LET CELL = 0
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      CELL = CELL + 1
      Y = Y + 1
      STOP
   X = X + 1
WAIT", @"WIDE == 4
X <= 4
CELL != 5
Y < WIDE
HIGH <= CELL", 10, 1),
            L(@"LET WIDE = 3
LET HIGH = 5
LET X = 0
LET Y = 0
LET SUM = 0
LET RATE = 2
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      SUM = SUM + RATE
      Y = Y + 1
      STOP
   SUM = SUM + X
   X = X + 1
WAIT", @"RATE == 4
SUM != 16
X < RATE
Y >= 1
HIGH > Y
WIDE != 4", 50, 2),

            // Level 6 - STOP reinforcement
            L(@"LET RATE = 3
LET CNT = 0
LET SUM = 1
LOOP CNT < 9
   CNT = CNT + 1
   SUM = SUM * RATE
   STOP
SUM = SUM + CNT
WAIT", @"RATE == 4
SUM > RATE
CNT >= 1", 22),
            L(@"LET ADD = 2
LET CNT = 0
LET SUM = 0
LOOP CNT < 9
   CNT = CNT + 1
   SUM = SUM + ADD
   SUM = SUM + CNT
   STOP
WAIT", @"ADD == 5
SUM >= 6
CNT <= 1", 10),
            L(@"LET LIM = 7
LET CNT = 0
LET SUM = 0
LOOP CNT < LIM
   CNT = CNT + 2
   SUM = SUM + CNT
   STOP
SUM = SUM + LIM
WAIT", @"LIM == 9
SUM <= 11
CNT != 3", 0),
            L(@"LET WIDE = 3
LET HIGH = 4
LET X = 0
LET Y = 0
LET SUM = 0
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      Y = Y + 1
      SUM = SUM + Y
      STOP
   SUM = SUM + X
   X = X + 1
WAIT", @"WIDE == 4
X != 5
SUM > WIDE
Y >= 1
HIGH > Y", 10),
            L(@"LET WIDE = 4
LET HIGH = 6
LET X = 0
LET Y = 0
LET SUM = 0
LET ADD = 1
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      Y = Y + 1
      SUM = SUM + ADD
      STOP
   SUM = SUM + X
   X = X + 1
SUM = SUM + HIGH
WAIT", @"ADD == 3
SUM > ADD
X >= 4
Y <= 1
HIGH >= 5
WIDE < SUM", 37, 1),

            // Level 7 - SKIP reading
            L(@"LET LIM = 3
LET CNT = 0
LET SUM = 0
LOOP CNT < LIM
   CNT = CNT + 1
   SUM = SUM + CNT
   SKIP
   SUM = 99
WAIT", @"LIM == 4
CNT >= 4
SUM <= 10", 12),
            L(@"LET RATE = 2
LET LIM = 4
LET CNT = 0
LET SUM = 0
LOOP CNT < LIM
   CNT = CNT + 1
   SUM = SUM + RATE
   SKIP
   RATE = 99
WAIT", @"RATE == 3
SUM <= 12
CNT != 5
LIM < SUM", 0),
            L(@"LET STEP = 2
LET CNT = 0
LET SUM = 0
LOOP CNT < 6
   CNT = CNT + STEP
   SUM = SUM + CNT
   SKIP
   SUM = 0
SUM = SUM + STEP
WAIT", @"STEP == 3
SUM != 13
CNT > STEP", 0),
            L(@"LET WIDE = 3
LET HIGH = 2
LET X = 0
LET Y = 0
LET SUM = 0
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      Y = Y + 1
      SUM = SUM + Y
      SKIP
      SUM = 99
   X = X + 1
WAIT", @"HIGH == 3
Y >= HIGH
SUM >= 18
X <= 3
WIDE != 4", 0),
            L(@"LET WIDE = 3
LET HIGH = 3
LET X = 0
LET Y = 0
LET SUM = 0
LET ADD = 1
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      Y = Y + 1
      SUM = SUM + ADD
      SKIP
      ADD = 20
   SUM = SUM + X
   X = X + 1
WAIT", @"ADD == 2
SUM >= 21
X <= 3
Y != 4
HIGH != 4
WIDE < SUM", 17, 1),

            // Level 8 - IF / ELSE
            L(@"LET CUT = 8
LET CNT = 0
LET SUM = 0
LOOP CNT < 3
   CNT = CNT + 1
   SUM = SUM + CNT
IF SUM >= CUT
   SUM = SUM + 1
ELSE
   SUM = SUM - 1
WAIT", @"CUT == 6
SUM <= 7
CNT != 4", 12),
            L(@"LET RATE = 2
LET LIM = 4
LET CNT = 0
LET SUM = 0
LET FLAG = 0
LOOP CNT < LIM
   CNT = CNT + 1
   SUM = SUM + RATE
IF SUM > LIM
   FLAG = SUM
ELSE
   FLAG = LIM
WAIT", @"RATE == 1
SUM != 5
FLAG > RATE
CNT <= LIM
LIM >= 3", 0),
            L(@"LET CUT = 5
LET CNT = 6
LET USED = 0
LET FLAG = 0
LOOP CNT > 0
   CNT = CNT - 2
   USED = USED + 1
IF USED == CUT
   FLAG = 1
ELSE
   FLAG = USED
WAIT", @"CUT == 3
FLAG < CUT
USED >= 3
CNT <= 1", 0),
            L(@"LET CUT = 10
LET WIDE = 3
LET HIGH = 2
LET X = 0
LET Y = 0
LET CELL = 0
LET FLAG = 0
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      CELL = CELL + 1
      Y = Y + 1
   X = X + 1
IF CELL >= CUT
   FLAG = 1
ELSE
   FLAG = CELL
WAIT", @"CUT == 6
FLAG >= 1
CELL <= 6
HIGH < CELL
WIDE <= 4
X >= 2
Y != 3", 0),
            L(@"LET CUT = 12
LET WIDE = 3
LET HIGH = 3
LET X = 0
LET Y = 0
LET SUM = 0
LET FLAG = 0
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      SUM = SUM + X
      Y = Y + 1
   X = X + 1
IF SUM != CUT
   FLAG = SUM
ELSE
   FLAG = -1
SUM = SUM + FLAG
WAIT", @"CUT == 9
SUM <= 8
FLAG != 0
HIGH < CUT
WIDE >= 2
X != 4
Y <= WIDE", 12, 2),

            // Level 9 - conditions in LOOP
            L(@"LET CUT = 2
LET LIM = 4
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I > CUT
      SUM = SUM + I
   ELSE
      SUM = SUM + CUT
WAIT", @"CUT == 1
SUM != 11
I > CUT
LIM >= 3", 12),
            L(@"LET MOD = 2
LET LIM = 5
LET I = 0
LET REM = 0
LET HIT = 0
LET MISS = 0
LOOP I < LIM
   I = I + 1
   REM = I % MOD
   IF REM == 0
      HIT = HIT + 1
   ELSE
      MISS = MISS + 1
WAIT", @"MOD == 3
HIT < MOD
MISS >= 4
I <= 5
LIM != 6", 0),
            L(@"LET CUT = 4
LET LIM = 6
LET I = 0
LET LOW = 0
LET HIGH = 0
LOOP I < LIM
   I = I + 1
   IF I <= CUT
      LOW = LOW + I
   ELSE
      HIGH = HIGH + I
WAIT", @"CUT == 3
LOW >= 6
HIGH <= 15
I != 7
LIM < HIGH", 0),
            L(@"LET CUT = 3
LET WIDE = 4
LET HIGH = 2
LET X = 0
LET Y = 0
LET SUM = 0
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      Y = Y + 1
      IF X < CUT
         SUM = SUM + Y
      ELSE
         SUM = SUM + X
   X = X + 1
WAIT", @"CUT == 2
SUM <= 16
X != 5
Y >= CUT
HIGH <= Y
WIDE <= 5", 0),
            L(@"LET CUT = 2
LET WIDE = 3
LET HIGH = 3
LET X = 0
LET Y = 0
LET SUM = 0
LET CNT = 0
LOOP X < WIDE
   Y = 0
   LOOP Y < HIGH
      Y = Y + 1
      IF Y > CUT
         SUM = SUM + X
      ELSE
         SUM = SUM + Y
      CNT = CNT + 1
   X = X + 1
WAIT", @"CUT == 1
SUM != 10
X > CUT
Y >= 3
CNT <= 9
HIGH <= 4
WIDE != 4", 12, 2),

            // Level 10 - compound conditions
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 5
LET I = 0
LET HIT = 0
LOOP I < LIM
   I = I + 1
   IF I >= LOW AND I <= HIGH
      HIT = HIT + I
   ELSE
      HIT = HIT + 0
WAIT", @"LOW == 1
HIT > LOW
I >= 5
HIGH <= 5", 12),
            L(@"LET MOD = 2
LET CUT = 3
LET LIM = 6
LET I = 0
LET REM = 0
LET HIT = 0
LOOP I < LIM
   I = I + 1
   REM = I % MOD
   IF REM == 0 AND I > CUT
      HIT = HIT + 1
   ELSE
      HIT = HIT + 0
WAIT", @"CUT == 1
HIT >= 3
I <= 6
LIM > REM
MOD <= 3", 0),
            L(@"LET LOW = 2
LET HIGH = 5
LET LIM = 7
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW OR I > HIGH
      SUM = SUM + I
   ELSE
      SUM = SUM + LOW
WAIT", @"HIGH == 4
SUM <= 25
I != 8
LIM > HIGH
LOW <= 3", 0),
            L(@"LET LOW = 1
LET HIGH = 3
LET WIDE = 4
LET YMAX = 3
LET X = 0
LET Y = 0
LET HIT = 0
LOOP X < WIDE
   Y = 0
   LOOP Y < YMAX
      Y = Y + 1
      IF X >= LOW AND Y <= HIGH
         HIT = HIT + 1
      ELSE
         HIT = HIT + 0
   X = X + 1
WAIT", @"LOW == 2
HIT != 7
X > LOW
Y >= 3
HIGH < X
WIDE >= 3
YMAX < WIDE", 0),
            L(@"LET LOW = 1
LET HIGH = 3
LET WIDE = 4
LET YMAX = 3
LET X = 0
LET Y = 0
LET SUM = 0
LET HIT = 0
LOOP X < WIDE
   Y = 0
   LOOP Y < YMAX
      Y = Y + 1
      IF X >= LOW AND Y <= HIGH
         SUM = SUM + X
         HIT = HIT + 1
      ELSE
         SUM = SUM + Y
   X = X + 1
WAIT", @"HIGH == 2
SUM > HIGH
HIT >= 6
X <= 4
Y != 4
LOW >= 0
WIDE < HIT
YMAX >= 2", 13, 1),

            // Level 11 - ELIF / LIST
            L(@"LET HIGH = 3
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LOOP I < 3
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      B = B + 2
WAIT", @"HIGH == 4
I >= 3
A <= 1
B != 3
LENGTH:BUF >= 0", 12),
            L(@"LET HIGH = 4
LET LIM = 4
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
WAIT", @"HIGH == 5
I <= 4
A != 2
B < HIGH
C >= 0
LENGTH:BUF == 0", 0),
            L(@"LET HIGH = 3
LET LIM = 5
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
B = B + LENGTH:BUF
WAIT", @"HIGH == 4
B != 4
C < HIGH
I >= 5
A <= 1
LENGTH:BUF >= 0", 0),
            L(@"LET HIGH = 4
LET LIM = 3
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
B = B + LENGTH:BUF
WAIT", @"HIGH == 5
I < HIGH
A >= 1
B <= 2
C != 1
LENGTH:BUF == 0
LIM < HIGH", 0),
            L(@"LET HIGH = 3
LET LIM = 4
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
B = B + LENGTH:BUF
WAIT", @"HIGH == 4
B >= 3
C <= 0
I != 5
A < HIGH
LENGTH:BUF >= 0
LIM <= 5", 12, 1),

            // Level 12 - FIRST / LAST
            L(@"LET HIGH = 3
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LOOP I < 3
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      B = B + 2
   B = B + LENGTH:BUF
WAIT", @"HIGH == 4
I <= 3
A != 2
B < HIGH
LENGTH:BUF == 0", 12),
            L(@"LET HIGH = 4
LET LIM = 4
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
   B = B + LENGTH:BUF
WAIT", @"HIGH == 5
I != 5
A < HIGH
B >= 3
C <= 0
LENGTH:BUF >= 0", 0),
            L(@"LET HIGH = 3
LET LIM = 5
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
   B = B + LENGTH:BUF
WAIT", @"HIGH == 4
B < HIGH
C >= 1
I <= 5
A != 2
LENGTH:BUF == 0", 0),
            L(@"LET HIGH = 4
LET LIM = 3
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
   B = B + LENGTH:BUF
WAIT", @"HIGH == 5
I >= 3
A <= 1
B != 3
C < HIGH
LENGTH:BUF >= 0
LIM <= 4", 0),
            L(@"LET HIGH = 3
LET LIM = 4
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
   B = B + LENGTH:BUF
WAIT", @"HIGH == 4
B <= 3
C != 1
I >= HIGH
A >= 1
LENGTH:BUF == 0
LIM >= 3", 12, 1),

            // Level 13 - list reading
            L(@"LET HIGH = 3
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LOOP I < 3
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      B = B + 2
   B = B + LENGTH:BUF
LIST AUX
B = B + LENGTH:AUX", @"HIGH == 4
I != 4
A < HIGH
B >= 2
LENGTH:BUF >= 0
LENGTH:AUX == 0", 12),
            L(@"LET HIGH = 4
LET LIM = 4
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
   B = B + LENGTH:BUF
LIST AUX
B = B + LENGTH:AUX", @"HIGH == 5
I < HIGH
A >= 1
B <= 3
C != 1
LENGTH:BUF == 0
LENGTH:AUX >= 0", 0),
            L(@"LET HIGH = 3
LET LIM = 5
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
   B = B + LENGTH:BUF
LIST AUX
B = B + LENGTH:AUX
WAIT", @"HIGH == 4
B >= 3
C <= 1
I != 6
A < HIGH
LENGTH:BUF >= 0
LENGTH:AUX == 0", 0),
            L(@"LET HIGH = 4
LET LIM = 3
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
   B = B + LENGTH:BUF
LIST AUX
B = B + LENGTH:AUX
WAIT", @"HIGH == 5
I <= 3
A != 2
B < HIGH
C >= 0
LENGTH:BUF == 0
LENGTH:AUX >= 0", 0),
            L(@"LET HIGH = 3
LET LIM = 4
LIST BUF
LET I = 0
LET A = 0
LET B = 0
LET C = 0
LOOP I < LIM
   I = I + 1
   IF I < 2
      A = A + 1
   ELIF I <= HIGH
      B = B + 1
   ELSE
      C = C + 1
   B = B + LENGTH:BUF
LIST AUX
B = B + LENGTH:AUX
WAIT", @"HIGH == 4
B != 4
C < HIGH
I >= 4
A <= 1
LENGTH:BUF >= 0
LENGTH:AUX == 0
LIM != 5", 12, 1),

            // Level 14 - PUSH basics
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 4
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH", @"HIGH == 5
LOW < HIGH
I >= 4
SUM <= 11
OUT == [2,2,3,4]
LENGTH:OUT == 4", 13),
            L(@"LET LOW = 3
LET HIGH = 5
LET LIM = 5
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
WAIT", @"LOW == 2
SUM >= 16
HIGH <= 5
I != 6
OUT == [2,2,3,4,5]
LENGTH:OUT >= 4", 0),
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 6
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
WAIT", @"HIGH == 5
SUM <= 21
LOW != 3
I > HIGH
OUT == [2,2,3,4,5,5]
LENGTH:OUT == 6
LIM > HIGH", 0),
            L(@"LET LOW = 3
LET HIGH = 5
LET LIM = 4
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
WAIT", @"LOW == 2
SUM != 12
HIGH > LOW
I >= 4
OUT == [2,2,3,4]
LENGTH:OUT >= 3
LIM <= I", 0),
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 5
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
WAIT", @"HIGH == 5
SUM > HIGH
LOW >= 2
I <= 5
OUT == [2,2,3,4,5]
LENGTH:OUT == 5
LIM <= 6", 13, 1),

            // Level 15 - PUSH filtering
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 4
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH", @"HIGH == 5
LOW >= 2
I <= 4
SUM != 12
OUT == [2,2,3,4]
LENGTH:OUT >= 3", 13),
            L(@"LET LOW = 3
LET HIGH = 5
LET LIM = 5
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
WAIT", @"LOW == 2
SUM <= 16
HIGH != 6
I > LOW
OUT == [2,2,3,4,5]
LENGTH:OUT == 5", 0),
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 6
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
WAIT", @"HIGH == 5
SUM != 22
LOW < HIGH
I >= 6
OUT == [2,2,3,4,5,5]
LENGTH:OUT >= 5
LIM > HIGH", 0),
            L(@"LET LOW = 3
LET HIGH = 5
LET LIM = 4
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
WAIT", @"LOW == 2
SUM > LOW
HIGH >= 5
I <= 4
OUT == [2,2,3,4]
LENGTH:OUT == 4
LIM <= 5", 0),
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 5
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
WAIT", @"HIGH == 5
SUM >= 16
LOW <= 2
I != 6
OUT == [2,2,3,4,5]
LENGTH:OUT >= 4
LIM >= 4", 13, 1),

            // Level 16 - PUSH transforms
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 4
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
LET HEAD = FIRST:OUT
SUM = SUM + HEAD", @"HIGH == 5
LOW <= 2
I != 5
SUM > HIGH
HEAD >= 2
OUT == [2,2,3,4]
LENGTH:OUT == 4", 13),
            L(@"LET LOW = 3
LET HIGH = 5
LET LIM = 5
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
LET HEAD = FIRST:OUT
SUM = SUM + HEAD
LET TAIL = LAST:OUT
SUM = SUM + TAIL", @"LOW == 2
SUM != 24
HEAD >= LOW
HIGH >= 5
I <= 5
TAIL != 6
OUT == [2,2,3,4,5]
LENGTH:OUT >= 4", 0),
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 6
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
LET HEAD = FIRST:OUT
SUM = SUM + HEAD
LET TAIL = LAST:OUT
SUM = SUM + TAIL", @"HIGH == 5
SUM > HIGH
TAIL >= 5
LOW <= 2
I != 7
HEAD < HIGH
OUT == [2,2,3,4,5,5]
LENGTH:OUT == 6", 0),
            L(@"LET LOW = 3
LET HIGH = 5
LET LIM = 4
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
LET HEAD = FIRST:OUT
SUM = SUM + HEAD
LET TAIL = LAST:OUT
SUM = SUM + TAIL", @"LOW == 2
SUM >= 17
HEAD <= 2
HIGH != 6
I > LOW
TAIL >= 4
OUT == [2,2,3,4]
LENGTH:OUT >= 3", 0),
            L(@"LET LOW = 2
LET HIGH = 4
LET LIM = 5
LIST OUT
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < LOW
      PUSH OUT = LOW
      SUM = SUM + LOW
   ELIF I <= HIGH
      PUSH OUT = I
      SUM = SUM + I
   ELSE
      PUSH OUT = HIGH
      SUM = SUM + HIGH
LET HEAD = FIRST:OUT
SUM = SUM + HEAD
LET TAIL = LAST:OUT
SUM = SUM + TAIL", @"HIGH == 5
SUM <= 23
TAIL != 6
LOW < HIGH
I >= 5
HEAD <= 2
OUT == [2,2,3,4,5]
LENGTH:OUT == 5
LIM <= HIGH", 13, 1),

            // Level 17 - INJECT basics
            L(@"LET CUT = 2
LET LIM = 4
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT", @"CUT == 3
SUM != 10
I > CUT
OUT == [3,2,1,9,3]
LENGTH:OUT >= 4", 12),
            L(@"LET CUT = 3
LET LIM = 5
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT", @"CUT == 4
SUM > CUT
I >= 5
OUT == [4,3,2,1,9,4]
LENGTH:OUT == 6
LIM <= 6", 0),
            L(@"LET CUT = 2
LET LIM = 6
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT", @"CUT == 3
SUM >= 15
I <= 6
OUT == [3,3,3,2,1,9,3]
LENGTH:OUT >= 6
LIM >= 5", 0),
            L(@"LET CUT = 3
LET LIM = 4
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT", @"CUT == 4
SUM <= 10
I != 5
OUT == [3,2,1,9,4]
LENGTH:OUT == 5
LIM != 5", 0),
            L(@"LET CUT = 2
LET LIM = 5
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT", @"CUT == 3
SUM != 13
I > CUT
OUT == [3,3,2,1,9,3]
LENGTH:OUT >= 5
LIM > CUT", 12, 1),

            // Level 18 - deque operations
            L(@"LET CUT = 2
LET LIM = 4
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT
LET HEAD = FIRST:OUT
LET TAIL = LAST:OUT
SUM = SUM + HEAD
SUM = SUM + TAIL", @"CUT == 3
SUM > CUT
HEAD >= 3
TAIL <= 3
I != 5
OUT == [3,2,1,9,3]
LENGTH:OUT == 5", 12),
            L(@"LET CUT = 3
LET LIM = 5
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT
LET HEAD = FIRST:OUT
LET TAIL = LAST:OUT
SUM = SUM + HEAD
SUM = SUM + TAIL", @"CUT == 4
SUM >= 22
HEAD <= 4
TAIL != 5
I > CUT
OUT == [4,3,2,1,9,4]
LENGTH:OUT >= 5", 0),
            L(@"LET CUT = 2
LET LIM = 6
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT
LET HEAD = FIRST:OUT
LET TAIL = LAST:OUT
SUM = SUM + HEAD
SUM = SUM + TAIL", @"CUT == 3
SUM <= 21
HEAD != 4
TAIL >= CUT
I >= 6
OUT == [3,3,3,2,1,9,3]
LENGTH:OUT == 7", 0),
            L(@"LET CUT = 3
LET LIM = 4
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT
LET HEAD = FIRST:OUT
LET TAIL = LAST:OUT
SUM = SUM + HEAD
SUM = SUM + TAIL", @"CUT == 4
SUM != 18
TAIL >= CUT
I >= 4
HEAD <= 3
OUT == [3,2,1,9,4]
LENGTH:OUT >= 4
LIM > HEAD", 0),
            L(@"LET CUT = 2
LET LIM = 5
LIST OUT
PUSH OUT = 9
LET I = 0
LET SUM = 0
LOOP I < LIM
   I = I + 1
   IF I < CUT
      INJECT OUT = I
      SUM = SUM + I
   ELIF I == CUT
      PUSH OUT = I
      SUM = SUM + CUT
   ELSE
      INJECT OUT = CUT
      SUM = SUM + CUT
LET HEAD = FIRST:OUT
LET TAIL = LAST:OUT
SUM = SUM + HEAD
SUM = SUM + TAIL", @"CUT == 3
SUM > CUT
HEAD >= 3
TAIL <= 3
I != 6
OUT == [3,3,2,1,9,3]
LENGTH:OUT == 6
LIM <= 6", 12, 1),

            // Level 19 - list filtering
            L(@"LET CUT = 4
LIST SRC
PUSH SRC = 1
PUSH SRC = 4
PUSH SRC = 7
PUSH SRC = 2
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < CUT
      CNT = CNT + 1
   ELIF VAL == CUT
      PUSH OUT = VAL
      SUM = SUM + VAL
   ELSE
      INJECT OUT = VAL
      SUM = SUM + VAL
WAIT", @"CUT == 3
VAL >= 2
SUM <= 11
CNT != 3
OUT == [7,4,0]
LENGTH:OUT >= 2
LENGTH:SRC == 0", 12),
            L(@"LET CUT = 5
LIST SRC
PUSH SRC = 2
PUSH SRC = 5
PUSH SRC = 8
PUSH SRC = 3
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < CUT
      CNT = CNT + 1
   ELIF VAL == CUT
      PUSH OUT = VAL
      SUM = SUM + VAL
   ELSE
      INJECT OUT = VAL
      SUM = SUM + VAL
WAIT", @"CUT == 4
VAL <= 3
SUM != 14
CNT < CUT
OUT == [8,5,0]
LENGTH:OUT == 3
LENGTH:SRC >= 0", 0),
            L(@"LET CUT = 4
LIST SRC
PUSH SRC = 1
PUSH SRC = 4
PUSH SRC = 9
PUSH SRC = 2
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < CUT
      CNT = CNT + 1
   ELIF VAL == CUT
      PUSH OUT = VAL
      SUM = SUM + VAL
   ELSE
      INJECT OUT = VAL
      SUM = SUM + VAL
WAIT", @"CUT == 3
VAL != 3
SUM > CUT
CNT >= 2
OUT == [9,4,0]
LENGTH:OUT >= 2
LENGTH:SRC == 0", 0),
            L(@"LET CUT = 5
LIST SRC
PUSH SRC = 2
PUSH SRC = 5
PUSH SRC = 10
PUSH SRC = 3
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < CUT
      CNT = CNT + 1
   ELIF VAL == CUT
      PUSH OUT = VAL
      SUM = SUM + VAL
   ELSE
      INJECT OUT = VAL
      SUM = SUM + VAL
WAIT", @"CUT == 4
VAL < CUT
SUM >= 15
CNT <= 2
OUT == [10,5,0]
LENGTH:OUT == 3
LENGTH:SRC >= 0", 0),
            L(@"LET CUT = 4
LIST SRC
PUSH SRC = 1
PUSH SRC = 4
PUSH SRC = 11
PUSH SRC = 2
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < CUT
      CNT = CNT + 1
   ELIF VAL == CUT
      PUSH OUT = VAL
      SUM = SUM + VAL
   ELSE
      INJECT OUT = VAL
      SUM = SUM + VAL
WAIT", @"CUT == 3
VAL >= 2
SUM <= 15
CNT != 3
OUT == [11,4,0]
LENGTH:OUT >= 2
LENGTH:SRC == 0", 12, 1),

            // Level 20 - map / reduce
            L(@"LET RATE = 2
LIST SRC
PUSH SRC = 1
PUSH SRC = 2
PUSH SRC = 3
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < RATE
      VAL = VAL + RATE
   ELIF VAL == RATE
      VAL = VAL * RATE
   ELSE
      VAL = VAL + 1
   PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"RATE == 3
VAL <= 9
SUM != 19
OUT == [0,4,5,9]
LENGTH:OUT == 4
LENGTH:SRC >= 0", 12),
            L(@"LET RATE = 3
LIST SRC
PUSH SRC = 2
PUSH SRC = 3
PUSH SRC = 4
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < RATE
      VAL = VAL + RATE
   ELIF VAL == RATE
      VAL = VAL * RATE
   ELSE
      VAL = VAL + 1
   PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"RATE == 4
VAL != 17
SUM > RATE
OUT == [0,6,7,16]
LENGTH:OUT >= 3
LENGTH:SRC == 0", 0),
            L(@"LET RATE = 2
LIST SRC
PUSH SRC = 3
PUSH SRC = 4
PUSH SRC = 5
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < RATE
      VAL = VAL + RATE
   ELIF VAL == RATE
      VAL = VAL * RATE
   ELSE
      VAL = VAL + 1
   PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"RATE == 3
SUM > RATE
VAL >= 6
OUT == [0,9,5,6]
LENGTH:OUT == 4
LENGTH:SRC >= 0", 0),
            L(@"LET RATE = 3
LIST SRC
PUSH SRC = 4
PUSH SRC = 5
PUSH SRC = 6
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < RATE
      VAL = VAL + RATE
   ELIF VAL == RATE
      VAL = VAL * RATE
   ELSE
      VAL = VAL + 1
   PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"RATE == 4
SUM >= 29
VAL <= 7
OUT == [0,16,6,7]
LENGTH:OUT >= 3
LENGTH:SRC == 0", 0),
            L(@"LET RATE = 2
LIST SRC
PUSH SRC = 5
PUSH SRC = 6
PUSH SRC = 7
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < RATE
      VAL = VAL + RATE
   ELIF VAL == RATE
      VAL = VAL * RATE
   ELSE
      VAL = VAL + 1
   PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"RATE == 3
VAL <= 8
SUM != 22
LENGTH:SRC == 0
LENGTH:OUT >= 3", 12, 1),

            // Level 21 - retry queues
            L(@"LET CUT = 5
LIST QUE
PUSH QUE = 1
PUSH QUE = 6
PUSH QUE = 2
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"CUT == 6
VAL != 9
RET < CUT
DONE == [0,7,6,8]
LENGTH:DONE >= 3
LENGTH:QUE == 0", 12),
            L(@"LET CUT = 6
LIST QUE
PUSH QUE = 2
PUSH QUE = 7
PUSH QUE = 3
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"CUT == 7
VAL > CUT
RET >= 2
DONE == [0,9,7,10]
LENGTH:DONE == 4
LENGTH:QUE >= 0", 0),
            L(@"LET CUT = 5
LIST QUE
PUSH QUE = 1
PUSH QUE = 8
PUSH QUE = 2
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"CUT == 6
VAL >= 8
RET <= 2
DONE == [0,7,8,8]
LENGTH:DONE >= 3
LENGTH:QUE == 0", 0),
            L(@"LET CUT = 6
LIST QUE
PUSH QUE = 2
PUSH QUE = 9
PUSH QUE = 3
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"CUT == 7
VAL <= 10
RET != 3
DONE == [0,9,9,10]
LENGTH:DONE == 4
LENGTH:QUE >= 0", 0),
            L(@"LET CUT = 5
LIST QUE
PUSH QUE = 1
PUSH QUE = 10
PUSH QUE = 2
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"CUT == 6
VAL != 9
RET < CUT
DONE == [0,7,10,8]
LENGTH:DONE >= 3
LENGTH:QUE == 0", 12, 1),

            // Level 22 - stack transforms
            L(@"LET CUT = 3
LIST STK
PUSH STK = 1
PUSH STK = 3
PUSH STK = 5
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:STK > 0
   VAL = POP:STK
   IF VAL < CUT
      INJECT OUT = VAL
   ELIF VAL == CUT
      PUSH OUT = VAL
   ELSE
      PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"CUT == 4
VAL < CUT
SUM >= 9
OUT == [1,3,0,5]
LENGTH:OUT == 4
LENGTH:STK >= 0", 12),
            L(@"LET CUT = 4
LIST STK
PUSH STK = 2
PUSH STK = 4
PUSH STK = 6
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:STK > 0
   VAL = POP:STK
   IF VAL < CUT
      INJECT OUT = VAL
   ELIF VAL == CUT
      PUSH OUT = VAL
   ELSE
      PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"CUT == 5
VAL >= 2
SUM <= 12
OUT == [2,4,0,6]
LENGTH:OUT >= 3
LENGTH:STK == 0", 0),
            L(@"LET CUT = 5
LIST STK
PUSH STK = 3
PUSH STK = 5
PUSH STK = 7
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:STK > 0
   VAL = POP:STK
   IF VAL < CUT
      INJECT OUT = VAL
   ELIF VAL == CUT
      PUSH OUT = VAL
   ELSE
      PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"CUT == 6
VAL <= 3
SUM != 16
OUT == [3,5,0,7]
LENGTH:OUT == 4
LENGTH:STK >= 0", 0),
            L(@"LET CUT = 6
LIST STK
PUSH STK = 4
PUSH STK = 6
PUSH STK = 8
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:STK > 0
   VAL = POP:STK
   IF VAL < CUT
      INJECT OUT = VAL
   ELIF VAL == CUT
      PUSH OUT = VAL
   ELSE
      PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"CUT == 7
VAL != 5
SUM > CUT
OUT == [4,6,0,8]
LENGTH:OUT >= 3
LENGTH:STK == 0", 0),
            L(@"LET CUT = 7
LIST STK
PUSH STK = 5
PUSH STK = 7
PUSH STK = 9
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET SUM = 0
LOOP LENGTH:STK > 0
   VAL = POP:STK
   IF VAL < CUT
      INJECT OUT = VAL
   ELIF VAL == CUT
      PUSH OUT = VAL
   ELSE
      PUSH OUT = VAL
   SUM = SUM + VAL
WAIT", @"CUT == 8
VAL < CUT
SUM >= 21
OUT == [5,7,0,9]
LENGTH:OUT == 4
LENGTH:STK >= 0", 12, 1),

            // Level 23 - dual buffers
            L(@"LET CUT = 4
LIST A
PUSH A = 1
PUSH A = 4
PUSH A = 7
LIST B
PUSH B = 2
PUSH B = 5
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      INJECT OUT = VAL
   ELSE
      PUSH OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 5
VAL >= 5
CNT <= 5
OUT == [2,4,1,0,7,5]
LENGTH:OUT >= 5
LENGTH:A == 0
LENGTH:B >= 0", 12),
            L(@"LET CUT = 5
LIST A
PUSH A = 2
PUSH A = 5
PUSH A = 8
LIST B
PUSH B = 3
PUSH B = 6
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      INJECT OUT = VAL
   ELSE
      PUSH OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 6
VAL <= 6
CNT != 6
OUT == [3,5,2,0,8,6]
LENGTH:OUT == 6
LENGTH:A >= 0
LENGTH:B == 0", 0),
            L(@"LET CUT = 6
LIST A
PUSH A = 3
PUSH A = 6
PUSH A = 9
LIST B
PUSH B = 4
PUSH B = 7
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      INJECT OUT = VAL
   ELSE
      PUSH OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 7
VAL != 8
CNT < CUT
OUT == [4,6,3,0,9,7]
LENGTH:OUT >= 5
LENGTH:A == 0
LENGTH:B >= 0", 0),
            L(@"LET CUT = 7
LIST A
PUSH A = 4
PUSH A = 7
PUSH A = 10
LIST B
PUSH B = 5
PUSH B = 8
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      INJECT OUT = VAL
   ELSE
      PUSH OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 8
VAL >= CUT
CNT >= 5
OUT == [5,7,4,0,10,8]
LENGTH:OUT == 6
LENGTH:A >= 0
LENGTH:B == 0", 0),
            L(@"LET CUT = 8
LIST A
PUSH A = 5
PUSH A = 8
PUSH A = 11
LIST B
PUSH B = 6
PUSH B = 9
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      INJECT OUT = VAL
   ELSE
      PUSH OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 9
VAL >= 9
CNT <= 5
OUT == [6,8,5,0,11,9]
LENGTH:OUT >= 5
LENGTH:A == 0
LENGTH:B >= 0", 12, 1),

            // Level 24 - threshold routing
            L(@"LET LOW = 5
LET HIGH = 9
LIST SRC
PUSH SRC = 2
PUSH SRC = 6
PUSH SRC = 10
PUSH SRC = 4
LIST A
LIST B
LIST C
INJECT A = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < LOW
      INJECT A = VAL
   ELIF VAL < HIGH
      PUSH B = VAL
   ELSE
      PUSH C = VAL
   CNT = CNT + 1
WAIT", @"HIGH == 10
VAL <= 4
CNT != 5
LENGTH:SRC == 0
LENGTH:A >= 2
LENGTH:B == 1
LENGTH:C >= 0", 13),
            L(@"LET LOW = 6
LET HIGH = 10
LIST SRC
PUSH SRC = 3
PUSH SRC = 7
PUSH SRC = 11
PUSH SRC = 5
LIST A
LIST B
LIST C
INJECT A = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < LOW
      INJECT A = VAL
   ELIF VAL < HIGH
      PUSH B = VAL
   ELSE
      PUSH C = VAL
   CNT = CNT + 1
WAIT", @"HIGH == 11
VAL != 6
CNT < HIGH
LENGTH:SRC >= 0
LENGTH:A == 3
LENGTH:B >= 0
LENGTH:C == 1", 0),
            L(@"LET LOW = 5
LET HIGH = 9
LIST SRC
PUSH SRC = 2
PUSH SRC = 6
PUSH SRC = 12
PUSH SRC = 4
LIST A
LIST B
LIST C
INJECT A = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < LOW
      INJECT A = VAL
   ELIF VAL < HIGH
      PUSH B = VAL
   ELSE
      PUSH C = VAL
   CNT = CNT + 1
WAIT", @"HIGH == 10
VAL < HIGH
CNT >= 4
LENGTH:SRC == 0
LENGTH:A >= 2
LENGTH:B == 1
LENGTH:C >= 0
LOW < HIGH", 0),
            L(@"LET LOW = 6
LET HIGH = 10
LIST SRC
PUSH SRC = 3
PUSH SRC = 7
PUSH SRC = 13
PUSH SRC = 5
LIST A
LIST B
LIST C
INJECT A = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < LOW
      INJECT A = VAL
   ELIF VAL < HIGH
      PUSH B = VAL
   ELSE
      PUSH C = VAL
   CNT = CNT + 1
WAIT", @"HIGH == 11
VAL >= 5
CNT <= 4
LENGTH:SRC >= 0
LENGTH:A == 3
LENGTH:B >= 0
LENGTH:C == 1
LOW > VAL", 0),
            L(@"LET LOW = 5
LET HIGH = 9
LIST SRC
PUSH SRC = 2
PUSH SRC = 6
PUSH SRC = 14
PUSH SRC = 4
LIST A
LIST B
LIST C
INJECT A = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:SRC > 0
   VAL = SHIFT:SRC
   IF VAL < LOW
      INJECT A = VAL
   ELIF VAL < HIGH
      PUSH B = VAL
   ELSE
      PUSH C = VAL
   CNT = CNT + 1
WAIT", @"HIGH == 10
VAL <= 4
CNT != 5
LENGTH:SRC == 0
LENGTH:A >= 2
LENGTH:B == 1
LENGTH:C >= 0
LOW <= 6", 13, 1),

            // Level 25 - bounded retries
            L(@"LET CUT = 5
LET LIM = 2
LIST QUE
PUSH QUE = 1
PUSH QUE = 6
PUSH QUE = 2
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      IF RET >= LIM
         STOP
      ELSE
         SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"LIM == 3
VAL != 8
RET < LIM
LENGTH:QUE >= 0
LENGTH:DONE == 4
CUT <= 6", 13),
            L(@"LET CUT = 6
LET LIM = 3
LIST QUE
PUSH QUE = 2
PUSH QUE = 7
PUSH QUE = 3
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      IF RET >= LIM
         STOP
      ELSE
         SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"LIM == 4
VAL > LIM
RET >= 2
LENGTH:QUE == 0
LENGTH:DONE >= 3
CUT >= 5", 0),
            L(@"LET CUT = 5
LET LIM = 2
LIST QUE
PUSH QUE = 1
PUSH QUE = 8
PUSH QUE = 2
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      IF RET >= LIM
         STOP
      ELSE
         SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"LIM == 3
VAL >= 7
RET <= 2
LENGTH:QUE >= 0
LENGTH:DONE == 4
CUT != 6", 0),
            L(@"LET CUT = 6
LET LIM = 3
LIST QUE
PUSH QUE = 2
PUSH QUE = 9
PUSH QUE = 3
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      IF RET >= LIM
         STOP
      ELSE
         SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"LIM == 4
VAL <= 9
RET != 3
LENGTH:QUE == 0
LENGTH:DONE >= 3
CUT > LIM", 0),
            L(@"LET CUT = 5
LET LIM = 2
LIST QUE
PUSH QUE = 1
PUSH QUE = 10
PUSH QUE = 2
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET RET = 0
LOOP LENGTH:QUE > 0
   VAL = SHIFT:QUE
   IF VAL < CUT
      RET = RET + 1
      INJECT QUE = VAL + CUT
      IF RET >= LIM
         STOP
      ELSE
         SKIP
   ELIF VAL == CUT
      PUSH DONE = VAL
   ELSE
      PUSH DONE = VAL
WAIT", @"LIM == 3
VAL != 8
RET < LIM
LENGTH:QUE >= 0
LENGTH:DONE == 4
CUT > RET", 13, 1),

            // Level 26 - aggregation
            L(@"LET CUT = 5
LIST DATA
PUSH DATA = 2
PUSH DATA = 5
PUSH DATA = 8
PUSH DATA = 3
LIST LOW
LIST HIGH
INJECT LOW = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
LOOP LENGTH:DATA > 0
   VAL = SHIFT:DATA
   SUM = SUM + VAL
   CNT = CNT + 1
   IF VAL < CUT
      INJECT LOW = VAL
   ELIF VAL > MAX
      PUSH HIGH = VAL
      MAX = VAL
   ELSE
      PUSH HIGH = VAL
WAIT", @"CUT == 6
VAL < CUT
SUM >= 18
CNT <= 4
MAX != 9
LOW == [3,5,2,0]
LENGTH:LOW == 4
LENGTH:HIGH >= 0
LENGTH:DATA == 0", 12),
            L(@"LET CUT = 6
LIST DATA
PUSH DATA = 3
PUSH DATA = 6
PUSH DATA = 9
PUSH DATA = 4
LIST LOW
LIST HIGH
INJECT LOW = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
LOOP LENGTH:DATA > 0
   VAL = SHIFT:DATA
   SUM = SUM + VAL
   CNT = CNT + 1
   IF VAL < CUT
      INJECT LOW = VAL
   ELIF VAL > MAX
      PUSH HIGH = VAL
      MAX = VAL
   ELSE
      PUSH HIGH = VAL
WAIT", @"CUT == 7
VAL >= 4
SUM <= 22
CNT != 5
MAX > CUT
LOW == [4,6,3,0]
LENGTH:LOW >= 3
LENGTH:HIGH == 1
LENGTH:DATA >= 0", 0),
            L(@"LET CUT = 7
LIST DATA
PUSH DATA = 4
PUSH DATA = 7
PUSH DATA = 10
PUSH DATA = 5
LIST LOW
LIST HIGH
INJECT LOW = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
LOOP LENGTH:DATA > 0
   VAL = SHIFT:DATA
   SUM = SUM + VAL
   CNT = CNT + 1
   IF VAL < CUT
      INJECT LOW = VAL
   ELIF VAL > MAX
      PUSH HIGH = VAL
      MAX = VAL
   ELSE
      PUSH HIGH = VAL
WAIT", @"CUT == 8
VAL <= 5
SUM != 27
CNT < CUT
MAX >= 10
LOW == [5,7,4,0]
LENGTH:LOW == 4
LENGTH:HIGH >= 0
LENGTH:DATA == 0", 0),
            L(@"LET CUT = 8
LIST DATA
PUSH DATA = 5
PUSH DATA = 8
PUSH DATA = 11
PUSH DATA = 6
LIST LOW
LIST HIGH
INJECT LOW = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
LOOP LENGTH:DATA > 0
   VAL = SHIFT:DATA
   SUM = SUM + VAL
   CNT = CNT + 1
   IF VAL < CUT
      INJECT LOW = VAL
   ELIF VAL > MAX
      PUSH HIGH = VAL
      MAX = VAL
   ELSE
      PUSH HIGH = VAL
WAIT", @"CUT == 9
VAL != 7
SUM > CUT
CNT >= 4
MAX <= 11
LOW == [6,8,5,0]
LENGTH:LOW >= 3
LENGTH:HIGH == 1
LENGTH:DATA >= 0", 0),
            L(@"LET CUT = 9
LIST DATA
PUSH DATA = 6
PUSH DATA = 9
PUSH DATA = 12
PUSH DATA = 7
LIST LOW
LIST HIGH
INJECT LOW = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LET MAX = 0
LOOP LENGTH:DATA > 0
   VAL = SHIFT:DATA
   SUM = SUM + VAL
   CNT = CNT + 1
   IF VAL < CUT
      INJECT LOW = VAL
   ELIF VAL > MAX
      PUSH HIGH = VAL
      MAX = VAL
   ELSE
      PUSH HIGH = VAL
WAIT", @"CUT == 10
VAL < CUT
SUM >= 34
CNT <= 4
MAX != 13
LOW == [7,9,6,0]
LENGTH:LOW == 4
LENGTH:HIGH >= 0
LENGTH:DATA == 0", 12, 1),

            // Level 27 - ordered merging
            L(@"LET CUT = 6
LIST A
PUSH A = 1
PUSH A = 4
PUSH A = 7
LIST B
PUSH B = 2
PUSH B = 5
PUSH B = 8
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH OUT = VAL
   ELSE
      INJECT OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 7
VAL >= 8
CNT <= 6
LENGTH:A >= 0
LENGTH:B == 0
LENGTH:OUT >= 6", 12),
            L(@"LET CUT = 7
LIST A
PUSH A = 2
PUSH A = 5
PUSH A = 8
LIST B
PUSH B = 3
PUSH B = 6
PUSH B = 9
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH OUT = VAL
   ELSE
      INJECT OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 8
VAL <= 9
CNT != 7
LENGTH:A == 0
LENGTH:B >= 0
LENGTH:OUT == 7", 0),
            L(@"LET CUT = 8
LIST A
PUSH A = 3
PUSH A = 6
PUSH A = 9
LIST B
PUSH B = 4
PUSH B = 7
PUSH B = 10
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH OUT = VAL
   ELSE
      INJECT OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 9
VAL != 11
CNT < CUT
LENGTH:A >= 0
LENGTH:B == 0
LENGTH:OUT >= 6", 0),
            L(@"LET CUT = 9
LIST A
PUSH A = 4
PUSH A = 7
PUSH A = 10
LIST B
PUSH B = 5
PUSH B = 8
PUSH B = 11
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH OUT = VAL
   ELSE
      INJECT OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 10
VAL > CUT
CNT >= 6
LENGTH:A == 0
LENGTH:B >= 0
LENGTH:OUT == 7", 0),
            L(@"LET CUT = 10
LIST A
PUSH A = 5
PUSH A = 8
PUSH A = 11
LIST B
PUSH B = 6
PUSH B = 9
PUSH B = 12
LIST OUT
INJECT OUT = 0
LET VAL = 0
LET CNT = 0
LOOP LENGTH:A > 0 OR LENGTH:B > 0
   IF LENGTH:A > 0
      VAL = SHIFT:A
   ELIF LENGTH:B > 0
      VAL = SHIFT:B
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH OUT = VAL
   ELSE
      INJECT OUT = VAL
   CNT = CNT + 1
WAIT", @"CUT == 11
VAL >= 12
CNT <= 6
LENGTH:A >= 0
LENGTH:B == 0
LENGTH:OUT >= 6", 12, 1),

            // Level 28 - priority scheduler
            L(@"LET CUT = 7
LIST NORM
PUSH NORM = 3
PUSH NORM = 6
PUSH NORM = 9
LIST PRIO
INJECT PRIO = 1
INJECT PRIO = 2
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET PCNT = 0
LOOP LENGTH:NORM > 0 OR LENGTH:PRIO > 0
   IF LENGTH:PRIO > 0
      VAL = SHIFT:PRIO
      PCNT = PCNT + 1
   ELIF LENGTH:NORM > 0
      VAL = SHIFT:NORM
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH DONE = VAL
   ELSE
      INJECT DONE = VAL
WAIT", @"CUT == 8
VAL <= 9
PCNT != 3
LENGTH:NORM == 0
LENGTH:PRIO >= 0
LENGTH:DONE == 6", 12),
            L(@"LET CUT = 8
LIST NORM
PUSH NORM = 4
PUSH NORM = 7
PUSH NORM = 10
LIST PRIO
INJECT PRIO = 2
INJECT PRIO = 3
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET PCNT = 0
LOOP LENGTH:NORM > 0 OR LENGTH:PRIO > 0
   IF LENGTH:PRIO > 0
      VAL = SHIFT:PRIO
      PCNT = PCNT + 1
   ELIF LENGTH:NORM > 0
      VAL = SHIFT:NORM
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH DONE = VAL
   ELSE
      INJECT DONE = VAL
WAIT", @"CUT == 9
VAL != 11
PCNT < CUT
LENGTH:NORM >= 0
LENGTH:PRIO == 0
LENGTH:DONE >= 5", 0),
            L(@"LET CUT = 9
LIST NORM
PUSH NORM = 5
PUSH NORM = 8
PUSH NORM = 11
LIST PRIO
INJECT PRIO = 3
INJECT PRIO = 4
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET PCNT = 0
LOOP LENGTH:NORM > 0 OR LENGTH:PRIO > 0
   IF LENGTH:PRIO > 0
      VAL = SHIFT:PRIO
      PCNT = PCNT + 1
   ELIF LENGTH:NORM > 0
      VAL = SHIFT:NORM
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH DONE = VAL
   ELSE
      INJECT DONE = VAL
WAIT", @"CUT == 10
VAL > CUT
PCNT >= 2
LENGTH:NORM == 0
LENGTH:PRIO >= 0
LENGTH:DONE == 6", 0),
            L(@"LET CUT = 10
LIST NORM
PUSH NORM = 6
PUSH NORM = 9
PUSH NORM = 12
LIST PRIO
INJECT PRIO = 4
INJECT PRIO = 5
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET PCNT = 0
LOOP LENGTH:NORM > 0 OR LENGTH:PRIO > 0
   IF LENGTH:PRIO > 0
      VAL = SHIFT:PRIO
      PCNT = PCNT + 1
   ELIF LENGTH:NORM > 0
      VAL = SHIFT:NORM
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH DONE = VAL
   ELSE
      INJECT DONE = VAL
WAIT", @"CUT == 11
VAL >= 12
PCNT <= 2
LENGTH:NORM >= 0
LENGTH:PRIO == 0
LENGTH:DONE >= 5", 0),
            L(@"LET CUT = 11
LIST NORM
PUSH NORM = 7
PUSH NORM = 10
PUSH NORM = 13
LIST PRIO
INJECT PRIO = 5
INJECT PRIO = 6
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET PCNT = 0
LOOP LENGTH:NORM > 0 OR LENGTH:PRIO > 0
   IF LENGTH:PRIO > 0
      VAL = SHIFT:PRIO
      PCNT = PCNT + 1
   ELIF LENGTH:NORM > 0
      VAL = SHIFT:NORM
   ELSE
      VAL = 0
   IF VAL < CUT
      PUSH DONE = VAL
   ELSE
      INJECT DONE = VAL
WAIT", @"CUT == 12
VAL <= 13
PCNT != 3
LENGTH:NORM == 0
LENGTH:PRIO >= 0
LENGTH:DONE == 6", 12, 1),

            // Level 29 - sentinels
            L(@"LET CUT = 5
LIST IN
PUSH IN = 3
PUSH IN = 5
PUSH IN = 0
PUSH IN = 8
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL < CUT
      INJECT DONE = VAL
      SUM = SUM + VAL
   ELSE
      PUSH DONE = VAL
      SUM = SUM + VAL
   CNT = CNT + 1
WAIT", @"CUT == 6
VAL != 1
SUM > CUT
CNT >= 2
DONE == [5,3,0]
LENGTH:DONE >= 2
LENGTH:IN == 1", 12),
            L(@"LET CUT = 6
LIST IN
PUSH IN = 4
PUSH IN = 6
PUSH IN = 0
PUSH IN = 9
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL < CUT
      INJECT DONE = VAL
      SUM = SUM + VAL
   ELSE
      PUSH DONE = VAL
      SUM = SUM + VAL
   CNT = CNT + 1
WAIT", @"CUT == 7
VAL < CUT
SUM >= 10
CNT <= 2
DONE == [6,4,0]
LENGTH:DONE == 3
LENGTH:IN >= 0", 0),
            L(@"LET CUT = 7
LIST IN
PUSH IN = 5
PUSH IN = 7
PUSH IN = 0
PUSH IN = 10
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL < CUT
      INJECT DONE = VAL
      SUM = SUM + VAL
   ELSE
      PUSH DONE = VAL
      SUM = SUM + VAL
   CNT = CNT + 1
WAIT", @"CUT == 8
VAL >= 0
SUM <= 12
CNT != 3
DONE == [7,5,0]
LENGTH:DONE >= 2
LENGTH:IN == 1", 0),
            L(@"LET CUT = 8
LIST IN
PUSH IN = 6
PUSH IN = 8
PUSH IN = 0
PUSH IN = 11
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL < CUT
      INJECT DONE = VAL
      SUM = SUM + VAL
   ELSE
      PUSH DONE = VAL
      SUM = SUM + VAL
   CNT = CNT + 1
WAIT", @"CUT == 9
VAL <= 0
SUM != 15
CNT < CUT
DONE == [8,6,0]
LENGTH:DONE == 3
LENGTH:IN >= 0", 0),
            L(@"LET CUT = 9
LIST IN
PUSH IN = 7
PUSH IN = 9
PUSH IN = 0
PUSH IN = 12
LIST DONE
INJECT DONE = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL < CUT
      INJECT DONE = VAL
      SUM = SUM + VAL
   ELSE
      PUSH DONE = VAL
      SUM = SUM + VAL
   CNT = CNT + 1
WAIT", @"CUT == 10
VAL != 1
SUM > CUT
CNT >= 2
DONE == [9,7,0]
LENGTH:DONE >= 2
LENGTH:IN == 1", 12, 1),

            // Level 30 - final dispatch
            L(@"LET CUT = 5
LIST IN
PUSH IN = 3
PUSH IN = 8
PUSH IN = 1
PUSH IN = 0
PUSH IN = 6
LIST GOOD
LIST RET
INJECT RET = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL >= CUT
      PUSH GOOD = VAL
      SUM = SUM + VAL
   ELSE
      INJECT RET = VAL + CUT
      CNT = CNT + 1
WAIT", @"CUT == 6
VAL < CUT
SUM >= 8
CNT <= 2
RET == [7,9,0]
LENGTH:RET == 3
LENGTH:IN >= 0
LENGTH:GOOD == 1", 12),
            L(@"LET CUT = 6
LIST IN
PUSH IN = 4
PUSH IN = 9
PUSH IN = 2
PUSH IN = 0
PUSH IN = 7
LIST GOOD
LIST RET
INJECT RET = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL >= CUT
      PUSH GOOD = VAL
      SUM = SUM + VAL
   ELSE
      INJECT RET = VAL + CUT
      CNT = CNT + 1
WAIT", @"CUT == 7
VAL >= 0
SUM <= 9
CNT != 3
RET == [9,11,0]
LENGTH:RET >= 2
LENGTH:IN == 1
LENGTH:GOOD >= 0", 0),
            L(@"LET CUT = 5
LIST IN
PUSH IN = 3
PUSH IN = 10
PUSH IN = 3
PUSH IN = 0
PUSH IN = 8
LIST GOOD
LIST RET
INJECT RET = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL >= CUT
      PUSH GOOD = VAL
      SUM = SUM + VAL
   ELSE
      INJECT RET = VAL + CUT
      CNT = CNT + 1
WAIT", @"CUT == 6
VAL <= 0
SUM != 11
CNT < CUT
RET == [9,9,0]
LENGTH:RET == 3
LENGTH:IN >= 0
LENGTH:GOOD == 1", 0),
            L(@"LET CUT = 6
LIST IN
PUSH IN = 4
PUSH IN = 11
PUSH IN = 1
PUSH IN = 0
PUSH IN = 9
LIST GOOD
LIST RET
INJECT RET = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL >= CUT
      PUSH GOOD = VAL
      SUM = SUM + VAL
   ELSE
      INJECT RET = VAL + CUT
      CNT = CNT + 1
WAIT", @"CUT == 7
VAL != 1
SUM > CUT
CNT >= 2
RET == [8,11,0]
LENGTH:RET >= 2
LENGTH:IN == 1
LENGTH:GOOD >= 0", 0),
            L(@"LET CUT = 5
LIST IN
PUSH IN = 3
PUSH IN = 12
PUSH IN = 2
PUSH IN = 0
PUSH IN = 10
LIST GOOD
LIST RET
INJECT RET = 0
LET VAL = 0
LET SUM = 0
LET CNT = 0
LOOP LENGTH:IN > 0
   VAL = SHIFT:IN
   IF VAL == 0
      STOP
   ELIF VAL >= CUT
      PUSH GOOD = VAL
      SUM = SUM + VAL
   ELSE
      INJECT RET = VAL + CUT
      CNT = CNT + 1
WAIT", @"CUT == 6
VAL < CUT
SUM >= 12
CNT <= 2
RET == [8,9,0]
LENGTH:RET == 3
LENGTH:IN >= 0
LENGTH:GOOD == 1", 12, 1),

        };
    }
}
