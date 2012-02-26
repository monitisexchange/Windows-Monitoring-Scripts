using System;
using System.Collections.Generic;

namespace Monitis.Prototype.Logic.Common
{
    /// <summary>
    /// Represents simple result for action
    /// </summary>
    public class ActionResult
    {
        public ActionResult()
        {
        }

        /// <summary>
        /// Init instance with <see cref="IsSuccessful"/> property
        /// </summary>
        /// <param name="isSuccessful"></param>
        public ActionResult(Boolean isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }

        /// <summary>
        /// True - action completes successful otherwise False
        /// </summary>
        public Boolean IsSuccessful { get; set; }

        /// <summary>
        /// Contains error list, if any.
        /// </summary>
        public IEnumerable<String> Errors
        {
            get { return _errors; }
        }

        public void AddError(String errorMessage)
        {
            _errors.Add(errorMessage);
        }

        private readonly List<String> _errors = new List<String>();
    }
}