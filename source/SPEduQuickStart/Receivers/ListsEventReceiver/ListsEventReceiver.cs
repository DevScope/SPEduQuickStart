using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using SPEduQuickStart.Code;

namespace SPEduQuickStart.Receivers.ListsEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class ListsEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            //string newSiteUrl = string.Empty;
            //try
            //{
            //    SPWeb web = properties.OpenWeb();
            //    SPList list = web.Lists[properties.ListId];
            //    SPListItem currentItem = list.GetItemById(properties.ListItemId);

            //    Dictionary<SitesCreation.Params, string> parameters = SitesCreation.IdentifyParameterByList(list);

            //    // wanting to create a subsite beneath this site
            //    newSiteUrl = string.Format(parameters[SitesCreation.Params.SiteUrlFormat], "" + currentItem[parameters[SitesCreation.Params.ListFieldCode]]);
            //    string newSiteTitle = string.Format(parameters[SitesCreation.Params.SiteTitleFormat], "" + currentItem[parameters[SitesCreation.Params.ListFieldCode]],
            //        "" + currentItem[parameters[SitesCreation.Params.ListFieldName]]);

            //    SPWebTemplateCollection webTemplates = web.GetAvailableWebTemplates(2070, true);
            //    SPWebTemplate webTemplate = (from SPWebTemplate t
            //                                 in webTemplates
            //                                 where t.Title == parameters[SitesCreation.Params.WebTemplateName]
            //                                 select t).FirstOrDefault();

            //    //classWebTemplateName = "STS#1"; // sandbox debug only
            //    SPWeb newSite = web.Webs.Add(newSiteUrl, newSiteTitle, string.Format(parameters[SitesCreation.Params.DescriptionFormat], newSiteTitle),
            //        (uint)web.Locale.LCID, webTemplate, false, false);
            //    newSite.Navigation.UseShared = true;

            //    // lastly update the Class list to contain a link to the new site
            //    currentItem[parameters[SitesCreation.Params.ListFieldSiteUrl]] = string.Format(parameters[SitesCreation.Params.ListFieldSiteUrlFormat], newSite.Url);
            //    currentItem.Update();

            //}
            //catch (Exception ex)
            //{
            //    string output = string.Format("Failure creating site:{0}, Exception:{1}", newSiteUrl, ex);
            //}
            base.ItemAdded(properties);
        }

    }
}
