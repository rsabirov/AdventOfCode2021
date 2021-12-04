using System;
using System.Collections.Generic;
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

            return gamma* epsilon;
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
    }
}