using System;
using NLog;

namespace ETModel
{
    public class NLogAdapter : ILog
    {
        private readonly Logger logger = LogManager.GetLogger("Logger");

        private bool OpenSystemLogToConsole = false;

        public void Trace(string message)
        {
            this.logger.Trace(message);
            Console.WriteLine("发现：" + message);
        }

        public void Warning(string message)
        {
            this.logger.Warn(message);
            Console.WriteLine("警告：" + message);
        }

        public void Info(string message)
        {
            this.logger.Info(message);
            Console.WriteLine("信息：" + message);
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
            if (OpenSystemLogToConsole)
            {
                Console.WriteLine("日志：" + message);
            }
        }

        public void Error(string message)
        {
            this.logger.Error(message);
            Console.WriteLine("错误：" + message);
        }

        public void Fatal(string message)
        {
            this.logger.Fatal(message);
            Console.WriteLine("致命错误：" + message);
        }

        public void Trace(string message, params object[] args)
        {
            this.logger.Trace(message, args);
            Console.WriteLine("发现：" + message, args);
        }

        public void Warning(string message, params object[] args)
        {
            this.logger.Warn(message, args);
            Console.WriteLine("警告：" + message, args);
        }

        public void Info(string message, params object[] args)
        {
            this.logger.Info(message, args);
            Console.WriteLine("信息：" + message, args);
        }

        public void Debug(string message, params object[] args)
        {
            this.logger.Debug(message, args);
            if (OpenSystemLogToConsole)
            {
                Console.WriteLine("日志：" + message, args);
            }
        }

        public void Error(string message, params object[] args)
        {
            this.logger.Error(message, args);
            Console.WriteLine("错误：" + message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            this.logger.Fatal(message, args);
            Console.WriteLine("致命错误：" + message, args);
        }
    }
}