using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

namespace OScriptSql
{
    [ContextClass("Запрос", "Query")]
    class Query : AutoContext<Query>, IOScriptQuery
    {

        private string _text;
        private DbConnection _connection;
        private DbCommand _command;
        private StructureImpl _parameters;

        private DBConnector _connector;

        public Query()
        {
            _parameters = new StructureImpl();
            _text = "";
        }

        [ScriptConstructor]
        public static IRuntimeContextInstance Constructor()
        {
            return new Query();
        }

        // props
        public StructureImpl Parameters()
        {
            return _parameters;
        }



        [ContextProperty("Текст", "Text")]
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
            }
        }

        [ContextMethod("Выполнить", "Execute")]
        public IValue Execute()
        {
            var result = new QueryResult();

            _command.CommandText = _text;
            _command.Prepare();

            if (_connector.DbType == (new EnumDBType()).sqlite)
            {
                foreach (IValue prm in _parameters)
                {
                    ((SQLiteCommand)_command).Parameters.AddWithValue("@" + ((KeyAndValueImpl)prm).Key.ToString(), ((KeyAndValueImpl)prm).Value);
                }
                var reader = _command.ExecuteReader();
                result = new QueryResult(reader);
            }
            else if (_connector.DbType == (new EnumDBType()).MSSQLServer)
            {
                foreach (IValue prm in _parameters)
                {
                    //((SqlCommand)_command).Parameters.AddWithValue("@" + ((KeyAndValueImpl)prm).Key.ToString(), ((KeyAndValueImpl)prm).Value);
                    var vl = ((KeyAndValueImpl)prm).Value.ToString();
                    ((SqlCommand)_command).Parameters.AddWithValue("@" + ((KeyAndValueImpl)prm).Key.ToString(), vl);
                }
                //Console.WriteLine(((SqlCommand)_command).Connection.State);
                //Console.WriteLine(0);
                var reader = ((SqlCommand)_command).ExecuteReader();

                //Console.WriteLine(1);
                result = new QueryResult(reader);
                //Console.WriteLine(2);
            }

            return result;
        }

        [ContextMethod("УстановитьПараметр", "SetParameter")]
        public void SetParameter(string ParametrName, IValue ParametrValue)
        {
            _parameters.Insert(ParametrName, ParametrValue);
        }

        //[ContextMethod("УстановитьСоединение", "SetConnection")]
        //public void SetConnection(string DBPath)
        //{
        //    _connection = new SQLiteConnection(string.Format("Data Source={0};", DBPath));
        //    _connection.Open();
        //    _command = new SQLiteCommand(_connection);
        //}

        [ContextMethod("УстановитьСоединение", "SetConnection")]
        public void SetConnection(DBConnector connector)
        {
            _connector = connector;
            _connection = connector.Connection;

            if (_connector.DbType == (new EnumDBType()).sqlite)
            {
                _command = new SQLiteCommand((SQLiteConnection)connector.Connection);
            }
            else if (_connector.DbType == (new EnumDBType()).MSSQLServer)
            {
                _command = new SqlCommand();
                _command.Connection = (SqlConnection)connector.Connection;
            }
        }


    }
}
