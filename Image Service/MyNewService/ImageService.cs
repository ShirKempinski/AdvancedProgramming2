using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Collections.Generic;

namespace ImageService
{
    public partial class ImageService : ServiceBase
    {
        private ILogging logger;
        private IController controller;
        private Server server;
        private int eventId;

        public ImageService(string[] args)
        {
            InitializeComponent();
            string eventSourceName = ConfigurationManager.AppSettings["SourceName"];
            string logName = ConfigurationManager.AppSettings["LogName"];
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }
            eventLogger = new EventLog();
            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLogger.Source = eventSourceName;
            eventLogger.Log = logName;
        }

        private void OnMsg(Object sender, MessageReceivedEventArgs msg)
        {
            eventLogger.WriteEntry("Status: " + msg.Status + " Message: " + msg.Message);
        }

        private void GetEntries(Object sender, LogEntriesEventArgs args)
        {
            // Obtain the Log Entries of the Event Log
            EventLogEntryCollection myEventLogEntryCollection = eventLogger.Entries;
            // Display the 'Message' property of EventLogEntry.
            for (int i = 0; i < myEventLogEntryCollection.Count; i++)
            {
                args.Args.Add(myEventLogEntryCollection[i].Message);
            }
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
            eventLogger.WriteEntry("Status: INFO Message: In OnStart");
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            //Reading from appconfig and creating necessary objects
            logger = new LoggingModal();
            int thumbnailSize = int.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            controller = new ImageController(new ImageModal(thumbnailSize, outputDir));
            logger.MessageReceived += OnMsg;
            logger.GetEntries += GetEntries;
            server = new Server(logger, controller);
            server.Start();
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.  
            eventLogger.WriteEntry("Status: INFO Message: Monitoring the System", EventLogEntryType.Information, eventId++);
        }
        protected override void OnStop() {
            eventLogger.WriteEntry("Status: INFO Message: In onStop.");
        }

        protected override void OnPause()
        {
            eventLogger.WriteEntry("Status: INFO Message: In onPause.");
        }

        protected override void OnShutdown()
        {
            eventLogger.WriteEntry("Status: INFO Message: In OnShutdown.");
            this.server.SendCommand(CommandEnum.CloseCommand, new List<String>(), "all");
        }

        protected override void OnContinue()
        {
            eventLogger.WriteEntry("Status: INFO Message: In OnContinue.");
        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}