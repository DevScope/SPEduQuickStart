using System.Web.Services;
using Microsoft.SharePoint;


namespace SPEduQuickStart.Code
{
    public class SitesCreation : SPGenerateHelpers
    {
        #region Bussiness Rules


        /// <summary>
        /// Generate Content Structure based on IA list
        /// </summary>
        /// <returns></returns>
        
        [WebMethod]
        public static void Generate()
        {
            using (SPSite oSite = new SPSite(Domain))
            {
                using (SPWeb oWeb = oSite.OpenWeb())
                {
                    //Get the IA List in Root
                    SPList oList = oSite.RootWeb.Lists.TryGetList("IA");
                    //If is Null or no Items Return
                    if (oList == null || oList.Items.Count <= 0) return;
                    foreach (SPListItem oItem in oList.Items)
                    {
                        //url=CalculateFinalURL(row)  //calcula o url com base nos campos da lista
                        string url = CalculateFinalUrl(oItem);

                        //web=GetWeb(url)
                        //if not exists
                        //parentID=FindParentWebGuid(row)
                        string parentUrl = FindParentWeb(oItem);
                        if (!WebExists(url))
                        {
                            //parent=GetWeb(parentID)
                            try
                            {
                                using (SPWeb parent = SPContext.Current.Site.OpenWeb(parentUrl))
                                {

                                    //  web=parent.NewWeb(url, row.template)
                                    SPWeb web = parent.Webs.Add(oItem["Code"].ToString(),
                                             oItem["Title"].ToString(),
                                             oItem["SiteDescription"].ToString(),
                                             2070,
                                             GetTemplate(parent, oItem["Template"].ToString()),
                                             false,
                                             false);

                                    web.Update();
                                    //SetAnonymousAccessToAll(web, oItem, "c:0(.s|true");
                                    ProcessNavigation(web);
                                    web.Navigation.UseShared = true;
                                    web.Update();
                                    oItem["Url"] = url;
                                    oItem.Update();
                                }
                            }
                            catch
                            {
                                //oItem["Url"] = @"ERROR://ParentWebDoesNotExistYet-RE-RUN";
                                //oItem.Update();
                            }
                        }
                        //if (oWeb.IsRootWeb) return;
                        else
                        {
                            using (SPWeb web = SPContext.Current.Site.OpenWeb(parentUrl + oItem["Code"]))
                            {
                                if (url == "/") return;
                                web.Title = oItem["Title"].ToString();
                                web.Description = oItem["SiteDescription"].ToString();
                                web.Update();
                                //SetAnonymousAccessToAll(web, oItem, "c:0!.s|windows");
                                //SetAnonymousAccessToAll(web, oItem, "c:0(.s|true");
                                web.Update();

                            }
                        }
                    }
                    oWeb.Update();
                }
            }
        }

        #endregion
    }
}
