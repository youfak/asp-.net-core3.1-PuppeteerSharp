using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharpTest
{
    /// <summary>
    /// 自定义日志记录
    /// </summary>
    public class NLogHelp
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static void ErrorLog(string throwMsg, Exception ex)
        {
            string errorMsg = string.Format("【异常信息】：{0} <br>【异常类型】：{1} <br>【堆栈调用】：{2}",
                new object[] { throwMsg, ex.GetType().Name, ex.StackTrace });
            errorMsg = errorMsg.Replace("\r\n", "<br>");
            errorMsg = errorMsg.Replace("位置", "<strong style=\"color:red\">位置</strong>");
            logger.Error(ex.Message + "||" + errorMsg.ToString(CultureInfo.InvariantCulture));
        }
        public static void InfoLog(string operateMsg)
        {
            string errorMsg = string.Format("【操作信息】：{0} <br>",
                new object[] { operateMsg });
            errorMsg = errorMsg.Replace("\r\n", "<br>");
            logger.Info(errorMsg.ToString(CultureInfo.InvariantCulture));
        }
    }

}
