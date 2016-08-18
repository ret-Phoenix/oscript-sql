using ScriptEngine.Machine.Contexts;
using ScriptEngine.HostedScript.Library.ValueTable;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript.Library;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;

namespace OScriptSqlite
{
    [ContextClass("РезультатЗапроса", "QueryResult")]
    class QueryResult : AutoContext<QueryResult>
    {
        private SQLiteDataReader _reader;
        //List<string> _columns;

        //[ContextProperty("Колонки", "Columns")]
        //public string Columns
        //{
        //    get { return _columns; }
        //}

        public QueryResult(SQLiteDataReader reader)
        {

            _reader = reader;

            //foreach (DbDataRecord record in reader)
            //{
            //    string id = record["id"].ToString();
            //    id = id.PadLeft(5 - id.Length, ' ');
            //    string value = record["value"].ToString();
            //    string result = "\u2502" + id + " \u2502";
            //    value = value.PadLeft(60, ' ');
            //    result += value + "\u2502";
            //    Console.WriteLine(result);
            //}
        }

        // Выгрузить(Unload)
        [ContextMethod("Выгрузить", "Unload")]
        public ValueTable Unload()
        {

            ValueTable resultTable = new ValueTable();

            for (int ColIdx = 0; ColIdx < _reader.FieldCount; ColIdx++)
            {
                resultTable.Columns.Add(_reader.GetName(ColIdx));
            }

            foreach (DbDataRecord record in _reader)
            {
                ValueTableRow row = resultTable.Add();

                for (int ColIdx = 0; ColIdx < _reader.FieldCount; ColIdx++)
                {
                    Console.WriteLine(record.GetValue(ColIdx).ToString());
                    
                    if (record.GetValue(ColIdx).ToString() == "System.DBNull")
                    {
                        row.Set(ColIdx, ValueFactory.Create());
                        continue;
                    }


                    if (record.GetFieldType(ColIdx).ToString() == "System.Int64")
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetInt64(ColIdx)) );
                    }
                    if (record.GetFieldType(ColIdx).ToString() == "System.String")
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetString(ColIdx)));
                    }
                    //if (record.GetFieldType(ColIdx).ToString() == "System.Byte[]")
                    //{
                    //    row.Set(ColIdx, ValueFactory.Create(Convert.ToBase64CharArray(record.GetValue(ColIdx))));
                    //}
                    //if (record.GetFieldType(ColIdx).ToString() == "System.Double")
                    //{
                    //    Console.WriteLine("    " + record.GetDouble(ColIdx).ToString());
                    //    row.Set(ColIdx, ValueFactory.Create(record.GetDouble(ColIdx).ToString()));
                    //}
                    if (record.GetFieldType(ColIdx).ToString() == "System.Decimal")
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetDecimal(ColIdx)));
                    }

                }

            }

            return resultTable;

        }
    }
}
