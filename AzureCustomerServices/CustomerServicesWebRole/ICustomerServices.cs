using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;

namespace CustomerServicesWebRole
{
	[ServiceContract]
	public interface ICustomerServices
	{
		[OperationContract]
		string GetFavorites(out string favoriteMovie, out string favoriteLanguage,
			string firstName, string lastName);

		[OperationContract]
		string UpdateFavoritesByName(string firstName, string lastName, string favoriteMovie,
		  string favoriteLanguage);

		[OperationContract]
		string GetCustomerList(out DataSet customers);

		[OperationContract]
		string AddACustomer(string firstName, string lastName, string favoriteMovie,
		  string favoriteLanguage);

		[OperationContract]
		string AddGpsData(string userId, string deviceId, string time, string longitude, string latitude, string altitude);

		[OperationContract]
		string GetGpsData(string userId, string deviceId, string time, out DataSet gpsData);

		[OperationContract]
		string DeleteGpsData(string userId, string deviceId, string time);
	}
}
