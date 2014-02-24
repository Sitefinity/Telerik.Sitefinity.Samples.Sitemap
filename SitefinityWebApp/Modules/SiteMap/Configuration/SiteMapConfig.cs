using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Modules.GenericContent.Configuration;
using System.Configuration;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Web.Configuration;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Events;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Modules.Events.Web.UI;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.Modules.News.Web.UI;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.Modules.Blogs.Web.UI;
using Telerik.Sitefinity.Services;

namespace SitefinityWebApp.Modules.SiteMap.Configuration
{
    /// <summary>
    /// Configuration class for the SiteMap Module
    /// </summary>
    public class SiteMapConfig : ConfigSection
    {
        /// <summary>
        /// The collection of Sitefinity Content Types to be indexed and their associated settings.
        /// </summary>
        [ConfigurationProperty("ContentTypes"), ConfigurationCollection(typeof(ContentType), AddItemName = "ContentType")]
        public ConfigElementDictionary<string, ContentType> ContentTypes
        {
            get { return (ConfigElementDictionary<string, ContentType>)this["ContentTypes"]; }
        }



        protected void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs args)
        {
            if (args.CommandName == "Bootstrapped")
            {

            }
        }

        private void CreateSampleWorker(object[] args)
        {
            #region News

            // ensure news config exists
            if (!this.ContentTypes.ContainsKey(NewsModule.ModuleName))
            {
                var contentType = new ContentType(this.ContentTypes);
                contentType.Name = NewsModule.ModuleName;
                contentType.Pages = new ConfigElementList<ContentPage>(contentType);
                this.ContentTypes.Add(contentType);
            }

            // ensure news pages config exist
            var newsConfig = this.ContentTypes[NewsModule.ModuleName];
            if (newsConfig.Pages.Count == 0)
            {
                // add one setting for each provider
                var providers = NewsManager.GetManager().Providers;
                foreach (var provider in providers)
                {
                    // retrieve all pages with a NewsView on them
                    var NewsPages = App.WorkWith().Pages()
                        .Where(p => p.Page != null && p.ShowInNavigation && p.Page.Controls.Where(c => c.ObjectType.StartsWith(typeof(NewsView).FullName)).Count() > 0)
                        .Get();

                    // attempt to locate the default page
                    foreach (var page in NewsPages)
                    {
                        string providerName;

                        // retrieve the news view control from the page
                        var newsView = page.Page.Controls.Where(c => c.ObjectType.StartsWith(typeof(NewsView).FullName)).First();
                        if (newsView == null) continue;

                        // determine if there is a modified control definition
                        var controlDefinition = newsView.Properties.Where(p => p.Name == "ControlDefinition").FirstOrDefault();
                        if (controlDefinition == null)
                            // control uses default provider
                            providerName = NewsManager.GetDefaultProviderName();
                        else
                        {
                            // search for modified provider name in the control
                            var providerNameProperty = controlDefinition.ChildProperties.Where(p => p.Name == "ProviderName").FirstOrDefault();
                            if (providerNameProperty == null)
                                // control is modified, but still uses default provider
                                providerName = NewsManager.GetDefaultProviderName();
                            else
                                // get selected provider name
                                providerName = providerNameProperty.Value;
                        }

                        // make sure specified provider matches the current provider we are working with
                        if (providerName != provider.Name) continue;

                        // make sure the mode of the control is not Details-only, skip this page if it is
                        var displayModeProperty = newsView.Properties.Where(p => p.Name == "ContentViewDisplayMode").SingleOrDefault();
                        if (displayModeProperty != null && displayModeProperty.Value == Telerik.Sitefinity.Web.UI.ContentUI.Enums.ContentViewDisplayMode.Detail.ToString()) continue;

                        // save default news page for this provider to the config
                        var newsPage = new ContentPage(newsConfig.Pages);
                        newsPage.DefaultPageUrl = page.GetFullUrl();
                        newsPage.Name = providerName;
                        newsPage.Include = true;
                        newsPage.ProviderName = providerName;
                        newsConfig.Pages.Add(newsPage);

                        // stop search for a matching news page, move to next provider (if any)
                        break;
                    }
                }
            }

            #endregion

            #region Events

            // ensure events config exists
            if (!this.ContentTypes.ContainsKey(EventsModule.ModuleName))
            {
                var contentType = new ContentType(this.ContentTypes);
                contentType.Name = EventsModule.ModuleName;
                contentType.Pages = new ConfigElementList<ContentPage>(contentType);
                this.ContentTypes.Add(contentType);
            }

            // ensure events pages config exists
            var eventsConfig = this.ContentTypes[EventsModule.ModuleName];
            if (eventsConfig.Pages.Count == 0)
            {
                // add one setting for each provider
                var providers = EventsManager.GetManager().Providers;
                foreach (var provider in providers)
                {
                    // retrieve all pages that contain an EventsView
                    var eventsPages = App.WorkWith().Pages()
                        .Where(p => p.Page != null && p.ShowInNavigation && p.Page.Controls.Where(c => c.ObjectType.StartsWith(typeof(EventsView).FullName)).Count() > 0)
                        .Get();

                    // attempt to locate the default page
                    foreach (var page in eventsPages)
                    {
                        string providerName;

                        // retrieve the events view control
                        var eventsView = page.Page.Controls.Where(c => c.ObjectType.StartsWith(typeof(EventsView).FullName)).First();
                        if (eventsView == null) continue;

                        // determine if there is a modified control definition
                        var controlDefinition = eventsView.Properties.Where(p => p.Name == "ControlDefinition").FirstOrDefault();
                        if (controlDefinition == null)
                            // control uses default provider
                            providerName = EventsManager.GetDefaultProviderName();
                        else
                        {
                            // search for modified provider name
                            var providerNameProperty = controlDefinition.ChildProperties.Where(p => p.Name == "ProviderName").FirstOrDefault();
                            if (providerNameProperty == null)
                                // control is modified but still uses default provider
                                providerName = EventsManager.GetDefaultProviderName();
                            else
                                // get custom provider name
                                providerName = providerNameProperty.Value;
                        }

                        // make sure specified provider matches the current provider
                        if (providerName != provider.Name) continue;

                        // skip page if it is a details-mode page
                        var displayModeProperty = eventsView.Properties.Where(p => p.Name == "ContentViewDisplayMode").SingleOrDefault();
                        if (displayModeProperty != null && displayModeProperty.Value == Telerik.Sitefinity.Web.UI.ContentUI.Enums.ContentViewDisplayMode.Detail.ToString()) continue;

                        // save page to config
                        var eventsPage = new ContentPage(eventsConfig.Pages);
                        eventsPage.DefaultPageUrl = page.GetFullUrl();
                        eventsPage.Name = providerName;
                        eventsPage.Include = true;
                        eventsPage.ProviderName = providerName;
                        eventsConfig.Pages.Add(eventsPage);

                        // stop search for default page, move to next provider (if any)
                        break;
                    }
                }
            }

            #endregion

            #region Blogs

            // ensure blog config exists
            if (!this.ContentTypes.ContainsKey(BlogsModule.ModuleName))
            {
                var contentType = new ContentType(this.ContentTypes);
                contentType.Name = BlogsModule.ModuleName;
                contentType.Pages = new ConfigElementList<ContentPage>(contentType);
                this.ContentTypes.Add(contentType);
            }

            // ensure blogs pages config exist
            var blogsConfig = this.ContentTypes[BlogsModule.ModuleName];
            if (blogsConfig.Pages.Count == 0)
            {
                // add a config setting for each provider
                var providers = BlogsManager.GetManager().Providers;
                foreach (var provider in providers)
                {
                    // retrieve all in the provider
                    var blogs = App.Prepare().SetContentProvider(provider.Name).WorkWith().Blogs().Get();

                    // add a config setting for each blog
                    foreach (var blog in blogs)
                    {
                        // make sure default page is set
                        if (!blog.DefaultPageId.HasValue || blog.DefaultPageId.Value == Guid.Empty) continue;

                        // get default page url
                        var defaultPage = App.WorkWith().Page(blog.DefaultPageId.Value).Get();
                        if (defaultPage == null) continue;

                        // save default blog page to config
                        var blogPage = new ContentPage(blogsConfig.Pages);
                        blogPage.DefaultPageUrl = defaultPage.GetFullUrl();
                        blogPage.Name = blog.UrlName;
                        blogPage.Include = true;
                        blogPage.ProviderName = provider.Name;
                        blogsConfig.Pages.Add(blogPage);
                    }
                }
            }

            #endregion
        }
        /// <summary>
        /// Called after the properties of this instance have been initialized.
        /// Load default values here.
        /// </summary>
        protected override void OnPropertiesInitialized()
        {
            base.OnPropertiesInitialized();

            SystemManager.RunWithElevatedPrivilegeDelegate worker = new SystemManager.RunWithElevatedPrivilegeDelegate(CreateSampleWorker);
            SystemManager.RunWithElevatedPrivilege(worker);
        }
    }
}