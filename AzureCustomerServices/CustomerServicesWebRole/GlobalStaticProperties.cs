using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace CustomerServicesWebRole
{
	internal static class GlobalStaticProperties
	{

		private static string _dbConnectionString;
		/// <summary>
		/// connection string to the database; only retrieve it the first time
		/// </summary>
		internal static string dbConnectionString
		{
			get
			{
				if (string.IsNullOrEmpty(_dbConnectionString))
				{
					_dbConnectionString = RoleEnvironment.GetConfigurationSettingValue("dbConnString");
					Trace.TraceInformation("[CustomerServicesWebRole.GlobalStaticProperties] " +
						" Setting dbConnectionString to {0}", _dbConnectionString);
				}
				return _dbConnectionString;
			}
		}

		private static int _MaxTryCount;
		/// <summary>
		/// max number of times to try reading the SQL database before giving up
		/// </summary>
		internal static int MaxTryCount
		{
			get
			{
				if (_MaxTryCount <= 0)
				{
					//hasn't been loaded yet, so load it 
					string maxTryCount = RoleEnvironment.GetConfigurationSettingValue("MaxTryCount");
					int intTest = 0;
					bool success = int.TryParse(maxTryCount, out intTest);
					//if it's <= 0, set it to 1.
					if (!success || intTest <= 0)
						_MaxTryCount = 1;
					else
						_MaxTryCount = intTest;
					Trace.TraceInformation("[CustomerServicesWebRole.GlobalStaticProperties] "
					  + "Setting MaxTryCount to {0}", MaxTryCount);
				}
				return _MaxTryCount;
			}
		}

		private static List<int> _retrySleepTime;
		/// <summary>
		/// amount of time to wait between retries when reading the SQL database
		/// This loads a list, which is then referenced in code. 
		/// This means my intervals are the same, just multiplied by the index.
		/// First retry waits 0 seconds, second waits 2.5, third waits 5, and last is irrelevant.
		/// (It stops if it retries 4 times.)
		/// </summary>
		internal static List<int> retrySleepTime
		{
			get
			{
				if (_retrySleepTime == null || _retrySleepTime.Count <= 0)
				{
					//hasn't been loaded yet, so load it 
					string interval = RoleEnvironment.GetConfigurationSettingValue("RetrySleepInterval");
					int intTest = 0;
					int intInterval = 0;
					bool success = int.TryParse(interval, out intTest);
					if (intTest <= 0)
						intInterval = 2500; //2.5 seconds
					else
						intInterval = intTest;
					Trace.TraceInformation("[CustomerServicesWebRole.GlobalStaticProperties] "
					  + "Setting Sleep Interval to {0}", intInterval);

					//put these in an array so they are completely dynamic rather than having
					//  variables for each one. You can change the interval and number of times
					//  to retry simply by changing the configuration settings.
					_retrySleepTime = new List<int>();
					//set the sleep times 0, 5, 10, etc.
					intTest = 0;
					_retrySleepTime.Add(0);
					for (int i = 1; i < MaxTryCount; i++)
					{
						intTest += intInterval;
						_retrySleepTime.Add(intTest);
					}

					for (int i = 0; i < MaxTryCount; i++)
					{
						Trace.TraceInformation("[CustomerServicesWebRole.GlobalStaticProperties] "
						  + "Setting retrySleepTime({0}) to {1}", i, _retrySleepTime[i]);
					}
				}
				return _retrySleepTime;
			}
		}

		private static string _ProcessQueueName;
		/// <summary>
		/// name of the queue. You should never hard code this. If you do, you will have to 
		/// re-publish your application in order to change it.
		/// </summary>
		internal static string ProcessQueueName
		{
			get
			{
				if (string.IsNullOrEmpty(_ProcessQueueName))
				{
					_ProcessQueueName = RoleEnvironment.GetConfigurationSettingValue("ProcessQueueName");
					Trace.TraceInformation("[CustomerServicesWebRole.GlobalStaticProperties] "
					  + "Setting ProcessQueueName to {0}", _ProcessQueueName);
				}
				return _ProcessQueueName;
			}
		}
	}
}