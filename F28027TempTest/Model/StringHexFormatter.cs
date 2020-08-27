using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace F28027TempTest.Model
{
    static class StringHexFormatter
    {
        public static string Prettify(string str)
        {
            Regex rgxHexLetters = new Regex("([0-9A-F])+");
            string strHexLetters = "0123456789ABCDEF";
            string strSpacers = " -:";
            
            str = str.ToUpper();

            string tmp = "";
            foreach (char item in str)
            {
                if(strHexLetters.Contains(item) || strSpacers.Contains(item))
                {
                    tmp += item;
                }
            }

            return tmp;
        }

        public static bool IsGood(string str)
        {
            Regex rgxFullFormat = new Regex("((([0-9A-F]{2})[\\- ]?)|([0-9A-F]{1}))+");
            return rgxFullFormat.IsMatch(str);
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            Regex rgxSpacers = new Regex("[ \\-:]");
            hexString = rgxSpacers.Replace(hexString, "");
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}
