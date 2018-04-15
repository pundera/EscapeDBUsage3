using EscapeDBUsage.Events;
using EscapeDBUsage.FlowStructures;
using EscapeDBUsage.Helpers;
using EscapeDBUsage.InteractionRequests;
using EscapeDBUsage.UIClasses;
using EscapeDBUsage.UIClasses.OtherViews;
using log4net;
using log4net.Repository.Hierarchy;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EscapeDBUsage.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel(IEventAggregator evAgg, UISprints sprints, DatabaseSchemaViewModel dataViewModel)
        {
            DataViewModel = dataViewModel;
            Sprints = sprints.Sprints;
            eventAgg = evAgg;

            rootTables = new NodeDbTableRoot(evAgg)
            {
                Name = "ROOT for DB Tables",
                Description = "just helper instance...",
                Nodes = new ObservableCollection<NodeDbTableToExcel>()
            };

            eventAgg.GetEvent<PathViewAddedEvent>().Subscribe(() => {
                Task.Run(() =>
                {
                    DoLoad();
                });
            });

            evAgg.GetEvent<SelectionChangedEvent>().Subscribe(SelectionChanged());
            evAgg.GetEvent<SelectedInPathChangedEvent>().Subscribe(SelectionChanged());


            Import = new DelegateCommand(() => DoImport());
            Save = new DelegateCommand(() => DoSave());
            Load = new DelegateCommand(() => DoLoad());

            ShowLog = new DelegateCommand(() =>
            {
                Process process = new Process();
                // Configure the process using the StartInfo properties.
                var rootAppender = ((Hierarchy)LogManager.GetRepository())
                                         .Root.Appenders.OfType<log4net.Appender.FileAppender>()
                                         .FirstOrDefault();

                string filename = rootAppender != null ? rootAppender.File : string.Empty;
                process.StartInfo.FileName = rootAppender.File;
                //process.StartInfo.Arguments = @"c:\!! LOGs\EscapeDBUsage.log";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                process.Start();
                //process.WaitForExit();// Waits here for the process to exit.
            });

            ShowDataFolder = new DelegateCommand(() =>
            {
                Process process = new Process();
                // Configure the process using the StartInfo properties.
                process.StartInfo.FileName = "explorer";
                process.StartInfo.Arguments = FoldersHelper.DataFolder;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                process.Start();
                //process.WaitForExit();// Waits here for the process to exit.
            });

            Refresh = new DelegateCommand(() => DoRefresh());
            RefreshColumns = new DelegateCommand(() => DoRefreshColumns());

            ExpandAll = new DelegateCommand(() => DoExpandAll());
            CollapseAll = new DelegateCommand(() => DoCollapseAll());

            SaveSprints = new DelegateCommand(() => DoSaveSprints());

            AddExcel = new DelegateCommand(() => DoAddExcel());

            AddTables = new DelegateCommand(() => DoAddTables());

        }

        public Action<NodeBase> SelectionChanged()
        {
            return node =>
            {

                try
                {

                    eventAgg.GetEvent<SelectionChangedEvent>().Unsubscribe(SelectionChanged());
                    eventAgg.GetEvent<SelectedInPathChangedEvent>().Unsubscribe(SelectionChanged());



                    SelectedExcel = null;
                    SelectedTab = null;
                    SelectedDbTable = null;
                    SelectedDbColumn = null;

                    ExcelVisible = false;
                    TabVisible = false;
                    TableVisible = false;
                    ColumnVisible = false;

                    if (node is NodeRoot)
                    {
                        Root = node as NodeRoot;
                    }

                    if (node is NodeExcel)
                    {
                        SelectedExcel = node as NodeExcel;
                        ExcelVisible = true;
                    }
                    if (node is NodeTab)
                    {
                        SelectedTab = node as NodeTab;
                        SelectedExcel = (node as NodeTab).NodeExcel;
                        ExcelVisible = true;
                        TabVisible = true;
                    }
                    if (node is NodeDbTable)
                    {
                        SelectedDbTable = node as NodeDbTable;
                        SelectedTab = (node as NodeDbTable).NodeTab;
                        SelectedExcel = (node as NodeDbTable).NodeTab.NodeExcel;
                        ExcelVisible = true;
                        TabVisible = true;
                        TableVisible = true;
                    }
                    if (node is NodeDbColumn)
                    {
                        SelectedDbColumn = node as NodeDbColumn;
                        SelectedDbTable = (node as NodeDbColumn).NodeDbTable;
                        SelectedTab = (node as NodeDbColumn).NodeDbTable.NodeTab;
                        SelectedExcel = (node as NodeDbColumn).NodeDbTable.NodeTab.NodeExcel;
                        ExcelVisible = true;
                        TabVisible = true;
                        TableVisible = true;
                        ColumnVisible = true;
                    }

                }
                finally
                {
                    eventAgg.GetEvent<SelectionChangedEvent>().Subscribe(SelectionChanged());
                    eventAgg.GetEvent<SelectedInPathChangedEvent>().Subscribe(SelectionChanged());
                }

            };
        }

        public DatabaseSchemaViewModel DataViewModel
        {
            get;
            set;
        }

        private void DoAddTables()
        {
            // 
        }

        public ICommand EraseFulltext { get { return (new DelegateCommand(() => DoEraseFulltext())); } }
        
        private void DoEraseFulltext() {
            FullTextColumnName = null;
            FullTextColumnDescription = null;
            CommonErasingFilter();
        }

        private void DoAddExcel()
        {
            var newExcel = new NodeExcel(eventAgg, root, this);
            newExcel.AreDescsShown = AreDescsShown;

            if (NodesExcel == null)
            {
                NodesExcel = new ObservableCollection<NodeExcel>();
            }
            
            NodesExcel.Insert(0, newExcel);
            SelectedExcel = newExcel;
            newExcel.IsSelected = true;
        }

        private NodeDbTableRoot rootTables;
        private NodeRoot root;
        public NodeRoot Root
        {
            get { return root; }
            set
            {
                SetProperty(ref root, value);
                if (value != null)
                {
                    try
                    {
                        //eventAgg.GetEvent<SelectedInMainChangedEvent>().Publish(value);
                    }
                    finally
                    {
                        
                    }
                }
            }
        }

        private void DoRefresh()
        {
            var list = NodesTable = new ObservableCollection<NodeDbTableToExcel>();
            RefreshHelper.RefreshTables(eventAgg, root, ref list);
            NodesTable = list;

            // slow?? :-)
            SetDescShown(AreDescsShown);

            //third tree view... (Tables-Columns -> Excels -> Sheets)
            DoRefreshColumns();
        }

        private void DoRefreshColumns()
        {
            var list = NodesTableColumns = new ObservableCollection<NodeDbTableColumnsToExcel>();
            RefreshHelper.RefreshTableColumns(eventAgg, root, ref list);
            NodesTableColumns = list;

            // slow?? :-)
            SetDescShown(AreDescsShown);
        }

        private void DoSave()
        {
            var success = SaveHelper.SaveSprints(Sprints);
            if (success)
            {
                Success();
            }
            else
            {
                Error();
            }
        }

        private void Error()
        {
            Task.Run(() =>
            {
                var s = "pack://application:,,,/Images/err{0:00}.png";
                for (var ix = 0; ix < 10; ix++)
                {
                    SuccessImageSource = string.Format(s, 9 - ix);
                    Thread.Sleep(25);
                }
                Thread.Sleep(800);
                SuccessImageSource = string.Format(s, 10);
                Thread.Sleep(300);
                SuccessImageSource = string.Format(s, 0);
                Thread.Sleep(800);
                SuccessImageSource = string.Format(s, 10);
                Thread.Sleep(300);
                SuccessImageSource = string.Format(s, 0);
                for (var ix = 0; ix < 10; ix++)
                {
                    SuccessImageSource = string.Format(s, ix);
                    Thread.Sleep(25);
                }
                SuccessImageSource = string.Format(s, 10);
            });
        }

        private void Success()
        {
            Task.Run(() =>
            {
                var s = "pack://application:,,,/Images/succ{0:00}.png";
                for (var ix = 0; ix < 19; ix++)
                {
                    SuccessImageSource = string.Format(s, 18 - ix);
                    Thread.Sleep(25);
                }
                Thread.Sleep(400);
                for (var ix = 0; ix < 19; ix++)
                {
                    SuccessImageSource = string.Format(s, ix);
                    Thread.Sleep(25);
                }
            });
        }

        private string successImageSource = "pack://application:,,,/Images/succ18.png";
        public string SuccessImageSource
        {
            get
            {
                return successImageSource;
            }
            set
            {
                SetProperty(ref successImageSource, value);
            }
        }

        private void DoLoad()
        {
            //var list = nodesExcel = new ObservableCollection<NodeExcel>();
            var listOfSprints = new ObservableCollection<UISprint>();

            //root = new NodeRoot(eventAgg) { Name = "ROOT", Description = "just help instance (node)..." };
            var success = LoadHelper.Load(ref listOfSprints, eventAgg, this, DataViewModel);
            //NodesExcel = list;
            //root.Nodes = NodesExcel;,
            if (success)
            {
                Sprints = listOfSprints;
                SelectedSprint = listOfSprints.Last();
                Success();
            }
            else Error(); 
        }

        private bool excelVisible;
        public bool ExcelVisible
        {
            get { return excelVisible; }
            set { SetProperty(ref excelVisible, value); }
        }

        private bool tabVisible;
        public bool TabVisible
        {
            get { return tabVisible; }
            set { SetProperty(ref tabVisible, value); }
        }

        private bool tableVisible;
        public bool TableVisible
        {
            get { return tableVisible; }
            set { SetProperty(ref tableVisible, value); }
        }

        private bool columnVisible;
        public bool ColumnVisible
        {
            get { return columnVisible; }
            set { SetProperty(ref columnVisible, value); }
        }

        private IEventAggregator eventAgg;

        private NodeExcel selectedExcel;
        public NodeExcel SelectedExcel
        {
            get { return selectedExcel; }
            set
            {
                SetProperty(ref selectedExcel, value);

                if (value == null)
                {
                    return;
                }

                try
                {
                    
                    //eventAgg.GetEvent<SelectedInMainChangedEvent>().Publish(value);
                } finally
                {
                    
                }

                //if (value.IsAlreadySelected) return;
                //value.IsAlreadySelected = false;
                //eventAgg.GetEvent<SelectedInMainChangedEvent>().Publish(value);
            }
        }

        private NodeTab selecedTab;
        public NodeTab SelectedTab
        {
            get { return selecedTab; }
            set {
                SetProperty(ref selecedTab, value);

                if (value == null)
                {
                    return;
                }

                (value as NodeTab).NodeExcel.IsExpanded = true;

                //eventAgg.GetEvent<SelectedInMainChangedEvent>().Publish(value);
            }
        }

        private NodeDbTable selecedDbTable;
        public NodeDbTable SelectedDbTable
        {
            get { return selecedDbTable; }
            set
            {
                SetProperty(ref selecedDbTable, value);

                if (value == null)
                {
                    return;
                }

                value.NodeTab.IsExpanded = true;

                //eventAgg.GetEvent<SelectedInMainChangedEvent>().Publish(value);
            }
        }

        private NodeDbColumn selecedDbColumn;
        public NodeDbColumn SelectedDbColumn
        {
            get { return selecedDbColumn; }
            set
            {
                SetProperty(ref selecedDbColumn, value);

                if (value == null)
                {
                    return;
                }

                value.NodeDbTable.IsExpanded = true;

                //eventAgg.GetEvent<SelectedInMainChangedEvent>().Publish(value);
            }
        }

        private void DoCollapseAll()
        {
            if (NodesExcel == null) return;
            foreach (var e in NodesExcel)
            {
                e.IsExpanded = false;
                if (e.Nodes != null)
                    foreach (var tab in e.Nodes)
                    {
                        tab.IsExpanded = false;
                        if (tab.Nodes != null)
                            foreach (var table in tab.Nodes)
                            {
                                table.IsExpanded = false;
                                if (table.Nodes != null)
                                    foreach (var c in table.Nodes)
                                    {
                                        c.IsExpanded = false;
                                    }
                            }
                    }
            }
        }

        private void DoImport()
        {
            var exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var dataPath = string.Format("{0}\\Data", exePath);

            var data = string.Format("{0}\\data.json", dataPath);
            var guide = string.Format("{0}\\guide.txt", dataPath);

            var fi = new FileInfo(guide);

            if (fi.Exists)
            {

                using (var stream = fi.OpenText())
                {
                    var text = stream.ReadToEnd();
                    stream.Close();

                    var lines = text.Split('\n');

                    var nodeRoot = new NodeRoot(eventAgg, this)
                    {
                        Name = "ROOT",
                        Description = "just help instance..."
                    };
                    NodesExcel = new ObservableCollection<NodeExcel>();
                    var masterDataNode = new NodeExcel(eventAgg, nodeRoot, this) { Name = "MasterData" };
                    nodeRoot.Nodes = new ObservableCollection<NodeExcel>();
                    nodeRoot.Nodes.Add(masterDataNode);
                    masterDataNode.Nodes = new ObservableCollection<NodeTab>();
                    NodesExcel.Add(masterDataNode);

                    NodeTab tab = null;

                    NodeDbTable dbTable = null;

                    foreach (var l in lines)
                    {
                        if (string.IsNullOrEmpty(l) || string.IsNullOrWhiteSpace(l))
                        {
                            continue;
                        }

                      
                        if (l[0] == '#')
                        {
                            // starting... -> 
                            tab = new NodeTab(eventAgg, masterDataNode, this) { Name = l.Substring(1, l.IndexOf('-') - 2), Description = l.Substring(l.IndexOf('-') + 2) };
                            dbTable = new NodeDbTable(eventAgg, tab, this) { Name = l.Substring(1, l.IndexOf('-') - 2).Replace(" ", "") };
                            tab.Nodes = new ObservableCollection<NodeDbTable>
                            {
                                dbTable
                            };
                            NodesExcel[0].Nodes.Add(tab);
                        }
                        else
                        {
                            var column = new NodeDbColumn(eventAgg, dbTable, this) { Name = (l.Substring(0, l.IndexOf('-') - 1)), Description = l.Substring(l.IndexOf('-') + 2) };
                            if (tab.Nodes[0].Nodes == null)
                            {
                                tab.Nodes[0].Nodes = new ObservableCollection<NodeDbColumn>();
                            }

                            tab.Nodes[0].Nodes.Add(column);
                        }
                    }

                }
            }

        }

        public ICommand Import { get; private set; }
        public ICommand Save { get; private set; }
        public ICommand Load { get; private set; }
        public ICommand ShowLog { get; private set; }
        public ICommand ShowDataFolder { get; private set; }
        public ICommand Refresh { get; private set; }
        public ICommand RefreshColumns { get; private set; }
        public ICommand ExpandAll { get; private set; }
        public ICommand CollapseAll { get; private set; }

        public ICommand SaveSprints { get; private set; }

        public ICommand AddTables { get; private set; }

        //AddExcel
        public ICommand AddExcel { get; private set; }

        private void DoExpandAll()
        {
            if (NodesExcel == null) return;
            foreach (var e in NodesExcel)
            {
                e.IsExpanded = true;
                if (e.Nodes != null)
                    foreach (var tab in e.Nodes)
                    {
                        tab.IsExpanded = true;
                        if (tab.Nodes != null)
                            foreach (var table in tab.Nodes)
                            {
                                table.IsExpanded = true;
                                if (table.Nodes != null)
                                    foreach (var c in table.Nodes)
                                    {
                                        c.IsExpanded = true;
                                    }
                            }
                    }
            }
        }


        private ObservableCollection<NodeExcel> nodesExcel;
        public ObservableCollection<NodeExcel> NodesExcel
        {
            get { return nodesExcel; }
            set { SetProperty(ref nodesExcel, value); }
        }

        private string fullTextColumnName;
        public string FullTextColumnName
        {
            get { return fullTextColumnName; }
            set
            {
                SetProperty(ref fullTextColumnName, value);
                DoFilter(FilterType.Name);
            }
        }

        private void DoFilter(FilterType name)
        {
            switch (name)
            {
                case FilterType.Name:
                    if (string.IsNullOrEmpty(fullTextColumnName))
                    {
                        CommonErasingFilter();
                    }
                    else
                    {
                        CommonFiltering(FilterType.Name);
                    }
                    break;
                case FilterType.Description:
                    if (string.IsNullOrEmpty(fullTextColumnDescription))
                    {
                        CommonErasingFilter();
                    }
                    else
                    {
                        CommonFiltering(FilterType.Description);
                    }
                    break;
            }
        }

        private void CommonErasingFilter()
        {
            foreach (var e in NodesExcel)
            {
                e.IsVisible = true;
                if (e.Nodes != null) foreach (var tab in e.Nodes)
                    {
                        tab.IsVisible = true;
                        if (tab.Nodes != null) foreach (var table in tab.Nodes)
                            {
                                table.IsVisible = true;
                                if (table.Nodes != null) foreach (var c in table.Nodes)
                                    {
                                        c.IsVisible = true;
                                    }
                            }
                    }
            }
        }

        private void CommonFiltering(FilterType type)
        {
            foreach (var e in NodesExcel)
            {
                var bExcel = false;
                if (e.Nodes != null) foreach (var tab in e.Nodes)
                    {
                        var bTab = false;
                        if (tab.Nodes != null) foreach (var table in tab.Nodes)
                            {
                                var bTable = false;
                                if (table.Nodes != null) foreach (var c in table.Nodes)
                                    {
                                        if ((type == FilterType.Name && c.Name.ToUpperInvariant().IndexOf(fullTextColumnName.ToUpperInvariant()) > -1)
                                            ||
                                            (type == FilterType.Description && c.Description.ToUpperInvariant().IndexOf(fullTextColumnDescription.ToUpperInvariant()) > -1))
                                        {
                                            c.IsVisible = true;
                                            bExcel = true;
                                            bTab = true;
                                            bTable = true;
                                        }
                                        else
                                        {
                                            c.IsVisible = false;
                                        }
                                    }
                                e.IsVisible = bExcel;
                                tab.IsVisible = bTab;
                                table.IsVisible = bTable;
                        }

                }
            }
        }

        private bool areDescsShown = false;
        public bool AreDescsShown
        {
            get
            {
                return areDescsShown;
            }
            set
            {
                SetProperty(ref areDescsShown, value);

                SetDescShown(value);
            }
        }

        private void SetDescShown(bool value)
        {
            // Excel
            if (NodesExcel != null)
            {
                foreach (var n in NodesExcel)
                {
                    n.AreDescsShown = value;
                    if (n.Nodes == null) continue;
                    foreach (var tab in n.Nodes)
                    {
                        tab.AreDescsShown = value;
                        if (tab.Nodes == null) continue;
                        foreach (var table in tab.Nodes)
                        {
                            table.AreDescsShown = value;
                            if (table.Nodes == null) continue;
                            foreach (var c in table.Nodes)
                            {
                                c.AreDescsShown = value;
                            }
                        }
                    }
                }
            }

            // Tables 
            if (NodesTable!=null)
            {
                foreach (var n in NodesTable)
                {
                    n.AreDescsShown = value;
                    if (n.Nodes == null) continue;
                    foreach (var n2 in n.Nodes)
                    {
                        n2.AreDescsShown = value;
                        if (n2.Nodes == null) continue;
                        foreach (var n3 in n2.Nodes)
                        {
                            n3.AreDescsShown = value;
                        }
                    }
                }
            }

            // Table - Columns
            if (NodesTableColumns != null)
            {
                foreach (var n in NodesTableColumns)
                {
                    n.AreDescsShown = value;
                    if (n.Nodes == null) continue;
                    foreach (var n2 in n.Nodes)
                    {
                        n2.AreDescsShown = value;
                        if (n2.Nodes == null) continue;
                        foreach (var n3 in n2.Nodes)
                        {
                            n3.AreDescsShown = value;
                            if (n3.Nodes == null) continue;
                            foreach (var n4 in n3.Nodes)
                            {
                                n4.AreDescsShown = value;
                            }
                        }
                    }
                }
            }

        }

        private string fullTextColumnDescription;
        public string FullTextColumnDescription
        {
            get { return fullTextColumnDescription; }
            set
            {
                SetProperty(ref fullTextColumnDescription, value);
                DoFilter(FilterType.Description);
            }
        }

        private ObservableCollection<NodeDbTableToExcel> nodesTable;
        public ObservableCollection<NodeDbTableToExcel> NodesTable { get { return nodesTable; } set { SetProperty(ref nodesTable, value); } }

        private ObservableCollection<NodeDbTableColumnsToExcel> nodesTableColumns;
        public ObservableCollection<NodeDbTableColumnsToExcel> NodesTableColumns { get { return nodesTableColumns; } set { SetProperty(ref nodesTableColumns, value); } }

        private ObservableCollection<UISprint> sprints;
        public ObservableCollection<UISprint> Sprints { get { return sprints; } set { SetProperty(ref sprints, value); } }


        private UISprint selectedSprint;
        public UISprint SelectedSprint
        {
            get { return selectedSprint; }
            set
            {
                if (value == null)
                {
                    return;
                }

                SetProperty(ref selectedSprint, value);
                if (selectedSprint != null)
                {
                    Root = selectedSprint.Root;
                    NodesExcel = selectedSprint.Root.Nodes;
                    //if (NodesExcel != null && NodesExcel.Count > 0) SelectedExcel = NodesExcel.First();

                    SetDescShown(AreDescsShown);

                    eventAgg.GetEvent<SelectedSprintChanged>().Publish(value);

                    try
                    {
                        //eventAgg.GetEvent<SelectionStructureChangedEvent>().Unsubscribe(StructureChanged());
                        //eventAgg.GetEvent<SelectedInMainChangedEvent>().Publish(value.Root);
                    }
                    finally
                    {
                        //eventAgg.GetEvent<SelectionStructureChangedEvent>().Unsubscribe(StructureChanged());
                    }
                }

            }
        }

        private void DoSaveSprints()
        {
            var b = SaveHelper.SaveSprints(Sprints);
        }
    }

    public enum FilterType
    {
        Name,
        Description
    }
}
