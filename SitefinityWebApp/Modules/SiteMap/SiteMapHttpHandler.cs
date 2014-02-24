using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml;
using SitefinityWebApp.Modules.SiteMap.Configuration;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data.Linq.Dynamic;
using Telerik.Sitefinity.Modules.Events;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.Modules.Blogs;
using Telerik.Sitefinity.News.Model;

namespace SitefinityWebApp.Modules.SiteMap
{
	public class SiteMapHttpHandler : IHttpHandler
	{
		// global properties
		protected SiteMapConfig config = Config.Get<SiteMapConfig>();
		protected string host;

		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		public void ProcessRequest(HttpContext context)
		{
			// prepare response type
			var response = context.Response;
			response.ContentType = "text/xml";

			// begin xml response
			var writer = new XmlTextWriter(response.OutputStream, Encoding.UTF8) { Formatting = Formatting.Indented };
			writer.WriteStartDocument();
			writer.WriteStartElement("urlset");
			writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
			writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");
			writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

			var vars = HttpContext.Current.Request.ServerVariables;
			string port;

			// parse content
			using (var api = App.WorkWith())
			{
				// append pages
				var pages = api.Pages().ThatArePublished().Where(p => p.ShowInNavigation == true && !p.IsBackend && p.NodeType == Telerik.Sitefinity.Pages.Model.NodeType.Standard).Get();
				foreach (var page in pages)
				{
					// build host
					var protocol = page.Page.RequireSsl ? "https://" : "http://";
					
					// append port
					port = page.Page.RequireSsl ? "443" : vars["SERVER_PORT"];
					if (port == "80" || port == "443")
						port = string.Empty;
					else
						port = string.Concat(":", port);

					var url = string.Concat(protocol, vars["SERVER_NAME"], port, VirtualPathUtility.ToAbsolute(page.GetFullUrl()));

					// append page to sitemap
					AppendUrl(writer, url, page.Page.LastModified);
				}

				// adjust host for content items (SSL generally not needed... future versions might support different modes)
				port = vars["SERVER_PORT"];
				if (port == "80")
					port = string.Empty;
				else
					port = string.Concat(":", port);
				host = "http://" + vars["SERVER_NAME"] + port;

				// append news
				AppendNews(writer);

				// append events
				AppendEvents(writer);

				// append blogs
				AppendBlogs(writer);
			}

			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();

			response.Flush();
			writer.Flush();
		}

		private void AppendNews(XmlTextWriter writer)
		{
			// ensure News module is active


			// add news from all providers
			foreach (var newsProvider in NewsManager.ProvidersCollection)
			{
				// check index settings for matching provider from configuration
				var newsConfig = config.ContentTypes[NewsModule.ModuleName].Pages.Elements.Where(e => e.ProviderName == newsProvider.Name).FirstOrDefault();
				if (newsConfig == null || !newsConfig.Include)
					continue;

				// append all news items from provider
				var news = App.Prepare().SetContentProvider(newsProvider.Name).WorkWith().NewsItems().Publihed().Get();
				foreach (var newsItem in news)
				{
					// build url
					var fullUrl = string.Format("{0}{1}{2}", host, VirtualPathUtility.ToAbsolute(newsConfig.DefaultPageUrl), newsItem.Urls.Where(u => u.RedirectToDefault == false).First().Url);

					// append to sitemap
					AppendUrl(writer, fullUrl, newsItem.LastModified);
				}
			}
		}
  
		private void AppendEvents(XmlTextWriter writer)
		{
			// append events
			foreach (var eventsProvider in EventsManager.ProvidersCollection)
			{
				// check index settings for matching provider from configuration
				var eventsConfig=config.ContentTypes[EventsModule.ModuleName].Pages.Elements.Where(e => e.ProviderName == eventsProvider.Name).FirstOrDefault();
				if (eventsConfig == null || !eventsConfig.Include)
					continue;

				// retrieve events from provider
				var events=App.Prepare().SetContentProvider(eventsProvider.Name).WorkWith().Events().Publihed().Get();
				foreach (var eventItem in events)
				{
					// build url
					var fullUrl=string.Format("{0}{1}{2}", host, VirtualPathUtility.ToAbsolute(eventsConfig.DefaultPageUrl), eventItem.Urls.Where(u => u.RedirectToDefault == false).First().Url);

					// append to sitemap
					AppendUrl(writer, fullUrl, eventItem.LastModified);
				}
			}
		}

		private void AppendBlogs(XmlTextWriter writer)
		{
			// append blogs
			foreach (var blogsProvider in BlogsManager.ProvidersCollection)
			{
				// get list of blogs in each provider
				var blogs = App.Prepare().SetContentProvider(blogsProvider.Name).WorkWith().Blogs().Get();
				foreach (var blog in blogs)
				{
					// check index settings for matching provider from configuration
					var blogsConfig = config.ContentTypes[BlogsModule.ModuleName].Pages.Elements.Where(e => e.Name == blog.UrlName).FirstOrDefault();
					if (blogsConfig == null || !blogsConfig.Include)
						continue;

					// append blog posts
					foreach (var blogPost in blog.BlogPosts().Where(p => p.Status == Telerik.Sitefinity.GenericContent.Model.ContentLifecycleStatus.Live))
					{
						// build url
						var fullUrl = string.Format("{0}{1}{2}", host, VirtualPathUtility.ToAbsolute(blogsConfig.DefaultPageUrl), blogPost.Urls.Where(u => u.RedirectToDefault == false).First().Url);

						// append to sitemap
						AppendUrl(writer, fullUrl, blogPost.LastModified);
					}
				}
			}
		}
  
		private void AppendUrl(XmlTextWriter writer, string fullUrl, DateTime LastModified)
		{
			// calculate change frequency
			string changeFreq = "monthly";
			var changeInterval = (DateTime.Now - LastModified).Days;

			if (changeInterval <= 1)
				changeFreq = "daily";
			else if (changeInterval <= 7 & changeInterval > 1)
				changeFreq = "daily";
			else if (changeInterval <= 30 & changeInterval > 7)
				changeFreq = "weekly";
			else if (changeInterval <= 30 & changeInterval > 365)
				changeFreq = "weekly";

			// append to sitemap
			writer.WriteStartElement("url");
			writer.WriteElementString("loc", fullUrl);
			writer.WriteElementString("lastmod", LastModified.ToString("yyyy-MM-ddThh:mm:sszzzz"));
			writer.WriteElementString("changefreq", changeFreq);
			writer.WriteElementString("priority", "0.5");
			writer.WriteEndElement();
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}