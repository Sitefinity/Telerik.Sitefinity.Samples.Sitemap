using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using SitefinityWebApp.Modules.SiteMap;
using Telerik.Sitefinity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Modules.Blogs.Web.UI;
using Telerik.Sitefinity.Modules.Blogs.Web.UI.Public;
using Telerik.Sitefinity.Modules.Events;
using Telerik.Sitefinity.Modules.Events.Configuration;
using Telerik.Sitefinity.Modules.Events.Data;
using Telerik.Sitefinity.Modules.Events.Web.UI;
using Telerik.Sitefinity.Modules.Events.Web.UI.Public;
using Telerik.Sitefinity.Modules.GenericContent.Web.UI;
using Telerik.Sitefinity.Modules.News.Web.UI;
using Telerik.Sitefinity.Samples.Common;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI.ContentUI.Contracts;
using Telerik.Sitefinity.Web.UI.ContentUI.Views.Backend.Master.Definitions;
using Telerik.Sitefinity.Web.UI.NavigationControls;

namespace SitefinityWebApp
{
	public class Global : System.Web.HttpApplication
	{
		private const string SamplesTemplateName = "SamplesMasterPage";
		private const string SamplesTemplateId = "4166a221-7131-442a-8f19-000000000002";
		private const string SamplesTemplateMasterPage = "~/App_Data/Sitefinity/WebsiteTemplates/Samples/App_Master/Samples.master";
		private const string SamplesThemePath = "~/App_Data/Sitefinity/WebsiteTemplates/Samples/App_Themes/Samples";
		private const string SamplesThemeName = "SamplesTheme";

		private const string HomePageId = "A6F9DB68-70CB-422B-BE43-000000000002";
		private const string AboutUsPageId = "A6F9DB68-70CB-422B-BE43-000000000003";
		private const string NewsPageId = "A6F9DB68-70CB-422B-BE43-000000000004";
		private const string BlogAPageId = "A6F9DB68-70CB-422B-BE43-000000000005";
		private const string BlogBPageId = "A6F9DB68-70CB-422B-BE43-000000000006";
		private const string EventsPageId = "A6F9DB68-70CB-422B-BE43-000000000007";
		private const string CustomEventsPageId = "A6F9DB68-70CB-422B-BE43-000000000008";
		private const string ContactUsPageId = "A6F9DB68-70CB-422B-BE43-000000000009";

		private const string SampleBlogAId = "f0e51514-aee5-429a-af66-000000000001";
		private const string SampleBlogBId = "f0e51514-aee5-429a-af66-000000000002";

		private const string CustomEventsProviderName = "CustomEventsProvider";

		private const string FILLER_TEXT = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer dapibus, tellus eu laoreet bibendum, dolor ante porttitor nunc, non convallis magna arcu eget dui. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.</p><p>Nunc condimentum condimentum diam id dapibus. Proin et risus leo. Mauris tristique feugiat ante, a tristique tellus vehicula rhoncus. Mauris odio nibh, lacinia sit amet vehicula in, feugiat ut turpis.</p><p>Suspendisse mi felis, lobortis non ultricies eu, fermentum vitae massa. Quisque laoreet elit placerat nisi placerat congue faucibus enim malesuada. Integer justo erat, faucibus nec tincidunt sed, fringilla id nisl. Sed enim libero, adipiscing eget dignissim et, suscipit non odio.</p>";

		protected void Application_Start(object sender, EventArgs e)
		{
			Bootstrapper.Initializing += new EventHandler<ExecutingEventArgs>(Bootstrapper_Initializing);
            SystemManager.ApplicationStart += this.SystemManager_ApplicationStart;
		}

        protected void SystemManager_ApplicationStart(object sender, EventArgs e)
        {
            SystemManager.RunWithElevatedPrivilegeDelegate worker = new SystemManager.RunWithElevatedPrivilegeDelegate(CreateSampleWorker);
            SystemManager.RunWithElevatedPrivilege(worker);
        }

