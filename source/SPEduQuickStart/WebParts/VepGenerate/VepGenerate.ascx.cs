using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using SPEduQuickStart.Code;
using System.Web.Services;

namespace SPEduQuickStart.WebParts.VepGenerate
{
    [ToolboxItemAttribute(false)]
    public partial class VepGenerate : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// BTNs the generate click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void BtnGenerateClick(object sender, EventArgs e)
        {
            string myScript = @"<script language='javascript'>alert({0});</script>";
            try
            {

                if (!SPGenerateHelpers.ListExists(SPContext.Current.Site.RootWeb, "IA"))
                {
                    bool setIa = SPGenerateHelpers.SetIa(SPContext.Current.Site.RootWeb);
                    bool setIaView = SPGenerateHelpers.CreateIaListView(SPContext.Current.Site.RootWeb);
                    if (setIa && setIaView)
                    {
                        LblErr.ForeColor = Color.DarkGreen;
                        LblErr.Text =
                            "A Lista de Tecnologia de Informação IA foi Gerada. Preencha a mesma conforme Documentação!...";
                        Literal ltrl = new Literal
                        {
                            Text =
                                "<script  language='javascript'>window.location.href='/Lists/IA/IA%20View.aspx';</script>"
                        };
                        Controls.Add(ltrl);
                    }
                }
                else
                {
                    LblErr.ForeColor = Color.DarkGreen;
                    SitesCreation.Generate();

                    LblErr.Text = "Procedimento Concluido!!...";
                    //Literal ltrl = new Literal
                    //{
                    //    Text =
                    //        "<script  language='javascript'>window.location.href='/';</script>"
                    //};
                    //Controls.Add(ltrl);
                }
            }
            catch (Exception ex)
            {
                if (Page.ClientScript.IsClientScriptBlockRegistered("DebugScript")) return;
                myScript = String.Format(myScript, ex);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "DebugScript", myScript);
            }
        }

        //public static void Gen()
        //{
        //    using (SPSite oSite = new SPSite(SPGenerateHelpers.Domain))
        //    {
        //        using (SPWeb oWeb = oSite.OpenWeb())
        //        {
        //            //Get the IA List in Root
        //            SPList oList = oSite.RootWeb.Lists.TryGetList("IA");
        //            //If is Null or no Items Return
        //            if (oList == null || oList.Items.Count <= 0) return;
        //            foreach (SPListItem oItem in oList.Items)
        //            {
        //                string url = SPGenerateHelpers.CalculateFinalUrl(oItem);
        //                string parentUrl = SPGenerateHelpers.FindParentWeb(oItem);
        //                if (!SPGenerateHelpers.WebExists(url))
        //                {
        //                    using (SPWeb parent = SPContext.Current.Site.OpenWeb(parentUrl))
        //                    {
        //                        SPWeb web = parent.Webs.Add(oItem["Code"].ToString(),
        //                                                    oItem["Title"].ToString(),
        //                                                    oItem["SiteDescription"].ToString(),
        //                                                    2070,
        //                                                    SPGenerateHelpers.GetTemplate(parent,
        //                                                                                  oItem["Template"].ToString
        //                                                                                      ()),
        //                                                    false,
        //                                                    false);

        //                        web.Update();
        //                        SPGenerateHelpers.ProcessNavigation(web);
        //                        web.Navigation.UseShared = true;
        //                        web.Update();
        //                        oItem["Url"] = url;
        //                        oItem.Update();
        //                    }

        //                }
        //                else
        //                {
        //                    using (SPWeb web = SPContext.Current.Site.OpenWeb(parentUrl + oItem["Code"]))
        //                    {
        //                        if (url == "/") return;
        //                        web.Title = oItem["Title"].ToString();
        //                        web.Description = oItem["SiteDescription"].ToString();
        //                        web.Update();
        //                    }
        //                }
        //            }
        //            oWeb.Update();
        //        }

        //    }
        //}

        protected override void RenderContents(HtmlTextWriter writer)
        {
            StringBuilder js = new StringBuilder();

            base.RenderContents(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, "/JS/SPEduQuickStart/jquery-1.8.1.min.js");


            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();

            //writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            //writer.AddAttribute(HtmlTextWriterAttribute.Type, "function setValue(value){document.getElementById('LblProgress').innerText = value;}");
            //writer.RenderBeginTag(HtmlTextWriterTag.Script);
            //writer.WriteLine(js.ToString());
            //writer.RenderEndTag();
        }
    }
}
