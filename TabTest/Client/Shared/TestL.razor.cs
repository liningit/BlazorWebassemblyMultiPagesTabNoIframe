using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using TabTest.Client.Pages;

namespace TabTest.Client.Shared
{
    public partial class TestL
    {
        [Inject] private NavigationManager NavigationManager { get; set; }
        public RenderFragment Body { get; set; }
        public async Task cli()
        {            
            Body = builder =>
             {
                 builder.OpenElement(0, "div");
                 
                 //builder.AddContent(1, Router.)
                 builder.CloseElement();
             };
        }
    }
}
