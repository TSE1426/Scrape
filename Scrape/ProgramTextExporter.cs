using System.IO;

namespace Scrape
{
    public static class ProgramTextExporter
    {
        public static string ExportToFile(CodeAreaManager codeAreaManager, string filePath)
        {
            string programText = codeAreaManager.BuildProgramText();
            File.WriteAllText(filePath, programText);
            return programText;
        }
    }
}