		protected void Bootstrapper_Initializing(object sender, Telerik.Sitefinity.Data.ExecutingEventArgs args)
		{
			if (args.CommandName == "RegisterRoutes")
			{
				SampleUtilities.RegisterModule<SiteMapModule>("SiteMap", "This sample presents showcases how to create a sitemap module which generates a search engine friendly sitemap.xml for your site.");
			}
		}

        private void CreateSampleWorker(object[] args)
        {            
            ConfigManager.Executed += this.ConfigManager_Executed;
            
            SampleUtilities.RegisterTheme(SamplesThemeName, SamplesThemePath);
            var result = SampleUtilities.RegisterTemplate(new Guid(SamplesTemplateId), SamplesTemplateName, SamplesTemplateName, SamplesTemplateMasterPage, SamplesThemeName);
            if (result)
            {
                var navigationControl = new NavigationControl();
                navigationControl.NavigationMode = NavigationModes.HorizontalDropDownMenu;
                navigationControl.NavigationAction = NavigationAction.OnMouseOver;
                navigationControl.Skin = "WebBlue";

                SampleUtilities.AddControlToTemplate(new Guid(SamplesTemplateId), navigationControl, "Menu", "Navigation");
            }

            // add aa custom events provider
            var eventsConfig = Config.Get<EventsConfig>();
            if (!eventsConfig.Providers.Elements.Any(p => p.Name == CustomEventsProviderName))
            {
                var customProvider = new DataProviderSettings(eventsConfig.Providers)
                {
                    Name = CustomEventsProviderName,
                    Description = "Alternate provider for storing Events data in database using OpenAccess ORM.",
                    ProviderType = typeof(OpenAccessEventProvider)
                };
                var values = new NameValueCollection();
                values.Add("applicationName", "/CustomEvents");
                customProvider.Parameters = values;
                eventsConfig.Providers.Add(customProvider);
                ConfigManager.GetManager().SaveSection(eventsConfig);
            }

            this.CreateNewsItems();
            this.CreateBlogPosts();
            this.CreateEvents(string.Empty);
            this.CreateEvents(CustomEventsProviderName);

            this.CreateHomePage();
            this.CreateAboutUsPage();
            this.CreateNewsPage();
            this.CreateBlogAPage();
            this.CreateBlogBPage();
            this.CreateEventsPage();
            this.CreateCustomEventsPage();
            this.CreateContactUsPage();

            SampleUtilities.CreateUsersAndRoles();
        }		

