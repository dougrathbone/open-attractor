using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace OpenAttractor
{
    public class AppSettings
    {
        public static double ClearScreenTimerInterval
        {
            get
            {
                return ConfigurationManager.AppSettings["NoTouchClearScreenTimer"] != null
                           ? double.Parse(ConfigurationManager.AppSettings["NoTouchClearScreenTimer"])
                           : 300000;
            }
        }

        public static bool DebugEnabled
        {
            get
            {
                return ConfigurationManager.AppSettings["EnableDebug"] != null
                           ? bool.Parse(ConfigurationManager.AppSettings["EnableDebug"])
                           : false;
            }
        }
    }
}
