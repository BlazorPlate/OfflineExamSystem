using Owin;

namespace OfflineExamSystem
{
    public partial class Startup
    {
        #region Public Methods
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
        #endregion Public Methods
    }
}