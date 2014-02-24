using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Modules.Pages;
using SitefinityWebApp.Modules.SiteMap.Configuration;
using Telerik.Sitefinity.Modules.Events;
using Telerik.Sitefinity.Modules.Events.Web.UI;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.Modules.News.Web.UI;
using Telerik.Sitefinity.Web.UrlEvaluation;
using System.Web.Routing;

namespace SitefinityWebApp.Modules.SiteMap
{
	/// <summary>
	/// Module for generating an XML Sitemap, including all pages and Sitefinity content types (Currently only supports News, Events, Blogs)
	/// </summary>
	public class SiteMapModule : ModuleBase
	{
		/// <summary>
		/// Initializes the service with specified settings.
		/// </summary>
		/// <param name="settings">The settings.</param>
		public override void Initialize(ModuleSettings settings)
		{
			base.Initialize(settings);
			Config.RegisterSection<SiteMapConfig>();

			// handle the sitemap path
			RouteTable.Routes.Add("SiteMap", new Route("sitemap.xml", new SiteMapRouteHandler()));
		}

		/// <summary>
		/// Installs this module in Sitefinity system for the first time.
		/// </summary>
		/// <param name="initializer">The Site Initializer. A helper class for installing Sitefinity modules.</param>
		public override void Install(SiteInitializer initializer)
		{
			// there is nothing to install; the module runs via the SiteMapHttpHandler
		}

		/// <summary>
		/// Upgrades this module from the specified version.
		/// </summary>
		/// <param name="initializer">The Site Initializer. A helper class for installing Sitefinity modules.</param>
		/// <param name="upgradeFrom">The version this module us upgrading from.</param>
		public override void Upgrade(SiteInitializer initializer, Version upgradeFrom)
		{
			// not needed
		}

		/// <summary>
		/// Gets the module config.
		/// </summary>
		/// <returns></returns>
		protected override ConfigSection GetModuleConfig()
		{
			return Config.Get<SiteMapConfig>();
		}

		/// <summary>
		/// Gets the landing page id for each module inherit from <see cref="T:Telerik.Sitefinity.Services.SecuredModuleBase"/> class.
		/// </summary>
		/// <value>
		/// The landing page id.
		/// </value>
		public override Guid LandingPageId
		{
			get { return new Guid("B200C729-A259-445F-B588-75B14F14F34B"); }
		}

		/// <summary>
		/// Gets the CLR types of all data managers provided by this module.
		/// </summary>
		/// <value>
		/// An array of <see cref="T:System.Type"/> objects.
		/// </value>
		public override Type[] Managers
		{
			get { return null; }
		}

		public static string ModuleName = "SiteMap";
	}
}