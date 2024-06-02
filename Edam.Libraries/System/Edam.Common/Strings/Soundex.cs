using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Strings
{

    public class TextString
    {

        public static string Soundex(string data)
        {
            StringBuilder result = new StringBuilder();

            if (data != null && data.Length > 0)
            {
                string previousCode = "", currentCode = "", currentLetter = "";

                // keep initial char
                result.Append(data[0]);

                //start at 0 in order to correctly encode "Pf..."
                for (int i = 0; i < data.Length; i++)
                {
                    currentLetter = data[i].ToString().ToLower();
                    currentCode = "";

                    if ("bfpv".Contains(currentLetter))
                        currentCode = "1";
                    else if ("cgjkqsxz".Contains(currentLetter))
                        currentCode = "2";
                    else if ("dt".Contains(currentLetter))
                        currentCode = "3";
                    else if (currentLetter == "l")
                        currentCode = "4";
                    else if ("mn".Contains(currentLetter))
                        currentCode = "5";
                    else if (currentLetter == "r")
                        currentCode = "6";

                    // do not add first code to result string
                    if (currentCode != previousCode && i > 0)
                        result.Append(currentCode);

                    if (result.Length == 4) break;

                    // always retain previous code, even empty
                    previousCode = currentCode;
                }
            }
            if (result.Length < 4)
                result.Append(new string('0', 4 - result.Length));

            return result.ToString().ToUpper();
        }

    }

}