		private void CreateNewsItems()
		{
			var newsCreated = App.WorkWith().NewsItems().Get().Count() > 0;

			if (!newsCreated)
			{
				var author = "admin";
				var newsTitle = "Sample News Item A";
				var newsContent = @"<div id=""lipsum"">
<p> Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris sit amet nisi ac risus scelerisque fermentum non sit amet purus. Morbi dolor sapien, luctus ac commodo posuere, scelerisque eu arcu. Mauris a dolor risus, quis mattis mauris. Mauris tellus lacus, auctor quis hendrerit ut, vulputate quis est. Sed massa est, gravida eu cursus et, laoreet non risus. Praesent bibendum condimentum tellus, ut blandit lorem vehicula mattis. Nam at nisl quis lacus accumsan tempor. Mauris viverra aliquam lorem, et molestie odio mattis in. Integer non ipsum lacus. Morbi sed justo arcu. Pellentesque volutpat scelerisque ligula nec ultrices. Mauris quam ipsum, fringilla ac molestie ac, interdum ut eros. Etiam sit amet lacus orci, quis eleifend nibh. </p>
<p> Praesent mauris odio, venenatis eu suscipit vel, rhoncus eu est. Nam semper diam a massa aliquam bibendum. Mauris eget quam orci, eu gravida odio. Curabitur tellus enim, rutrum sed venenatis nec, aliquet vitae nibh. Aenean eget nisi non libero cursus sagittis. Sed pellentesque eleifend libero, sed eleifend ipsum interdum at. Mauris eget ultricies odio. Donec eget fringilla lectus. Curabitur elit lacus, aliquam in facilisis eu, laoreet faucibus sem. Fusce varius dolor at nulla facilisis id semper massa sodales. </p>
<p> Aenean placerat viverra risus hendrerit dictum. Quisque mattis, magna id egestas lobortis, dui lorem egestas orci, sit amet pulvinar felis odio quis ligula. Vivamus erat nisi, lobortis vitae tincidunt a, blandit eu lectus. Fusce vitae purus vitae sapien adipiscing molestie fringilla ut tortor. Quisque ipsum tellus, hendrerit in lobortis vitae, placerat sed dui. Maecenas vehicula vehicula nibh non ullamcorper. Nam pulvinar, dolor id mollis pretium, sem urna posuere ligula, quis facilisis dui eros non lectus. Nulla sodales venenatis quam, at molestie risus tincidunt quis. Curabitur et justo vel arcu condimentum suscipit. Aenean a odio massa. Praesent aliquam mauris sit amet lectus venenatis ac vestibulum massa egestas. Nunc in velit quam. Fusce malesuada dolor eu erat rutrum cursus in eu erat. Maecenas mi justo, interdum eget ultricies non, viverra sed libero. </p>
<p> Duis neque odio, pellentesque vitae fermentum vel, condimentum sed nisi. Nulla interdum, tortor nec ultrices congue, lectus mi tincidunt ante, et egestas massa erat vitae enim. Integer sagittis interdum elit. Suspendisse potenti. Integer a ligula nibh, ac consequat felis. Cras a magna nunc, a semper odio. Donec nulla mauris, luctus quis egestas nec, malesuada eu ligula. Vivamus malesuada lorem id massa sollicitudin in sagittis felis pellentesque. Cras vel nulla eget nunc tristique hendrerit. Ut porttitor mattis cursus. </p>
<p> Integer in mauris in eros malesuada ullamcorper. Phasellus vel nunc vel nunc malesuada tristique. Sed in felis quis nisl vehicula ultrices. Cras nec nunc eu massa imperdiet aliquam. Ut interdum, leo ut adipiscing egestas, lectus purus commodo felis, ac condimentum purus massa ut nibh. Vestibulum non nisi leo, eget faucibus nunc. In hac habitasse platea dictumst. Maecenas mauris urna, sollicitudin ac rhoncus quis, adipiscing ac massa. Duis et nisl augue. Vestibulum ullamcorper lectus egestas diam euismod ultrices. Quisque quis luctus mi. </p>
</div>";
				var summary = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris sit amet nisi ac risus scelerisque fermentum non sit amet purus. Morbi dolor sapien, luctus ac commodo posuere, scelerisque eu arcu. Mauris a dolor risus, quis mattis mauris. Mauris tellus lacus, auctor quis hendrerit ut, vulputate quis est.";

				SampleUtilities.CreateNewsItem(newsTitle, newsContent, summary, author);

				newsTitle = "Sample News Item B";

				SampleUtilities.CreateNewsItem(newsTitle, newsContent, summary, author);

				newsTitle = "Sample News Item C";

				SampleUtilities.CreateNewsItem(newsTitle, newsContent, summary, author);
			}
		}

