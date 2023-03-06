using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakpointManager.Common
{
  public abstract class TreeNode
  {
    public abstract string DisplayName { get; }

    public abstract ItemType ItemType { get; }
  }

  public class SolutionNode : TreeNode
  {
    public SolutionNode(string name)
    {
      this.Name = name;
    }

    private string Name { get; set; }

    public override string DisplayName
    {
      get { return this.Name; }
    }

    public override ItemType ItemType
    {
      get { return ItemType.Solution; }
    }
  }

  public class ProjectNode : TreeNode
  {
    public ProjectNode(string name)
    {
      this.Name = name;
    }

    public string Name { get; set; }

    public override string DisplayName
    {
      get { return this.Name; }
    }

    public override ItemType ItemType
    {
      get { return ItemType.Project; }
    }
  }

  public class FileNode : TreeNode
  {
    public FileNode(string name)
    {
      this.Name = name;
    }

    public string Name { get; set; }

    public override string DisplayName
    {
      get { return this.Name; }
    }

    public override ItemType ItemType
    {
      get { return ItemType.File; }
    }
  }

  public class PublicMethodNode : TreeNode
  {
    public PublicMethodNode(EntryPoint m, string filePath)
    {
      this.Name = m.Name;
      this.FilePath = filePath;
      this.LineNo = m.LineNo + 1;
    }

    public override string DisplayName
    {
      get { return Name; }
    }

    public override ItemType ItemType
    {
      get { return ItemType.Method; }
    }

    public Breakpoint2 Breakpoint { get; set; }

    public string Name { get; set; }
    public string FilePath { get; set; }
    public int LineNo { get; set; }
  }
}
