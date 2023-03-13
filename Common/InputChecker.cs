using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TheoryOfTelevision.Common
{
    public class InputChecker
    {

        public static bool IsDigitKey(Key key)
        {
            var isNum = (key >= Key.NumPad0) && (key <= Key.NumPad9);
            var isDigit = (key >= Key.D0) && (key <= Key.D9);
            return isNum || isDigit;
        }

        public static bool CheckForEditableDigitField(Key key)
        {
            var isNumber = IsDigitKey(key);
            var isEditKey = (key == Key.Left) || (key == Key.Right) || (key == Key.Back) || (key == Key.Delete);
            return isNumber || isEditKey;
        }

    }
}
