Progress.Sitefinity.Samples.Sitemap
==================================

### This repository is not automatically upgraded to latest Sitefintiy version. The repository is monitored for pull requests and fixes. The latest official version of Sitefinity that supports this sample is 8.0. Be aware that using a higher version could cause unexpected behavior. If you successfully upgrade the example to a greater version, please share your work with the community by submitting your changes via pull request.

The Sitemap sample project showcases how to create a sitemap module that generates a search engine-friendly **sitemap.xml** file for your site. This file is very useful for bigger sitemaps, for which search engines like Google can generate a sitemap. This sitemap appears in the search results. 

This Sitemap sample module can creta this **sitemap.xml** file automatically for the sites you create with Sitefinity CMS. As a result, you can enjoy the SEO benefits of letting search engines know about your site structure. The initial version of this module indexes all sitemap pages, as well as all _News_, _Events_, and _Blog Post_ content items. A custom configuration class stores the default pages for these content items, so that the individual detail page URLs can be included in the sitemap. 

Using the Sitemap sample, you can:

* Generate a sitemap.xml file with URLs of all site pages and content items
* Create a custom configuration class to persist module settings 

### Requirements

* Sitefinity CMS license
* .NET Framework 4
* Visual Studio 2012
* Microsoft SQL Server 2008R2 or later versions

### Prerequisites

Clear the NuGet cache files. To do this:

1. In Windows Explorer, open the **%localappdata%\NuGet\Cache** folder.
2. Select all files and delete them.

### Nuget package restoration
The solution in this repository relies on NuGet packages with automatic package restore while the build procedure takes place.   
For a full list of the referenced packages and their versions see the [packages.config](https://github.com/Sitefinity-SDK/Telerik.Sitefinity.Samples.Sitemap/blob/master/SitefinityWebApp/packages.config) file.    
For a history and additional information related to package versions on different releases of this repository, see the [Releases page](https://github.com/Sitefinity-SDK/Telerik.Sitefinity.Samples.Sitemap/releases).    


### Installation instructions: SDK Samples from GitHub
1. In Solution Explorer, navigate to _SitefinityWebApp_ » *App_Data* » _Sitefinity_ » _Configuration_ and select the **StartupConfig.config** file. 
2. Modify the **dbType**, **sqlInstance** and **dbName** values to match your server settings.
3. Build the solution.

For version-specific details about the required Sitefinity NuGet packages for this sample application, click on [Releases] (https://github.com/Sitefinity-SDK/Telerik.Sitefinity.Samples.Sitemap/releases).

### Login

To login into the Sitefinity CMS backend, use the following credentials:    
**Username:** admin   
**Password:** password

### Additional resources

Progress Sitefinity CMS Documentation: [Development: Use and extend Sitefinity CMS functionality](http://docs.sitefinity.com/develop-create-and-manage-website-content)

