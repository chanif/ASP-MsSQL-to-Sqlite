using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Label_Status.Text = GetData();
    }

    protected string GetData()
    {
        string sqlConnectString = ConfigurationManager.ConnectionStrings["Koneksi"].ToString();
        string strSQL = "SELECT COUNT(*) FROM t_cust";

        try
        {
            SqlConnection conn = new SqlConnection(sqlConnectString);
            SqlCommand cmd = new SqlCommand(strSQL, conn);

            conn.Open();
            string count = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return count;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    protected void ExportTextFile(object sender, EventArgs e)
    {
        string nama_tabel = " t_cust ";
        string txt = string.Empty;
        string temp = string.Empty;
        StringBuilder konten = new StringBuilder();

        string constr = ConfigurationManager.ConnectionStrings["Koneksi"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("exec sp_help t_cust"))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);

                        //Build the Text file data.

                        List<int> ListPenanda = new List<int>();
                        int counter = 0;

                        var numericTypes = new[] { typeof(Byte), typeof(Decimal), typeof(Double),
                                            typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
                                            typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};

                        konten.Clear();
                        konten.Append("DROP TABLE t_cust;\nCREATE TABLE t_cust(Area VARCHAR(75) NULL,District VARCHAR(75) NULL,Teritorry VARCHAR(75) NULL,CustomerCode VARCHAR(75) NOT NULL,CustomerName VARCHAR(150) NULL,CustomerAddressLine1 VARCHAR(75) NULL,CustomerAddressLine2 VARCHAR(75) NULL,CustomerAddressLine3 VARCHAR(75) NULL,CustomerZipCode VARCHAR(75) NULL,CustomerCityCode VARCHAR(75) NULL,CustomerCountryCode VARCHAR(75) NULL,CustomerPhoneNumber VARCHAR(75) NULL,CustomerFaxNumber VARCHAR(75) NULL,CustomerMobileNumber VARCHAR(75) NULL,CustomerEmail VARCHAR(75) NULL,CustomerTaxNumber VARCHAR(75) NULL,CustomerTaxOffice VARCHAR(75) NULL,CustomerLicenceNumber VARCHAR(75) NULL,CustomerCreditLimit REAL(22, 2) NULL,CustomerFinanceCode VARCHAR(75) NULL,CustomerCurrency VARCHAR(75) NULL,CustomerDocCurrency VARCHAR(75) NULL,CustomerBank VARCHAR(75) NULL,CustomerAccount VARCHAR(75) NULL,CustomerIsBlocked CHAR(1) NULL,CustomerIsCreditCustomer CHAR(1) NULL,CustomerOwner VARCHAR(75) NULL,CustomerParentCustomerCode VARCHAR(250) NULL,CustomerParentCustomerName VARCHAR(250) NULL,CustomerIndustryVolume BIGINT NULL,CustomerInternalVolume BIGINT NULL,CustSegType VARCHAR(3) NULL,CustSegInternalClassification VARCHAR(75) NULL,CustSegIndustryClassification VARCHAR(75) NULL,CustSegQualityClassification VARCHAR(75) NULL,CustSegInternalClassificationDescription VARCHAR(75) NULL,CustSegIndustryClassificationDescription VARCHAR(75) NULL,CustSegQualityClassificationDescription VARCHAR(75) NULL,CustSegTradeCategory VARCHAR(75) NULL,CustSegSubTradeCategory VARCHAR(75) NULL,CustSegFrequency VARCHAR(75) NULL,CustSegTradeSegment VARCHAR(75) NULL,CustSegPlanogramStatus VARCHAR(75) NULL,CustSegPaymentTerm VARCHAR(75) NULL,CustSegStatus VARCHAR(75) NULL,CustomerRegisteredName VARCHAR(150) NULL,CustomerAvailableCreditNote REAL(30, 4) NULL,CustomerCreditNoteLimit REAL(30, 4) NULL,CustomerCashLimit REAL(30, 4) NULL,Cluster VARCHAR(75) NULL,AddressGeographicalX REAL(18, 6) NULL,AddressGeographicalY REAL(18, 6) NULL,Routing VARCHAR(80) NULL,CustSegCSUCustomer INT NULL,Cust_City VARCHAR(75) NULL,Cust_District VARCHAR(75) NULL,Provinsi VARCHAR(400) NULL,Dati_II VARCHAR(400) NULL,Kecamatan VARCHAR(400) NULL,Kelurahan VARCHAR(400) NULL);\n\n\n");

                        foreach (DataColumn column in dt.Columns)
                        {
                            konten.Append(column.ColumnName + "\t\t");

                            if (numericTypes.Contains(column.DataType))
                            {
                                ListPenanda.Add(1);
                            }
                            else
                            {
                                ListPenanda.Add(0);
                            }
                        }
                        konten.Append("\n\n");

                        foreach (DataRow row in dt.Rows)
                        {
                            counter = 0;
                            foreach (DataColumn column in dt.Columns)
                            {
                                if (ListPenanda[counter] == 1)
                                {
                                    temp = row[column.ColumnName].ToString();
                                    temp = temp.Replace(",", ".");
                                    if (temp == "")
                                    {
                                        konten.Append("\"\"");
                                    }
                                    else
                                    {
                                        konten.Append(temp);
                                    }
                                }
                                else
                                {
                                    konten.Append(row[column.ColumnName].ToString() + "\t\t");
                                }
                                konten.Append("\n");

                            }
                        }

                        txt = konten.ToString();

/*
 * 
                        int total = ListPenanda.Count;

                        foreach (DataRow row in dt.Rows)
                        {
                            txt += "INSERT INTO " + nama_tabel + " VALUES (";
                            
                            counter = 0;
                            foreach (DataColumn column in dt.Columns)
                            {
                                if (ListPenanda[counter] == 1)
                                {
                                    temp = row[column.ColumnName].ToString();
                                    temp = temp.Replace(",", ".");
                                    if (temp == "")
                                    {
                                        txt += "\"\"";
                                    }
                                    else
                                    {
                                        txt += temp;
                                    }
                                }
                                else
                                {
                                    txt += '"' + row[column.ColumnName].ToString() + '"';
                                }

                                counter++;
                                if (counter < total)
                                {
                                    txt += ", ";
                                }
                            }

                            //Add new line.
                            txt += ");\n";
                        }
                         * */


                        using (StreamWriter _testData = new StreamWriter(Server.MapPath("~/data/data.txt"), false))
                        {
                            _testData.WriteLine(txt); // Write the file.
                        }
                    }
                }
            }
        }
    }

    /*
    protected void ExportTextFile(object sender, EventArgs e)
    {
        string nama_tabel = " t_cust ";
        string txt = string.Empty;
        string coba = string.Empty;
        StringBuilder konten = new StringBuilder();

        string constr = ConfigurationManager.ConnectionStrings["Koneksi"].ConnectionString;
        using (SqlConnection con = new SqlConnection(constr))
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM" + nama_tabel))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);

                        //Build the Text file data.
                        
                        string temp = string.Empty;
                        
                        List<int> ListPenanda = new List<int>();
                        int counter = 0;

                        var numericTypes = new[] { typeof(Byte), typeof(Decimal), typeof(Double),
                                            typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
                                            typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};


                        txt = "DROP TABLE t_cust;\nCREATE TABLE t_cust(Area VARCHAR(75) NULL,District VARCHAR(75) NULL,Teritorry VARCHAR(75) NULL,CustomerCode VARCHAR(75) NOT NULL,CustomerName VARCHAR(150) NULL,CustomerAddressLine1 VARCHAR(75) NULL,CustomerAddressLine2 VARCHAR(75) NULL,CustomerAddressLine3 VARCHAR(75) NULL,CustomerZipCode VARCHAR(75) NULL,CustomerCityCode VARCHAR(75) NULL,CustomerCountryCode VARCHAR(75) NULL,CustomerPhoneNumber VARCHAR(75) NULL,CustomerFaxNumber VARCHAR(75) NULL,CustomerMobileNumber VARCHAR(75) NULL,CustomerEmail VARCHAR(75) NULL,CustomerTaxNumber VARCHAR(75) NULL,CustomerTaxOffice VARCHAR(75) NULL,CustomerLicenceNumber VARCHAR(75) NULL,CustomerCreditLimit REAL(22, 2) NULL,CustomerFinanceCode VARCHAR(75) NULL,CustomerCurrency VARCHAR(75) NULL,CustomerDocCurrency VARCHAR(75) NULL,CustomerBank VARCHAR(75) NULL,CustomerAccount VARCHAR(75) NULL,CustomerIsBlocked CHAR(1) NULL,CustomerIsCreditCustomer CHAR(1) NULL,CustomerOwner VARCHAR(75) NULL,CustomerParentCustomerCode VARCHAR(250) NULL,CustomerParentCustomerName VARCHAR(250) NULL,CustomerIndustryVolume BIGINT NULL,CustomerInternalVolume BIGINT NULL,CustSegType VARCHAR(3) NULL,CustSegInternalClassification VARCHAR(75) NULL,CustSegIndustryClassification VARCHAR(75) NULL,CustSegQualityClassification VARCHAR(75) NULL,CustSegInternalClassificationDescription VARCHAR(75) NULL,CustSegIndustryClassificationDescription VARCHAR(75) NULL,CustSegQualityClassificationDescription VARCHAR(75) NULL,CustSegTradeCategory VARCHAR(75) NULL,CustSegSubTradeCategory VARCHAR(75) NULL,CustSegFrequency VARCHAR(75) NULL,CustSegTradeSegment VARCHAR(75) NULL,CustSegPlanogramStatus VARCHAR(75) NULL,CustSegPaymentTerm VARCHAR(75) NULL,CustSegStatus VARCHAR(75) NULL,CustomerRegisteredName VARCHAR(150) NULL,CustomerAvailableCreditNote REAL(30, 4) NULL,CustomerCreditNoteLimit REAL(30, 4) NULL,CustomerCashLimit REAL(30, 4) NULL,Cluster VARCHAR(75) NULL,AddressGeographicalX REAL(18, 6) NULL,AddressGeographicalY REAL(18, 6) NULL,Routing VARCHAR(80) NULL,CustSegCSUCustomer INT NULL,Cust_City VARCHAR(75) NULL,Cust_District VARCHAR(75) NULL,Provinsi VARCHAR(400) NULL,Dati_II VARCHAR(400) NULL,Kecamatan VARCHAR(400) NULL,Kelurahan VARCHAR(400) NULL);\n";

                        foreach (DataColumn column in dt.Columns)
                        {
                            //Add the Header row for Text file.
                            //txt += column.ColumnName + "\t\t";

                            if (numericTypes.Contains(column.DataType))
                            {
                                ListPenanda.Add(1);
                            }
                            else
                            {
                                ListPenanda.Add(0);
                            }
                        }

                        int total = ListPenanda.Count;

                        var dbFile = @"E:\ASP\data\data.sqlite";
                        if (File.Exists(dbFile)) File.Delete(dbFile);

                        var connString = string.Format(@"Data Source={0}; Pooling=false; FailIfMissing=false;", dbFile);

                        //test using System.Data.Common and SQLiteFactory
                        //Test1(connString);


                        using (var dbConn = new System.Data.SQLite.SQLiteConnection(connString))
                        {
                            dbConn.Open();
                            //using (System.Data.SQLite.SQLiteCommand sqlite = dbConn.CreateCommand())
                            using (var sqlite = new SQLiteCommand(dbConn))
                            {
                                //create table
                                sqlite.CommandText = @"CREATE TABLE IF NOT EXISTS " + nama_tabel + " (Area VARCHAR(75) NULL,District VARCHAR(75) NULL,Teritorry VARCHAR(75) NULL,CustomerCode VARCHAR(75) NOT NULL,CustomerName VARCHAR(150) NULL,CustomerAddressLine1 VARCHAR(75) NULL,CustomerAddressLine2 VARCHAR(75) NULL,CustomerAddressLine3 VARCHAR(75) NULL,CustomerZipCode VARCHAR(75) NULL,CustomerCityCode VARCHAR(75) NULL,CustomerCountryCode VARCHAR(75) NULL,CustomerPhoneNumber VARCHAR(75) NULL,CustomerFaxNumber VARCHAR(75) NULL,CustomerMobileNumber VARCHAR(75) NULL,CustomerEmail VARCHAR(75) NULL,CustomerTaxNumber VARCHAR(75) NULL,CustomerTaxOffice VARCHAR(75) NULL,CustomerLicenceNumber VARCHAR(75) NULL,CustomerCreditLimit REAL(22, 2) NULL,CustomerFinanceCode VARCHAR(75) NULL,CustomerCurrency VARCHAR(75) NULL,CustomerDocCurrency VARCHAR(75) NULL,CustomerBank VARCHAR(75) NULL,CustomerAccount VARCHAR(75) NULL,CustomerIsBlocked CHAR(1) NULL,CustomerIsCreditCustomer CHAR(1) NULL,CustomerOwner VARCHAR(75) NULL,CustomerParentCustomerCode VARCHAR(250) NULL,CustomerParentCustomerName VARCHAR(250) NULL,CustomerIndustryVolume BIGINT NULL,CustomerInternalVolume BIGINT NULL,CustSegType VARCHAR(3) NULL,CustSegInternalClassification VARCHAR(75) NULL,CustSegIndustryClassification VARCHAR(75) NULL,CustSegQualityClassification VARCHAR(75) NULL,CustSegInternalClassificationDescription VARCHAR(75) NULL,CustSegIndustryClassificationDescription VARCHAR(75) NULL,CustSegQualityClassificationDescription VARCHAR(75) NULL,CustSegTradeCategory VARCHAR(75) NULL,CustSegSubTradeCategory VARCHAR(75) NULL,CustSegFrequency VARCHAR(75) NULL,CustSegTradeSegment VARCHAR(75) NULL,CustSegPlanogramStatus VARCHAR(75) NULL,CustSegPaymentTerm VARCHAR(75) NULL,CustSegStatus VARCHAR(75) NULL,CustomerRegisteredName VARCHAR(150) NULL,CustomerAvailableCreditNote REAL(30, 4) NULL,CustomerCreditNoteLimit REAL(30, 4) NULL,CustomerCashLimit REAL(30, 4) NULL,Cluster VARCHAR(75) NULL,AddressGeographicalX REAL(18, 6) NULL,AddressGeographicalY REAL(18, 6) NULL,Routing VARCHAR(80) NULL,CustSegCSUCustomer INT NULL,Cust_City VARCHAR(75) NULL,Cust_District VARCHAR(75) NULL,Provinsi VARCHAR(400) NULL,Dati_II VARCHAR(400) NULL,Kecamatan VARCHAR(400) NULL,Kelurahan VARCHAR(400) NULL);";
                                sqlite.ExecuteNonQuery();

                                //clear table
                                sqlite.CommandText = @"DELETE FROM " + nama_tabel + ";";
                                sqlite.ExecuteNonQuery();

                                using (var transaction = dbConn.BeginTransaction())
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {

                                        //txt += "INSERT INTO " + nama_tabel + " VALUES (";
                                        konten.Clear();
                                        konten.Append("INSERT INTO " + nama_tabel + " VALUES (");

                                        counter = 0;
                                        foreach (DataColumn column in dt.Columns)
                                        {

                                            if (ListPenanda[counter] == 1)
                                            {
                                                temp = row[column.ColumnName].ToString();
                                                temp = temp.Replace(",", ".");
                                                if (temp == "")
                                                {
                                                    //konten.Append("\"\"");
                                                    konten.Append("NULL");
                                                }
                                                else
                                                {
                                                    konten.Append(temp);
                                                }
                                            }
                                            else
                                            {
                                                konten.Append('"' + row[column.ColumnName].ToString() + '"');
                                            }

                                            counter++;
                                            if (counter < total)
                                            {
                                                konten.Append(", ");
                                            }
                                        }

                                        //Add new line.
                                        //txt += ");\n";
                                        konten.Append(");");

                                        sqlite.CommandText = konten.ToString();
                                        sqlite.ExecuteNonQuery();
                                    }
                                    transaction.Commit();
                                }

                                //txt = konten.ToString();
                                //sqlite.CommandText = konten.ToString();
                                //sqlite.ExecuteNonQuery();
                            }
                            if (dbConn.State != System.Data.ConnectionState.Closed) dbConn.Close();
                        }


                        /*
                        using (StreamWriter _testData = new StreamWriter(Server.MapPath("~/data/data.sql"), false))
                        {
                            _testData.WriteLine(txt); // Write the file.
                        }

                        using (StreamWriter _testData = new StreamWriter(Server.MapPath("~/data/data.txt"), false))
                        {
                            _testData.WriteLine(txt); // Write the file.
                        } 
*/
                        //Download the Text file.
                        /*
                        Response.Clear();
                        Response.Buffer = true;
                        Response.AddHeader("content-disposition", "attachment;filename=SqlExport.txt");
                        Response.Charset = "";
                        Response.ContentType = "application/text";
                        Response.Output.Write(txt);
                        Response.Flush();
                        Response.End();
                         * 
                    }
                }
            }
        }
    }*/
}
