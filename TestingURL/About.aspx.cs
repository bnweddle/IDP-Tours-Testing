using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestingURL
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string Token = "s8ZF07hMnCht23P/ZR0E4146jCgYFOYHFlkjmxhPrYQ/5PNHIW";
            //string user = "me";

            string Token = Page.RouteData.Values["Token"] as string;
            string User = Page.RouteData.Values["UserName"] as string;

            int Varification = Convert.ToInt32(Teast.One_Time_Urls.Query_Submit_Token(Token, User));

            if (Varification != 1)
                uxHeader.InnerText = "404";
            else
                uxHeader.InnerText = "Success!";

            if (!IsPostBack)
            {
                
            }
        }
    }
}