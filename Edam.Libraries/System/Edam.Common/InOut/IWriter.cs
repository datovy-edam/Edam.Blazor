using System;
using Edam.Diagnostics;

namespace Edam.InOut
{
   public interface IWriter
   {
      Object DataContext { get; set; }
      String FileExtension { get; set; }
      IResultsLog Open();
      IResultsLog Write(String name, String textContent);
      IResultsLog Close();
   }
}
