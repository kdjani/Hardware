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
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
	public class CustomerServices : ICustomerServices
	{
		public string GetFavorites(out string favoriteMovie, out string favoriteLanguage,
		  string firstName, string lastName)
		{
			string errorMessage = string.Empty;
			favoriteMovie = string.Empty;
			favoriteLanguage = string.Empty;
			CustomerFavorites cf = new CustomerFavorites();
			errorMessage = cf.GetCustomerFavorites(out favoriteMovie, out favoriteLanguage, firstName, lastName);
			return errorMessage;
		}

		public string UpdateFavoritesByName(string firstName, string lastName, string favoriteMovie,
		  string favoriteLanguage)
		{
			string errorMessage = string.Empty;
			CustomerFavoritesUpdate cfu = new CustomerFavoritesUpdate();
			errorMessage = cfu.SetCustomerFavorites(firstName, lastName, favoriteMovie, favoriteLanguage);
			return errorMessage;
		}

		public string AddACustomer(string firstName, string lastName,
		  string favoriteMovie, string favoriteLanguage)
		{
			string errorMessage = string.Empty;
			CustomerFavoritesAdd cfa = new CustomerFavoritesAdd();
			errorMessage = cfa.AddCustomer(firstName, lastName, favoriteMovie, favoriteLanguage);
			return errorMessage;
		}

		public string GetCustomerList(out DataSet customers)
		{
			string errorMessage = string.Empty;
			customers = new DataSet();
			CustomerList cl = new CustomerList();
			errorMessage = cl.GetListOfCustomers(out customers);
			return errorMessage;
		}

		public string AddGpsData(string userId, string deviceId, string time,
		  string longitude, string latitude, string altitude)
		{
			string errorMessage = string.Empty;
			GpsDataAdd gda = new GpsDataAdd();
			errorMessage = gda.AddGpsData(userId, deviceId, time, longitude, latitude, altitude);
			return errorMessage;
		}

        public string GetGpsData(string userId, string deviceId, string startTime, string endTime, out DataSet gpsData)
		{
			string errorMessage = string.Empty;
			GpsDataGet cfa = new GpsDataGet();
            errorMessage = cfa.GetGpsData(userId, deviceId, startTime, endTime, out gpsData);
			return errorMessage;
		}

        public string DeleteGpsData(string userId, string deviceId, string startTime, string endTime)
		{
			string errorMessage = string.Empty;
			GpsDataDelete cfa = new GpsDataDelete();
            errorMessage = cfa.DeleteGpsData(userId, deviceId, startTime, endTime);
			return errorMessage;
		}
	}
}
