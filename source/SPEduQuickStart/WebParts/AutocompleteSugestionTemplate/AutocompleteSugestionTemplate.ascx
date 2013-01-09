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
    var fields = null;
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
            //Templates +=  siteTemplate.get_title()+ "-" + siteTemplate.get_name()  +'\n';
        }
    }

    function failed(sender, args) {
        alert("Failed when obtain internal Websites Templates.");
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
            this.fields = init_fields_v3();
            var id = $(this.fields['Template'].getElementsByTagName('input')).attr('id');

                $('#' + id).autocomplete({
                    source: availableTags
                });
        });
    }

    function init_fields_v3() {
        var toFind, res, myMatch, disp, fin, type
        res = {};
        toFind = "td.ms-formbody";
        if ($("td.ms-formbodysurvey").length > 0) {
            toFind = "td.ms-formbodysurvey";
        }
        $(toFind).each(function () {
            myMatch = $(this).html().match(/FieldName="(.+)"\s+FieldInternalName="(.+)"\s+FieldType="(.+)"\s+/);
            if (myMatch != null) {
                disp = myMatch[1];
                fin = myMatch[2];
                type = myMatch[3];
                if (type == 'SPFieldNote') {
                    if ($(this).find('script').length > 0) {
                        type = type + "_HTML";
                    } else if ($(this).find("div[id$='_TextField_inplacerte']").length > 0) {
                        type = type + "_EHTML";
                    }
                }
                if (type == 'SPFieldLookup') {
                    if ($(this).find('input').length > 0) {
                        type = type + "_Input";
                    }
                }
                res[fin] = this.parentNode;
                $(res[fin]).attr('FieldDispName', disp);
                $(res[fin]).attr('FieldType', type);
            }
        });
        return res;
    }
</script>

