using BenchmarkDotNet.Exporters.Csv;
using CsvHelper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AcademyCoding
{
    class Program
    {
        static void Main(string[] args)
        {
            string strConn = @"server=DEVPC; DataBase=academyCoding; Trusted_Connection = True";

            SqlConnection conn = new SqlConnection(strConn);
            string query = "select * from dbo.alunos";
            DataTable dataTable = new DataTable();

            try
            {
                //abrir conexão 
                conn.Open();
                Console.WriteLine("A conexão foi efetuada com sucesso.");

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dataTable);
                
                //var enderecoDoArquivo = "";

                ToCSV.ExportToCSVFile(File.CreateText(@"G:\Meu Drive\Projetos\academyCoding\AcademyCoding\Banco.csv").BaseStream, dataTable);
                conn.Close();
                da.Dispose();
            }
            catch (SqlException sqle)
            {
                Console.WriteLine("falha ao efetuar a canexão. Erro: "+ sqle);
            }
               //var enderecoDoArquivo = "";
               using (var reader = new StreamReader(@"G:\Meu Drive\Projetos\academyCoding\AcademyCoding\CLIENTES.csv"))
               using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
               {
                       using (var dr = new CsvDataReader(csv))
                        {
                            var dt = new DataTable();
                           dt.Load(dr);
                           ToCSV.CreatesSqlTable(dt, "CLIENTES", strConn);
                        }
               }

        }
    }
}
