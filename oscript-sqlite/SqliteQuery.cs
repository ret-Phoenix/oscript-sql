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

namespace OScriptSqlite
{
    [ContextClass("Запрос", "Query")]
    class Query : AutoContext<Query>, IOScriptQuery
    {

        private string _text;
        private SQLiteConnection _connection;
        private SQLiteCommand _command;
        private StructureImpl _parameters;

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
            _command.CommandText = _text;
            _command.Prepare();
            foreach (IValue prm in _parameters)
            {
                _command.Parameters.AddWithValue("@" + ((KeyAndValueImpl)prm).Key.ToString(), ((KeyAndValueImpl)prm).Value);
            }
            
            var reader = _command.ExecuteReader();
            var result = new QueryResult(reader);

            return result;
        }

        [ContextMethod("УстановитьПараметр", "SetParameter")]
        public void SetParameter(string ParametrName, IValue ParametrValue)
        {
            //_command.Parameters.AddWithValue(ParametrName, ParametrValue);
            _parameters.Insert(ParametrName, ParametrValue);
        }

        [ContextMethod("УстановитьСоединение", "SetConnection")]
        public void SetConnection(string DBPath)
        {
            _connection = new SQLiteConnection(string.Format("Data Source={0};", DBPath));
            _connection.Open();
            _command = new SQLiteCommand(_connection);
        }

    }
}
