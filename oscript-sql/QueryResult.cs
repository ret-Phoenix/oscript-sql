using System.Globalization;
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

namespace OScriptSql
{
    [ContextClass("РезультатЗапроса", "QueryResult")]
    class QueryResult : AutoContext<QueryResult>
    {
        private DbDataReader _reader;
        //List<string> _columns;

        //[ContextProperty("Колонки", "Columns")]
        //public string Columns
        //{
        //    get { return _columns; }
        //}

        public QueryResult()
        {
        }


        public QueryResult(DbDataReader reader)
        {
            _reader = reader;
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
                    //Console.WriteLine(record.GetValue(ColIdx).ToString());
                    Console.WriteLine(record.GetFieldType(ColIdx).ToString());
                    
                    if (record.IsDBNull(ColIdx))
                    {
                        row.Set(ColIdx, ValueFactory.Create());
                        continue;
                    }


                    if (record.GetFieldType(ColIdx) == typeof(Int64))
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetInt64(ColIdx)) );
                    }

//                    if (record.GetFieldType(ColIdx).ToString() == "System.Int64")
//                    {
//                        row.Set(ColIdx, ValueFactory.Create(record.GetInt64(ColIdx)) );
//                    }
                    if (record.GetFieldType(ColIdx).ToString() == "System.String")
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetString(ColIdx)));
                    }
                    if (record.GetFieldType(ColIdx).ToString() == "System.Byte[]")
                    {
                    	var data = (byte[])record[ColIdx];
                    	var newData = new BinaryDataContext(data);
                    	row.Set(ColIdx, ValueFactory.Create(newData));
                    }
                    if (record.GetFieldType(ColIdx).ToString() == "System.Double")
                    {
                    	double val = record.GetDouble(ColIdx);
                    	string newVal = val.ToString();
                        row.Set(ColIdx, ValueFactory.Create( newVal));
                    }
                    if (record.GetFieldType(ColIdx).ToString() == "System.Decimal")
                    {
                    	var val = record.GetDecimal(ColIdx);
                    	string newVal = val.ToString();
                        row.Set(ColIdx, ValueFactory.Create(newVal));
                    }

                }

            }
            _reader.Close();
            return resultTable;

        }
    }
}
