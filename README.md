Telerik.Sitefinity.Samples.Sitemap
==================================

The Sitemap sample project showcases how to create a sitemap module that generates a search engine-friendly **sitemap.xml** file for your site. This file is very useful for bigger sitemaps, for which search engines like Google can generate a sitemap. This sitemap appears in the search results. 

This Sitemap sample module can creta this **sitemap.xml** file automatically for the sites you create with Sitefinity. As a result, you can enjoy the SEO benefits of letting search engines know about your site structure. The initial version of this module indexes all sitemap pages, as well as all _News_, _Events_, and _Blog Post_ content items. A custom configuration class stores the default pages for these content items, so that the individual detail page URLs can be included in the sitemap. 

Using the Sitemap sample, you can:

* Generate a sitemap.xml file with URLs of all site pages and content items
* Create a custom configuration class to persist module settings 

### Requirements

* Sitefinity 6.3 license

* .NET Framework 4

* Visual Studio 2012

* Microsoft SQL Server 2008R2 or later versions


### Installation instructions: SDK Samples from GitHub

1. Clone the [Telerik.Sitefinity.Samples.Dependencies](https://github.com/Sitefinty-SDK/Telerik.Sitefinity.Samples.Dependencies) repo to get all assemblies necessary to run for the samples.
2. Fix broken references in the class libraries, for example in **SitefinityWebApp** and **Telerik.Sitefinity.Samples.Common**:

  1. In Solution Explorer, open the context menu of your project node and click _Properties_.  
  
    The Project designer is displayed.
  2. Select the _Reference Paths_ tab page.
  3. Browse and select the folder where **Telerik.Sitefinity.Samples.Dependencies** folder is located.
  4. Click the _Add Folder_ button.


3. In Solution Explorer, navigate to _SitefinityWebApp_ -> *App_Data* -> _Sitefinity_ -> _Configuration_ and select the **DataConfig.config** file. 
4. Modify the **connectionString** value to match your server address.
5. Build the solution.

### Login

To login to Sitefinity backend, use the following credentials: 

**Username:** admin

**Password:** password

### Additional resources

[Developers Guide](http://www.sitefinity.com/documentation/documentationarticles/developers-guide)

