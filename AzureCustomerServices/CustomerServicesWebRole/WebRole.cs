using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.Storage;

namespace CustomerServicesWebRole
{
	public class WebRole : RoleEntryPoint
	{
		public override bool OnStart()
		{
			// Get a reference to the initial default configuration.
			string wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

			// First, get a reference to the storage account where the diagnostics will be written. 
			// It is recommended that you use a separate account for diagnostics and data, so the 
			//   performance of your data access is not impacted by the diagnostics.
			CloudStorageAccount storageAccount =
				CloudStorageAccount.Parse(
				RoleEnvironment.GetConfigurationSettingValue(wadConnectionString));

			// Get an reference to the diagnostic manager for the role instance, 
			//   and then get the default initial configuration, which we will then change.

			RoleInstanceDiagnosticManager roleInstanceDiagnosticManager = CloudAccountDiagnosticMonitorExtensions.CreateRoleInstanceDiagnosticManager(
			RoleEnvironment.GetConfigurationSettingValue(wadConnectionString),
			RoleEnvironment.DeploymentId,
			RoleEnvironment.CurrentRoleInstance.Role.Name,
			RoleEnvironment.CurrentRoleInstance.Id);

			DiagnosticMonitorConfiguration config = DiagnosticMonitor.GetDefaultInitialConfiguration();

			// Change the polling interval for checking for configuration changes
			//   and the buffer quota for the logs. 
			config.ConfigurationChangePollInterval = TimeSpan.FromSeconds(30.0);
			config.DiagnosticInfrastructureLogs.BufferQuotaInMB = 256;

			// The diagnostics data is written locally and then transferred to Azure Storage. 
			// These are the transfer intervals for doing that operation.
			config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0); //for trace logs
			config.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0); //for iis logs

			// Configure the monitoring of one Windows performance counter
			// and add it to the configuration.
			int sampleRate = 0;
			int scheduledTransferPeriod = 0;
			bool success = false;
			//this is sample rate, in seconds, for the performance monitoring in %CPU.
			//By making this configurable, you can change the azure config rather than republish the role.
			success = int.TryParse(RoleEnvironment.GetConfigurationSettingValue("PerfMonSampleRate"),
			  out sampleRate);
			if (!success || sampleRate <= 0)
				sampleRate = 60;  //default is 60 seconds
			success =
			  int.TryParse(RoleEnvironment.GetConfigurationSettingValue("PerfMonScheduledTransferPeriod"),
			  out scheduledTransferPeriod);
			if (!success || scheduledTransferPeriod <= 0)
				scheduledTransferPeriod = 120;  //default is 120 seconds

			PerformanceCounterConfiguration perfConfig
				= new PerformanceCounterConfiguration();
			perfConfig.CounterSpecifier = @"\Processor(*)\% Processor Time";
			perfConfig.SampleRate = TimeSpan.FromSeconds((double)sampleRate);
			config.PerformanceCounters.DataSources.Add(perfConfig);
			config.PerformanceCounters.ScheduledTransferPeriod =
			  TimeSpan.FromSeconds((double)scheduledTransferPeriod);

			// Configure monitoring of Windows Application and System Event logs,
			// including the quota and scheduled transfer interval, and add them 
			// to the configuration.
			WindowsEventLogsBufferConfiguration eventsConfig
				= new WindowsEventLogsBufferConfiguration();
			eventsConfig.BufferQuotaInMB = 256;
			eventsConfig.ScheduledTransferLogLevelFilter = Microsoft.WindowsAzure.Diagnostics.LogLevel.Undefined; //was warning
			eventsConfig.ScheduledTransferPeriod = TimeSpan.FromMinutes(2.0); //was 10
			eventsConfig.DataSources.Add("Application!*");
			eventsConfig.DataSources.Add("System!*");
			config.WindowsEventLog = eventsConfig;

			//set the configuration to be used by the current role instance
			roleInstanceDiagnosticManager.SetCurrentConfiguration(config);

			//add an event handler for the configuration being changed while the role is running
			RoleEnvironment.Changing +=
			  new EventHandler<RoleEnvironmentChangingEventArgs>(RoleEnvironment_Changing);
			return base.OnStart();
		}

		/// <summary>
		/// If they change any of the configuration values while the role is running,
		/// recycle it. You can also have this check which setting got changed and 
		/// handle it rather than recycling the role. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void RoleEnvironment_Changing(object sender, RoleEnvironmentChangingEventArgs e)
		{

			// If a configuration setting is changing
			if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
			{
				// Set e.Cancel to true to restart this role instance
				e.Cancel = true;
			}
		}
	}
}
