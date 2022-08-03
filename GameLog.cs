using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRegistWinForm
{
    /// <summary>
    /// a. 必须调用 RegisterLog() 才会写入文件
    /// b. 可按需要 调用DayResetLogFile()来重置log文件
    /// </summary>
    class GameLog
    {
        // 是否已初始化
        private static bool _is_inited = false;
        // 日志文件路径
        private static string _log_file_path = "";

        // 日志文件缓存大小
        private static int _cache_max_size = 2048;
        private static byte[] _cache_pool = null;
        private static int _cache_pool_size = 0;

        // 是否将日志写入文件
        public const bool m_IsEnableLogFile = true;

        // 日志前缀，只用于写到文件时
        private static Dictionary<LogType, string> _logPrefix = new Dictionary<LogType, string>();
        // 上一次写入到文件的时间戳 （毫秒）
        private static double _last_flush_ts = -1;
        // 日志文件刷新最小时间间隔 单位秒
        private const float logFileFlushInterval = 0.5f;
        // 日志文件刷新最小缓存大小比率
        private const float logFileFlushRate = 0.4f;

        /// <summary>
        /// Log 的类别
        /// </summary>
        public enum LogType
        {
            Error,
            Log,
            Exception,
        }

        static GameLog()
        {
            _is_inited = false;
            _cache_pool_size = 0;
            _cache_pool = new byte[_cache_max_size];

            _logPrefix.Add(LogType.Error, "[Err]");
            _logPrefix.Add(LogType.Log, "[Log]");
            _logPrefix.Add(LogType.Exception, "[Excp]");
        }

        #region 初始化
        /// <summary>
        /// 注册log文件日志
        /// </summary>
        /// <param name="robotIdx">机器人批次</param>
        public static void RegisterLog()
        {
            _cache_pool_size = 0;

            if (!Directory.Exists("log"))
            {
                Directory.CreateDirectory("log");
            }

            _log_file_path = string.Format("./log/format_{1}.log", AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMdd"));
            CheckLogFile(_log_file_path);

            _is_inited = true;

            // 写入日志头
            GameLog.Log("-----------------日志注册成功---------------");
        }

        /// <summary>
        /// 跨天 就重新创建新的日志文件
        /// </summary>
        /// <param name="robotIdx"></param>
        public static void DayResetLogFile()
        {
            if (!Directory.Exists("log"))
            {
                Directory.CreateDirectory("log");
            }
            _log_file_path = string.Format("log/format_{1}.log", AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMdd"));
        }
        #endregion 

        #region 写入文件
        private static void CheckLogFile(string path)
        {
            //每次重新创建
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static void CacheLog(string msg)
        {
            byte[] bs = Encoding.UTF8.GetBytes(msg);

            // 如果缓存空间不足，立即刷新
            if (_cache_pool_size + bs.Length >= _cache_max_size)
            {
                FlushLog();
            }

            // 写入缓存空间
            if (_cache_pool_size + bs.Length < _cache_max_size)
            {
                Buffer.BlockCopy(bs, 0, _cache_pool, _cache_pool_size, bs.Length);
                _cache_pool_size += bs.Length;
            }
            else
            {
                Console.WriteLine("CacheLog cache size not enough.cache_size:" + _cache_max_size);
            }
        }

        // 刷新数据到文件
        public static void FlushLog()
        {
            if (!_is_inited)
            {
                return;
            }

            if (_cache_pool == null || _cache_pool_size <= 0)
            {
                return;
            }

            try
            {
                bool isFlush = false;
                // 超过最小时间间隔时刷新
                float interval = (float)(GetTimeStamp() - _last_flush_ts);
                if (interval >= logFileFlushInterval * 1000)
                {
                    isFlush = true;
                }
                // 缓存空间中字节数超过一定比率时刷新
                if (_cache_pool_size >= (_cache_max_size * logFileFlushRate))
                {
                    isFlush = true;
                }

                if (isFlush)
                {
                    using (FileStream fs = new FileStream(_log_file_path, FileMode.Append, FileAccess.Write))
                    {
                        fs.Write(_cache_pool, 0, _cache_pool_size);
                        fs.Flush();
                        _cache_pool_size = 0;
                        _last_flush_ts = GetTimeStamp();
                    }
                }
            }
            catch (Exception ex)
            {
                _cache_pool_size = 0;
                Console.WriteLine("LogModule FlushLog error.");
                Console.WriteLine(ex);
            }
        }
        #endregion

        #region log方法

        public static void Log(string message)
        {
            // 写入文件
            if (m_IsEnableLogFile)
            {
                CacheLog(FormatFileLog(LogType.Log, message, null));
            }
        }

        public static void LogFormat(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        public static void LogError(string message)
        {
            // 写入文件
            if (m_IsEnableLogFile)
            {
                CacheLog(FormatFileLog(LogType.Error, message, null));
            }
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            LogError(string.Format(format, args));
        }

        public static void LogException(System.Exception e)
        {
            // 写入文件
            if (m_IsEnableLogFile)
            {
                CacheLog(FormatFileLog(LogType.Error, e.Message, e.StackTrace));
            }
        }


        private static string FormatFileLog(LogType type, string msg, string stackTrace)
        {
            string prefix = "[Log]";
            if (_logPrefix.ContainsKey(type))
            {
                prefix = _logPrefix[type];
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]"));
            sb.Append(prefix);
            if (type == LogType.Log)
            {
                sb.AppendLine(msg);
            }
            else
            {
                sb.AppendLine(msg);
                sb.AppendLine(stackTrace);
            }
            return sb.ToString();
        }
        #endregion

        public static double GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalMilliseconds;
        }
    }
}
