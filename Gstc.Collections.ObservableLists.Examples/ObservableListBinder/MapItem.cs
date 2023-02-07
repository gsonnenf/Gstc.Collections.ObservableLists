using System.ComponentModel;
using System.Text.RegularExpressions;
using Gstc.Collections.ObservableLists.Binding;

namespace Gstc.Collections.ObservableLists.Examples.ObservableListBinder {
    public class MapItem : ICustomPropertyMap<IItemA, IItemB> {
        public static string ConvertNum1(int num) => "Number: " + num.ToString();
        public static int ConvertNum1(string numString) {
            var reduced = Regex.Replace(numString, "[^0-9]", "");
            var isSuccess = int.TryParse(reduced, out var result);
            return isSuccess ? result : 0;
        }
        public static int ConvertNum2(int num) => num * -1;
        public void PropertyChangedSourceToTarget(PropertyChangedEventArgs args, IItemA itemS, IItemB itemT) {
            if (args.PropertyName == nameof(itemS.Num1)) itemT.Num1String = ConvertNum1(itemS.Num1);
            else if (args.PropertyName == nameof(itemS.Num2)) itemT.Num2 = ConvertNum2(itemS.Num2);
        }

        public void PropertyChangedTargetToSource(PropertyChangedEventArgs args, IItemB itemT, IItemA itemS) {
            if (args.PropertyName == nameof(itemT.Num1String)) itemS.Num1 = ConvertNum1(itemT.Num1String);
            else if (args.PropertyName == nameof(itemT.Num2)) itemS.Num2 = ConvertNum2(itemT.Num2);
        }
    }
}
