using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.UIClasses.DatabaseSchema
{
    public class NodeDbSchemaBase: BindableBase
    {

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private bool isChecked = false;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value); }
        }

        private bool isExpanded = false;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetProperty(ref isExpanded, value); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                SetProperty(ref isSelected, value);
                SelectedNode = this;
            }
        }

        private NodeDbSchemaBase selectedNode;
        public NodeDbSchemaBase SelectedNode
        {
            get { return selectedNode; }
            set
            {
                selectedNode = value;
                //EventAggregator.GetEvent<EventSelectedChanged>().Publish(value);
            }
        }

        private bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetProperty(ref isVisible, value); }
        }


    }
}
