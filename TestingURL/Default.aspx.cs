using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace TestingURL
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Debug.WriteLine("-01-");
                Teast.One_Time_Urls.Query_Add_Token("me");
                Debug.WriteLine("-02-");
            }

            Response.Redirect("https://localhost:44300/About/0/0");
        }
    }
}