using System;
using System.Collections.Generic;
using System.Linq;

namespace OfflineExamSystem.Helpers
{
    public interface IreturnUrlHandler
    {
        #region Public Methods
        void Register(string returnUrlPrefix, string loginRoute);

        string GetRoute(string returnUrl);
        #endregion Public Methods
    }

    public class returnUrlHandler : IreturnUrlHandler
    {
        #region Private Fields
        private const string DefaultRoute = "Home";
        private readonly Dictionary<string, string> redirectRules = new Dictionary<string, string>();
        #endregion Private Fields

        #region Public Methods
        public void Register(string returnUrlPrefix, string loginRoute)
        {
            redirectRules.Add(returnUrlPrefix, loginRoute);
        }

        public string GetRoute(string returnUrl)
        {
            var key = redirectRules.Keys.FirstOrDefault(x =>
                returnUrl.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            return !string.IsNullOrEmpty(key) ? redirectRules[key] : DefaultRoute;
        }
        #endregion Public Methods
    }
}