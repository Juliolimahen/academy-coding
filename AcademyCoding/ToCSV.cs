
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Data.Sql;



namespace AcademyCoding
{
    //dataTable para arquivo
    class ToCSV
    {
        public static void ExportToCSVFile(Stream stream, DataTable table)
        {
            var encoding = Encoding.GetEncoding("ISO-8859-15");
            using (TextWriter writer = new StreamWriter(stream, encoding, 1024, true))
            {
                using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    foreach (DataColumn  column in table.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }
                    csv.NextRecord();
                    foreach (DataRow row in table.Rows)
                    {
                        for(var i=0; i< table.Columns.Count; i++)
                        {
                            csv.WriteField(row[i]);
                        }
                        csv.NextRecord();
                    }
                    writer.Flush();
                }
            }
        }
        // ler csv e criar tabela
        public static void CreatesSqlTable(DataTable dt, string tablename, string strconnection)
        {
            string table = "";
            table += "IF NOT EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[" + tablename + "]')) ";
            table += "BEGIN ";
            table += "CREATE TABLE " + tablename + " ";
            table += "("; 
            for (int i = 0; i <dt.Columns.Count; i++)
            {
                if (i != dt.Columns.Count -1)
                    table += dt.Columns[i].ColumnName + " " + "varchar(max)" + ",";
                else
                    table += dt.Columns[i].ColumnName + " " + "varchar(max)";
            }
            table += ") ";
            table += "END";
            InsertQuery(table, strconnection);
            CopyData(strconnection, dt, tablename);
        }
        public static void InsertQuery(string qry, string connection)
        {
            SqlConnection sqlConnection = new SqlConnection(connection);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = qry;
            cmd.Connection = sqlConnection;

            sqlConnection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.Close();
        }
        public static void CopyData(string connStr, DataTable dt, string tablename)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connStr, SqlBulkCopyOptions.TableLock))
            {
                bulkCopy.DestinationTableName = tablename;
                bulkCopy.WriteToServer(dt);
            }
        }
    }
}
