using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TestService
{
    public static class DAC
    {
		//private static string m_endpointAddress = @"http://gpslogger.cloudapp.net/CustomerServices.svc";
		private static string m_endpointAddress = @"http://localhost:25670/CustomerServices.svc";

		private static GpsSvc.CustomerServicesClient getClient()
		{
			GpsSvc.CustomerServicesClient prx = new GpsSvc.CustomerServicesClient();
			prx.Endpoint.Address = new System.ServiceModel.EndpointAddress(m_endpointAddress);
			//this sets the timeout of the service call, which should give you enough time
			//  to finish debugging your service call
			prx.InnerChannel.OperationTimeout = new TimeSpan(0, 5, 0);
			return prx;
		}

		internal static string AddGpsData(string userId, string deviceId, string time, string longitude, string latitude, string altitude)
		{
			GpsSvc.CustomerServicesClient prx = getClient();
			return prx.AddGpsData(userId, deviceId, time, longitude, latitude, altitude);
		}

		internal static string GetGpsData(string userId, string deviceId, string time, out DataSet gpsData)
		{
			GpsSvc.CustomerServicesClient prx = getClient();
		    return prx.GetGpsData(out gpsData, userId, deviceId, time);
		}

		internal static string DeleteGpsData(string userId, string deviceId, string time)
		{
			GpsSvc.CustomerServicesClient prx = getClient();
			return prx.DeleteGpsData(userId, deviceId, time);
		}
    }
}