		private void CreateBlogPosts()
		{
			SampleUtilities.CreateBlog(new Guid(SampleBlogAId), "Sample Blog A", "Sample Blog A");

			var postsCreated = App.WorkWith().Blog(new Guid(SampleBlogAId)).BlogPosts().Get().Count() > 0;

			if (!postsCreated)
			{
				var title = "Blog A Sample Post A";
				var content = @"<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus non lectus at sem consequat suscipit. Etiam nunc sem, condimentum a feugiat sit amet, consectetur sed justo. Nullam purus erat, tincidunt sed commodo ac, mollis vel neque. Vestibulum volutpat nulla in enim aliquet vitae ornare nunc gravida. </p>
<p>Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Etiam ipsum nisl, tempor at tincidunt quis, congue vel purus. Duis sodales, est quis mollis posuere, mi mauris mollis sapien, ut rhoncus leo justo sed dolor. Etiam blandit tincidunt velit ac pellentesque. Cras nec leo lacus. Maecenas vel ipsum vitae dolor ultrices tempus. </p>
<p>Nam a erat felis, a pellentesque diam. Phasellus nibh lectus, adipiscing nec pharetra in, semper eu ipsum. Nam venenatis dignissim risus nec fringilla. Quisque placerat leo non ligula dictum sed auctor neque bibendum. Proin arcu risus, ullamcorper in dictum sed, tincidunt et odio. Praesent sed orci eget metus commodo egestas.</p>
<p>Quisque sollicitudin tellus quis est egestas nec pretium leo volutpat. Aliquam euismod viverra sollicitudin. Morbi quis leo velit, eget tincidunt nisi. Phasellus non convallis nunc. Ut ligula lorem, hendrerit eget eleifend quis, ullamcorper vel quam. Quisque ullamcorper dolor quis nunc volutpat eget elementum nunc lobortis. </p>
<p>Aenean laoreet sodales libero eget feugiat. Praesent purus nibh, blandit eu fermentum ac, pulvinar sed mi. Sed lobortis euismod tortor, vitae mattis nulla pulvinar quis. Quisque a augue a felis consectetur aliquam ac non metus. Fusce sapien erat, ultrices vitae scelerisque sed, gravida ut elit. In hac habitasse platea dictumst. Aliquam quis lectus sit amet lectus porta imperdiet. Pellentesque sed libero vitae nulla elementum venenatis nec vitae dolor. Quisque egestas purus id nulla fringilla sed malesuada ligula venenatis. In sit amet purus velit. Quisque ac aliquam urna.</p>";
				var author = "admin";

				SampleUtilities.CreateBlogPost(new Guid(SampleBlogAId), title, content, author, string.Empty);

				title = "Blog A Sample Post B";

				SampleUtilities.CreateBlogPost(new Guid(SampleBlogAId), title, content, author, string.Empty);
			}

			SampleUtilities.CreateBlog(new Guid(SampleBlogBId), "Sample Blog B", "Sample Blog B");

			postsCreated = App.WorkWith().Blog(new Guid(SampleBlogBId)).BlogPosts().Get().Count() > 0;

			if (!postsCreated)
			{
				var title = "Blog B Sample Post A";
				var content = @"<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus non lectus at sem consequat suscipit. Etiam nunc sem, condimentum a feugiat sit amet, consectetur sed justo. Nullam purus erat, tincidunt sed commodo ac, mollis vel neque. Vestibulum volutpat nulla in enim aliquet vitae ornare nunc gravida. </p>
<p>Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Etiam ipsum nisl, tempor at tincidunt quis, congue vel purus. Duis sodales, est quis mollis posuere, mi mauris mollis sapien, ut rhoncus leo justo sed dolor. Etiam blandit tincidunt velit ac pellentesque. Cras nec leo lacus. Maecenas vel ipsum vitae dolor ultrices tempus. </p>
<p>Nam a erat felis, a pellentesque diam. Phasellus nibh lectus, adipiscing nec pharetra in, semper eu ipsum. Nam venenatis dignissim risus nec fringilla. Quisque placerat leo non ligula dictum sed auctor neque bibendum. Proin arcu risus, ullamcorper in dictum sed, tincidunt et odio. Praesent sed orci eget metus commodo egestas.</p>
<p>Quisque sollicitudin tellus quis est egestas nec pretium leo volutpat. Aliquam euismod viverra sollicitudin. Morbi quis leo velit, eget tincidunt nisi. Phasellus non convallis nunc. Ut ligula lorem, hendrerit eget eleifend quis, ullamcorper vel quam. Quisque ullamcorper dolor quis nunc volutpat eget elementum nunc lobortis. </p>
<p>Aenean laoreet sodales libero eget feugiat. Praesent purus nibh, blandit eu fermentum ac, pulvinar sed mi. Sed lobortis euismod tortor, vitae mattis nulla pulvinar quis. Quisque a augue a felis consectetur aliquam ac non metus. Fusce sapien erat, ultrices vitae scelerisque sed, gravida ut elit. In hac habitasse platea dictumst. Aliquam quis lectus sit amet lectus porta imperdiet. Pellentesque sed libero vitae nulla elementum venenatis nec vitae dolor. Quisque egestas purus id nulla fringilla sed malesuada ligula venenatis. In sit amet purus velit. Quisque ac aliquam urna.</p>";
				var author = "admin";

				SampleUtilities.CreateBlogPost(new Guid(SampleBlogBId), title, content, author, string.Empty);

				title = "Blog B Sample Post B";

				SampleUtilities.CreateBlogPost(new Guid(SampleBlogBId), title, content, author, string.Empty);

				title = "Blog B Sample Post C";

				SampleUtilities.CreateBlogPost(new Guid(SampleBlogBId), title, content, author, string.Empty);
			}
		}

