using EscapeDBUsage.Classes;
using EscapeDBUsage.ModelClasses.DbSchema;
using EscapeDBUsage.UIClasses.DatabaseSchema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeDBUsage.Helpers
{
    public static class DbSchemaHelper
    {
        private static string select = @"SELECT
	                                              [type] = o.type,	  
                                                  [tableName] = o.name, 
	                                              [columnName] = c.name, 
	                                              [position] = col.ORDINAL_POSITION,
	                                              [objectType] = o.type_desc,
	                                              [dataType] = col.DATA_TYPE,
	                                              [isNullable] = col.IS_NULLABLE,
	                                              [Length] = col.CHARACTER_MAXIMUM_LENGTH,
	                                              [isIdentity] = c.is_identity,
	                                              [isComputed] = c.is_computed,
	                                              [default] = col.COLUMN_DEFAULT
                                            FROM sys.objects o
                                            JOIN sys.schemas s ON o.[schema_id] = s.[schema_id]
                                            JOIN sys.columns c ON o.[object_id] = c.[object_id]
                                            JOIN INFORMATION_SCHEMA.COLUMNS AS col ON col.TABLE_NAME = o.name
                                                                                              AND col.COLUMN_NAME = c.name
                                            WHERE o.[type] IN ('U', 'V')
                                            ORDER BY [type], [tableName], [position]";

        public static List<SchemaDbTable> GetTablesWithColumns(DbConnection connection)
        {
            var tables = new List<SchemaDbTable>();

            var columns = new List<SchemaDbColumn>();

            var connString = ConnectionHelper.ToConnectionString(connection);
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(select, conn);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                         var type = reader.GetString(0);
                         var tableName = reader.GetString(1); 
	                     var columnName = reader.GetString(2);
	                     var position = reader.GetInt32(3);
	                     var objectType = reader.GetString(4);
	                     var dataType = reader.GetString(5);
	                     var isNullable = reader.GetString(6);
	                     var length = reader.GetValue(7);
                         var isIdentity = reader.GetBoolean(8);
                         var isComputed = reader.GetBoolean(9);
	                     var defaultValue = reader.GetValue(10);

                         int parsedLength;

                         var c = new SchemaDbColumn()
                         {
                             Name = columnName,
                             DataType = dataType,
                             DefaultValue = defaultValue,
                             IsComputed = isComputed,
                             IsIdentity = isIdentity,
                             IsNullable = isNullable.Equals("YES"),
                             ObjectType = objectType,
                             Position = position,
                             TableName = tableName,
                             Type = type,
                             Length = int.TryParse(length.ToString(), out parsedLength) ? parsedLength : (int?)null
                         };
   
                         columns.Add(c);
                    }
                }

                conn.Close();
            }

            tables = columns.GroupBy(c => c.TableName).Select((x) => new SchemaDbTable() { Name = x.First().TableName, Columns = columns.Where((c2) => c2.TableName.Equals(x.First().TableName)).ToList() }).ToList();
            return tables;
        }

        public static ObservableCollection<NodeDbSchemaTable> ConvertDbSchemaTables(List<SchemaDbTable> list)
        {
            if (list == null || list.Count == 0) return new ObservableCollection<NodeDbSchemaTable>();

            var ret = new ObservableCollection<NodeDbSchemaTable>(list.Select((x) => new NodeDbSchemaTable()
            {
                Name = x.Name,
                Description = x.Description,
                Columns = new ObservableCollection<NodeDbSchemaColumn>(x.Columns.Select(c => new NodeDbSchemaColumn()
                {
                    Name = c.Name,
                    Description = c.Description,
                    DataType = c.DataType,
                    DefaultValue = c.DefaultValue,
                    IsComputed = c.IsComputed,
                    IsIdentity = c.IsIdentity,
                    IsNullable = c.IsNullable,
                    ObjectType = c.ObjectType,
                    Position = c.Position,
                    TableName = c.TableName,
                    Length = c.Length
                }))
            }));
            return ret;
        }

        public static List<SchemaDbTable> ConvertDbSchemaTables(ObservableCollection<NodeDbSchemaTable> collection)
        {
            if (collection == null || collection.Count == 0) return new List<SchemaDbTable>();

            var ret = collection.Select((x) => new SchemaDbTable()
            {
                Name = x.Name,
                Description = x.Description,
                Columns = new List<SchemaDbColumn>(x.Columns.Select(c => new SchemaDbColumn()
                {
                    Name = c.Name,
                    Description = c.Description,
                    DataType = c.DataType,
                    DefaultValue = c.DefaultValue,
                    IsComputed = c.IsComputed,
                    IsIdentity = c.IsIdentity,
                    IsNullable = c.IsNullable,
                    ObjectType = c.ObjectType,
                    Position = c.Position,
                    TableName = c.TableName,
                    Length = c.Length
                }))
            }).ToList();
            return ret;
        }
    }
}
