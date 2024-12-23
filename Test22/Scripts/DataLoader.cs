using System;
using System.IO;

namespace EXAM.Scripts
{
    public class DataLoader
    {
        public static (DateTime startTime, int[] temperatures) LoadDataFromFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            DateTime startTime = DateTime.Parse(lines[0]);
            int[] temperatures = Array.ConvertAll(lines[1].Split(','), int.Parse);
            return (startTime, temperatures);
        }
    }
}