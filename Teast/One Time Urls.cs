using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Transactions;

namespace Teast
{
    public class One_Time_Urls
    {
        //static void Main(string[] args)
        //{
        //    //Create_CSPRNG();
        //    Query_Add_Token("me");
        //}

    public static string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Touyr_Email_Trial;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static void Query_Add_Token(string UserName)
        {
            string token = Create_CSPRNG();
            using (TransactionScope scope = new TransactionScope())
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("[dbo].[URLAddToken]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("Token", SqlDbType.NVarChar, (128)).Value = token;
                        command.Parameters.Add("UserName", SqlDbType.Char, (50)).Value = UserName;

                        string URL = "https://localhost:44300/About/" + token + "/" + UserName;
                        Test.Program.sendEmail("STUFF", "nneely@niar.wichita.edu", URL);
                        Debug.WriteLine(URL);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        scope.Complete();

                        //while(reader.Read())
                        //{

                        //}
                    }
                }
            }

        }

        public static string Query_Submit_Token(string token, string userName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("[dbo].[URLSubmitToken]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("Token", SqlDbType.NVarChar, (128)).Value = token;
                    command.Parameters.Add("UserName", SqlDbType.Char, (50)).Value = userName;
                    command.Parameters.Add("Response", SqlDbType.NVarChar, (128)).Direction = ParameterDirection.Output;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    try
                    {
                        return command.Parameters["Response"].Value.ToString();
                    }
                    catch
                    {
                        return "Invalid Token";
                    }
                }
            }
        }

        public static string Create_CSPRNG ()
        {
            string token;

            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[50];
                rng.GetBytes(tokenData);

                token = Convert.ToBase64String(tokenData);
            }
            //Console.WriteLine(token);
            //Console.ReadLine();
            //This is for removing all of those annoying characters

            List<char> charsToRemove = new List<char>() { '@', '/', '&', '\'', '<', '>', '?', ':', ':', '+', '*', '|', '^', '~', '[', ']', '{', '}'  };
            token = Remove(token, charsToRemove);
            return token;
        }

        //This is the functon for removing specific characters from a string
        //https://www.techiedelight.com/remove-specific-characters-from-string-csharp/
        public static string Remove(string StringtoEdit, List<Char> CharsToRemove)
        {
            foreach (char c in CharsToRemove) {
                StringtoEdit = StringtoEdit.Replace(c.ToString(), string.Empty);
            }
            return StringtoEdit;
        }
    }
}
