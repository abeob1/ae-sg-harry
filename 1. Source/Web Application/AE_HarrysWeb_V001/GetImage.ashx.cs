using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Configuration;

namespace AE_HarrysWeb_V001
{
    /// <summary>
    /// Summary description for GetImage
    /// </summary>
    public class GetImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString["file"] != null)
            {
               
                string ImageFileLocation = Convert.ToString(WebConfigurationManager.AppSettings["ImageFileLocation"]);

                if (ImageFileLocation != null)
                {

                    string ImageFile = ImageFileLocation + context.Request.QueryString["file"];

                    if (File.Exists(ImageFile))
                    {
                        context.Response.ContentType = "image/jpg";
                        context.Response.AddHeader("content-disposition", "attachment; filename=" + ImageFile);
                        context.Response.TransmitFile(ImageFile);
                        context.Response.Flush();
                        context.Response.End();
                    }
                    else
                    {
                        context.Response.Write(context.Request.QueryString["file"].ToString() + " is not existed <br />");
                        context.Response.Write("<a href=\"Homepage.aspx\">Go Home</a>");

                    }
                }
            }
            }
       


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}