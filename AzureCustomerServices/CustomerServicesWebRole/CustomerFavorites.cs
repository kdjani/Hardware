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
	internal class CustomerFavorites
	{

		/// <summary>
		/// Given the first and last name, return the favorite movie and language.
		/// </summary>
		internal string GetCustomerFavorites(out string favoriteMovie, out string favoriteLanguage,
		  string firstName, string lastName)
		{
			//write to diagnostics that this routine was called, along with the calling parameters.
			Trace.TraceInformation("[GetCustomerFavorites] called. FirstName = {0}, LastName = {1}",
			  firstName, lastName);
			string errorMessage = string.Empty;
			favoriteMovie = string.Empty;
			favoriteLanguage = string.Empty;

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
							using (SqlCommand cmd = new SqlCommand("Customer_GetByName", cnx))
							{
								cmd.CommandType = CommandType.StoredProcedure;

								SqlParameter prm = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
								prm.Direction = ParameterDirection.Input;
								prm.Value = firstName;
								cmd.Parameters.Add(prm);

								prm = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
								prm.Direction = ParameterDirection.Input;
								prm.Value = lastName;
								cmd.Parameters.Add(prm);

								SqlDataAdapter da = new SqlDataAdapter(cmd);
								DataTable dt = new DataTable();
								da.Fill(dt);
								//the call to get the data was successful
								//any error after this is not caused by connection problems, so no retry is needed
								success = true;

								if (dt == null || dt.Rows.Count <= 0)
								{
									errorMessage = string.Format("Error retrieving favorites; "
										+ "record not found for '{0}' '{1}'.",
										firstName, lastName);
								}
								else
								{
									DataRow dr = dt.Rows[0];
									favoriteMovie = dr["FavoriteMovie"].ToString();
									favoriteLanguage = dr["FavoriteLanguage"].ToString();

									Trace.TraceInformation("[GetCustomerFavorites] FirstName = {0}, LastName = {1}, "
									  + "FavoriteMovie = {2}, FavoriteLanguage = {3}",
										firstName, lastName, favoriteMovie, favoriteLanguage);
								}
							}//using SqlCommand
						} //using SqlConnection
					}
					catch (SqlException ex)
					{
						//This is handling the SQL Exception. It traces the method and parameters, the retry #, 
						//  how long it's going to sleep, and the exception that occurred.
						//Note that it is using the array retrySleepTime set up in GlobalStaticProperties.
						errorMessage = "Error retrieving customer favorites.";
						Trace.TraceError("[GetCustomerFavorites] firatName = {0}, lastName = {1}, Try #{2}, "
						  + "will sleep {3}ms. SQL Exception = {4}",
							firstName, lastName, tryCount,
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
				Trace.TraceError("[GetCustomerFavorites] firstName = {0}, lastName = {1}, "
				  + "Overall Exception thrown = {2}", firstName, lastName, ex.ToString());
				errorMessage = "Error getting customer favorites.";
			}
			return errorMessage;
		}
	}
}