		private void CreateEvents(string providerName)
		{
			if (string.IsNullOrEmpty(providerName)) providerName = EventsManager.GetDefaultProviderName();

			var eventsCreated = App.Prepare().SetContentProvider(providerName).WorkWith().Events().Get().Count() > 0;
			var year = DateTime.Now.Year;
			var rand = new Random(DateTime.Now.Millisecond);

			if (!eventsCreated)
			{
				var title = providerName + " Sample Event A";
				var content = FILLER_TEXT;
				var month = rand.Next(1, 12);
				var startDate = new DateTime(year, month, 01, 18, 0, 0);
				var endDate = new DateTime(year, month, 02, 4, 0, 0);
				var street = "Organization";
				var city = "123 Fake Str";
				var state = "TX";
				var country = "USA";
				var contactEmail = "no-reply@telerik.com";
				var contactName = "John Doe";
				var contactWeb = "http://www.sitefinity.com";

				this.CreateEvent(providerName, title, content, startDate, endDate, street, city, state, country, contactEmail, contactName, contactWeb);

				title = providerName + " Sample Event B";
				content = FILLER_TEXT;
				month = rand.Next(1, 12);
				startDate = new DateTime(year, month, 12, 3, 0, 0);
				endDate = new DateTime(year, month, 12, 7, 0, 0);

				this.CreateEvent(providerName, title, content, startDate, endDate, street, city, state, country, contactEmail, contactName, contactWeb);

				title = providerName + " Sample Event C";
				content = FILLER_TEXT;
				month = rand.Next(1, 12);
				startDate = new DateTime(year, month, 4, 21, 0, 0);
				endDate = new DateTime(year, month, 4, 23, 0, 0);

				this.CreateEvent(providerName, title, content, startDate, endDate, street, city, state, country, contactEmail, contactName, contactWeb);

				title = providerName + " Sample Event D";
				content = FILLER_TEXT;
				month = rand.Next(1, 12);
				startDate = new DateTime(year, month, 1, 5, 0, 0);
				endDate = new DateTime(year, month, 1, 10, 0, 0);

				this.CreateEvent(providerName, title, content, startDate, endDate, street, city, state, country, contactEmail, contactName, contactWeb);

				title = providerName + " Sample Event E";
				content = FILLER_TEXT;
				month = rand.Next(1, 12);
				startDate = new DateTime(year, month, 1, 5, 0, 0);
				endDate = new DateTime(year, month, 1, 10, 0, 0);

				this.CreateEvent(providerName, title, content, startDate, endDate, street, city, state, country, contactEmail, contactName, contactWeb);

				title = providerName + " Sample Event F";
				content = FILLER_TEXT;
				month = rand.Next(1, 12);
				startDate = new DateTime(year, month, 1, 5, 0, 0);
				endDate = new DateTime(year, month, 1, 10, 0, 0);

				this.CreateEvent(providerName, title, content, startDate, endDate, street, city, state, country, contactEmail, contactName, contactWeb);

				title = providerName + " Sample Event G";
				content = FILLER_TEXT;
				month = rand.Next(1, 12);
				startDate = new DateTime(year, month, 1, 5, 0, 0);
				endDate = new DateTime(year, month, 1, 10, 0, 0);

				this.CreateEvent(providerName, title, content, startDate, endDate, street, city, state, country, contactEmail, contactName, contactWeb);
			}
		}

