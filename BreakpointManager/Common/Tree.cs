using Microsoft.Build.Framework.XamlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakpointManager.Common
{
  public class Tree<T> : IEnumerable<Tree<T>>
  {
    public Tree(T text, ItemType itemType)
    {
      this.ItemType = itemType;
      this.Node = text;
      this.Children = new LinkedList<Tree<T>>();
    }

    public ItemType ItemType { get; set; }
    public T Node { get; set; }
    public Tree<T> Parent { get; set; }
    public ICollection<Tree<T>> Children { get; set; }
    public bool Checked { get; set; }

    public IEnumerator<Tree<T>> GetEnumerator()
    {
      yield return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public Tree<T> AddChild(T child, ItemType itemType)
    {
      Tree<T> childNode = new Tree<T>(child, itemType) { Parent = this };
      this.Children.Add(childNode);
      return childNode;
    }

    public Tree<T> CreateNode(T child, ItemType itemType)
    {
      Tree<T> childNode = new Tree<T>(child, itemType) { Parent = this };
      return childNode;
    }

    public Tree<T> AppendChildNode(Tree<T> childNode)
    {
      this.Children.Add(childNode);
      return childNode;
    }
  }
}
