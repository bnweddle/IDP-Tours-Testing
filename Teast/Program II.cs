using System;
using System.Data.SqlClient;
//using System.Drawing;
using System.IO;
using System.Net.Mail;
using IronBarCode;
using System.Diagnostics;

namespace Test
{
    class Program
    {
        public static string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Touyr_Email_Trial;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static string i2Path = @"C:\Users\nneely\Desktop\Teast Folder\Test\Teast\bin\Debug\QRcode.png";
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
            mailMessage.Body = "<a href='http://www.google.com'><img src='C:\\Users\\nneely\\Desktop\\Teast Folder\\Test\\Teast\\bin\\Debug\\QRcode.png'/></a>";

            //adding the attachment
            mailMessage.Attachments.Add(new Attachment("C:/Users/nneely/Desktop/Cheeze.png"));
            mailMessage.Attachments.Add(new Attachment(i2Path));

            // mailMessage.AlternateViews.Add(ICSview);
            smtpClient.Send(mailMessage);
            Console.WriteLine("Sent Successfully");
        }

        public static System.Drawing.Bitmap GenerateMyQCCode(string QCText)
        {
            QRCodeWriter.CreateQrCode(QCText, 250, QRCodeWriter.QrErrorCorrectionLevel.Medium).SaveAsImage("QRcode.png");

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

        public void Query(string procedure)
        {
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                using(SqlCommand command = new SqlCommand(procedure, connection))
                {

                }
            }
        }
    }
}
