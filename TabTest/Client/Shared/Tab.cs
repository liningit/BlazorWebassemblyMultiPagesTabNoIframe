using System;
using Microsoft.AspNetCore.Components;

namespace TabTest.Client.Shared
{
    public class Tab
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Style { get { return IsActive ? "height:100%;width:100%" : "display:none"; } }
        public int TabWidth { get; set; }
        public RenderFragment Body { get; set; }
    }
}
