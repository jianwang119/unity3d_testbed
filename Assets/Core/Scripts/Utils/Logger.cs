using UnityEngine;

namespace Core
{
    public class Logger
    {
        // TODO use log level for needs
        public static bool EnableLog = true;

        public static void Log(object message)
        {
            if (EnableLog)
            {
                Debug.Log(message);
            }
        }

        public static void Log(object message, Object context)
        {
            if (EnableLog)
            {
                Debug.Log(message, context);
            }
        }

        public static void LogFormat(string format, params object[] args)
        {
            if (EnableLog)
            {
                Debug.LogFormat(format, args);
            }
        }

        public static void LogFormat(Object context, string format, params object[] args)
        {
            if (EnableLog)
            {
                Debug.LogFormat(context, format, args);
            }
        }

        public static void LogWarning(object message)
        {
            if (EnableLog)
            {
                Debug.LogWarning(message);
            }
        }

        public static void LogWarning(object message, Object context)
        {
            if (EnableLog)
            {
                Debug.LogWarning(message, context);
            }
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            if (EnableLog)
            {
                Debug.LogWarningFormat(format, args);
            }
        }

        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            if (EnableLog)
            {
                Debug.LogWarningFormat(context, format, args);
            }
        }

        public static void LogError(object message)
        {
            if (EnableLog)
            {
                Debug.LogError(message);
            }
        }

        public static void LogError(object message, Object context)
        {
            if (EnableLog)
            {
                Debug.LogError(message, context);
            }
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            if (EnableLog)
            {
                Debug.LogErrorFormat(format, args);
            }
        }

        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            if (EnableLog)
            {
                Debug.LogErrorFormat(context, format, args);
            }
        }

        public static void LogAssertion(object message)
        {
            if (EnableLog)
            {
                Debug.LogAssertion(message);
            }
        }

        public static void LogAssertion(object message, Object context)
        {
            if (EnableLog)
            {
                Debug.LogAssertion(message, context);
            }
        }

        public static void LogAssertionFormat(string format, params object[] args)
        {
            if (EnableLog)
            {
                Debug.LogAssertionFormat(format, args);
            }
        }

        public static void LogAssertionFormat(Object context, string format, params object[] args)
        {
            if (EnableLog)
            {
                Debug.LogAssertionFormat(context, format, args);
            }
        }

        public static void LogException(System.Exception exception)
        {
            if (EnableLog)
            {
                Debug.LogException(exception, null);
            }
        }

        public static void LogException(System.Exception exception, Object context)
        {
            if (EnableLog)
            {
                Debug.LogException(exception, context);
            }
        }
    }
}

