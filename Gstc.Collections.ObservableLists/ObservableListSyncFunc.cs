using System;
using System.Collections.Generic;

namespace Gstc.Collections.ObservableLists {

    /// <summary>
    /// This class is an implementation of the abstract class ObservableListAdapter with the
    /// Convert method defined at instantiation using an anonymous function in the constructor.
    ///
    /// For Details see the ObservableListAdapter class. 
    /// </summary>
    /// <typeparam name="TInput">Source element type (e.g. Model class)</typeparam>
    /// <typeparam name="TOutput">Destination element type (e.g. ViewModel class</typeparam>
    public class ObservableListSyncFunc<TInput, TOutput> : ObservableListSync<TInput, TOutput> {


        /// <summary>
        /// Generates an ObservableList of type TOutput that is synchronized to a list of TInput. 
        /// This method wraps a conventional List{TInput} in an ObservableList{TInput}, then generates an
        /// an ObservableList{TOutput}. The ObservableList{TInput} is accessible via SourceCollection property.
        /// Added or removed items are propagated back to the original list.
        /// It is recommended that TOutput is a passthrough map to TInput.
        /// </summary>
        /// <param name="list">A list to make observable and convert to type TOutput.</param>
        /// <param name="convert">Conversion method from TInput to TOutput</param>
        /// <param name="convertBack">Conversion method from TOutput to TInput</param>
        /// <returns></returns>
        public static ObservableListSync<TInput, TOutput> FromList(List<TInput> list, Func<TInput, TOutput> convert, Func<TOutput, TInput> convertBack) {
            var obvList = new ObservableList<TInput>(list);
            return new ObservableListSyncFunc<TInput, TOutput>(obvList, convert, convertBack);
        }

        private readonly Func<TInput, TOutput> _convert;
        private readonly Func<TOutput, TInput> _convertBack;


        /// <summary>
        /// Creates a Observable list that automatically synchronizes between items of type TInput to TOutput. 
        /// An input list of type ObservableList should be provided after initialization.
        /// </summary>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        public ObservableListSyncFunc(Func<TInput, TOutput> convert, Func<TOutput, TInput> convertBack) : base() {
            _convert = convert;
            _convertBack = convertBack;
        }

        /// <summary>
        ///  Creates a Observable list that automatically synchronizes Observable lists between items of type TInput to TOutput. 
        /// </summary>
        /// <param name="sourceObservableList"></param>
        /// <param name="convert"></param>
        /// <param name="convertBack"></param>
        public ObservableListSyncFunc(ObservableList<TInput> sourceObservableList, Func<TInput, TOutput> convert, Func<TOutput, TInput> convertBack) : base(sourceObservableList) {
            _convert = convert;
            _convertBack = convertBack;
        }

        /// <summary>
        /// Converts item of type TInput to TOutput.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override TOutput Convert(TInput item) => _convert(item);

        /// <summary>
        /// Converts item of type TOutput to TInput.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override TInput ConvertBack(TOutput item) => _convertBack(item);


    }
}
