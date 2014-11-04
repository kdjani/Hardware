using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Data.SqlClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Data;
using System.Threading;

namespace CustomerServicesWebRole
{
	internal class GpsDataGet
	{

		/// <summary>
		/// Given the first and last name, return the favorite movie and language.
		/// </summary>
		internal string GetGpsData(string userId, string deviceId, string time, out DataSet gpsData)
		{
			//write to diagnostics that this routine was called, along with the calling parameters.
			Trace.TraceInformation("[GetGpsData] called. UserId = {0}, DeviceId = {1}, Time = {2}",
			  userId, deviceId, time);
			string errorMessage = string.Empty;
			gpsData = new DataSet();

			//tryCount is the number of times to retry if the SQL execution or connection fails.
			//This is compared against tryMax, which is in the configuration 
			//   and set in GlobalStaticProperties.
			int tryCount = 0;

			//success is set to true when the SQL Execution succeeds.
			//Any subsequent errors are caused by your own code, and shouldn't cause a SQL retry.
			bool success = false;

			//This is the overall try/catch block to handle non-SQL exceptions and trace them.
			try
			{
				//This is the top of the retry loop. 
				do
				{
					//blank this out in case it loops back around and works the next time
					errorMessage = string.Empty;
					//increment the number of tries
					tryCount++;

					//this is the try block for the SQL code 
					try
					{
						//put all SQL code in using statements, to make sure you are disposing of 
						//  connections, commands, datareaders, etc.
						//note that this gets the connection string from GlobalStaticProperties,
						//  which retrieves it the first time from the Role Configuration.
						using (SqlConnection cnx
						  = new SqlConnection(GlobalStaticProperties.dbConnectionString))
						{
							//This can fail due to a bug in ADO.Net. They are not removing dead connections
							//  from the connection pool, so you can get a dead connection, and when you 
							//  try to execute this, it will fail. An immediate retry almost always succeeds.
							cnx.Open();

							//Execute the stored procedure and get the data.
							using (SqlCommand cmd = new SqlCommand("GpsData_GetByIdAndTime", cnx))
							{
								cmd.CommandType = CommandType.StoredProcedure;

								SqlParameter prm = new SqlParameter("@UserId", SqlDbType.NVarChar, 50);
								prm.Direction = ParameterDirection.Input;
								prm.Value = userId;
								cmd.Parameters.Add(prm);

								prm = new SqlParameter("@DeviceId", SqlDbType.NVarChar, 50);
								prm.Direction = ParameterDirection.Input;
								prm.Value = deviceId;
								cmd.Parameters.Add(prm);

								prm = new SqlParameter("@Time", SqlDbType.NVarChar, 50);
								prm.Direction = ParameterDirection.Input;
								prm.Value = time;
								cmd.Parameters.Add(prm);

								SqlDataAdapter da = new SqlDataAdapter(cmd);
								DataTable dt = new DataTable();
								da.Fill(dt);
								success = true;
								gpsData.Tables.Add(dt);

							}//using SqlCommand
						} //using SqlConnection
					}
					catch (SqlException ex)
					{
						errorMessage = "Error retrieving customer favorites.";
						Trace.TraceError("[GetCustomerFavorites]Try #{0}, "
						  + "will sleep {1}ms. SQL Exception = {2}",
							tryCount,
							GlobalStaticProperties.retrySleepTime[tryCount - 1], ex.ToString());

						//if it is not the last try, sleep before looping back around and trying again
						if (tryCount < GlobalStaticProperties.MaxTryCount
						  && GlobalStaticProperties.retrySleepTime[tryCount - 1] > 0)
							Thread.Sleep(GlobalStaticProperties.retrySleepTime[tryCount - 1]);
					}
					//it loops until it has tried more times than specified, or the SQL Execution succeeds
				} while (tryCount < GlobalStaticProperties.MaxTryCount && !success);
			}
			//catch any general exception that occurs and send back an error message
			catch (Exception ex)
			{
				Trace.TraceError("[GetCustomerFavorites], "
				  + "Overall Exception thrown = {0}", ex.ToString());
				errorMessage = "Error getting customer favorites.";
			}
			return errorMessage;
		}
	}
}