		public void CreateEvent(string providerName, string title, string content, DateTime startDate, DateTime endDate, string street, string city, string state, string country, string contatcEmail, string contactWeb, string contactName)
		{
			var eventId = Guid.Empty;

			App.Prepare().SetContentProvider(providerName).WorkWith().Event().CreateNew()
				.Do(e =>
				{
					eventId = e.Id;
					e.Title = title;
					e.Content = content;
					e.EventStart = startDate;
					e.EventEnd = endDate;
					e.Street = street;
					e.City = city;
					e.State = state;
					e.Country = country;
					e.ContactEmail = contatcEmail;
					e.ContactName = contactName;
					e.ContactWeb = contactWeb;

					e.PublicationDate = DateTime.Today;
					e.ExpirationDate = DateTime.Today.AddDays(365);
                    e.ApprovalWorkflowState.Value = SampleUtilities.ApprovalWorkflowStatePublished;
				}).Publish().SaveChanges();
		}

		private void CreateHomePage()
		{
			var result = SampleUtilities.CreatePage(new Guid(HomePageId), "Home");

			if (result)
			{
				SampleUtilities.SetTemplateToPage(new Guid(HomePageId), new Guid(SamplesTemplateId));

				// add welcome and instructions to page
				var generalInformationBlock = new ContentBlock();
				generalInformationBlock.Html = @"<h1>SiteMap Intra-Site Module Example</h1><p>This is the home page for the SiteMap Intra-Site module example website. To view the sitemap visit <a href=""sitemap.xml"">sitemap.xml</a>.</p>";

				SampleUtilities.AddControlToPage(new Guid(HomePageId), generalInformationBlock, "Content", "Content block");
			}
		}

		private void CreateAboutUsPage()
		{
			var result = SampleUtilities.CreatePage(new Guid(AboutUsPageId), "About Us");

			if (result)
			{
				SampleUtilities.SetTemplateToPage(new Guid(AboutUsPageId), new Guid(SamplesTemplateId));

				var titleContentBlock = new ContentBlockBase();
				titleContentBlock.Html = "<h1>About Us</h1>";

				SampleUtilities.AddControlToPage(new Guid(AboutUsPageId), titleContentBlock, "Content", "Content block");

				var overviewContentBlock = new ContentBlockBase();
				overviewContentBlock.Html = @"<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur sed ligula 
										vitae est posuere congue sed pretium quam. Pellentesque at velit in massa ornare 
										sagittis at in lacus. Fusce placerat leo eu sem sagittis posuere. Cras a arcu nec 
										turpis pulvinar vehicula. Proin sed metus nec leo rutrum mattis vel vel est. Pellentesque 
										lacinia nulla id lorem cursus fringilla.</p>
										<p>Proin non commodo velit. Vestibulum ante ipsum primis in faucibus orci luctus et 
										ultrices posuere cubilia Curae; Donec elementum imperdiet volutpat. Integer sit amet felis 
										in magna iaculis molestie vel in velit. Nulla facilisi. Sed iaculis imperdiet turpis, in 
										vulputate lorem mattis non. In condimentum, neque vitae elementum sodales, metus mi congue 
										urna, a imperdiet sem massa at felis. Aliquam erat volutpat. Nullam eget lorem enim, at 
										venenatis felis. Maecenas et euismod diam.</p>
										<p>Pellentesque tortor tellus, cursus dignissim hendrerit non, ornare a tellus. Donec ut 
										consequat massa. Phasellus fermentum, justo vel congue tincidunt, metus diam sagittis eros, 
										sed ornare tellus enim sit amet sem. Duis gravida tortor eget risus varius vel ullamcorper 
										risus semper. </p>";

				SampleUtilities.AddControlToPage(new Guid(AboutUsPageId), overviewContentBlock, "Content", "Content block");
			}
		}

