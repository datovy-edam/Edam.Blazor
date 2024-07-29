//using Edam.DataObjects.Trees;
using Edam.DataObjects.Trees;
using Edam.Web.FileSystemHelper;
using KristofferStrube.Blazor.FileSystem;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace Edam.Web.Components.Pages;

public partial class FileSystemComponent
{
   [Parameter]
   public string Title { get; set; } = "Projects";

   public class TreeItemData: TreeItemData<FileSystemItemInfo?>
   {

      public new bool Expanded
      {
         get { return Value.IsExpanded; }
         set { Value.IsExpanded = value; }
      }
      public new string Text
      {
         get { return Value.Title; }
         set { Value.Title = value; }
      }

      public new HashSet<TreeItemData<FileSystemItemInfo?>>? Children = 
         new HashSet<TreeItemData<FileSystemItemInfo?>>();

      public TreeItemData(FileSystemItemInfo? item)
      {
         Value = item;
      }
   }

   private bool Renaming { get; set; }
   private string RenamingText { get; set; }

   /// <summary>
   /// The current directory could be set while selecting a folder child item.
   /// </summary>
   private FileSystemItemInfo? _CurrentDirectory = null;
   private FileSystemItemInfo? _SeletedValue;
   private FileSystemItemInfo? SelectedValue
   {
      get { return _SeletedValue; }
      set { _SeletedValue = value == null ? _SeletedValue : value; }
   }

   private FileSystemDirectory Directory { get; set; }

   /// <summary>
   /// Root Tree Items.
   /// </summary>
   private HashSet<TreeItemData<FileSystemItemInfo?>> TreeItems { get; set; }

   private string SelectedTitle
   {
      get { return SelectedValue == null ? "" : SelectedValue.Title; }
   }
   private string SelectedName
   {
      get { return SelectedValue == null ? "" : SelectedValue.Title; }
   }

   /// <summary>
   /// Initialize Component Async...
   /// </summary>
   /// <returns></returns>
   protected override async Task OnInitializedAsync()
   {
      Directory = await FileSystemDirectory.InitializeDirectory(
         StorageManagerService);
      TreeItems = ToTreeItemData(Directory.Instance);
      await base.OnInitializedAsync();
   }

   /// <summary>
   /// Component After Render method.
   /// </summary>
   /// <param name="firstRender"></param>
   protected async override Task OnAfterRenderAsync(bool firstRender)
   {
      await base.OnAfterRenderAsync(firstRender);

      if (firstRender)
      {
         StateHasChanged();
      }

      //if (_stateHasChanged)
      //{
      //   _stateHasChanged = false;
      //   StateHasChanged();
      //}

   }

   /// <summary>
   /// Convert ToTreeItemData.
   /// </summary>
   /// <param name="itemData"></param>
   /// <param name="items"></param>
   /// <param name="parent"></param>
   /// <returns></returns>
   private HashSet<TreeItemData<FileSystemItemInfo>> ToTreeItemData(
      TreeItemData itemData, HashSet<TreeItemData<FileSystemItemInfo>> items,
      FileSystemItemInfo? parent)
   {
      items.Add(itemData);
      foreach(var item in parent.Children)
      {
         var childData = new TreeItemData(item);
         ToTreeItemData(childData, items, item);
      }
      return items;
   }

   /// <summary>
   /// Convert ToTreeItemData.
   /// </summary>
   /// <param name="items"></param>
   /// <returns></returns>
   private HashSet<TreeItemData<FileSystemItemInfo>>?
      ToTreeItemData(HashSet<FileSystemItemInfo?> items)
   {
      HashSet<TreeItemData<FileSystemItemInfo>> itms = 
         new HashSet<TreeItemData<FileSystemItemInfo>>();
      foreach (var item in items)
      {
         var itemData = new TreeItemData(item);
         ToTreeItemData(itemData, itms, item);
      }
      return itms;
   }

   /// <summary>
   /// Try to set current directory.
   /// </summary>
   /// <returns></returns>
   private bool TrySettingCurrentDirectory()
   {
      if (SelectedValue == null)
      {
         return false;
      }
      _CurrentDirectory = SelectedValue.Parent;
      return true;
   }

   /// <summary>
   /// Add a child directory in the current selected directory.
   /// </summary>
   /// <param name="args">event args</param>
   public async void OnAddDirectory(MouseEventArgs args)
   {
      await Directory.AddBranchAsync(SelectedValue);
      StateHasChanged();
   }

   /// <summary>
   /// Add a child file in the current selected directory.
   /// </summary>
   /// <param name="args"></param>
   public async void OnAddFile(MouseEventArgs args)
   {
      await Directory.AddItemAsync(SelectedValue);
      StateHasChanged();
   }

   public async void SaveItemAsync()
   {
      if (!TrySettingCurrentDirectory())
      {
         return;
      }


   }

   /// <summary>
   /// Delete the selected Item (a Directory or File).
   /// </summary>
   /// <param name="args">event args</param>
   public async void OnDeleteItem(MouseEventArgs args)
   {
      if (!TrySettingCurrentDirectory())
      {
         return;
      }

      var directoryHandle =
         _CurrentDirectory.Handle as FileSystemDirectoryHandle;

      //var item = await directoryHandle.ResolveAsync(SelectedValue.Handle);
      //if (item.Length == 0)
      //{
      //   return;
      //}

      //await directoryHandle.RemoveEntryAsync(
      //   SelectedValue.Name, new() { Recursive = true });

      //_CurrentDirectory.Children.Remove(SelectedValue);
      //_CurrentDirectory.InstanceChildren.Remove(SelectedValue.Handle);

      await Directory.DeleteItemAsync(_CurrentDirectory, SelectedValue);
      StateHasChanged();
   }

   /// <summary>
   /// Edit the content of the selected Item (something like a text file).
   /// </summary>
   /// <param name="args">event args</param>
   public void OnEditItem(MouseEventArgs args)
   {
      //this.editFileHandle = fileHandle;
      //var file = await fileHandle.GetFileAsync();
      //text = await file.TextAsync();
      //StateHasChanged();
   }

   public async void OnFileContentChange()
   {
      await Directory.WriteItemAsync(SelectedValue, "some data");
   }

   /// <summary>
   /// On text edit while renaming enter key was pressed... if so, stop
   /// renaming
   /// </summary>
   /// <param name="args"></param>
   public async void OnKeyDown(KeyboardEventArgs args)
   {
      if (args.Key == "Enter")
      {
         Renaming = false;
         StateHasChanged();
      }
   }

   /// <summary>
   /// On text edit while renaming enter key was pressed... if so, stop
   /// renaming
   /// </summary>
   /// <param name="args"></param>
   public async void OnBlur(FocusEventArgs args)
   {
      Renaming = false;
      SelectedValue.Title = RenamingText;

      Directory.RenameCatalogItemAsync(SelectedValue, RenamingText);

      RenamingText = String.Empty;
      StateHasChanged();
   }

   /// <summary>
   /// Rename the selected Item (a Directory or File).
   /// </summary>
   /// <param name="args">event args</param>
   public void OnRenameItem(MouseEventArgs e)
   {
      if (SelectedValue == null)
      {
         return;
      }
      Renaming = true;
      RenamingText = SelectedValue.Title;
   }

}