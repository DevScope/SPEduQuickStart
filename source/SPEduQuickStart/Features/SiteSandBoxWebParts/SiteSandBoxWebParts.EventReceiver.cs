using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;

namespace SPEduQuickStart.Features.SiteSandBoxWebParts
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("bdc3d17b-a8b8-45db-a249-10f3e90d8181")]
    public class SiteSandBoxWebPartsEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            using (SPSite site = properties.Feature.Parent as SPSite)
            {
                using (SPWeb web = site.OpenWeb())
                {
                    string urlFile = "/Lists/IA/EditForm.aspx";
                    string urlFile1 = "/Lists/IA/NewForm.aspx";
                    SPFile file = web.GetFile(urlFile);

                    using (SPLimitedWebPartManager webpartMgr = file.GetLimitedWebPartManager(PersonalizationScope.Shared))
                    {

                        CheckifExists(webpartMgr, "Jquery AutoComplete");
                        SPEduQuickStart.WebParts.AutocompleteSugestionTemplate.AutocompleteSugestionTemplate wp = new SPEduQuickStart.WebParts.AutocompleteSugestionTemplate.AutocompleteSugestionTemplate();
                        wp.Title = "Jquery AutoComplete";
                        webpartMgr.AddWebPart(wp, "Top", 1);

                    }
                    SPFile file2 = web.GetFile(urlFile1);
                    using (SPLimitedWebPartManager webpartMgr = file2.GetLimitedWebPartManager(PersonalizationScope.Shared))
                    {

                        CheckifExists(webpartMgr, "Jquery AutoComplete");
                        SPEduQuickStart.WebParts.AutocompleteSugestionTemplate.AutocompleteSugestionTemplate wp = new SPEduQuickStart.WebParts.AutocompleteSugestionTemplate.AutocompleteSugestionTemplate();
                        wp.Title = "Jquery AutoComplete";
                        webpartMgr.AddWebPart(wp, "Top", 1);

                    }
                }
            }

        }

        private static void CheckifExists(SPLimitedWebPartManager webpartMgr, string webPartTitle)
        {

            List<System.Web.UI.WebControls.WebParts.WebPart> toRemove = new List<System.Web.UI.WebControls.WebParts.WebPart>();
            foreach (System.Web.UI.WebControls.WebParts.WebPart part in webpartMgr.WebParts)
            {

                if (part.Title == webPartTitle)
                {
                    toRemove.Add(part);
                }

            }

            foreach (var item in toRemove)
            {
                webpartMgr.DeleteWebPart(item);
            }

        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
