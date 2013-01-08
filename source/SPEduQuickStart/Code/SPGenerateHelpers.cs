using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;

namespace SPEduQuickStart.Code
{
    public class SPGenerateHelpers
    {
        #region Vars and Constants

        //private const string rooSite = "Content";

        public static readonly string Domain =
            HttpContext.Current.Request.Url.Scheme + Uri.SchemeDelimiter +
            HttpContext.Current.Request.Url.Host +
            (HttpContext.Current.Request.Url.IsDefaultPort
                 ? ""
                 : ":" + HttpContext.Current.Request.Url.Port);

        public string[] AnonymousListTeamSite = new[] 
                { "Calendário", "Anúncios", "Debate de Equipa", 
                    "Hiperligações", "Tarefas", "Páginas do Site" };

        #endregion

        #region Static Methods

        /// <summary>
        /// Finds the parent web.
        /// </summary>
        /// <param name="oItem">The o item.</param>
        /// <returns></returns>
        public static string FindParentWeb(SPItem oItem)
        {
            return VirtualPathUtility.AppendTrailingSlash(
                VirtualPathUtility.Combine(CalculateFinalUrl(oItem), ".."));
        }

        /// <summary>
        /// SPEduQuickStart Information Architecture Strategy
        /// </summary>
        /// <param name="oItem">List Item with each configuration for the new sub-site</param>
        /// <returns></returns>
        public static string CalculateFinalUrl(SPItem oItem)
        {
            //string url = SPContext.Current.Site.Url;  //Assert, should be root site
            string url = "/";  //Assert, should be root site

            if (!String.IsNullOrEmpty(Convert.ToString(oItem["Root"])))
                url = VirtualPathUtility.Combine(url, oItem["Root"] + "/");

            if (!String.IsNullOrEmpty(Convert.ToString(oItem["CurricularYear"])))
                url = VirtualPathUtility.Combine(url, oItem["CurricularYear"] + "/");

            if (!String.IsNullOrEmpty(Convert.ToString(oItem["Parent"])))
                url = VirtualPathUtility.Combine(url, oItem["Parent"] + "/");

            if (!String.IsNullOrEmpty(Convert.ToString(oItem["Code"])))
                url = VirtualPathUtility.Combine(url, oItem["Code"] + "/");

            return VirtualPathUtility.AppendTrailingSlash(url);
        }

        /// <summary>
        /// SPEduQuickStart Information Architecture Navigation
        /// </summary>
        /// <param name="web"> </param>
        /// <returns></returns>
        public static void ProcessNavigation(SPWeb web)
        {

            // Let the subsite use the parent site's top link bar.
            web.Navigation.UseShared = true;

            // Get a collection of navigation nodes.
            SPNavigationNodeCollection nodes = web.ParentWeb.Navigation.UseShared ?
                web.ParentWeb.Navigation.QuickLaunch : web.ParentWeb.Navigation.TopNavigationBar;

            // Check for an existing link to the subsite.
            SPNavigationNode node = nodes
                .Cast<SPNavigationNode>()
                .FirstOrDefault(n => n.Url.Equals(web.ServerRelativeUrl));

            // No link, so add one.
            if (node != null) return;
            // Create the node.
            node = new SPNavigationNode(web.Title, web.ServerRelativeUrl);

            // Add it to the collection.
            nodes.AddAsLast(node);
        }
        
        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="templateTitle">The template title.</param>
        /// <returns></returns>
        public static SPWebTemplate GetTemplate(SPWeb web, string templateTitle)
        {
            SPWebTemplateCollection webTemplates = web.GetAvailableWebTemplates(2070, true);
            SPWebTemplate webTemplate = (from SPWebTemplate t
                                         in webTemplates
                                         where t.Title == templateTitle
                                         select t).FirstOrDefault();
            return webTemplate;
        }

