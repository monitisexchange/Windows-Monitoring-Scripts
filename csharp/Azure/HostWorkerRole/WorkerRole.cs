using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace HostWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("HostWorkerRole entry point called", "Information");
            SetupCounters();
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                EmulateProcessorActivity();
                EmulateMemoryActivity();
                Trace.WriteLine("Working", "Information");
            }
        }

        /// <summary>
        /// Simple random algo for emulate memory usages
        /// </summary>
        private static void EmulateMemoryActivity()
        {
            Random random = new Random();
            int randomValue = random.Next(1, 2684356);
            List<Int32> buffer = new List<int>();
            for (int i = 0; i < randomValue; i++)
            {
                buffer.Add(i);
            }
            GC.Collect();
        }

        /// <summary>
        /// Simple random algo for emulate memory usages
        /// </summary>
        private static void EmulateProcessorActivity()
        {
            Random random = new Random();
            int randomValue = random.Next(1, Int32.MaxValue);
            for (int i = 0; i < randomValue; i++)
            {
            }
        }

        /// <summary>
        /// Setup counters for diagnostic monitor
        /// </summary>
        private void SetupCounters()
        {
            DiagnosticMonitorConfiguration diagnosticMonitorConfiguration = DiagnosticMonitor.GetDefaultInitialConfiguration();
            TraceConfig(diagnosticMonitorConfiguration);

            diagnosticMonitorConfiguration.PerformanceCounters.BufferQuotaInMB = 5;

            diagnosticMonitorConfiguration.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);

            // Use 30 seconds for the perf counter sample rate.
            TimeSpan perfSampleRate = TimeSpan.FromSeconds(30D);

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Memory\Available Bytes",
                SampleRate = perfSampleRate
            });

            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                CounterSpecifier = @"\Processor(_Total)\% Processor Time",
                SampleRate = perfSampleRate
            });

            // Apply the updated configuration to the diagnostic monitor.
            // The first parameter is for the connection string configuration setting.
            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagnosticMonitorConfiguration);
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        public void TraceConfig(DiagnosticMonitorConfiguration configuration)
        {
            if (configuration == null)
            {
                Trace.WriteLine("configuration is null");
                return;
            }
            try
            {
                // Display the general settings of the configuration
                Trace.WriteLine("*** General configuration settings ***");
                Trace.WriteLine("Config change poll interval: " + configuration.ConfigurationChangePollInterval);
                Trace.WriteLine("Overall quota in MB: " + configuration.OverallQuotaInMB);

                // Display the diagnostic infrastructure logs
                Trace.WriteLine("*** Diagnostic infrastructure settings ***");
                Trace.WriteLine("DiagnosticInfrastructureLogs buffer quota in MB: " + configuration.DiagnosticInfrastructureLogs.BufferQuotaInMB);
                Trace.WriteLine("DiagnosticInfrastructureLogs scheduled transfer log filter: " + configuration.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter);
                Trace.WriteLine("DiagnosticInfrastructureLogs transfer period: " + configuration.DiagnosticInfrastructureLogs.ScheduledTransferPeriod);

                // List the Logs info
                Trace.WriteLine("*** Logs configuration settings ***");
                Trace.WriteLine("Logs buffer quota in MB: " + configuration.Logs.BufferQuotaInMB);
                Trace.WriteLine("Logs scheduled transfer log level filter: " + configuration.Logs.ScheduledTransferLogLevelFilter);
                Trace.WriteLine("Logs transfer period: " + configuration.Logs.ScheduledTransferPeriod);

                // List the Directories info
                Trace.WriteLine("*** Directories configuration settings ***");
                Trace.WriteLine("Directories buffer quota in MB: " + configuration.Directories.BufferQuotaInMB);
                Trace.WriteLine("Directories scheduled transfer period: " + configuration.Directories.ScheduledTransferPeriod);
                int count = configuration.Directories.DataSources.Count, index;
                if (0 == count)
                {
                    Trace.WriteLine("No data sources for Directories");
                }
                else
                {
                    for (index = 0; index < count; index++)
                    {
                        Trace.WriteLine("Directories configuration data source:");
                        Trace.WriteLine("\tContainer: " + configuration.Directories.DataSources[index].Container);
                        Trace.WriteLine("\tDirectory quota in MB: " + configuration.Directories.DataSources[index].DirectoryQuotaInMB);
                        Trace.WriteLine("\tPath: " + configuration.Directories.DataSources[index].Path);
                        Trace.WriteLine("");
                    }
                }

                // List the event log info
                Trace.WriteLine("*** Event log configuration settings ***");
                Trace.WriteLine("Event log buffer quota in MB: " + configuration.WindowsEventLog.BufferQuotaInMB);
                count = configuration.WindowsEventLog.DataSources.Count;
                if (0 == count)
                {
                    Trace.WriteLine("No data sources for event log");
                }
                else
                {
                    for (index = 0; index < count; index++)
                    {
                        Trace.WriteLine("Event log configuration data source:" + configuration.WindowsEventLog.DataSources[index]);
                    }
                }
                Trace.WriteLine("Event log scheduled transfer log level filter: " + configuration.WindowsEventLog.ScheduledTransferLogLevelFilter);
                Trace.WriteLine("Event log scheduled transfer period: " + configuration.WindowsEventLog.ScheduledTransferPeriod.ToString());

                // List the performance counter info
                Trace.WriteLine("*** Performance counter configuration settings ***");
                Trace.WriteLine("Performance counter buffer quota in MB: " + configuration.PerformanceCounters.BufferQuotaInMB);
                Trace.WriteLine("Performance counter scheduled transfer period: " + configuration.PerformanceCounters.ScheduledTransferPeriod.ToString());
                count = configuration.PerformanceCounters.DataSources.Count;
                if (0 == count)
                {
                    Trace.WriteLine("No data sources for PerformanceCounters");
                }
                else
                {
                    for (index = 0; index < count; index++)
                    {
                        Trace.WriteLine("PerformanceCounters configuration data source:");
                        Trace.WriteLine("\tCounterSpecifier: " + configuration.PerformanceCounters.DataSources[index].CounterSpecifier);
                        Trace.WriteLine("\tSampleRate: " + configuration.PerformanceCounters.DataSources[index].SampleRate.ToString());
                        Trace.WriteLine("");
                    }
                }
            }

            catch (Exception e)
            {
                Trace.WriteLine("Exception during ShowConfig: " + e.ToString());
                // Take other action as needed.
            }
        }
    }
}