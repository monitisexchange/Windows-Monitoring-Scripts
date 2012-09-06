using System;
using System.Collections.Generic;

namespace AgentCore
{
    public class ConsoleTestInfoAnalizer : IInfoAnalizer
    {
        public void Analize(IEnumerable<ClrMethodOperation> operations, int timeSinceLastAnalizingInMs)
        {
            foreach (var clrMethodOperation in operations)
            {
                Console.WriteLine(clrMethodOperation);
            }
        }
    }
}