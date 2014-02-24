using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Configuration;
using System.Configuration;

namespace SitefinityWebApp.Modules.SiteMap.Configuration
{
	/// <summary>
	/// Configuration Element for storing list pages for Sitefinity Content Types
	/// </summary>
	public class ContentType : ConfigElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ContentType"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public ContentType(ConfigElement parent) : base(parent) { this.Pages = new ConfigElementList<ContentPage>(this); }

		/// <summary>
		/// Gets or sets the name of the ContentType. This is the same as the Module Name of the Content Type.
		/// </summary>
		/// <value>
		/// The name of the Content Module.
		/// </value>
		[ConfigurationProperty("Name", Options = ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired, DefaultValue = "")]
		public string Name
		{
			get { return (string)base["Name"]; }
			set { base["Name"] = value; }
		}

		/// <summary>
		/// List of ContentPage configuration elements containing the default list page for each provider of this ContentType
		/// </summary>
		/// <value>
		/// The list of default list pages, one per ContentType provider.
		/// </value>
		[ConfigurationProperty("Pages", IsRequired = true)]
		public ConfigElementList<ContentPage> Pages
		{
			get { return (ConfigElementList<ContentPage>)base["Pages"]; }
			set { base["Pages"] = value; }
		}
	}
}