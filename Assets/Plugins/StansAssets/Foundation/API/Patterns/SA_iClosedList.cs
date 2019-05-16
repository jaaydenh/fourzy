using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Events;

namespace SA.Foundation.Patterns
{
    public interface SA_iClosedList<T>
    {
        SA_iEvent<T> OnItemAdded { get; }
        SA_iEvent<T> OnItemRemoved { get; }

        IEnumerable<T> Items { get; }

    }
}