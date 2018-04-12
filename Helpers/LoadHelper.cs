using EscapeDBUsage.ModelClasses;
using EscapeDBUsage.UIClasses;
using EscapeDBUsage.UIClasses.DatabaseSchema;
using EscapeDBUsage.ViewModels;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Helpers
{
    public static class LoadHelper
    {
        //public static List<Sprint> LoadSprints()
        //{
        //    var list = new List<Sprint>();
        //    var exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        //    var dataPath = string.Format("{0}\\Data", exePath);

        //    var data = string.Format("{0}\\sprints.json", dataPath);
        //    var fi = new FileInfo(data);

        //    using (var stream = fi.OpenText())
        //    {
        //        var json = stream.ReadToEnd();
        //        var rr = JsonConvert.DeserializeObject<Sprints>(json).List;
        //        list = rr;
        //    }

        //    return list;

        //}

        public static bool Load(ref ObservableCollection<UISprint> sprints, IEventAggregator evAgg, MainViewModel viewModel, DatabaseSchemaViewModel dataViewModel)
        {
            Logger.Logger.Log.Info("Loading sprints...");

            var ret = false;

            //var exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            //var dataPath = string.Format("{0}\\Data", exePath);

            var data = FoldersHelper.DataPath; // string.Format("{0}\\sprints.json", dataPath);
            var di = new DirectoryInfo(FoldersHelper.DataFolder);
            if (!di.Exists) di.Create();

            var fi = new FileInfo(data);

            try
            {

                using (var stream = fi.OpenText())
                {
                    var json = stream.ReadToEnd();
                    var rr = JsonConvert.DeserializeObject<Sprints>(json);

                    foreach (var s in rr.List)
                    {
                        var nodeRoot = new NodeRoot(evAgg, viewModel);
                        var sprint = new UISprint(evAgg, sprints, nodeRoot, viewModel, dataViewModel) { Guid = s.Guid, Number = s.Number, Name = s.Name, Version = s.Version };
                        sprint.DbSchemaTables = DbSchemaHelper.ConvertDbSchemaTables(s.DbSchemaTables);
                        sprints.Add(sprint);

                        var result = new ObservableCollection<NodeExcel>();
                        nodeRoot.Nodes = result;
                        if (s.Excels == null) continue;
                        foreach (var r in s.Excels)
                        {
                            var ne = new NodeExcel(evAgg, nodeRoot, viewModel) { Name = r.Name, Description = r.Description };
                            result.Add(ne);
                            ne.Nodes = new ObservableCollection<NodeTab>();
                            if (r.Nodes == null) continue;
                            foreach (var t in r.Nodes)
                            {
                                var nt = new NodeTab(evAgg, ne, viewModel) { Name = t.Name, Description = t.Description };
                                ne.Nodes.Add(nt);
                                nt.Nodes = new ObservableCollection<NodeDbTable>();
                                if (t.Nodes == null) continue;
                                foreach (var table in t.Nodes)
                                {
                                    var ntable = new NodeDbTable(evAgg, nt, viewModel) { Name = table.Name, Description = table.Description };
                                    nt.Nodes.Add(ntable);
                                    ntable.Nodes = new ObservableCollection<NodeDbColumn>();
                                    if (table.Nodes == null) continue;
                                    foreach (var c in table.Nodes)
                                    {
                                        var nc = new NodeDbColumn(evAgg, ntable, viewModel) { Name = c.Name, Description = c.Description };
                                        ntable.Nodes.Add(nc);
                                    }
                                }
                            }
                        }
                    }
                }
                ret = true;
                Logger.Logger.Log.Info("Loading sprints -> SUCCESS!");
                return ret;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Error loading sprints...", ex);
                ret = false;

                Logger.Logger.Log.Info("Creating sprints ...");

                try
                {
                    using (var s = fi.CreateText())
                    {
                        var sss = "{ \"List\": [ { \"Number\": 0, \"Name\": \"Sprint 0\", \"Version\": \"0.0.0.0\" } ] }";
                        s.Write(sss);
                    }

                    Logger.Logger.Log.Info("Creating sprints -> SUCCESS!");
                    // again -> 
                    return Load(ref sprints, evAgg, viewModel, dataViewModel);
                }
                catch (Exception ex2)
                {
                    Logger.Logger.Log.Error("Error creating sprints data file...", ex2);
                }

                return ret;
            }
}
    }
}
