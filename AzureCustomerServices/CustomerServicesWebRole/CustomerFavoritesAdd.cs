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
    internal class CustomerFavoritesAdd
    {
        internal string AddCustomer(string firstName, string lastName,
          string favoriteMovie, string favoriteLanguage)
        {
            Trace.TraceInformation("[AddCustomer] called. FirstName = {0}, LastName = {1}, Movie = {2}, Language = {3}",
                firstName, lastName, favoriteMovie, favoriteLanguage);

            string errorMessage = string.Empty;

            int tryCount = 0;
            bool success = false;

            try
            {
                do
                {
                    errorMessage = string.Empty; //blank this out in case it loops back around, and works the next time
                    tryCount++;

                    try
                    {
                        using (SqlConnection cnx = new SqlConnection(GlobalStaticProperties.dbConnectionString))
                        {
                            cnx.Open();
                            using (SqlCommand cmd = new SqlCommand("Customer_Add", cnx))
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

                                prm = new SqlParameter("@FavoriteMovie", SqlDbType.NVarChar, 50);
                                prm.Direction = ParameterDirection.Input;
                                if (favoriteMovie.Length > 0)
                                    prm.Value = favoriteMovie;
                                else
                                    prm.Value = DBNull.Value;
                                cmd.Parameters.Add(prm);

                                prm = new SqlParameter("@FavoriteLanguage", SqlDbType.NVarChar, 50);
                                prm.Direction = ParameterDirection.Input;
                                if (favoriteLanguage.Length > 0)
                                    prm.Value = favoriteLanguage;
                                else
                                    prm.Value = DBNull.Value;
                                cmd.Parameters.Add(prm);

                                cmd.ExecuteScalar();
                                success = true;

                            }//using SqlCommand
                        } //using SqlConnection
                    }
                    catch (SqlException ex)
                    {
                        errorMessage = "Error adding customer.";
                        Trace.TraceError("[AddCustomer] firatName = {0}, lastName = {1}, Try #{2}, will sleep {3}ms. SQL Exception = {4}",
                            firstName, lastName, tryCount, GlobalStaticProperties.retrySleepTime[tryCount - 1], ex.ToString());
                        if (tryCount < GlobalStaticProperties.MaxTryCount && GlobalStaticProperties.retrySleepTime[tryCount - 1] > 0)
                            Thread.Sleep(GlobalStaticProperties.retrySleepTime[tryCount - 1]);
                    }
                } while (tryCount < GlobalStaticProperties.MaxTryCount && !success);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[AddCustomer] firstName = {0}, lastName = {1}, Overall Exception thrown = {2}", firstName, lastName, ex.ToString());
                errorMessage = "Error adding customer.";
            }

            return errorMessage;
        }

    }
}