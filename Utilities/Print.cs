using System;
using System.Diagnostics;
using System.Text;

public static class FastConcatUtility
{
    static readonly StringBuilder _stringBuilder = new StringBuilder(256);

    public static string FastConcat<T>(this string str1, T str2)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            _stringBuilder.Append(str1);
            _stringBuilder.Append(str2);

            return _stringBuilder.ToString();
        }
    }

    public static string FastConcat(this string str1, string str2, string str3)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            _stringBuilder.Append(str1);
            _stringBuilder.Append(str2);
            _stringBuilder.Append(str3);

            return _stringBuilder.ToString();
        }
    }

    public static string FastConcat(this string str1, string str2, string str3, string str4)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            _stringBuilder.Append(str1);
            _stringBuilder.Append(str2);
            _stringBuilder.Append(str3);
            _stringBuilder.Append(str4);


            return _stringBuilder.ToString();
        }
    }

    public static string FastConcat(this string str1, string str2, string str3, string str4, string str5)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            _stringBuilder.Append(str1);
            _stringBuilder.Append(str2);
            _stringBuilder.Append(str3);
            _stringBuilder.Append(str4);
            _stringBuilder.Append(str5);

            return _stringBuilder.ToString();
        }
    }

    public static string FastJoin(this string[] str)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            for (int i = 0; i < str.Length; i++)
                _stringBuilder.Append(str[i]);

            return _stringBuilder.ToString();
        }
    }

    public static string FastJoin(this string[] str, string str1)
    {
        lock (_stringBuilder)
        {
            _stringBuilder.Length = 0;

            for (int i = 0; i < str.Length; i++)
                _stringBuilder.Append(str[i]);

            _stringBuilder.Append(str1);

            return _stringBuilder.ToString();
        }
    }
}

namespace Utility
{
    public interface ILogger
    {
        void Log (string txt, string stack = null, LogType type = LogType.Log);
    }
#if UNITY_5 || UNITY_5_3_OR_NEWER
    public class SlowLoggerUnity : ILogger
    {
        public void Log(string txt, string stack = null, LogType type = LogType.Log)
        {
            switch (type)
            {
                case LogType.Log:
                    UnityEngine.Debug.Log(stack != null ? txt.FastConcat(stack) : txt);
                break;
                case LogType.Exception:
                    UnityEngine.Debug.LogError("Log of exceptions not supported");
                break;
                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(stack != null ? txt.FastConcat(stack) : txt);
                break;
                case LogType.Error:
                    UnityEngine.Debug.LogError(stack != null ? txt.FastConcat(stack) : txt);
                break;
            }
        }
    }
#endif
    public class SimpleLogger : ILogger
    {
        public void Log(string txt, string stack = null, LogType type = LogType.Log)
        {
            switch (type)
            {
                case LogType.Log:
                    Console.SystemLog(stack != null ? txt.FastConcat(stack) : txt);
                    break;
                case LogType.Exception:
                    Console.SystemLog("Log of exceptions not supported");
                    break;
                case LogType.Warning:
                    Console.SystemLog(stack != null ? txt.FastConcat(stack) : txt);
                    break;
                case LogType.Error:
                    Console.SystemLog(stack != null ? txt.FastConcat(stack) : txt);
                    break;
            }
        }
    }

    public enum LogType
    {
        Log,
        Exception,
        Warning,
        Error
    }

    public static class Console
    {
        static StringBuilder _stringBuilder = new StringBuilder(256);
#if UNITY_5 || UNITY_5_3_OR_NEWER
        public static ILogger logger = new SlowLoggerUnity();
#else
        public static ILogger logger = new SimpleLogger();
#endif
        public static volatile bool BatchLog = false;

        public static void Log(string txt)
        {
            logger.Log(txt);
        }

        public static void LogError(string txt)
        {
            string toPrint;
        
            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("-!!!!!!-> ");
                _stringBuilder.Append(txt);

                toPrint = _stringBuilder.ToString();
            }

            logger.Log(toPrint, null, LogType.Error);
        }

        public static void LogError(string txt, string stack)
        {
            string toPrint;

            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("-!!!!!!-> ");
                _stringBuilder.Append(txt);

                toPrint = _stringBuilder.ToString();
            }

            logger.Log(toPrint, stack, LogType.Error);
        }
        public static void LogException(Exception e)
        {
#if UNITY_5 || UNITY_5_3_OR_NEWER
            LogException(e, null);
#endif
        }
#if UNITY_5 || UNITY_5_3_OR_NEWER
        public static void LogException(Exception e, UnityEngine.Object obj)
        {
            UnityEngine.Debug.LogException(e, obj);
        }
#endif
        public static void LogWarning(string txt)
        {
            string toPrint;

            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("------> ");
                _stringBuilder.Append(txt);

                toPrint = _stringBuilder.ToString();
            }

            logger.Log(toPrint, null, LogType.Warning);
        }

        /// <summary>
        /// Use this function if you don't want the message to be batched
        /// </summary>
        /// <param name="txt"></param>
        public static void SystemLog(string txt)
        {
#if !NETFX_CORE
            string toPrint;

            lock (_stringBuilder)
            {
                string currentTimeString = DateTime.UtcNow.ToLongTimeString(); //ensure includes seconds
                string processTimeString = (DateTime.UtcNow - Process.GetCurrentProcess().StartTime).ToString();

                _stringBuilder.Length = 0;
                _stringBuilder.Append("[").Append(currentTimeString);
                _stringBuilder.Append("][").Append(processTimeString);
                _stringBuilder.Length = _stringBuilder.Length - 3; //remove some precision that we don't need
                _stringBuilder.Append("] ").AppendLine(txt);

                toPrint = _stringBuilder.ToString();
            }

#if !UNITY_EDITOR
            System.Console.WriteLine(toPrint);
#else
            UnityEngine.Debug.Log(toPrint);
#endif
#endif
        }
    }
}