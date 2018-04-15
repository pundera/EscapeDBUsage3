using EscapeDBUsage.UIClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.FlowStructures
{
    public class SelectionStructureXXX
    {
        //public SelectionStructure(bool isSelected, NodeBase node, bool isJustRoot = false)
        //{
        //    IsSelected = isSelected;
        //    Node = node;

        //    IsJustParent = isJustRoot;

        //    CreateStructureFromNode();
        //}

        //public NodeBase Node { get; set; }

        //public bool IsSelected { get; set; }
        //public SelectionStructure Parent { get; set; }
        //public List<SelectionStructure> Children { get; set; }
        //public bool IsJustParent { get; private set; }

        //public List<SelectionStructure> GetPathList()
        //{
        //    var ret = new List<SelectionStructure>();
        //    if (Parent != null) ret.AddRange(GetPathList());
        //    return ret;
        //}

        //public List<SelectionStructure> GetAllParents(NodeBase node)
        //{
        //    var ret = new List<SelectionStructure>();

        //    SelectionStructure parent = null;
        //    var nodeParent = node.GetParent();
        //    if (nodeParent != null)
        //        parent = new SelectionStructure(nodeParent.IsSelected, nodeParent) { Children = nodeParent.GetNodes().Select(x => new SelectionStructure(x.IsSelected, nodeParent)).ToList()};
        //    ret.Add(parent);
        //    ret.AddRange(GetAllParents(nodeParent));

        //    return ret;
        //}

        //public void CreateStructureFromNode()
        //{
        //    SelectionStructure parent = null;

        //    Logger.Logger.Log.Info(string.Format("Node --> {0}", Node.Name));

        //    if (Parent == null)
        //    {

        //        var nodeParent = Node.GetParent();
        //        if (nodeParent != null) parent = new SelectionStructure(nodeParent.IsSelected, nodeParent, true);
        //        this.Parent = parent;
        //    }

        //    if (IsJustParent) return;

        //    var nodesChildren = Node.GetNodes();
        //    if (nodesChildren != null && nodesChildren.Count>0)
        //    {

        //        this.Children = nodesChildren.Select(x => new SelectionStructure(x.IsSelected, x)).ToList();
        //    }   


        //}
    }
}
