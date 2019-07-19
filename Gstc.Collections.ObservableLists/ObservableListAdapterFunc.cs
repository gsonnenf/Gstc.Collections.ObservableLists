using System;

namespace Gstc.Collections.ObservableLists {

    /// <summary>
    /// This class is an implementation of the abstract class ObservableListAdapter with the
    /// Convert method defined at instantiation using an anonymous function in the constructor.
    ///
    /// For Details see the ObservableListAdapter class. 
    /// </summary>
    /// <typeparam name="TInput">Source element type (e.g. Model class)</typeparam>
    /// <typeparam name="TOutput">Destination element type (e.g. ViewModel class</typeparam>
    public class ObservableListAdapterFunc<TInput, TOutput> : ObservableListAdapter<TInput, TOutput> {

        private Func<TInput, TOutput> _convert;

        private Func<TOutput, TInput> _convertBack;


        public ObservableListAdapterFunc(Func<TInput, TOutput> convert, Func<TOutput, TInput> convertBack) {
            _convert = convert;
            _convertBack = convertBack;
        }

        public ObservableListAdapterFunc(IObservableCollection<TInput> sourceCollection, Func<TInput, TOutput> convert, Func<TOutput, TInput> convertBack) : base(sourceCollection) {
            _convert = convert;
            _convertBack = convertBack;
        }

        public override TOutput Convert(TInput item) => _convert(item);

        public override TInput Convert(TOutput item) => _convertBack(item);


    }
}