		private void CreateNewsPage()
		{
			var result = SampleUtilities.CreatePage(new Guid(NewsPageId), "News");

			if (result)
			{
				SampleUtilities.SetTemplateToPage(new Guid(NewsPageId), new Guid(SamplesTemplateId));

				var templateKey = SampleUtilities.GetControlTemplateKey(typeof(MasterListView), "Titles, dates and summaries");
				SampleUtilities.RegisterNewsFrontendView("NewsFrontend", templateKey, typeof(MasterListView), "NewsFrontendTitleDatesAndSummariesList");

				var newsControl = new NewsView();
                newsControl.MasterViewName = "NewsFrontendTitleDatesAndSummariesList";

				SampleUtilities.AddControlToPage(new Guid(NewsPageId), newsControl, "Content", "News View");
			}
		}

		private void CreateBlogAPage()
		{
			var result = SampleUtilities.CreatePage(new Guid(BlogAPageId), "Blog A");

			if (result)
			{
				SampleUtilities.SetTemplateToPage(new Guid(BlogAPageId), new Guid(SamplesTemplateId));

				var templateKey = SampleUtilities.GetControlTemplateKey(typeof(MasterPostsView), "Titles, dates and summaries");
				SampleUtilities.RegisterBlogPostsFrontendView("BlogPostsFrontend", templateKey, typeof(MasterPostsView), "BlogPostsFrontendBlogList");

				var blogsControl = new BlogPostView();
				blogsControl.MasterViewName = "BlogPostsFrontendBlogList";
				var parent = new List<Guid>() { new Guid(SampleBlogAId) };
				((IContentViewMasterDefinition)blogsControl.ControlDefinition.Views[blogsControl.MasterViewName]).ItemsParentsIds = parent;

				var masterDefinition = blogsControl.ControlDefinition.GetDefaultMasterView().GetDefinition() as MasterGridViewDefinition;
				if (masterDefinition != null)
					masterDefinition.ItemsParentsIds = parent;

				SampleUtilities.AddControlToPage(new Guid(BlogAPageId), blogsControl, "Content", "Blog A View");

				// save default page for blog posts
				App.WorkWith().Blog(new Guid(SampleBlogAId)).Do(b => b.DefaultPageId = new Guid(BlogAPageId)).SaveChanges();
			}
		}

		private void CreateBlogBPage()
		{
			var result = SampleUtilities.CreatePage(new Guid(BlogBPageId), "Blog B");

			if (result)
			{
				SampleUtilities.SetTemplateToPage(new Guid(BlogBPageId), new Guid(SamplesTemplateId));

				var templateKey = SampleUtilities.GetControlTemplateKey(typeof(MasterPostsView), "Titles, dates and summaries");
				SampleUtilities.RegisterBlogPostsFrontendView("BlogPostsFrontend", templateKey, typeof(MasterPostsView), "BlogPostsFrontendBlogList");

				var blogsControl = new BlogPostView();
				blogsControl.MasterViewName = "BlogPostsFrontendBlogList";
				var parent = new List<Guid>() { new Guid(SampleBlogBId) };
				((IContentViewMasterDefinition)blogsControl.ControlDefinition.Views[blogsControl.MasterViewName]).ItemsParentsIds = parent;
				
				SampleUtilities.AddControlToPage(new Guid(BlogBPageId), blogsControl, "Content", "Blog B View");

				// save default page for blog posts
				App.WorkWith().Blog(new Guid(SampleBlogBId)).Do(b => b.DefaultPageId = new Guid(BlogBPageId)).SaveChanges();
			}
		}

