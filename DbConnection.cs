using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace HospitalApp
{
   public class DbConnection
    {
        public SqlConnection con;
        //SqlConnection con = new SqlConnection();
        //string constring = ConfigurationManager.ConnectionStrings["CC"].ConnectionString;

        public SqlConnection DbConnectionn()
        {

            try
            {

                if (con == null || Convert.ToString(con.State) == "Closed")
                {
                    // con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["CC"].ToString());
                    con = new SqlConnection(WebConfigurationManager.ConnectionStrings["CC"].ConnectionString);
                }


                return con;
            }
            catch (Exception e)
            {
              
                throw e;
            }
          


        }

        ////public SqlConnection connect()
        ////{
        ////    try
        ////    {
        ////        if (con.State == ConnectionState.Closed)
        ////        {
        ////            con.ConnectionString = constring;
        ////            con.Open();
        ////        }

        ////    }
        ////    catch (Exception e)
        ////    {

        ////    }
        ////    return con;
        ////}

        ////public SqlConnection disconnect()
        ////{
        ////    try
        ////    {
        ////        if (con.State == ConnectionState.Open)
        ////        {
        ////            con.Close();
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {

        ////    }
        ////    return con;
        ////}
    }
}
