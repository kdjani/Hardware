using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//new ones
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Globalization;

namespace CustomerServicesWebRole
{
	internal class GpsDataAdd
    {
		internal string AddGpsData(string userId, string deviceId, string time,
		  string longitude, string latitude, string altitude)
        {
			Trace.TraceInformation("[AddGpsData] called. UserId = {0}, DeviceId = {1}, Time = {2}, Longitude = {3}, Latitude = {4}, Altitude = {5}",
				userId, deviceId, time, longitude, latitude, altitude);

            DateTime startTime =  DateTime.ParseExact(time, "ddMMyyHHmmss", CultureInfo.InvariantCulture);
            time = startTime.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            time = string.Format("{0} {1}:{2}:{3}", time, startTime.Hour, startTime.Minute, startTime.Second);

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
							using (SqlCommand cmd = new SqlCommand("dbo.GpsData_Add", cnx))
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

								prm = new SqlParameter("@Longitude", SqlDbType.NVarChar, 50);
								prm.Direction = ParameterDirection.Input;
								prm.Value = longitude;
								cmd.Parameters.Add(prm);

								prm = new SqlParameter("@Latitude", SqlDbType.NVarChar, 50);
								prm.Direction = ParameterDirection.Input;
								prm.Value = latitude;
								cmd.Parameters.Add(prm);

								prm = new SqlParameter("@Altitude", SqlDbType.NVarChar, 50);
								prm.Direction = ParameterDirection.Input;
								prm.Value = altitude;
                                cmd.Parameters.Add(prm);


                                cmd.ExecuteScalar();
                                success = true;

                            }//using SqlCommand
                        } //using SqlConnection
                    }
                    catch (SqlException ex)
                    {
                        errorMessage = "Error adding gps data.";
						Trace.TraceError("[AddGpsData]Try #{0}, will sleep {1}ms. SQL Exception = {2}",
                            tryCount, GlobalStaticProperties.retrySleepTime[tryCount - 1], ex.ToString());
                        if (tryCount < GlobalStaticProperties.MaxTryCount && GlobalStaticProperties.retrySleepTime[tryCount - 1] > 0)
                            Thread.Sleep(GlobalStaticProperties.retrySleepTime[tryCount - 1]);
                    }
                } while (tryCount < GlobalStaticProperties.MaxTryCount && !success);
            }
            catch (Exception ex)
            {
				Trace.TraceError("[AddGpsData] Overall Exception thrown = {0}", ex.ToString());
                errorMessage = "Error adding customer.";
            }

            return errorMessage;
        }
    }
}