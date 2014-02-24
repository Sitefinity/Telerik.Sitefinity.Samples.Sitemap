using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Configuration;
using System.Configuration;

namespace SitefinityWebApp.Modules.SiteMap.Configuration
{
	/// <summary>
	/// Configuration Element for storing the default list page for a Sitefinity Content Type
	/// </summary>
	public class ContentPage : ConfigElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContentPage"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public ContentPage(ConfigElement parent) : base(parent) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ContentPage"/> class.
		/// </summary>
		/// <param name="parent">The parent config element.</param>
		/// <param name="ProviderName">Name of the Sitefinity Content Type Provider.</param>
		/// <param name="PageID">The Guid of the Sitefinity Page ID.</param>
		public ContentPage(ConfigElement parent, string ProviderName, string PageID) : base(parent)
		{
			this.ProviderName = ProviderName;
			this.DefaultPageUrl = PageID;
		}

		/// <summary>
		/// For flat content types (everything except blogs) this is the same as the ProviderName.
		/// For Blogs, this is the Url-Name for the blog.
		/// </summary>
		/// <value>
		/// The Content Provider name (or Url-Name for a blog).
		/// </value>
		[ConfigurationProperty("Name", Options = ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired, DefaultValue = "")]
		public string Name
		{
			get { return (string)base["Name"]; }
			set { base["Name"] = value; }
		}

		/// <summary>
		/// Gets or sets the name of the Sitefinity content type provider.
		/// </summary>
		/// <value>
		/// The provider name for the Sitefinity content type
		/// </value>
		[ConfigurationProperty("ProviderName", Options = ConfigurationPropertyOptions.IsRequired, DefaultValue = "")]
		public string ProviderName
		{
			get { return (string)base["ProviderName"]; }
			set { base["ProviderName"] = value; }
		}

		/// <summary>
		/// Gets or sets the default page URL for this content type and provider.
		/// </summary>
		/// <value>
		/// The default page URL for this content type and provider.
		/// </value>
		[ConfigurationProperty("DefaultPageUrl", Options = ConfigurationPropertyOptions.IsRequired, DefaultValue = "")]
		public string DefaultPageUrl
		{
			get { return (string)base["DefaultPageUrl"]; }
			set { base["DefaultPageUrl"] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ContentPage"/> should be included in the Sitemap.
		/// </summary>
		/// <value>
		///   <c>true</c> if this content type should be included in the Sitemap; otherwise, <c>false</c>.
		/// </value>
		[ConfigurationProperty("Include", Options = ConfigurationPropertyOptions.IsRequired, DefaultValue = true)]
		public bool Include
		{
			get { return (bool)base["Include"]; }
			set { base["Include"] = value; }
		}
	}
}