using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace SitefinityWebApp.Modules.SiteMap
{
	/// <summary>
	/// Route Handler for serving the ~/sitemap.xml file path
	/// </summary>
	public class SiteMapRouteHandler : IRouteHandler
	{
		/// <summary>
		/// Provides the object that processes the request.
		/// </summary>
		/// <param name="requestContext">An object that encapsulates information about the request.</param>
		/// <returns>
		/// An object that processes the request.
		/// </returns>
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			// simply return the SiteMap Handler
			return new SiteMapHttpHandler();
		}
	}
}