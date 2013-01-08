<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VepGenerate.ascx.cs" Inherits="SPEduQuickStart.WebParts.VepGenerate.VepGenerate" %>

<script type="text/javascript">
    var clientContext = null;
    var web = null;
    var strNotificationID;
    var li = "";
    var div;
    var lock = true;

    function showNotification() {
        strNotificationID = SP.UI.Notify.addNotification("AGUARDE: <font color='#AA0000'>A Processar Informação..</font>", true);
    }

    function removeNotification() {
        
        if (strNotificationID != null)
            SP.UI.Notify.removeNotification(strNotificationID);
    }

    function showErroNotification(descr) {

        var htmlerro = "<font color='red'>" + descr + "   </font>";
        SP.UI.Notify.addNotification(htmlerro, false);
    }

    
    function GenerateSites() {
        div = document.getElementById("lblProgress");
        ExecuteOrDelayUntilScriptLoaded(Initialize, "sp.js");
    }

    function Initialize() {
        clientContext = new SP.ClientContext.get_current();
        web = clientContext.get_web();
        
        clientContext.load(web);
        clientContext.executeQueryAsync(Function.createDelegate(this, this.onSuccess),
            Function.createDelegate(this, this.onFail));
    }
    function onSuccess(sender, args) {
        Load();
    }
    function onFail(sender, args) {
        showErroNotification('Failed to get list. Error:' + args.get_message() + '\n' + args.get_stackTrace() + '');
    }


    function Load() {
        
        var list = web.get_lists().getByTitle("IA");
        var camlQuery = new SP.CamlQuery();
        var q = '<View><RowLimit>1000</RowLimit></View>';
        camlQuery.set_viewXml(q);
        this.listItems = list.getItems(camlQuery);
        clientContext.load(listItems, 'Include(Title, SiteDescription,SiteTemplate,SiteTemplate_x003a_Code, Root, CurricularYear, Parent, Code, VisibleBy)');
        clientContext.executeQueryAsync(Function.createDelegate(this, this.onListItemsLoadSuccess),
        Function.createDelegate(this, this.onQueryFailed));


    }

    function onListItemsLoadSuccess(sender, args) {
       

        var listEnumerator = this.listItems.getEnumerator();
        while (listEnumerator.moveNext()) {
                var item = listEnumerator.get_current();
                GenerateWeb(item);
        }  
       
        
    }

    function onQueryFailed(sender, args) {
         showErroNotification('REQUEST FAILED: ' + args.get_message() + '\n' + args.get_stackTrace() + '');
    }


    function CalculateFinalUrl(item) {
        var url = "";
        if (item.get_item("Root") != null)
            { url += item.get_item("Root"); }
        if (item.get_item("CurricularYear") != null)
            { url += "/" + item.get_item("CurricularYear"); }
        if (item.get_item("Parent") != null)
            { url += "/" + item.get_item("Parent"); }
        if (url != "") {
            return url + "/" + item.get_item("Code");
        }
        else {
            return ""+ item.get_item("Code");
        }
    }
    

    function GenerateWeb(item) {
        //debugger;
        if (this.lock) {
            //removeNotification();
            var webCreateInfo = new SP.WebCreationInformation();
            webCreateInfo.set_description(item.get_item("SiteDescription"));
            var languageId = web.get_language();
            webCreateInfo.set_language(languageId);
            webCreateInfo.set_title(item.get_item("Title"));
            var url = CalculateFinalUrl(item);
            webCreateInfo.set_url(url);
            webCreateInfo.set_useSamePermissionsAsParentSite(true);
            var templatecode = item.get_item("SiteTemplate_x003a_Code").get_lookupValue();
            webCreateInfo.set_webTemplate(templatecode);
            CreateWebsite(webCreateInfo);
        }
        else {
            setTimeout(function () {
                //showNotification();
                GenerateWeb(item);
            }, 1000);
        }
    }

    function CreateWebsite(webCreateInfo) {
        this.lock = false;
        this.oNewWebsite =this.web.get_webs().add(webCreateInfo);
        clientContext.load(this.oNewWebsite, 'ServerRelativeUrl', 'Created');
        clientContext.executeQueryAsync(Function.createDelegate(this, this.onCreateWebSuccess), Function.createDelegate(this, this.onQueryFailed));
        
    }

    function onCreateWebSuccess(sender, args) {
        var html = "<p>Foi criado com sucesso o site <a href='" + location.protocol + "//" + location.host + '' + this.oNewWebsite.get_serverRelativeUrl() + "' >" + this.oNewWebsite.get_title() + "</a></p><br/>";
        var myDiv = document.getElementById("lblProgress");
        myDiv.innerHTML += html;
        this.lock = true;
    }

    function onQueryFailed(sender, args) {
        showErroNotification('REQUEST FAILED: ' + args.get_message() + '\n' + args.get_stackTrace() + '');
    }




    function GetWebTemplates() {

        var context = new SP.ClientContext.get_current();

        var web = context.get_web();
        //var languageId = web.get_language();
        //alert(languageId);
        //this.templateCollection = web.getAvailableWebTemplates(languageId, false);
        this.templateCollection = web.getAvailableWebTemplates(2070, false);

        context.load(templateCollection);

        context.executeQueryAsync(Function.createDelegate(this, this.success), Function.createDelegate(this, this.failed));
    }

    function success() {

        var Templates = "";

        var siteTemplatesEnum = templateCollection.getEnumerator();

        while (siteTemplatesEnum.moveNext()) {

            var siteTemplate = siteTemplatesEnum.get_current();

            Templates += siteTemplate.get_title() + "-" + siteTemplate.get_name()  +'\n';

        }

        alert("Site Templates - " + '\n' + Templates);

    }

    function failed(sender, args) {
        alert("Failed");
    }

    function GenerateTemplates() {

        ExecuteOrDelayUntilScriptLoaded(GetWebTemplates, "sp.js");
    }

</script>
​
<br />
<br />
<a href="#" onclick="javascript:GenerateTemplates();">Get Web Templates</a>
<br />
<a href="#" onclick="javascript:GenerateSites();">Click Here for Generate Sites Structures</a>
<br />
<asp:LinkButton ID="BtnGen" runat="server" OnClick="BtnGenerateClick" Text="Gen" Visible="true"></asp:LinkButton>
<br />
<br />
<br />
<div id="lblProgress"></div>
<asp:Label id="LblErr" runat="server"></asp:Label>
