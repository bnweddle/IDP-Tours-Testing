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
using System.Net.Mail;
using IronBarCode;
using System.IO;

namespace Teast
{
    public class One_Time_Urls
    {
        static void Main(string[] args)
        {
            //Create_CSPRNG();
            Query_Add_Token("me");
        }

        public static string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Touyr_Email_Trial;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static string i2Path = @"C:\Users\nneely\Desktop\Teast Folder\Test\Teast\bin\Debug\QRcode.png";
        public static string DBURL = "https://localhost:44300/About/";

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
                        command.Parameters.Add("UserID", SqlDbType.Char, (50)).Value = UserName;

                        string URL = DBURL + "/" + token + "/" + UserName;
                        sendEmail("STUFF", "nneely@niar.wichita.edu", URL);
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
                    command.Parameters.Add("UserID", SqlDbType.Char, (50)).Value = userName;
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

        //Program II code

        //static void Main(string[] args)
        //{
        //    const string i2Path = @"C:\Users\nneely\Desktop\Teast Folder\Test\Teast\bin\Debug\QRcode.png";
        //    Console.WriteLine("Hello World!");
        //    sendEmail("STUFF", "nneely@niar.wichita.edu", "https://localhost:44300/About/nPMKni31jG32hugu9T5vjQ4EueM4islS7JSke44qV4g1dBw1z8F4u9ztg5IgX2Sm4=/me");
        //    System.IO.File.Delete(i2Path); //-- see about this
        //    Console.ReadLine();
        //}

        /// <summary>
        /// Currently this is a test for sending autmated emails
        /// </summary>
        public static void sendEmail(string description, string emailTo, string QRLink)
        {
            SmtpClient smtpClient = new SmtpClient("robodb.hq.wsuniar.org", 25);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.EnableSsl = false;

            //generates the link
            var QRCode = GenerateMyQCCode(QRLink);

            MailMessage mailMessage = new MailMessage("nneely@niar.wichita.edu", emailTo);
            mailMessage.Subject = "Teayst";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = string.Format("{0}", description);
            mailMessage.Body = "<a href='"+ QRLink + "'><img src='C:\\Users\\nneely\\Desktop\\Teast Folder\\Test\\Teast\\bin\\Debug\\QRcode.png'/></a>";

            //adding the attachment
            //mailMessage.Attachments.Add(new Attachment("C:/Users/nneely/Desktop/Cheeze.png"));
            //mailMessage.Attachments.Add(new Attachment(i2Path));

            // mailMessage.AlternateViews.Add(ICSview);
            smtpClient.Send(mailMessage);
            Console.WriteLine("Sent Successfully");
        }

        public static System.Drawing.Bitmap GenerateMyQCCode(string QCText)
        {
            var Img = QRCodeWriter.CreateQrCode(QCText, 125, QRCodeWriter.QrErrorCorrectionLevel.Medium);
            Img.SaveAsPng(i2Path);

            var QRcode_1 = System.Drawing.Image.FromFile(i2Path);
            var QRcode_2 = new System.Drawing.Bitmap(QRcode_1);
            return QRcode_2;
        }

        public static System.Drawing.Image FromFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);
            return (System.Drawing.Bitmap)System.Drawing.Image.FromStream(ms);
        }
    }
}
