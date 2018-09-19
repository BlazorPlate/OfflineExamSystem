using System.Collections.Generic;
using System.Web;

namespace OfflineExamSystem.Helpers
{
    public static class CacheHelper
    {
        #region Public Methods
        public static void ClearCache()
        {
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            Dictionary<string, object> cacheItems = new Dictionary<string, object>();
            while (enumerator.MoveNext())
                cacheItems.Add(enumerator.Key.ToString(), enumerator.Value);
            foreach (string key in cacheItems.Keys)
                HttpRuntime.Cache.Remove(key);
        }
        #endregion Public Methods
    }
}