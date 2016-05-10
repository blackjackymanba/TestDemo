using System;
using System.IO;
using NLog;
using NLog.Config;
using System.Threading;


namespace GPMGateway.Common
{
    public class Nlog
    {
        static public Logger CurClasslogger = LogManager.GetCurrentClassLogger();
        public Logger getLogger { get; set; }
        public int curThreadId = Thread.CurrentThread.ManagedThreadId;   //获取当前线程ID
        public Nlog(string className)
        {
            var context = new InstallationContext();
            context.LogOutput = Console.Out;
            context.LogLevel = LogLevel.FromString("Debug");
            getLogger = LogManager.GetLogger(className + " | ThreadId:" + curThreadId);
        }
        /// <summary>
        /// 追踪日志 LV.1
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void Trace(string msg, params object[] args)
        {
            msg = replaceRN(msg);              
            getLogger.Debug(msg, args);
        }

        /// <summary>
        /// 调试日志 LV.2
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void Debug(string msg, params object[] args)
        {
            msg = replaceRN(msg);
            getLogger.Debug(msg, args);
        }

        /// <summary>
        /// 信息日志 LV.3
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void Info(string msg, params object[] args)
        {
            msg = replaceRN(msg);
            getLogger.Info(msg, args);
        }

        /// <summary>
        /// 警告日志 LV.4
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void Warn(string msg, params object[] args)
        {
            msg = replaceRN(msg);
            getLogger.Warn(msg, args);
        }

        /// <summary>
        /// 错误日志 LV.5
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void Error(string msg, params object[] args)
        {
            msg = replaceRN(msg);
            getLogger.Error(msg, args);
        }

        /// <summary>
        /// 致命错误日志 LV.6
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void Fatal(string msg, params object[] args)
        {
            msg = replaceRN(msg);
            getLogger.Fatal(msg, args);
        }


        public static string replaceRN(string msg)
        {
            msg = msg.Replace("\n", " ").Replace("\r", " ");
            return msg;
        }


    }
}
