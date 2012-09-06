using System.Collections.Generic;

namespace AgentCore
{
    public interface IInfoAnalizer
    {
        void Analize(IEnumerable<ClrMethodOperation> operations, int timeSinceLastAnalizingInMs);
    }
}