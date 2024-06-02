using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -----------------------------------------------------------------------------

namespace Edam.Text
{

   public class TextFile
   {
      public List<string> Lines { get; set; }

      public void FromFile(string filePath)
      {
         Lines = System.IO.File.ReadAllLines(filePath).ToList<string>();
      }
      public void ToConsole()
      {
         Console.WriteLine(Lines);
      }
      public void ToFile(string filePath)
      {
         System.IO.File.WriteAllLines(filePath,Lines);
      }
      public TextFile MixCaseToUnderscoredWords(
         TextTokenOption option = TextTokenOption.Unknown)
      {
         TextFile file = new TextFile();
         file.Lines = new List<string>();

         TextLine l;
         foreach (string line in Lines)
         {
            l = new TextLine(line);
            l.Tokenize();
            file.Lines.Add(l.MixCaseToUnderscoredWords(option));
         }
         return file;
      }
   }

}
