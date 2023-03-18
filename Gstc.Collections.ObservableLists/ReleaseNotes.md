##Version 2.0.0
Complete refactor. See documentation.

##Version 2.0.1     
     Features:
      a. IReadOnlyList{T} added to IObservableList and implementations.
      b. IList interface now does type checking similar to List{T}
      c. IList interface added back into ObservableIListLocking
      d. Utils folder added, with public SyncingFlagScope, ReentrancyMonitor and LockRwScope mechanism that were previously implemented internally in classes.
      e. ObservableList{T} is now internally a type of ObservableList{T,List{T}}, addRange/Move are now ovveridable virtual methods.


      Bugfixes:
      a. ObservableListBind inProgress flag has been made exception safe.
      b. int IList.Add(object) now calls IList_AddCustom(TItem) which is overridable. This allows custom index return values.

      Minor breaking changes:
      a. ObservableIListLocking moved to main namespace.
      b. ObservableLists now throws ReentrancyException derived from InvalidOperationException, instead of InvalidOperationException
      c. ObservableListBinds now throws added type OneWayBindingException derived from NotSupportedException, instead of NotSupportedException
      d. The names of some abstract classes have been changed.
      e. Names of internally locking mechanisms changed, refactored.
      f. Some classes use 'default' instead of 'null'.

      Other:
      Updated unit tests.
      Some benchmark code.
      fixed typos.
