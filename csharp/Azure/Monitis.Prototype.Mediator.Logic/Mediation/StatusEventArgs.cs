using System;

namespace Monitis.Prototype.Logic.Mediation
{
    /// <summary>
    /// Class represents event args for <see cref="Mediator"/> status change
    /// </summary>
    public class StatusEventArgs : EventArgs
    {
        public String Status { get; set; }
    }
}