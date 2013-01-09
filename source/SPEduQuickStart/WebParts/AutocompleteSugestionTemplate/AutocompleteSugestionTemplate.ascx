<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutocompleteSugestionTemplate.ascx.cs" Inherits="SPEduQuickStart.WebParts.AutocompleteSugestionTemplate.AutocompleteSugestionTemplate" %>
<script type="text/javascript">
    var clientContext = null;
    var web = null;
  
    function GetWebTemplates() {

        var context = new SP.ClientContext.get_current();

        var website = clientContext.get_web();
        var languageId = web.get_language();
         this.templateCollection = web.getAvailableWebTemplates(languageId, false);
         clientContext.load(templateCollection);

         clientContext.executeQueryAsync(Function.createDelegate(this, this.success), Function.createDelegate(this, this.failed));
    }

    function success() {

        var Templates = "";

        var siteTemplatesEnum = templateCollection.getEnumerator();

        this.availableTags = [];

        while (siteTemplatesEnum.moveNext()) {

            var siteTemplate = siteTemplatesEnum.get_current();
            availableTags.push(siteTemplate.get_title())
            //Templates +=  + "-" + siteTemplate.get_name()  +'\n';

        }

        //alert("Site Templates - " + '\n' + Templates);

    }

    function failed(sender, args) {
        alert("Failed");
    }



    function Initialize() {
        clientContext = new SP.ClientContext.get_current();
        web = clientContext.get_web();

        clientContext.load(web);
        clientContext.executeQueryAsync(Function.createDelegate(this, this.onSuccess),
            Function.createDelegate(this, this.onFail));
    }
    function onSuccess(sender, args) {
        GetWebTemplates();
    }
    function onFail(sender, args) {
        showErroNotification('Failed to get list. Error:' + args.get_message() + '\n' + args.get_stackTrace() + '');
    }


    ExecuteOrDelayUntilScriptLoaded(Initialize, "sp.js");
    function LoadAutoComplete() {
        $(document).ready(function () {
            

                $('#ctl00_m_g_053f256c_889e_4cf4_b980_838ed1fcc5a6_ctl00_ctl05_ctl03_ctl00_ctl00_ctl04_ctl00_ctl00_TextField').autocomplete({
                    source: availableTags
                });

                $('#ctl00_m_g_101deaa4_29e0_4d62_9440_bfed1295f62e_ctl00_ctl05_ctl03_ctl00_ctl00_ctl04_ctl00_ctl00_TextField').autocomplete({
                    source: availableTags
                });
        });
    }
</script>