        /// <summary>
        /// Create Information Architecture List from Columns
        /// </summary>
        /// <param name="web"></param>
        /// <returns>True or False</returns>
        public static bool SetIa(SPWeb web)
        {
            try
            {

                Guid gIa = web.Lists.Add("IA",
                    "List for Information Architecture (Be careful when delete)",
                        SPListTemplateType.CustomGrid);

                SPList lgIa = web.Lists[gIa];

                SPField root = web.Site.RootWeb.Fields.GetFieldByInternalName("Root");
                lgIa.Fields.Add(root);
                SPField curricularYear = web.Site.RootWeb.Fields.GetFieldByInternalName("CurricularYear");
                lgIa.Fields.Add(curricularYear);
                SPField template = web.Site.RootWeb.Fields.GetFieldByInternalName("Template");
                lgIa.Fields.Add(template);
                SPField siteTitle = web.Site.RootWeb.Fields.GetFieldByInternalName("SiteDescription");
                lgIa.Fields.Add(siteTitle);
                SPField code = web.Site.RootWeb.Fields.GetFieldByInternalName("Code");
                lgIa.Fields.Add(code);
                SPField parent = web.Site.RootWeb.Fields.GetFieldByInternalName("Parent");
                lgIa.Fields.Add(parent);
                SPField visibleBy = web.Site.RootWeb.Fields.GetFieldByInternalName("VisibleBy");
                lgIa.Fields.Add(visibleBy);
                SPField url = web.Site.RootWeb.Fields.GetFieldByInternalName("Url");
                lgIa.Fields.Add(url);

                lgIa.Title = "IA";
                lgIa.Update();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Create Information Architecture Grid View
        /// In order to use Grid Views in Office 365
        /// must have installed Office in the Client PC
        /// </summary>
        /// <param name="web"></param>
        /// <returns>True or False</returns>
        public static bool CreateIaListView(SPWeb web)
        {
            try
            {
                SPList list = web.Lists["IA"];

                StringCollection strColl =
                    new StringCollection 
                        { "Title", "SiteDescription", "Template",  
                            "Root", "CurricularYear", "Parent", "Code", "VisibleBy", "Url" };
                list.Views.Add("IA View", strColl, @"", 100, true, true, SPViewCollection.SPViewType.Grid, false);
                list.Update();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// If Web exist when not using SPWeb
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True or False</returns>
        public static bool WebExists(string path)
        {
            try
            {
                using (SPWeb web = SPContext.Current.Site.OpenWeb(path))
                {
                    return web.Exists;

                    // web.Exists = false;
                    // web.Title > FileNotFoundException!!!
                }
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }


        /// <summary>
        /// Sets the anonymous access to all authenticated.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="oItem">The o item.</param>
        /// <param name="user">The user.</param>
        /// <exception cref="System.Exception"></exception>
        public static void SetAnonymousAccessToAll(SPWeb web, SPItem oItem, string user)
        {
            if (web.Url == "/") return;
            switch (oItem["VisibleBy"].ToString())
            {
                //SharepointAdminsOnly
                case "AuthenticatedUsers":
                    // check if it has unique permissions
                    if (!web.HasUniqueRoleAssignments)
                    {
                        //This method breaks the role assignment inheritance 
                        //for the list item, and creates unique role assignments 
                        //for the list item with the copyRoleAssignments parameter 
                        //which specifies whether to copy role assignments from the 
                        //parent object and with the clearSubscopes parameter which 
                        //specifies whether to clear role assignments from child objects.
                        web.BreakRoleInheritance(true, false);
                    }
                    switch (DetectSiteLanguage(web))
                    {
                        //pt-PT
                        case 2070:
                            web.Groups["Visualizadores"].Users.Add(user, null, null, null);
                            break;
                        //eng-USA
                        case 1033:
                            web.Groups["Viewers"].Users.Add(user, null, null, null);
                            break;
                        default:
                            throw new Exception("Not Supported Language!");
                    }
                    break;
            }
        }

        /// <summary>
        /// Detects the site language.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <returns></returns>
        public static uint DetectSiteLanguage(SPWeb web)
        {
            return web.Language;
        }

        /// <summary>
        /// Sets the masterpage.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="masterPageUrl">The master page URL.</param>
        public static void SetMasterpage(SPWeb web, string masterPageUrl)
        {
            Uri masterPageUri = new Uri(String.Concat(web.Site.RootWeb.Url, masterPageUrl));

            web.MasterUrl = masterPageUri.AbsolutePath;

        }

        /// <summary>
        /// Sets the custom masterpage.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="masterPageUrl">The master page URL.</param>
        public static void SetCustomMasterpage(SPWeb web, string masterPageUrl)
        {
            Uri masterPageUri = new Uri(String.Concat(web.Site.RootWeb.Url, masterPageUrl));

            web.CustomMasterUrl = masterPageUri.AbsolutePath;
        }

        /// <summary>
        /// Lists exists.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="listName">Name of the list.</param>
        /// <returns></returns>
        public static bool ListExists(SPWeb web, string listName)
        {
            return web.Lists.Cast<SPList>().Any(list => String.Equals(list.Title, listName));
        }

        /// <summary>
        /// Cleans up quick launch.
        /// </summary>
        /// <param name="header">The header.</param>
        public static void CleanUpQuickLaunch(string header)
        {
            using (SPSite site = SPContext.Current.Site)
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPNavigationNodeCollection quickLaunch = web.Navigation.QuickLaunch;

                    // try to get quick launch header
                    SPNavigationNode nodeHeader = quickLaunch.Cast<SPNavigationNode>().FirstOrDefault(n =>
                        n.Title == header);

                    //if header not found remove it
                    if (nodeHeader != null)
                    {
                        quickLaunch.Delete(nodeHeader);
                    }

                }
            }
        }

        /// <summary>
        /// Cleans up top navigation bar.
        /// </summary>
        /// <param name="header">The header.</param>
        public static void CleanUpTopNavigationBar(string header)
        {
            using (SPSite site = SPContext.Current.Site)
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPNavigationNodeCollection topNavigation = web.Navigation.TopNavigationBar;

                    // try to get quick launch header
                    SPNavigationNode nodeHeader = topNavigation.Cast<SPNavigationNode>().FirstOrDefault(n =>
                        n.Title == header);

                    //if header not found remove it
                    if (nodeHeader != null)
                    {
                        topNavigation.Delete(nodeHeader);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the top nav nodes.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="title">The title.</param>
        /// <param name="url">The URL.</param>
        /// <param name="parentUrl">The parent URL.</param>
        public static void ProcessTopNavNodes(SPWeb web, string title, string url, string parentUrl)
        {
            if (parentUrl == "/")
            {
                return;
            }
            web.Navigation.UseShared = true;
            SPNavigationNode parentNode = web.Navigation.GetNodeByUrl(parentUrl);
            SPNavigationNode node = new SPNavigationNode(title, url);
          
            parentNode.Children.AddAsLast(node);
            node.Update();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Top domain if not using Context
        /// </summary>
        public readonly string TopDomain =
            HttpContext.Current.Request.Url.Scheme + Uri.SchemeDelimiter +
            HttpContext.Current.Request.Url.Host +
            (HttpContext.Current.Request.Url.IsDefaultPort
                 ? ""
                 : ":" + HttpContext.Current.Request.Url.Port);


        /// <summary>
        /// Sets the public access to lists.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="oItem">The o item.</param>
        /// <param name="lists">The lists.</param>
        public void SetPublicAccessToLists(SPWeb web, SPItem oItem, IEnumerable<string> lists)
        {
            switch (oItem["VisibleBy"].ToString())
            {
                case "Anonymous":
                    foreach (SPList list in lists.Select(item => web.Lists[item]))
                    {
                        try
                        {
                            // check if it has unique permissions
                            if (!list.HasUniqueRoleAssignments)
                            {
                                list.BreakRoleInheritance(true);
                            }

                            // make sure people can edit their own items
                            list.WriteSecurity = 2;

                            // grant permissions to anonymous users
                            list.AnonymousPermMask64 =
                                (SPBasePermissions.Open |
                                 SPBasePermissions.OpenItems |
                                 SPBasePermissions.ViewFormPages |
                                 SPBasePermissions.ViewListItems |
                                 SPBasePermissions.AddListItems);

                            list.Update();
                        }
                        catch
                        {
                            continue;
                        }

                    }
                    break;
            }
        }



        #endregion
    }
}
