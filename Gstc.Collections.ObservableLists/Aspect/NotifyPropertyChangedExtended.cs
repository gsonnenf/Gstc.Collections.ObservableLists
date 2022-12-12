/*
using Gstc.Collections.ObservableLists.ComponentModel;
using System.ComponentModel;

namespace Gstc.Collections.ObservableLists.Notify {

    /// <summary>
    /// Provides functionality for generating Property changed events on collections.
    /// When reentrancy is enabled (it is not currently enabled) it will create a threadsafe list.
    /// </summary>
    public class NotifyPropertyChangedExtended : IListPropertyChangedHooks {

        #region Fields and Properties
        protected const string CountString = "Count";
        protected const string IndexerName = "Item[]";
        public event PropertyChangedEventHandler PropertyChanged;
        public object Sender { get; set; }
        #endregion

        public NotifyPropertyChangedExtended() { }

        public NotifyPropertyChangedExtended(object sender) { Sender = sender; }

        public virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(Sender, e);

        #region Methods
        public void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        public void OnPropertyChangedCountAndIndex() {
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
        }

        public void OnPropertyChangedIndex() {
            OnPropertyChanged(IndexerName);
        }
        #endregion

        //TODO: Check if needed PropertyChanged Reentrancy protection.

    }
}
*/
