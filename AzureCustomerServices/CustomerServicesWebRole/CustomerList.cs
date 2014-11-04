using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//new 
using System.Diagnostics;
using System.Data.SqlClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Data;
using System.Threading; 

namespace CustomerServicesWebRole
{
    internal class CustomerList
    {
        internal string GetListOfCustomers(out DataSet customers)
        {
            Trace.TraceInformation("[GetListOfCustomers] called.");
            string errorMessage = string.Empty;
            
            customers = new DataSet();

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
                            using (SqlCommand cmd = new SqlCommand("Customer_List", cnx))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                SqlDataAdapter da = new SqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                success = true;
                                customers.Tables.Add(dt);

                                Trace.TraceInformation("[GetListOfCustomers] RowCount = {0}", dt.Rows.Count);
                            }//using SqlCommand
                        } //using SqlConnection
                    }
                    catch (SqlException ex)
                    {
                        errorMessage = "Error retrieving customer favorites.";
                        Trace.TraceError("[GetListOfCustomers] Try #{0}, will sleep {1}ms. SQL Exception = {2}",
                            tryCount, GlobalStaticProperties.retrySleepTime[tryCount - 1], ex.ToString());
                        if (tryCount < GlobalStaticProperties.MaxTryCount && GlobalStaticProperties.retrySleepTime[tryCount - 1] > 0)
                            Thread.Sleep(GlobalStaticProperties.retrySleepTime[tryCount - 1]);
                    }
                } while (tryCount < GlobalStaticProperties.MaxTryCount && !success);
            }
            catch (Exception ex)
            {
                Trace.TraceError("[GetListOfCustomers] Overall Exception thrown = {0}", ex.ToString());
                errorMessage = "Error getting customer list.";
            }

            return errorMessage;
        }

    }
}