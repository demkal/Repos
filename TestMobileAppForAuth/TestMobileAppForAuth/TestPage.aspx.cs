using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestMobileAppForAuth
{
    public partial class TestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAuthenticate_Click(object sender, EventArgs e)
        {
            var client_id = ConfigurationManager.AppSettings["instagram.clientid"].ToString();
            var redirect_uri = ConfigurationManager.AppSettings["instagram.redirecturi"].ToString();
            Response.Redirect("https://api.instagram.com/oauth/authorize/?client_id=" + client_id + "&redirect_uri=" + redirect_uri + "&response_type=code");
        }
    }
}