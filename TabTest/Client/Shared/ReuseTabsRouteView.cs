using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TabTest.Client.Shared
{
    public class ReuseTabsRouteView : RouteView
    {
        [Inject]
        public TabSetTool TabSetTool { get; set; }

        [Inject]
        public NavigationManager Navmgr { get; set; }

        [Parameter]
        public RenderFragment<RenderFragment> ChildContent { get; set; }
        protected override void Render(RenderTreeBuilder builder)
        {
            var layoutType = RouteData.PageType.GetCustomAttribute<LayoutAttribute>()?.LayoutType ?? DefaultLayout;

            var body = CreateBody(RouteData, Navmgr.Uri);

            builder.OpenComponent<CascadingValue<ReuseTabsRouteView>>(0);
            builder.AddAttribute(1, "Name", "RouteView");
            builder.AddAttribute(2, "Value", this);

            builder.AddAttribute(3, "ChildContent", (RenderFragment)(b =>
            {
                b.OpenComponent(20, layoutType);
                b.AddAttribute(21, "Body", body);
                b.CloseComponent();
            }));

            builder.CloseComponent();
            var url = "/" + Navmgr.ToBaseRelativePath(Navmgr.Uri);
            if (url != "/#")
            {
                var selTab = TabSetTool.Pages.FirstOrDefault(m => m.Url == url && (m.Title == TabSetTool.Title || string.IsNullOrEmpty(m.Title)));
                if (selTab == null)
                {
                    TabSetTool.Pages.Add(new Tab
                    {
                        Body = body,
                        Url = url,
                        Title = TabSetTool.Title,
                        IsActive = true,
                    });
                }
                else
                {
                    selTab.Title = TabSetTool.Title;
                    selTab.Body = body;
                    selTab.IsActive = true;
                }
            }
        }

        private RenderFragment CreateBody(RouteData routeData, string url)
        {
            return builder =>
            {
                builder.OpenComponent(0, routeData.PageType);
                foreach (var routeValue in routeData.RouteValues)
                {
                    builder.AddAttribute(1, routeValue.Key, routeValue.Value);
                }
                builder.CloseComponent();
            };
        }


    }
}
