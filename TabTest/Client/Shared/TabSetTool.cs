using Microsoft.AspNetCore.Components;

namespace TabTest.Client.Shared
{
    public class TabSetTool
    {
        public TabSetTool(NavigationManager navigationManager)
        {
            NavigationManager = navigationManager;
        }

        public NavigationManager NavigationManager { get; set; }

        public List<Tab> Pages { get; } = new();

        public async Task CloseActiveTab()
        {
            var index = Pages.FindIndex(m => m.IsActive);
            var closeTab = Pages[index];
            Pages.RemoveAt(index);
            Tab activeTab = null;
            if (index > 0)
            {
                activeTab = Pages[index - 1];
            }
            else if (Pages.Count > 0)
            {
                activeTab = Pages[0];
            }
            if (activeTab != null)
            {
                activeTab.IsActive = true;
                activeTab.TabWidth = 0;
                AddTab(activeTab.Title, activeTab.Url);
            }
            else
            {
                AddTab("首页", "/");
            }
            if (closeTab?.OnClose != null)
            {
                await closeTab.OnClose();
            }
        }
        public void GoTo(string title, string url, Func<Task> onClose = null)
        {
            var index = Pages.FindIndex(m => m.IsActive);
            if (index < 0)
            {
                AddTab(title, url);
                return;
            }
            else
            {
                var ex = Pages.FirstOrDefault(m => m.Url == url && (m.Title == title || string.IsNullOrEmpty(m.Title)));
                if (ex != null && ex != Pages[index])
                {
                    Pages.RemoveAt(index);
                    AddTab(title, url);
                    return;
                }
                Pages[index] = new Tab
                {
                    Url = url,
                    Title = title,
                    IsActive = true,
                    OnClose = onClose
                };
                NavigationManager.NavigateTo(url);
            }
        }
        public void AddTab(string title, string url, bool isScrollToTab)
        {
            AddTab(title, url, null, isScrollToTab);
        }
        public void AddTab(string title, string url, Func<Task> onClose = null, bool isScrollToTab = true)
        {
            Pages.ForEach(x =>
            {
                x.IsActive = false;
            });
            var selTab = Pages.FirstOrDefault(m => m.Url == url && (m.Title == title || string.IsNullOrEmpty(m.Title)));
            if (selTab == null)
            {
                Pages.Add(new Tab
                {
                    Url = url,
                    Title = title,
                    IsActive = true,
                    OnClose = onClose
                });
            }
            else
            {
                if (isScrollToTab)
                {
                    selTab.TabWidth = 0;
                }
                if (string.IsNullOrEmpty(selTab.Title))
                {
                    selTab.Title = title;
                }
                selTab.OnClose = onClose;
                selTab.IsActive = true;
            }
            NavigationManager.NavigateTo(url);
        }
    }
}
