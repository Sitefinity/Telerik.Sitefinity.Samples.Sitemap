Telerik.Sitefinity.Samples.Sitemap
==================================

[![Build Status](http://sdk-jenkins-ci.cloudapp.net/buildStatus/icon?job=Telerik.Sitefinity.Samples.Sitemap.CI)](http://sdk-jenkins-ci.cloudapp.net/job/Telerik.Sitefinity.Samples.Sitemap.CI/)

The Sitemap sample project showcases how to create a sitemap module that generates a search engine-friendly **sitemap.xml** file for your site. This file is very useful for bigger sitemaps, for which search engines like Google can generate a sitemap. This sitemap appears in the search results. 

This Sitemap sample module can creta this **sitemap.xml** file automatically for the sites you create with Sitefinity. As a result, you can enjoy the SEO benefits of letting search engines know about your site structure. The initial version of this module indexes all sitemap pages, as well as all _News_, _Events_, and _Blog Post_ content items. A custom configuration class stores the default pages for these content items, so that the individual detail page URLs can be included in the sitemap. 

Using the Sitemap sample, you can:

* Generate a sitemap.xml file with URLs of all site pages and content items
* Create a custom configuration class to persist module settings 

### Requirements

* Sitefinity 7.1 Beta license
 
  **NOTE**: For more information about acquiring a Sitefinity Beta license, contact [sales@sitefinity.com](sales@sitefinity.com)

* .NET Framework 4

* Visual Studio 2012

* Microsoft SQL Server 2008R2 or later versions

### Prerequisites

Clear the NuGet cache files. To do this:

1. In Windows Explorer, open the **%localappdata%\NuGet\Cache** folder.
2. Select all files and delete them.


### Installation instructions: SDK Samples from GitHub


1. In Solution Explorer, navigate to _SitefinityWebApp_ -> *App_Data* -> _Sitefinity_ -> _Configuration_ and select the **DataConfig.config** file. 
2. Modify the **connectionString** value to match your server address.
3. Build the solution.

For version-specific details about the required Sitefinity NuGet packages for this sample application, click on [Releases]
 (https://github.com/Sitefinity-SDK/Telerik.Sitefinity.Samples.Sitemap/releases).

### Login

To login to Sitefinity backend, use the following credentials: 

**Username:** admin

**Password:** password

### Additional resources

[Developers Guide](http://www.sitefinity.com/documentation/documentationarticles/developers-guide)

