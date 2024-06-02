using System;
using System.IO;

// -----------------------------------------------------------------------------
using Edam.Diagnostics;

namespace Edam.InOut
{

   public class FolderWriter : IWriter
   {
      // TODO: simplify using FolderInfo
      private String m_FolderFullPath;
      private String m_FolderPath;
      private String m_FolderName;

      public Object DataContext { get; set; }
      public String FileExtension { get; set; }

      public FolderWriter(
         String folderPath, String folderName, String fileExtension,
         Object dataContext = null)
      {
         Initialization(folderPath, folderName, fileExtension, dataContext);
      }

      public FolderWriter(FileInfo folder, Object dataContext = null)
      {
         Initialization(
            folder.Path, folder.Name, folder.Extension, dataContext);
      }

      public void Initialization(
         String folderPath, String folderName, String fileExtension,
         Object dataContext = null)
      {
         DataContext = dataContext;
         m_FolderPath = folderPath ?? ".";
         m_FolderName = folderName ?? "TempFolder";
         m_FolderFullPath = m_FolderPath + "/" + m_FolderName;
         m_FolderFullPath = m_FolderFullPath.Replace("//", "/");
         FileExtension = fileExtension ?? "fwd";
      }

      public IResultsLog Open()
      {
         ResultLog result = new ResultLog();
         if (Directory.Exists(m_FolderFullPath))
         {
            result.Succeeded();
            return result;
         }

         try
         {
            Directory.CreateDirectory(m_FolderFullPath);
            result.Succeeded();
         }
         catch(Exception e)
         {
            result.Failed(e);
         }
         return result;
      }

      public IResultsLog Write(String name, String textContent)
      {
         ResultLog result = new ResultLog();

         try
         {
            string fpath = 
               m_FolderFullPath + "/" + name + "." + FileExtension;
            File.WriteAllText(fpath, textContent);
            result.Succeeded();
         }
         catch (Exception e)
         {
            result.Failed(e);
         }
         return result;
      }

      public IResultsLog Write(String textContent)
      {
         return Write(m_FolderName, textContent);
      }

      public IResultsLog Close()
      {
         ResultLog result = new ResultLog();
         result.Succeeded();
         return result;
      }

   }

}
