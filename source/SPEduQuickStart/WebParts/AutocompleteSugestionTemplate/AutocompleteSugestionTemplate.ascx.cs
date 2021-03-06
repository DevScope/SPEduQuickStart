﻿using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace SPEduQuickStart.WebParts.AutocompleteSugestionTemplate
{
    [ToolboxItemAttribute(false)]
    public partial class AutocompleteSugestionTemplate : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public AutocompleteSugestionTemplate()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string css = "<link type='text/css' rel='stylesheet' href='/JS/SPEduQuickStart/jquery-ui.css' />";
            
            string script = @"<script type='text/javascript'>
                                    $(document).ready(function() { 
                                        this.fields = init_fields_v3();
                                        var id = $(this.fields['Template'].getElementsByTagName('input')).attr('id');
                                        $('#'+id).focus(function() {
                                         LoadAutoComplete();
                                        });
                                    });
                            </script>";

            string availableTags = @"<script type='text/javascript'>
                       var availableTags = [];
					</script>";

            ClientScriptManager scriptext = this.Page.ClientScript;
            scriptext.RegisterClientScriptBlock(this.Page.GetType(), "key2", HttpUtility.HtmlDecode(css), false);
            scriptext.RegisterClientScriptBlock(this.Page.GetType(), "key3", HttpUtility.HtmlDecode(availableTags), false);
            scriptext.RegisterStartupScript(this.Page.GetType(), "key4", HttpUtility.HtmlDecode(script), false);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            StringBuilder js = new StringBuilder();

            base.RenderContents(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, "/JS/SPEduQuickStart/jquery-1.8.1.min.js");


            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();


            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, "/JS/SPEduQuickStart/jquery-ui.js");


            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();

        }
    }
}
