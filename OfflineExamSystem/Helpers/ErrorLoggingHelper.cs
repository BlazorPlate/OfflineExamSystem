using System;
using System.IO;
using System.Web;

namespace OfflineExamSystem.Helpers
{
    public static class ErrorLoggingHelper
    {
        #region Private Fields
        private static String Errormsg;
        #endregion Private Fields

        #region Public Methods
        public static void SendErrorToText(string ex)
        {
            Errormsg = ex;

            try
            {
                string filepath = HttpContext.Current.Server.MapPath("~/ErrorLogging/");  //Text File Path

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + "Error Message:" + " " + Errormsg;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
        #endregion Public Methods
    }
}