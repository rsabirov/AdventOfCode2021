using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2021
{
    public sealed class Solutions
    {
        [TestCase("Day01_problem.txt", ExpectedResult = 1301)]
        [TestCase("Day01_test.txt", ExpectedResult = 7)]
        public int Day01_1(string fileName)
        {
            var input = ReadAllLines(fileName).Select(int.Parse).ToArray();

            int increaseCount = 0;
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i - 1] < input[i])
                    increaseCount++;
            }

            return increaseCount;
        }

        [TestCase("Day01_problem.txt", ExpectedResult = 1346)]
        [TestCase("Day01_test.txt", ExpectedResult = 5)]
        public int Day01_2(string fileName)
        {
            var input = ReadAllLines(fileName).Select(int.Parse).ToArray();

            const int windowSize = 3;
            int increaseCount = 0;
            var prev = input.Take(windowSize).Sum();
            for (int i = 1; i < input.Length - windowSize + 1; i++)
            {
                var curr = input.Skip(i).Take(windowSize).Sum();
                if (curr > prev)
                    increaseCount++;
                prev = curr;
            }

            return increaseCount;
        }

        [TestCase("Day02_problem.txt", ExpectedResult = 1507611)]
        [TestCase("Day02_test.txt", ExpectedResult = 150)]
        public int Day02_1(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(parts => (command: parts[0], param: int.Parse(parts[1])))
                .ToArray();

            var depth = 0;
            var position = 0;
            foreach (var line in inputs)
            {
                switch (line.command)
                {
                    case "forward":
                        position += line.param;
                        break;
                    case "down":
                        depth += line.param;
                        break;
                    case "up":
                        depth -= line.param;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return depth * position;
        }

        [TestCase("Day02_problem.txt", ExpectedResult = 1880593125)]
        [TestCase("Day02_test.txt", ExpectedResult = 900)]
        public int Day02_2(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(parts => (command: parts[0], param: int.Parse(parts[1])))
                .ToArray();

            var depth = 0;
            var position = 0;
            var aim = 0;
            foreach (var line in inputs)
            {
                switch (line.command)
                {
                    case "forward":
                        position += line.param;
                        depth += aim * line.param;
                        break;
                    case "down":
                        aim += line.param;
                        break;
                    case "up":
                        aim -= line.param;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return depth * position;
        }

        [TestCase("Day03_problem.txt", ExpectedResult = 3687446)]
        [TestCase("Day03_test.txt", ExpectedResult = 198)]
        public int Day03_1(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(line => line.Select(c => int.Parse(c.ToString())).ToArray())
                .ToArray();

            var gamma = 0;
            var epsilon = 0;

            for (int i = 0; i < inputs[0].Length; i++)
            {
                gamma <<= 1;
                epsilon <<= 1;

                var count0 = inputs.Count(line => line[i] == 0);
                var count1 = inputs.Count(line => line[i] == 1);

                if (count0 < count1)
                    gamma++;
                else
                    epsilon++;
            }

            return gamma * epsilon;
        }

        [TestCase("Day03_problem.txt", ExpectedResult = 4406844)]
        [TestCase("Day03_test.txt", ExpectedResult = 230)]
        public int Day03_2(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(line => line.Select(c => int.Parse(c.ToString())).ToArray())
                .ToArray();

            var oxygen = Reduce(inputs, false);
            var co2 = Reduce(inputs, true);

            return co2 * oxygen;

            int Reduce(int[][] arrays, bool keepFewer)
            {
                var list = arrays.ToList();

                int i = 0;
                while (list.Count > 1 && i < list[0].Length)
                {
                    var count0 = list.Count(line => line[i] == 0);
                    var count1 = list.Count(line => line[i] == 1);

                    if (keepFewer)
                    {
                        var valueToKeep = count0 <= count1 ? 0 : 1;
                        list = list.Where(line => line[i] == valueToKeep).ToList();
                    }
                    else
                    {
                        var valueToKeep = count1 >= count0 ? 1 : 0;
                        list = list.Where(line => line[i] == valueToKeep).ToList();
                    }

                    i++;
                }

                return BitsToNumber(list[0]);
            }

            int BitsToNumber(int[] bits)
            {
                var val = 0;
                for (int i = bits.Length - 1; i >= 0; i--)
                {
                    var j = bits.Length - 1 - i;
                    if (bits[i] == 1)
                        val += 1 << j;
                }

                return val;
            }
        }

        [TestCase("Day04_problem.txt", ExpectedResult = 35670)]
        [TestCase("Day04_test.txt", ExpectedResult = 4512)]
        public int Day04_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var steps = inputs[0].Split(',').Select(int.Parse).ToArray();
            var (matrix, size) = ParseMatrix(inputs.Skip(2).ToArray());
            var boolMatrix = CreateBoolMatrix(matrix.Count, size);

            foreach (var step in steps)
            {
                var (endOfGame, winningMatrix, winningBoolMatrix) = MarkAndCheck(matrix, boolMatrix, size, step);
                if (!endOfGame)
                    continue;

                return CalcScore(winningMatrix, winningBoolMatrix, step);
            }

            return 0;

            (List<int[][]> matrix, int size) ParseMatrix(string[] s)
            {
                var res = new List<int[][]>();
                var size = s[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

                int row = 0;
                while (row < s.Length)
                {
                    if (string.IsNullOrEmpty(s[row]))
                    {
                        row++;
                        continue;
                    }

                    var curr = new int[size][];
                    for (int i = 0; i < size; i++)
                    {
                        curr[i] = s[row++].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    }
                    res.Add(curr);
                }
                return (res, size);
            }

            List<bool[,]> CreateBoolMatrix(int count, int size)
            {
                var result = new List<bool[,]>();
                for (int i = 0; i < count; i++)
                    result.Add(new bool[size, size]);

                return result;
            }

            (bool endOfGame, int[][] winningMatrix, bool[,] winningBoolMatrix) MarkAndCheck(List<int[][]> matrix, List<bool[,]> boolMatrix, int size, int step)
            {
                for (int i = 0; i < matrix.Count; i++)
                {
                    var m = matrix[i];
                    var b = boolMatrix[i];

                    for (int x = 0; x < size; x++)
                    {
                        for (int y = 0; y < size; y++)
                        {
                            if (m[x][y] == step)
                            {
                                b[x, y] = true;
                                break;
                            }
                        }
                    }

                    for (int j = 0; j < size; j++)
                    {
                        if (Enumerable.Range(0, size).Select(x => b[j, x]).All(_ => _))
                            return (true, m, b);
                        if (Enumerable.Range(0, size).Select(x => b[x, j]).All(_ => _))
                            return (true, m, b);
                    }
                }

                return (false, null, null);
            }

            int CalcScore(int[][] winningMatrix, bool[,] winningBoolMatrix, int step)
            {
                var result = 0;
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        if (!winningBoolMatrix[x, y])
                            result += winningMatrix[x][y];
                    }
                }

                return result * step;
            }
        }

        [TestCase("Day04_problem.txt", ExpectedResult = 22704)]
        [TestCase("Day04_test.txt", ExpectedResult = 1924)]
        public int Day04_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var steps = inputs[0].Split(',').Select(int.Parse).ToArray();
            var (matrix, size) = ParseMatrix(inputs.Skip(2).ToArray());
            var boolMatrix = CreateBoolMatrix(matrix.Count, size);

            foreach (var step in steps)
            {
                var (winningMatrix, winningBoolMatrix) = MarkAndCheck(matrix, boolMatrix, size, step);

                if (matrix.Count > 1)
                {
                    winningMatrix.ForEach(m => matrix.Remove(m));
                    winningBoolMatrix.ForEach(m => boolMatrix.Remove(m));

                    continue;
                }

                if (winningMatrix.Count > 0)
                    return CalcScore(matrix[0], boolMatrix[0], step);
            }

            return 0;

            (List<int[][]> matrix, int size) ParseMatrix(string[] s)
            {
                var res = new List<int[][]>();
                var size = s[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

                int row = 0;
                while (row < s.Length)
                {
                    if (string.IsNullOrEmpty(s[row]))
                    {
                        row++;
                        continue;
                    }

                    var curr = new int[size][];
                    for (int i = 0; i < size; i++)
                    {
                        curr[i] = s[row++].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    }
                    res.Add(curr);
                }
                return (res, size);
            }

            List<bool[,]> CreateBoolMatrix(int count, int size)
            {
                var result = new List<bool[,]>();
                for (int i = 0; i < count; i++)
                    result.Add(new bool[size, size]);

                return result;
            }

            (List<int[][]> winningMatrix, List<bool[,]> winningBoolMatrix) MarkAndCheck(List<int[][]> matrix, List<bool[,]> boolMatrix, int size, int step)
            {
                var winningMatrix = new List<int[][]>();
                var winningBoolMatrix = new List<bool[,]>();

                for (int i = 0; i < matrix.Count; i++)
                {
                    var m = matrix[i];
                    var b = boolMatrix[i];

                    for (int x = 0; x < size; x++)
                    {
                        for (int y = 0; y < size; y++)
                        {
                            if (m[x][y] == step)
                            {
                                b[x, y] = true;
                                break;
                            }
                        }
                    }

                    for (int j = 0; j < size; j++)
                    {
                        if (Enumerable.Range(0, size).Select(x => b[j, x]).All(_ => _)
                            || Enumerable.Range(0, size).Select(x => b[x, j]).All(_ => _))
                        {
                            winningMatrix.Add(m);
                            winningBoolMatrix.Add(b);
                        }
                    }
                }

                return (winningMatrix, winningBoolMatrix);
            }

            int CalcScore(int[][] winningMatrix, bool[,] winningBoolMatrix, int step)
            {
                var result = 0;
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        if (!winningBoolMatrix[x, y])
                            result += winningMatrix[x][y];
                    }
                }

                return result * step;
            }
        }

        [TestCase("Day05_problem.txt", ExpectedResult = 6666)]
        [TestCase("Day05_test.txt", ExpectedResult = 5)]
        public int Day05_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var lines = ParseLines(inputs);

            // only consider horizontal and vertical lines
            lines = lines.Where(l => l.b.x == l.e.x || l.b.y == l.e.y).ToList();

            var field = new int[1000, 1000];
            foreach (var line in lines)
            {
                ProcessLine(field, line);
            }

            return CalcAnswer(field);

            void ProcessLine(int[,] field, Line line)
            {
                if (line.b.x == line.e.x)
                {
                    for (int y = Math.Min(line.b.y, line.e.y);
                        y <= Math.Max(line.b.y, line.e.y); y++)
                    {
                        field[line.b.x, y]++;
                    }
                }
                if (line.b.y == line.e.y)
                {
                    for (int x = Math.Min(line.b.x, line.e.x);
                        x <= Math.Max(line.b.x, line.e.x); x++)
                    {
                        field[x, line.b.y]++;
                    }
                }
            }

            int CalcAnswer(int[,] field)
            {
                var result = 0;
                for (int x = 0; x < 1000; x++)
                {
                    for (int y = 0; y < 1000; y++)
                        if (field[x, y] >= 2)
                            result++;
                }

                return result;
            }

            List<Line> ParseLines(string[] inputs)
            {
                var lines = new List<Line>();
                foreach (var input in inputs)
                {
                    var parts = input.Split(" -> ");
                    lines.Add(new Line(
                        ParsePoint(parts[0]),
                        ParsePoint(parts[1])));
                }

                return lines;
            }

            Point ParsePoint(string s)
            {
                var parts = s.Split(',').Select(int.Parse).ToArray();
                return new Point(parts[0], parts[1]);
            }
        }

        [TestCase("Day05_problem.txt", ExpectedResult = 19081)]
        [TestCase("Day05_test.txt", ExpectedResult = 12)]
        public int Day05_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var lines = ParseLines(inputs);

            var field = new int[1000, 1000];
            foreach (var line in lines)
            {
                ProcessLine(field, line);
            }

            return CalcAnswer(field);

            void ProcessLine(int[,] field, Line line)
            {
                var dx = line.e.x - line.b.x;
                var dy = line.e.y - line.b.y;
                var steps = Math.Max(Math.Abs(dx), Math.Abs(dy)) + 1;
                int x = line.b.x;
                int y = line.b.y;
                dx = dx == 0 ? 0 : dx / Math.Abs(dx);
                dy = dy == 0 ? 0 : dy / Math.Abs(dy);

                for (int i = 0; i < steps; i++)
                {
                    field[x, y]++;

                    x += dx;
                    y += dy;
                }
            }

            int CalcAnswer(int[,] field)
            {
                var result = 0;
                for (int x = 0; x < 1000; x++)
                {
                    for (int y = 0; y < 1000; y++)
                        if (field[x, y] >= 2)
                            result++;
                }

                return result;
            }

            List<Line> ParseLines(string[] inputs)
            {
                var lines = new List<Line>();
                foreach (var input in inputs)
                {
                    var parts = input.Split(" -> ");
                    lines.Add(new Line(
                        ParsePoint(parts[0]),
                        ParsePoint(parts[1])));
                }

                return lines;
            }

            Point ParsePoint(string s)
            {
                var parts = s.Split(',').Select(int.Parse).ToArray();
                return new Point(parts[0], parts[1]);
            }
        }

        [TestCase("Day06_problem.txt", ExpectedResult = 362639)]
        [TestCase("Day06_test.txt", ExpectedResult = 5934)]
        public int Day06_1(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .SelectMany(line => line.Split(','))
                .Select(int.Parse)
                .ToList();

            for (int i = 0; i < 80; i++)
            {
                var currentCount = inputs.Count;
                for (int j = 0; j < currentCount; j++)
                {
                    if (inputs[j] == 0)
                    {
                        inputs[j] = 6;
                        inputs.Add(8);
                    }
                    else
                    {
                        inputs[j]--;
                    }
                }
            }

            return inputs.Count;
        }

        [TestCase("Day06_problem.txt", ExpectedResult = 1639854996917)]
        [TestCase("Day06_test.txt", ExpectedResult = 26984457539)]
        public long Day06_2(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .SelectMany(line => line.Split(','))
                .Select(int.Parse)
                .ToList();

            long ans = 0;
            long[] counters = new long[10];
            foreach (var stage in inputs)
                counters[stage]++;

            for (int i = 0; i < 256; i++)
            {
                var next = new long[10];
                for (int j = 1; j <= 8; j++)
                    next[j - 1] = counters[j];

                // timer 0 is a new born
                // internal timer would reset to 6
                next[6] += counters[0];

                // and it would create a new lanternfish with an internal timer of 8
                next[8] += counters[0];

                counters = next;
            }

            return counters.Sum();
        }

        [TestCase("Day07_problem.txt", ExpectedResult = 347011)]
        [TestCase("Day07_test.txt", ExpectedResult = 37)]
        public int Day07_1(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .SelectMany(line => line.Split(','))
                .Select(int.Parse)
                .ToList();

            var min = Enumerable.Range(inputs.Min(), inputs.Max() - inputs.Min())
                .Select(CalcCost)
                .Min();

            return min;

            int CalcCost(int x)
            {
                return inputs.Sum(a => Math.Abs(x - a));
            }
        }

        [TestCase("Day07_problem.txt", ExpectedResult = 98363777)]
        [TestCase("Day07_test.txt", ExpectedResult = 168)]
        public int Day07_2(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .SelectMany(line => line.Split(','))
                .Select(int.Parse)
                .ToList();

            var min = Enumerable.Range(inputs.Min(), inputs.Max() - inputs.Min())
                .Select(CalcCost)
                .Min();

            return min;

            int CalcCost(int x)
            {
                return inputs.Sum(a => (1 + Math.Abs(x - a)) * Math.Abs(x - a) / 2);
            }
        }

        [TestCase("Day08_problem.txt", ExpectedResult = 264)]
        [TestCase("Day08_test.txt", ExpectedResult = 26)]
        public int Day08_1(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(line =>
                {
                    var parts = line.Split('|');

                    return (
                        left: parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries),
                        right: parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries));
                })
                .ToList();

            var sizes = new HashSet<int>(new[] { 2, 3, 4, 7 });

            return inputs.Sum(item => item.right.Count(i => sizes.Contains(i.Length)));
        }

        [TestCase("Day08_test.txt", ExpectedResult = 61229)]
        [TestCase("Day08_problem.txt", ExpectedResult = 1063760)]
        public int Day08_2(string fileName)
        {
            var inputs = ReadAllLines(fileName)
                .Select(line =>
                {
                    var parts = line.Split('|');

                    return (
                        left: parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries),
                        right: parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries));
                })
                .ToList();

            var results = inputs.Select(i => Solve(i.left, i.right));

            return results.Sum();


            int Solve(string[] left, string[] right)
            {
                left = left.Select(Normalize).ToArray();
                right = right.Select(Normalize).ToArray();

                var map = new Dictionary<string, int>();
                var reverseMap = new Dictionary<int, string>();
                AddDigit(map, reverseMap, 1, left.FirstOrDefault(i => i.Length == 2));
                AddDigit(map, reverseMap, 7, left.FirstOrDefault(i => i.Length == 3));
                AddDigit(map, reverseMap, 4, left.FirstOrDefault(i => i.Length == 4));
                AddDigit(map, reverseMap, 8, left.FirstOrDefault(i => i.Length == 7));

                // can be 2, 3, 5
                var sizeFive = left.Where(i => i.Length == 5).ToList();
                // can be 0, 6, 9
                var sizeSix = left.Where(i => i.Length == 6).ToList();

                // 3
                var three = sizeFive.FirstOrDefault(i => IsSubset(i, reverseMap[1]));
                sizeFive.Remove(three);
                AddDigit(map, reverseMap, 3, three);
                // three = five.FirstOrDefault(i => IsSubset(i, reverseMap[7]));
                // AddDigit(map, reverseMap, 3, three);

                // 9
                var nine = sizeSix.FirstOrDefault(i => IsSubset(i, reverseMap[4]));
                sizeSix.Remove(nine);
                AddDigit(map, reverseMap, 9, nine);

                // 5
                var fourMinusOne = Substract(reverseMap[4], reverseMap[1]);
                var five = sizeFive.FirstOrDefault(i => IsSubset(i, fourMinusOne));
                sizeFive.Remove(five);
                AddDigit(map, reverseMap, 5, five);

                // 2
                AddDigit(map, reverseMap, 2, sizeFive[0]);

                // 6
                var six = sizeSix.FirstOrDefault(i => IsSubset(i, fourMinusOne));
                sizeSix.Remove(six);
                AddDigit(map, reverseMap, 6, six);

                // 0
                AddDigit(map, reverseMap, 0, sizeSix[0]);
                
                var result = 0;
                var multiplicand = 1000;
                foreach (var item in right)
                {
                    var digit = map[item];
                    result += digit * multiplicand;

                    multiplicand /= 10;
                }

                return result;
            }

            void AddDigit(Dictionary<string, int> map, Dictionary<int, string> reverseMap, int digit, string? pattern)
            {
                if (pattern == null)
                    return;

                map.Add(pattern, digit);
                reverseMap.Add(digit, pattern);
            }

            string Normalize(string pattern)
            {
                return new string(pattern.OrderBy(_ => _).ToArray());
            }

            bool IsSubset(string primary, string sub)
            {
                return sub.All(primary.Contains);
            }

            string Substract(string primary, string sub)
            {
                var set = primary.Where(c => !sub.Contains(c)).ToArray();
                return new string(set);
            }
        }

        [TestCase("Day09_test.txt", ExpectedResult = 15)]
        [TestCase("Day09_problem.txt", ExpectedResult = 417)]
        public int Day09_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var map = ParseInput(inputs);
            
            return CalcRiskLevel(map);

            int CalcRiskLevel(int[][] map)
            {
                var risk = 0;
                for (int x = 0; x < map.Length; x++)
                {
                    for (int y = 0; y < map[0].Length; y++)
                    {
                        if (IsLowPoint(map, x, y))
                            risk += 1 + map[x][y];
                    }
                }

                return risk;
            }

            bool IsLowPoint(int[][] map, int x, int y)
            {
                var dx = new[] { +0, +1, +0, -1 };
                var dy = new[] { -1, +0, +1, +0 };
                var sizeX = map.Length;
                var sizeY = map[0].Length;

                for (int i = 0; i < dx.Length; i++)
                {
                    var tx = x + dx[i];
                    var ty = y + dy[i];

                    if (ty < 0 || tx < 0 || ty >= sizeY || tx >= sizeX)
                        continue;
                    if (map[x][y] >= map[tx][ty])
                        return false;
                }

                return true;
            }

            int[][] ParseInput(string[] inputs)
            {
                var result = new List<int[]>();
                foreach (var line in inputs)
                {
                    result.Add(line.Select(c => int.Parse(c.ToString())).ToArray());
                }

                return result.ToArray();
            }
        }

        [TestCase("Day09_test.txt", ExpectedResult = 1134)]
        [TestCase("Day09_problem.txt", ExpectedResult = 1148965)]
        public int Day09_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var map = ParseInput(inputs);

            var basins = new List<int>();
            for (int x1 = 0; x1 < map.Length; x1++)
            {
                for (int y1 = 0; y1 < map[0].Length; y1++)
                {
                    if (IsLowPoint(map, x1, y1))
                        basins.Add(CalcBasinSize(map, x1, y1));
                }
            }

            return basins.OrderByDescending(_ => _).Take(3).Aggregate(1, (a, b) => a * b);

            int CalcBasinSize(int[][] map, int x, int y)
            {
                var dx = new[] { +0, +1, +0, -1 };
                var dy = new[] { -1, +0, +1, +0 };

                var sizeX = map.Length;
                var sizeY = map[0].Length;

                var v = new bool[sizeX, sizeY];

                var q = new Queue<(int x, int y)>();
                q.Enqueue((x, y));
                v[x, y] = true;

                while (q.Count > 0)
                {
                    var p = q.Dequeue();


                    for (int i = 0; i < dx.Length; i++)
                    {
                        var h = map[p.x][p.y];
                        var tx = p.x + dx[i];
                        var ty = p.y + dy[i];

                        if (ty < 0 || tx < 0 || ty >= sizeY || tx >= sizeX)
                            continue;

                        if (map[tx][ty] == 9 || map[tx][ty] <= h || v[tx, ty])
                            continue;

                        q.Enqueue((tx, ty));
                        v[tx, ty] = true;
                    }
                }

                return Enumerable
                    .Range(0, sizeX)
                    .SelectMany(x => Enumerable.Range(0, sizeY).Select(y => v[x, y]))
                    .Count(_ => _);

            }

            bool IsLowPoint(int[][] map, int x, int y)
            {
                var dx = new[] { +0, +1, +0, -1 };
                var dy = new[] { -1, +0, +1, +0 };
                var sizeX = map.Length;
                var sizeY = map[0].Length;

                for (int i = 0; i < dx.Length; i++)
                {
                    var tx = x + dx[i];
                    var ty = y + dy[i];

                    if (ty < 0 || tx < 0 || ty >= sizeY || tx >= sizeX)
                        continue;
                    if (map[x][y] >= map[tx][ty])
                        return false;
                }

                return true;
            }

            int[][] ParseInput(string[] inputs)
            {
                var result = new List<int[]>();
                foreach (var line in inputs)
                {
                    result.Add(line.Select(c => int.Parse(c.ToString())).ToArray());
                }

                return result.ToArray();
            }
        }

        [TestCase("Day12_problem.txt", ExpectedResult = 4338)]
        [TestCase("Day12_test.txt", ExpectedResult = 10)]
        [TestCase("Day12_test2.txt", ExpectedResult = 19)]
        public int Day12_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var graph = ParseInput(inputs);
            RemoveUnreachable(graph);
            var paths = Traverse(graph).ToList();

            return paths.Count();

            IEnumerable<string[]> Traverse(Graph graph)
            {
                var visits = new Dictionary<string, int>();
                foreach (var vertex in graph.Vertexes)
                    visits[vertex] = 0;

                var allPaths = new List<string[]>();

                TraverseImpl(graph, graph.Start, visits, allPaths, new Stack<string>());

                return allPaths;
            }

            void TraverseImpl(Graph graph, string vertex, Dictionary<string, int> visits, List<string[]> allPaths, Stack<string> path)
            {
                path.Push(vertex);
                visits[vertex]++;

                if (vertex == graph.End)
                {
                    allPaths.Add(path.ToArray());

                    path.Pop();
                    visits[vertex]--;
                    return;
                }

                foreach (var nextVertex in graph.Adjacent(vertex))
                {
                    if (visits[nextVertex] > 0 && !AllowsMultipleVisits(nextVertex))
                        continue;

                    TraverseImpl(graph, nextVertex, visits, allPaths, path);
                }

                path.Pop();
                visits[vertex]--;
            }

            bool AllowsMultipleVisits(string vertex)
            {
                return char.IsUpper(vertex[0]);
            }

            void RemoveUnreachable(Graph graph)
            {
                foreach (var vertex in graph.Vertexes.Where(v => !AllowsMultipleVisits(v)).ToArray())
                {
                    var adjacent = graph.Adjacent(vertex).ToArray();
                    if (adjacent.Length is 0 or 1 && !AllowsMultipleVisits(adjacent[0]))
                        graph.RemoveVertex(vertex);
                }
            }

            Graph ParseInput(string[] inputs)
            {
                var graph = new Graph("start", "end");
                foreach (var line in inputs)
                {
                    var nodes = line.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    graph.AddEdge(nodes[1], nodes[0]);
                }

                return graph;
            }
        }

        [TestCase("Day12_problem.txt", ExpectedResult = 114189)]
        [TestCase("Day12_test.txt", ExpectedResult = 36)]
        [TestCase("Day12_test2.txt", ExpectedResult = 103)]
        public int Day12_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var graph = ParseInput(inputs);
            var paths = Traverse(graph).ToList();

            foreach (var path in paths)
                Console.WriteLine(string.Join(", ", path));

            return paths.Count();

            IEnumerable<string[]> Traverse(Graph graph)
            {
                var visits = new Dictionary<string, int>();
                foreach (var vertex in graph.Vertexes)
                    visits[vertex] = 0;

                var allPaths = new List<string[]>();

                TraverseImpl(graph, graph.Start, visits, allPaths, new Stack<string>());

                return allPaths;
            }

            void TraverseImpl(Graph graph, string vertex, Dictionary<string, int> visits, List<string[]> allPaths, Stack<string> path)
            {
                path.Push(vertex);
                visits[vertex]++;

                if (vertex == graph.End)
                {
                    allPaths.Add(path.ToArray());

                    path.Pop();
                    visits[vertex]--;
                    return;
                }

                foreach (var nextVertex in graph.Adjacent(vertex))
                {
                    if (!AllowedToVisits(graph, visits, nextVertex))
                        continue;

                    TraverseImpl(graph, nextVertex, visits, allPaths, path);
                }

                path.Pop();
                visits[vertex]--;
            }

            bool AllowedToVisits(Graph graph, Dictionary<string, int> visits, string vertex)
            {
                if (AllowsMultipleVisits(vertex))
                    return true;

                if (vertex == graph.Start || vertex == graph.End)
                    return visits[vertex] == 0;

                if (visits[vertex] == 0)
                    return true;

                return visits
                    .Where(kv => !AllowsMultipleVisits(kv.Key))
                    .Where(kv => kv.Key != graph.Start && kv.Key != graph.End)
                    .All(kv => kv.Value <= 1);
            }

            bool AllowsMultipleVisits(string vertex)
            {
                return char.IsUpper(vertex[0]);
            }

            Graph ParseInput(string[] inputs)
            {
                var graph = new Graph("start", "end");
                foreach (var line in inputs)
                {
                    var nodes = line.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    graph.AddEdge(nodes[1], nodes[0]);
                }

                return graph;
            }
        }

        [TestCase("Day13_test.txt", ExpectedResult = 17)]
        [TestCase("Day13_problem.txt", ExpectedResult = 790)]
        public int Day13_1(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var (paper, folds) = ParseInput(inputs);

            foreach (var fold in folds.Take(1))
            {
                if (fold > 0)
                    paper.FoldX(fold);
                else
                    paper.FoldY(Math.Abs(fold));
            }

            return paper.MarkedCount();

            (Paper, int[]) ParseInput(string[] inputs)
            {
                var folds = new List<int>();
                var paper = new Paper();

                foreach (var line in inputs)
                {
                    var parts = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                        paper.Mark(int.Parse(parts[0]), int.Parse(parts[1]));
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        Debug.Assert(line.StartsWith("fold along "));
                        var foldParts = line.Substring("fold along ".Length).Split("=", StringSplitOptions.RemoveEmptyEntries);
                        var fold = int.Parse(foldParts[1]) * (foldParts[0] == "x" ? 1 : -1);

                        folds.Add(fold);
                    }
                }
                
                return (paper, folds.ToArray());
            }
        }

        [TestCase("Day13_test.txt", ExpectedResult = 16)] // prints O
        [TestCase("Day13_problem.txt", ExpectedResult = 96)] // prints PGHZBFJC
        public int Day13_2(string fileName)
        {
            var inputs = ReadAllLines(fileName);
            var (paper, folds) = ParseInput(inputs);

            foreach (var fold in folds)
            {
                if (fold > 0)
                    paper.FoldX(fold);
                else
                    paper.FoldY(Math.Abs(fold));
            }

            paper.Print();

            return paper.MarkedCount();

            (Paper, int[]) ParseInput(string[] inputs)
            {
                var folds = new List<int>();
                var paper = new Paper();

                foreach (var line in inputs)
                {
                    var parts = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                        paper.Mark(int.Parse(parts[0]), int.Parse(parts[1]));
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        Debug.Assert(line.StartsWith("fold along "));
                        var foldParts = line.Substring("fold along ".Length).Split("=", StringSplitOptions.RemoveEmptyEntries);
                        var fold = int.Parse(foldParts[1]) * (foldParts[0] == "x" ? 1 : -1);

                        folds.Add(fold);
                    }
                }
                
                return (paper, folds.ToArray());
            }
        }

        private static string[] ReadAllLines(string fileName)
        {
            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "../../../Input/", fileName);
            return File.ReadAllLines(filePath);
        }

        private static string[] ReadEmptyLineSeparated(string fileName)
        {
            var result = new List<string>();
            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "../../../Input/", fileName);

            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var current = "";
                    while (!string.IsNullOrWhiteSpace(line))
                    {
                        current += " " + line;
                        line = reader.ReadLine();
                    }
                    result.Add(current);
                }
            }

            return result.ToArray();
        }

        record Point(int x, int y);

        record Line(Point b, Point e);

        internal class Graph
        {
            private readonly Dictionary<string, List<string>> _graph;

            public string Start { get; }
            public string End { get; }

            public Graph(string start, string end)
            {
                _graph = new Dictionary<string, List<string>>();
                Start = start;
                End = end;
            }

            public void AddEdge(string vertex1, string vertex2)
            {
                AddEdgeDirectional(vertex1, vertex2);
                AddEdgeDirectional(vertex2, vertex1);
            }

            public IEnumerable<string> Adjacent(string vertex)
            {
                return _graph[vertex];
            }

            public IEnumerable<string> Vertexes => _graph.Keys;

            private void AddEdgeDirectional(string vertex1, string vertex2)
            {
                if (!_graph.TryGetValue(vertex1, out var list))
                {
                    list = new List<string>();
                    _graph[vertex1] = list;
                }

                list.Add(vertex2);
            }

            public void RemoveVertex(string vertex)
            {
                foreach (var adjVertex in Adjacent(vertex))
                    _graph[adjVertex].Remove(vertex);
                _graph.Remove(vertex);
            }
        }

        internal class Paper
        {
            private readonly HashSet<Point> _points = new();
            
            public void Mark(int x, int y)
            {
                _points.Add(new Point(x, y));
            }

            public int MarkedCount()
            {
                return _points.Count;
            }

            public void FoldX(int x)
            {
                foreach (var point in _points.ToList())
                {
                    if (point.x == x)
                        _points.Remove(point);

                    if (point.x > x)
                    {
                        var newPoint = point with { x = x - (point.x - x) };
                        _points.Remove(point);
                        _points.Add(newPoint);
                    }
                }
            }

            public void FoldY(int y)
            {
                foreach (var point in _points.ToList())
                {
                    if (point.y == y)
                        _points.Remove(point);

                    if (point.y > y)
                    {
                        var newPoint = point with {y = y - (point.y - y)};
                        _points.Remove(point);
                        _points.Add(newPoint);
                    }
                }
            }

            public void Print()
            {
                var maxX = _points.Select(p => p.x).Max();
                var maxY = _points.Select(p => p.y).Max();

                for (int y = 0; y <= maxY; y++)
                {
                    var chars = Enumerable.Range(0, maxX + 1)
                        .Select(x => new Point(x, y))
                        .Select(p => _points.Contains(p) ? '#' : '.')
                        .ToArray();
                    
                    Console.WriteLine(chars, 0, chars.Length);
                }
            }
        }
    }
}