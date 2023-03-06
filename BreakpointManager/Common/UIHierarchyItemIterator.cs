using EnvDTE;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BreakpointManager.Common
{
  public sealed class UIHierarchyItemIterator : IEnumerable<UIHierarchyItem>
  {
    UIHierarchyItems items;

    /// <summary>
    /// Initializes a new instance of the <see cref="UIHierarchyItemIterator"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    public UIHierarchyItemIterator(UIHierarchyItems items)
    {
      if (items == null)
      {
        throw new ArgumentNullException("items");
      }

      this.items = items;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<UIHierarchyItem> GetEnumerator()
    {
      return (Enumerate(items)
          .Select(item => item)).GetEnumerator();
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private IEnumerable<UIHierarchyItem> Enumerate(UIHierarchyItems items)
    {
      foreach (UIHierarchyItem item in items)
      {
        yield return item;

        foreach (UIHierarchyItem subItem in Enumerate(item.UIHierarchyItems))
        {
          yield return subItem;
        }
      }
    }
  }
}