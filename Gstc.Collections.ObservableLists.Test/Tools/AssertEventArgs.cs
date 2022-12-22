using System.Collections.Specialized;
using NUnit.Framework;

namespace Gstc.Collections.ObservableLists.Test.Tools;

public class AssertEventArgs<TItem> {
    public void OnCollectionChanged_Reset(object sender, NotifyCollectionChangedEventArgs args) {
        Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
        Assert.That(args.OldItems, Is.Null);
        Assert.That(args.NewItems, Is.Null);
        Assert.That(args.OldStartingIndex == -1);
        Assert.That(args.NewStartingIndex == -1);
    }

    public NotifyCollectionChangedEventHandler OnCollectionChanged_Add(int index, TItem item) =>
        (_, args) => {
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
            Assert.That(args.OldStartingIndex == -1);
            Assert.That(args.NewStartingIndex == index);
            Assert.That(args.OldItems, Is.Null);
            Assert.That(args.NewItems[0], Is.EqualTo(item));
        };

    public NotifyCollectionChangedEventHandler OnCollectionChanged_AddRange3(int startingIndex, TItem item1, TItem item2, TItem item3) =>
        (sender, args) => {
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
            Assert.That(args.OldStartingIndex, Is.EqualTo(-1));
            Assert.That(args.NewStartingIndex, Is.EqualTo(startingIndex));
            Assert.That(args.OldItems, Is.Null);
            Assert.That(args.NewItems[0], Is.EqualTo(item1));
            Assert.That(args.NewItems[1], Is.EqualTo(item2));
            Assert.That(args.NewItems[2], Is.EqualTo(item3));
        };

    public NotifyCollectionChangedEventHandler OnCollectionChanged_Moved(TItem item, int newIndex, int oldIndex) =>
        (sender, args) => {
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Move));
            Assert.That(args.OldStartingIndex, Is.EqualTo(oldIndex));
            Assert.That(args.NewStartingIndex, Is.EqualTo(newIndex));
            Assert.That(args.OldItems[0], Is.EqualTo(item));
            Assert.That(args.NewItems[0], Is.EqualTo(item));
        };

    public NotifyCollectionChangedEventHandler OnCollectionChanged_Removed(int index, TItem item) =>
        (sender, args) => {
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
            Assert.That(args.OldStartingIndex == index);
            Assert.That(args.NewStartingIndex == -1);
            Assert.That(args.OldItems[0], Is.EqualTo(item));
            Assert.That(args.NewItems, Is.Null);
        };

    public NotifyCollectionChangedEventHandler OnCollectionChanged_Replace(int startingIndex, TItem oldItem, TItem newItem) =>
        (sender, args) => {
            Assert.That(args.Action, Is.EqualTo(NotifyCollectionChangedAction.Replace));
            Assert.That(args.OldStartingIndex, Is.EqualTo(startingIndex));
            Assert.That(args.NewStartingIndex, Is.EqualTo(startingIndex));
            Assert.That(args.OldItems[0], Is.EqualTo(oldItem));
            Assert.That(args.NewItems[0], Is.EqualTo(newItem));
        };
}
