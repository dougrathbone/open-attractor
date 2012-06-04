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
        public static double ThrobTimerInterval
        {
            get
            {
                return ConfigurationManager.AppSettings["NoTouchThrobTimer"] != null
                           ? double.Parse(ConfigurationManager.AppSettings["NoTouchThrobTimer"])
                           : 15000;
            }
        }

        public static bool DebugEnabled
        {
            get
            {
                return ConfigurationManager.AppSettings["EnableDebug"] != null && bool.Parse(ConfigurationManager.AppSettings["EnableDebug"]);
            }
        }

        public static string BackgroundPath
        {
            get
            {
                var settingValue = ConfigurationManager.AppSettings["BackgroundImagePath"];

                return settingValue != null && !String.IsNullOrWhiteSpace(settingValue)
                    ? settingValue
                    : "/OpenAttractor;component/Resources/WoodBackground.png";
            }
        }

        public static int MaximumAssetWidth
        {
            get
            {
                var settingValue = ConfigurationManager.AppSettings["MaximumAssetWidth"];
                int temp;

                return settingValue != null && int.TryParse(settingValue, out temp)
                    ? temp
                    : 1920;
            }
        }
    }
}
