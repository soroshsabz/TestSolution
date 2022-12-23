using SharpRaven;
using System;
using System.Configuration;
using NLog;
using SharpRaven.Data;

namespace BSN.Resa.Commons.General
{
    public class SentryErrorTracker
    {
        private static readonly string SentryClientKey = ConfigurationManager.AppSettings["SentryClientKey"];
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static void CaptureException(Exception exceptionWrapper, bool? debug = null)
        {
            if (debug ?? Debug)
                return;

            if (string.IsNullOrWhiteSpace(SentryClientKey))
                return;

            Exception exception = exceptionWrapper;

            if (exceptionWrapper.InnerException != null)
                exception = exceptionWrapper.InnerException;

            new RavenClient(SentryClientKey).Capture(new SentryEvent(exception));
        }

        public static void CaptureSentryMessage(SentryMessage message, bool? debug = null)
        {
            try
            {
                if (debug ?? Debug)
                    return;

                if (string.IsNullOrWhiteSpace(SentryClientKey))
                    return;

                new RavenClient(SentryClientKey).Capture(new SentryEvent(message));
            }
            catch (Exception)
			{
                Logger.Info(message);
            }
        }


#if DEBUG
        private const bool Debug = true;
#elif !DEBUG
        private const bool Debug = false;
#endif
    }
}
