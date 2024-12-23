using System.IO;


namespace EXAM.Scripts
{
    public class ReportGenerator
    {
        public static void GenerateReport(string reportContent, string filePath)
        {
            File.WriteAllText(filePath, reportContent);
        }
    }
}
