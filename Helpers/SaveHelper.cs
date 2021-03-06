﻿using EscapeDBUsage.ModelClasses;
using EscapeDBUsage.ModelClasses.DbSchema;
using EscapeDBUsage.UIClasses;
using EscapeDBUsage.UIClasses.DatabaseSchema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Helpers
{
    public static class SaveHelper
    {
        public static bool Save(IList<NodeExcel> rootList)
        {

            var list = new List<Excel>();

            foreach (var e in rootList)
            {
                var ex = new Excel() { Guid = e.Guid, Name = e.Name, Description = e.Description };
                ex.Nodes = new List<Tab>();
                list.Add(ex);

                if (e.Nodes == null) continue;
                foreach (var t in e.Nodes)
                {
                    var tab = new Tab() { Guid = e.Guid, Name = t.Name, Description = (t as NodeBase).Description };
                    tab.Nodes = new List<DbTable>();
                    ex.Nodes.Add(tab);

                    if (t.Nodes == null) continue;
                    foreach (var table in t.Nodes)
                    {
                        var dbTable = new DbTable() { Guid = e.Guid, Name = table.Name, Description = (table as NodeBase).Description };
                        dbTable.Nodes = new List<DbColumn>();
                        tab.Nodes.Add(dbTable);

                        if (table.Nodes == null) continue;
                        foreach (var column in table.Nodes)
                        {
                            var dbColumn = new DbColumn() { Guid = e.Guid, Name = column.Name, Description = (column as NodeBase).Description };
                            dbTable.Nodes.Add(dbColumn);
                        }

                    }
                }
            }

            var exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var dataPath = string.Format("{0}\\Data", exePath);

            var data = string.Format("{0}\\data.json", dataPath);

            var json = JsonConvert.SerializeObject(list, Formatting.Indented);

            var fi = new FileInfo(data);

            using (var stream = fi.CreateText())
            {
                stream.Write(json);
            }

            return true;
        }

        internal static bool SaveSprints(ObservableCollection<UISprint> sprints)
        {
            Logger.Logger.Log.Info("Saving sprints...");

            var ret = false;

            var newSprints = new Sprints()
            {
                List = sprints.Select(x => new Sprint()
                {
                    Guid = x.Guid.Equals(Guid.Empty) ? Guid.NewGuid() : x.Guid,
                    Number = x.Number,
                    Name = x.Name,
                    Version = x.Version,
                    Excels = x.Root.Nodes != null ? x.Root.Nodes.Select(e =>
                          new Excel()
                          {
                              Guid = (e as NodeBase).Guid.Equals(Guid.Empty) ? Guid.NewGuid() : (e as NodeBase).Guid,
                              Name = e.Name,
                              Description = (e as NodeBase).Description,
                              Nodes = e.Nodes != null ? e.Nodes.Select(tab =>
                                  new Tab()
                                  {
                                      Guid = (tab as NodeBase).Guid.Equals(Guid.Empty) ? Guid.NewGuid() : (tab as NodeBase).Guid,
                                      Name = (tab as NodeBase).Name,
                                      Description = (tab as NodeBase).Description,
                                      Nodes = tab.Nodes != null ? tab.Nodes.Select(table =>
                                          new DbTable()
                                          {
                                              Guid = (table as NodeBase).Guid.Equals(Guid.Empty) ? Guid.NewGuid() : (table as NodeBase).Guid,
                                              Name = table.Name,
                                              Description = (table as NodeBase).Description,
                                              Nodes = table.Nodes != null ? table.Nodes.Select(column =>
                                              new DbColumn()
                                              {
                                                  Guid = (column as NodeBase).Guid.Equals(Guid.Empty) ? Guid.NewGuid() : (column as NodeBase).Guid,
                                                  Name = column.Name,
                                                  Description = (column as NodeBase).Description
                                              }
                                              ).ToList() : new List<DbColumn>()
                                          }
                                      ).ToList() : new List<DbTable>()
                                  }
                              ).ToList() : new List<Tab>()
                          }
                    ).ToList() : new List<Excel>()
                }).ToList()
            };

            var ix = 0;
            foreach (var s in sprints)
            {
                var obsTables = s.DbSchemaTables;
                newSprints.List[ix].DbSchemaTables = DbSchemaHelper.ConvertDbSchemaTables(obsTables);

                ix++;
            }

            //var exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            //var dataPath = string.Format("{0}\\Data", exePath);
            var data = FoldersHelper.DataPath; //string.Format("{0}\\sprints.json", dataPath);

            var json = JsonConvert.SerializeObject(newSprints, Formatting.Indented);
            var fi = new FileInfo(data);

            try
            {
                using (var stream = fi.CreateText())
                {
                    stream.Write(json);
                }
                ret = true;
                Logger.Logger.Log.Info("Saving sprints -> SUCCESS!");
                return ret;
            } catch (Exception ex)
            {
                Logger.Logger.Log.Error("Error saving sprints...", ex);
                ret = false;
                return ret;
            }
        }
    }
}
