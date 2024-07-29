using Edam.Web.Components.Pages;
using Microsoft.AspNetCore.Components;

namespace Edam.Web.Models;

public class TabItemInfo
{
   public RenderFragment? Content { get; set; }
   public string Label { get; set; }
   public Guid Id { get; set; } = Guid.NewGuid();
   public bool ShowCloseIcon { get; set; } = true;

   public string? ValueText { get; set; } = null;
   public CodeEditorComponent? CodeEditor { get; set; } = null;

   public TabItemInfo(string? label = null)
   {
      Label = label ?? Id.ToString();
   }

}
