using System.Collections.Generic;

namespace OfflineExamSystem.Helpers
{
    public class Select2PagedResult
    {
        #region Public Properties
        public int Total { get; set; }
        public List<Select2Result> Results { get; set; }
        #endregion Public Properties
    }

    public class Select2Result
    {
        #region Public Properties
        public string id { get; set; }
        public string text { get; set; }
        #endregion Public Properties
    }
}