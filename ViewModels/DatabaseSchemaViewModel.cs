using EscapeDBUsage.Classes;
using EscapeDBUsage.Events;
using EscapeDBUsage.UIClasses;
using EscapeDBUsage.UIClasses.DatabaseSchema;
using EscapeDBUsage.UIControlGetters;
using EscapeDBUsage.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EscapeDBUsage.ViewModels
{
    public class DatabaseSchemaViewModel : BindableBase
    {
        public DatabaseSchemaViewModel(IEventAggregator evAgg)
        {
            this.eventAgg = evAgg;
            eventAgg.GetEvent<SelectedSprintChanged>().Subscribe((sprint) =>
            {
                SelectedSprint = sprint;
                //ExpandAll.Execute(null);
            });


            UncheckAll = new DelegateCommand(() =>
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    foreach (var t in SelectedSprint.DbSchemaTables)
                    {
                        var tableUncheck = new Action(() =>
                        {
                            t.IsChecked = false;
                            if (t.Columns != null)
                            {
                                foreach (var c in t.Columns)
                                {
                                    c.IsChecked = false;
                                }
                            }
                        });
                        Dispatcher.CurrentDispatcher.Invoke(tableUncheck, DispatcherPriority.ApplicationIdle, null);
                    }
                });
            });

            CheckAll = new DelegateCommand(() =>
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    foreach (var t in SelectedSprint.DbSchemaTables)
                    {
                        var tableCheck = new Action(() =>
                        {
                            t.IsChecked = true;
                            if (t.Columns != null)
                            {
                                foreach (var c in t.Columns)
                                {
                                    c.IsChecked = true;
                                }
                            }
                        });
                        Dispatcher.CurrentDispatcher.Invoke(tableCheck, DispatcherPriority.ApplicationIdle, null);
                    }
                });
            });

            CollapseAll = new DelegateCommand(() =>
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    foreach (var t in SelectedSprint.DbSchemaTables)
                    {
                        var tableCollapse = new Action(() =>
                        {
                            t.IsExpanded = false;
                        });
                        Dispatcher.CurrentDispatcher.Invoke(tableCollapse, DispatcherPriority.ApplicationIdle, null);
                    }
                });
            });

            Action expandAllAction = async () =>
            {
                Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                {
                    //IsDbSchemaTabVisible = false;
                }), DispatcherPriority.Normal, null);

                await Task.Run(() =>
                {
                    foreach (var t in SelectedSprint.DbSchemaTables)
                    {
                        Action tableExpand = () =>
                        {
                            t.IsExpanded = true;
                        };

                        Dispatcher.CurrentDispatcher.Invoke(tableExpand, DispatcherPriority.Normal, null);
                    }
                });
                //.ContinueWith(new Action<Task>((t) => {

                //}));

                Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                {
                    //IsDbSchemaTabVisible = true;
                }), DispatcherPriority.Normal, null);

            };

            ExpandAll = new DelegateCommand(() =>
            {
                //VisualChildrenHelper.FreezeAll(VisualChildrenHelper.FindVisualChild<DatabaseSchemaView>(Application.Current.MainWindow));
                expandAllAction();
            });

            EraseSchemaFulltext = new DelegateCommand(() =>
            {
                SchemaTableFulltext = null;
                SchemaColumnFulltext = null;
            });

            EraseSchemaFulltextExclude = new DelegateCommand(() =>
            {
                SchemaTableFulltextExclude = null;
                SchemaColumnFulltextExclude = null;
            });

            ZoomDefault = new DelegateCommand(() => {
                ZoomFactor = 1;
            });


            ZoomIn = new DelegateCommand(() => {
                if (zoomFactor * 1.25 <= 4) ZoomFactor = zoomFactor * 1.25;
                else ZoomFactor = 4;
            });

            ZoomOut = new DelegateCommand(() => {
                if (zoomFactor * 0.75 >= 0.125) ZoomFactor = zoomFactor * 0.75;
                else ZoomFactor = 0.125;
            });
        }

        private bool isDbSchemaTabVisible = true;
        public bool IsDbSchemaTabVisible
        {
            get { return isDbSchemaTabVisible; }
            set
            {
                SetProperty(ref isDbSchemaTabVisible, value);
            }
        }

        private IEventAggregator eventAgg;

        private string schemaColumnFulltext;
        public string SchemaColumnFulltext
        {
            get { return schemaColumnFulltext; }
            set
            {
                SetProperty(ref schemaColumnFulltext, value);

                if (value == null)
                {
                    EraseFulltext();
                }
                else
                {
                    if (value.Equals(string.Empty)) SchemaColumnFulltext = null;
                    DoFulltext();
                }
            }
        }

        private string schemaColumnFulltextExclude;
        public string SchemaColumnFulltextExclude
        {
            get { return schemaColumnFulltextExclude; }
            set
            {
                SetProperty(ref schemaColumnFulltextExclude, value);

                

                if (value == null)
                {
                    EraseFulltextExclude();
                }
                else
                {
                    if (value.Equals(string.Empty)) SchemaColumnFulltextExclude = null;
                    DoFulltext();
                }
            }
        }

        private UISprint selectedSprint;
        public UISprint SelectedSprint
        {
            get { return selectedSprint; }
            set
            {
                SetProperty(ref selectedSprint, value);
            }
        }


        public ICommand CheckAll { get; private set; }
        public ICommand UncheckAll { get; private set; }

        public ICommand CollapseAll { get; private set; }
        public ICommand ExpandAll { get; private set; }

        private void DoFulltext()
        {
            foreach (var t in SelectedSprint.DbSchemaTables)
            {


                var inValue = SchemaTableFulltext;
                var exValue = SchemaTableFulltextExclude;

                var includes = new string[] {};
                var excludes = new string[] {};
                CreateIncludesAndExcludes(out includes, out excludes, inValue, exValue);
                var list = CreateFulltextValueList(includes, excludes);

                var tableName = t.Name.ToUpperInvariant();

                var shouldBeVisible = ShouldBeVisible(list, tableName);

                t.IsVisible = shouldBeVisible;


                if (shouldBeVisible && t.Columns != null)
                {
                    var inValueC = SchemaColumnFulltext;
                    var exValueC = SchemaColumnFulltextExclude;
                    var includesC = new string[] {};
                    var excludesC = new string[] {};
                    CreateIncludesAndExcludes(out includesC, out excludesC, inValueC, exValueC);
                    var listC = CreateFulltextValueList(includesC, excludesC);

                    foreach (var c in t.Columns)
                    {
                        var columnName = c.Name.ToUpperInvariant();

                        var shouldBeVisibleC = ShouldBeVisible(listC, columnName);
                        c.IsVisible = shouldBeVisibleC;
                    }
                }

            }
        }

        private static bool ShouldBeVisible(List<FulltextValue> list,string text)
        {
            var shouldBeVisible = false;
            if (list.Count == 0) shouldBeVisible = true;

            foreach (var v in list)
            {
                if (!string.IsNullOrEmpty(v.Value) && v.IsInclude && text.Contains(v.Value)) shouldBeVisible = true;
            }

            if (shouldBeVisible)
            {
                foreach (var v in list)
                {
                    if (!string.IsNullOrEmpty(v.Value) && !v.IsInclude && text.Contains(v.Value)) shouldBeVisible = false;
                }
            }

            return shouldBeVisible;
        }

        private void CreateIncludesAndExcludes(out string[] includes, out string[] excludes, string inValue, string exValue)
        {
            includes = inValue != null ? inValue.ToUpperInvariant().Split(' ') : new string[] { };
            excludes = exValue != null ? exValue.ToUpperInvariant().Split(' ') : new string[] { };
        }

        private static List<FulltextValue> CreateFulltextValueList(string[] includes, string[] excludes)
        {
            var valueListIncludes = includes.Select((x) => new FulltextValue() { IsInclude = true, Value = x }).ToList();
            var valueListExcludes = excludes.Select((x) => new FulltextValue() { IsInclude = false, Value = x }).ToList();
            var list = valueListIncludes.Concat(valueListExcludes).ToList();
            return list;
        }

        private void EraseFulltext()
        {
            DoFulltext();
        }

        private void EraseFulltextExclude()
        {
            DoFulltext();
        }

        //SchemaTableFulltextExclude
        private string schemaTableFulltextExclude;
        public string SchemaTableFulltextExclude
        {
            get { return schemaTableFulltextExclude; }
            set
            {
                SetProperty(ref schemaTableFulltextExclude, value);

                

                if (value == null)
                {
                    EraseFulltextExclude();
                }
                else
                {
                    if (value.Equals(string.Empty)) SchemaTableFulltextExclude = null;
                    DoFulltext();
                }
            }
        }

        private string schemaTableFulltext;
        public string SchemaTableFulltext
        {
            get { return schemaTableFulltext; }
            set
            {
                SetProperty(ref schemaTableFulltext, value);

                

                if (value == null)
                {
                    EraseFulltext();
                }
                else
                {
                    if (value.Equals(string.Empty)) SchemaTableFulltext = null;
                    DoFulltext();
                }
            }
        }
        public ICommand EraseSchemaFulltext { get; private set; }
        public ICommand EraseSchemaFulltextExclude { get; private set; }

        private double zoomFactor = 1;
        public double ZoomFactor
        {
            get {
                return zoomFactor;
            }
            set
            {
                SetProperty(ref zoomFactor, value);
            }
        }

        public ICommand ZoomDefault { get; private set; }
        public ICommand ZoomIn { get; private set; }
        public ICommand ZoomOut { get; private set; }
    }

    public enum SchemaFultextType
    {
        Table,
        Column
    }
}
