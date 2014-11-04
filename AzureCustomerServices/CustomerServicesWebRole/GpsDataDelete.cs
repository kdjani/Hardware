using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//new ones
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace CustomerServicesWebRole
{
	internal class GpsDataDelete
    {
		internal string DeleteGpsData(string userId, string deviceId, string time)
        {
			Trace.TraceInformation("[DeleteGpsData] called. UserId = {0}, DeviceId = {1}, Time = {2}",
				userId, deviceId, time);

            string errorMessage = string.Empty;

            int tryCount = 0;
            bool success = false;

            try
            {
                do
                {
                    errorMessage = string.Empty;
                    tryCount++;

                    try
                    {
                        using (SqlConnection cnx = new SqlConnection(GlobalStaticProperties.dbConnectionString))
                        {
                            cnx.Open();
							using (SqlCommand cmd = new SqlCommand("GpsData_DeleteByIdAndTime", cnx))
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

                                cmd.ExecuteScalar();
                                success = true;

                            }//using SqlCommand
                        } //using SqlConnection
                    }
                    catch (SqlException ex)
                    {
                        errorMessage = "Error deleting gps data.";
						Trace.TraceError("[DeleteGpsData]Try #{0}, will sleep {1}ms. SQL Exception = {2}",
                            tryCount, GlobalStaticProperties.retrySleepTime[tryCount - 1], ex.ToString());
                        if (tryCount < GlobalStaticProperties.MaxTryCount && GlobalStaticProperties.retrySleepTime[tryCount - 1] > 0)
                            Thread.Sleep(GlobalStaticProperties.retrySleepTime[tryCount - 1]);
                    }
                } while (tryCount < GlobalStaticProperties.MaxTryCount && !success);
            }
            catch (Exception ex)
            {
				Trace.TraceError("[DeleteGpsData] Overall Exception thrown = {0}", ex.ToString());
                errorMessage = "Error adding customer.";
            }

            return errorMessage;
        }
    }
}