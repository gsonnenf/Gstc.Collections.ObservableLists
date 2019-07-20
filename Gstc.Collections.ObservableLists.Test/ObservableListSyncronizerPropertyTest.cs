using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gstc.Collections.ObservableLists.Test {
    [TestFixture]
    public class ObservableListSyncronizerPropertyTest {

        public ObservableList<SourceItemA> SourceObvListA;
        public ObservableList<DestItemA> DestObvListA;
        public ObservableListSynchronizer<SourceItemA, DestItemA> ObvListSyncA;

        [SetUp]
        public void TestInit() {

        }

        [Test, Description("Test 2-way Notify between properties.")]
        public void TestMethod_CopyOnInitialize() {
            SourceObvListA = getSampleSourceList();
            DestObvListA = new ObservableList<DestItemA>();

            ObvListSyncA = new ObservableListSynchronizerFunc<SourceItemA, DestItemA>(
                (sourceItem) => new DestItemA { MyNum = sourceItem.MyNum.ToString(), MyStringUpper = sourceItem.MyStringLower.ToUpper() },
                (destItem) => new SourceItemA { MyNum = int.Parse(destItem.MyNum), MyStringLower = destItem.MyStringUpper.ToLower() },
                SourceObvListA,
                DestObvListA,
                true,
                true
            );

            



        }






            #region Test Helpers

            private ObservableList<SourceItemA> getSampleSourceList() {
            return new ObservableList<SourceItemA> {
                new SourceItemA { MyNum = 10, MyStringLower = "x" },
                new SourceItemA { MyNum = 15, MyStringLower = "y" },
                new SourceItemA { MyNum = 20, MyStringLower = "z" },
                };
        }

        private ObservableList<DestItemA> getSampleDestList() {
            return new ObservableList<DestItemA> {
                new DestItemA { MyNum = "1000", MyStringUpper = "A" },
                new DestItemA { MyNum = "1500", MyStringUpper = "B" },
                new DestItemA { MyNum = "2000", MyStringUpper = "C" },
                new DestItemA { MyNum = "3000", MyStringUpper = "D" },
                };
        }



        public class SourceItemA {
            public int MyNum { get; set; }
            public string MyStringLower { get; set; }

            public override bool Equals(object obj) {
                var temp = obj as SourceItemA;
                if (temp == null) return false;
                if (temp.MyNum == MyNum && temp.MyStringLower == MyStringLower) return true;
                return false;
            }
        }

        public class DestItemA {
            public string MyNum { get; set; }
            public string MyStringUpper { get; set; }
            public override bool Equals(object obj) {
                var temp = obj as DestItemA;
                if (temp == null) return false;
                if (temp.MyNum == MyNum && temp.MyStringUpper == MyStringUpper) return true;
                return false;
            }
        }
        #endregion
    }
}
