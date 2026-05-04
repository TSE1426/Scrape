using System;

namespace Scrape
{
    internal class RunProgram
    {
        public static string Start(CodeAreaManager codeAreaManager)
        {
            if (codeAreaManager == null)
            {
                throw new ArgumentNullException(nameof(codeAreaManager));
            }

            return codeAreaManager.BuildProgramText();
        }
    }
}
