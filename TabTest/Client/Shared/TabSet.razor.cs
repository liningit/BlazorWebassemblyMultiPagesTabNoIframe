using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TabTest.Client.Shared
{
    public partial class TabSet : ComponentBase
    {

        [Inject]
        IJSRuntime JS { get; set; }
        [Inject]
        public TabSetTool TabSetTool { get; set; }

        [CascadingParameter(Name = "RouteView")]
        public ReuseTabsRouteView RouteView { get; set; }
        public List<Tab> Tabs => TabSetTool.Pages;

        private int OffsetLeft { get; set; }

        private async Task MoveLeft()
        {
            var tabsWidth = await JS.InvokeAsync<int>("GetWidth", ".middletab");
            MoveTo(OffsetLeft - tabsWidth);
        }
        private async Task MoveRight()
        {
            var tabsWidth = await JS.InvokeAsync<int>("GetWidth", ".middletab");

            var moveS = OffsetLeft + tabsWidth;
            var temp = 0;
            Tab soTab = null;
            foreach (var tab in Tabs)
            {
                temp += tab.TabWidth;
                if (temp > moveS)
                {
                    soTab = tab;
                    break;
                }
            }
            if (soTab == null)
            {
                soTab = Tabs.LastOrDefault();
            }
            if (soTab != null)
            {
                await ScrollToTab(soTab);
            }

        }
        private async Task CloseTab(Tab tab)
        {
            var index = Tabs.IndexOf(tab);
            Tabs.Remove(tab);

            if (tab.IsActive)
            {
                Tab activeTab = null;
                if (index > 0)
                {
                    activeTab = Tabs[index - 1];
                }
                else if (Tabs.Count > 0)
                {
                    activeTab = Tabs[0];
                }
                if (activeTab != null)
                {
                    await ActivateTab(activeTab);
                }
            }
        }
        private async Task CloseOther()
        {

            Tabs.RemoveAll(m => !m.IsActive);
            if (Tabs.Count > 0)
            {
                await ScrollToTab(Tabs.FirstOrDefault());
            }
        }
        private async Task ActivateTab(Tab tab)
        {
            var tabsWidth = await JS.InvokeAsync<int>("GetWidth", ".middletab");
            var index = Tabs.IndexOf(tab);
            var sumWidth = Tabs.Take(index).Sum(m => m.TabWidth);
            var tabWidth = tab.TabWidth == 0 ? GetLength(tab.Title) * 8 + 20 : tab.TabWidth;
            if (sumWidth <= OffsetLeft || sumWidth + tabWidth >= OffsetLeft + tabsWidth)
            {
                var pre = index - 1;
                if (pre < 0)
                {
                    pre = 0;
                }
                await ScrollToTab(Tabs[pre]);
            }
            TabSetTool.AddTab(tab.Title, tab.Url);

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Tabs == null)
            {
                return;
            }
            var hasAdd = false;
            for (var i = 0; i < Tabs.Count; i++)
            {
                var item = Tabs[i];
                if (item.TabWidth <= 0)
                {
                    hasAdd = true;
                    item.TabWidth = await JS.InvokeAsync<int>("GetTabWidth", i);
                }
            }
            if (hasAdd)
            {
                await ScrollToTab(Tabs.FirstOrDefault(m => m.IsActive));
                StateHasChanged();
            }
        }

        public async Task ScrollToTab(Tab tab)
        {
            var top = Tabs.IndexOf(tab);
            var offset = Tabs.Take(top).Sum(m => m.TabWidth);
            var other = Tabs.Skip(top).Sum(m => m.TabWidth == 0 ? GetLength(m.Title) * 8 + 20 : m.TabWidth);

            var tabsWidth = await JS.InvokeAsync<int>("GetWidth", ".middletab");
            if (offset + other < tabsWidth - 100)
            {
                OffsetLeft = 0;
                return;
            }
            if (other < tabsWidth - 100)
            {
                MoveTo(offset + other - tabsWidth + 100);
                return;
            }
            OffsetLeft = offset;
        }
        public static int GetLength(string str)
        {

            if (str.Length == 0) return 0;

            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }

            return tempLen;
        }
        private void MoveTo(int x)
        {
            if (x <= 0)
            {
                OffsetLeft = 0;
                return;
            }
            var temp = 0;
            bool isSet = false;
            foreach (var item in Tabs)
            {
                if (temp + item.TabWidth > x)
                {
                    isSet = true;
                    temp += item.TabWidth;
                    break;
                }
                temp += item.TabWidth;
            }
            if (isSet)
            {
                OffsetLeft = temp;
            }
            else
            {
                OffsetLeft = x;
            }
        }
    }
}
