using EscapeDBUsage.UIClasses.DatabaseSchema;
using EscapeDBUsage.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EscapeDBUsage.UIClasses
{
    public class UISprint: BindableBase
    {
        public UISprint(IEventAggregator evAgg, ObservableCollection<UISprint> sprints, NodeRoot root, MainViewModel viewModel, DatabaseSchemaViewModel databaseSchemaViewModel)
        {
            this.viewModel = viewModel;
            this.databaseSchemaViewModel = databaseSchemaViewModel;

            EvAgg = evAgg;
            Root = root;
            Sprints = sprints;
            InsertSprint = new DelegateCommand(() => DoInsertSprint());
            RemoveSprint = new DelegateCommand(() => DoRemoveSprint());
            CopySprint = new DelegateCommand(() => DoCopySprint());

            this.ConfirmationRequest = new InteractionRequest<IConfirmation>();

        }

        private MainViewModel viewModel;
        private DatabaseSchemaViewModel databaseSchemaViewModel;

        public IEventAggregator EvAgg { get; private set; }
        public ICommand InsertSprint { get; private set; }
        public ICommand RemoveSprint { get; private set; }
        public ICommand CopySprint { get; private set; }

        public InteractionRequest<IConfirmation> ConfirmationRequest { get; private set; }

        public ObservableCollection<UISprint> Sprints { get; private set; }

        public NodeRoot Root { get; set; }

        private void DoCopySprint()
        {
            this.ConfirmationRequest.Raise(
                    new Confirmation { Content = "Are you sure to COPY all data from one Sprint to another?", Title = "Confirmation - COPY!" },
                        c => { if (c.Confirmed) if (Sprints.Count > Sprints.IndexOf(this) + 1) Sprints[Sprints.IndexOf(this) + 1].Root = this.Root.Copy(); });
        }

        private void DoRemoveSprint()
        {
            Sprints.Remove(this);
        }

        private void DoInsertSprint()
        {
            Sprints.Insert(Sprints.IndexOf(this) + 1, new UISprint(EvAgg, Sprints, new NodeRoot(EvAgg, viewModel), viewModel, databaseSchemaViewModel) { Name = "NAME", Number = 99, Version = "9.9.9.9" });
        }

        private Guid guid;
        public Guid Guid
        {
            get { return guid; }
            set { SetProperty(ref guid, value); }
        }

        private int number;
        public int Number
        {
            get { return number; }
            set { SetProperty(ref number, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string version;
        public string Version
        {
            get { return version; }
            set { SetProperty(ref version, value); }
        }

        private ObservableCollection<NodeDbSchemaTable> dbSchemaTables;
        public ObservableCollection<NodeDbSchemaTable> DbSchemaTables {
            get
            {
                return dbSchemaTables;
            }
            set
            {
                SetProperty(ref dbSchemaTables, value);
            }
        }

        public override string ToString()
        {
            return Number + ".) " + Name ?? "<no name>"+ " -> ver.:" + Version ?? "<no version>";
        }

    }
}