		private void CreateEventsPage()
		{
			var result = SampleUtilities.CreatePage(new Guid(EventsPageId), "Events");

			if (result)
			{
				SampleUtilities.SetTemplateToPage(new Guid(EventsPageId), new Guid(SamplesTemplateId));

				var templateKey = SampleUtilities.GetControlTemplateKey(typeof(MasterView), "Titles, cities, dates");
				SampleUtilities.RegisterEventsFrontendView("EventsFrontend", templateKey, typeof(MasterView), "EventsFrontendDateList");

				var eventsControl = new EventsView();
				eventsControl.MasterViewName = "EventsFrontendDateList";
				eventsControl.ControlDefinition.GetDefaultDetailView().TemplateKey = SampleUtilities.GetControlTemplateKey(typeof(Telerik.Sitefinity.Modules.Events.Web.UI.Public.DetailsView), "Full event item");

				SampleUtilities.AddControlToPage(new Guid(EventsPageId), eventsControl, "Content", "Events View");
			}
		}

		private void CreateCustomEventsPage()
		{
			var result = SampleUtilities.CreatePage(new Guid(CustomEventsPageId), "Custom Events");

			if (result)
			{
				SampleUtilities.SetTemplateToPage(new Guid(CustomEventsPageId), new Guid(SamplesTemplateId));

				var templateKey = SampleUtilities.GetControlTemplateKey(typeof(MasterView), "Titles, cities, dates");
				SampleUtilities.RegisterEventsFrontendView("EventsFrontend", templateKey, typeof(MasterView), "EventsFrontendDateList");

				var eventsControl = new EventsView();
				eventsControl.MasterViewName = "EventsFrontendDateList";
				eventsControl.ControlDefinition.GetDefaultDetailView().TemplateKey = SampleUtilities.GetControlTemplateKey(typeof(Telerik.Sitefinity.Modules.Events.Web.UI.Public.DetailsView), "Full event item");
				eventsControl.ControlDefinition.ProviderName = CustomEventsProviderName;

				SampleUtilities.AddControlToPage(new Guid(CustomEventsPageId), eventsControl, "Content", "Events View");
			}
		}

		private void CreateContactUsPage()
		{
			var result = SampleUtilities.CreatePage(new Guid(ContactUsPageId), "Contact Us", new Guid(AboutUsPageId));

			if (result)
			{
				SampleUtilities.SetTemplateToPage(new Guid(ContactUsPageId), new Guid(SamplesTemplateId));

				var titleContentBlock = new ContentBlockBase();
				titleContentBlock.Html = @"<h1>Contact Us</h1>
<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec sed diam lectus, vel facilisis odio. Donec ac rutrum felis. Curabitur bibendum est vel elit iaculis a scelerisque massa rhoncus. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.</p>";

				SampleUtilities.AddControlToPage(new Guid(ContactUsPageId), titleContentBlock, "Content", "Content block");
			}
		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}

		protected void Application_Error(object sender, EventArgs e)
		{
		}

		protected void Session_End(object sender, EventArgs e)
		{
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}

		private void ConfigManager_Executed(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs args)
		{
			if (args.CommandName == "SaveSection")
			{
				var section = args.CommandArguments as VirtualPathSettingsConfig;
				if (section != null)
				{
					// Reset the Virtual path manager, whenever the section of the VirtualPathSettingsConfig is saved.
					// This is needed so that the prefixes for templates in our module assembly are taken into account.
					VirtualPathManager.Reset();
				}
			}
		}
	}
}
