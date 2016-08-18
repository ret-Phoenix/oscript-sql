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
    [ContextClass("ЗапросSQlite", "SqliteQuery")]
    class SqliteQuery : AutoContext<SqliteQuery>, IOScriptQuery
    {

        private string _text;
        private SQLiteConnection _connection;
        private SQLiteCommand _command;
        private StructureImpl _parameters;

        public SqliteQuery()
        {
            _parameters = new StructureImpl();
            _text = "";
        }

        [ScriptConstructor]
        public static IRuntimeContextInstance Constructor()
        {
            return new SqliteQuery();
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
            //Console.WriteLine("run");
            //_command = new SQLiteCommand(_text, _connection);
            _command.CommandText = _text;
            _command.Prepare();
            foreach (IValue prm in _parameters)
            {
                Console.WriteLine("@" + ((KeyAndValueImpl)prm).Key.ToString() + ": " + ((KeyAndValueImpl)prm).Value);
                _command.Parameters.AddWithValue("@" + ((KeyAndValueImpl)prm).Key.ToString(), ((KeyAndValueImpl)prm).Value);
            }
            //Console.WriteLine("1");
            //_command.Parameters.AddWithValue("@category_id", 1);
            //Console.WriteLine("2");

            SQLiteDataReader reader = _command.ExecuteReader();
            QueryResult result = new QueryResult(reader);

            return result;

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

            //return ValueFactory.Create();

            //const string databaseName = @"C:\cyber.db";
            //SQLiteConnection connection =
            //new SQLiteConnection(string.Format("Data Source={0};", databaseName));
            //connection.Open();
            //SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'example';", connection);
            //SQLiteDataReader reader = command.ExecuteReader();
            //Console.Write("\u250C" + new string('\u2500', 5) + "\u252C" + new string('\u2500', 60) + "\u2510");
            //Console.WriteLine("\n\u2502" + " id \u2502" + new string(' ', 30) + "value" + new string(' ', 25) + "\u2502");
            //Console.Write("\u251C" + new string('\u2500', 5) + "\u253C" + new string('\u2500', 60) + "\u2524\n");
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
            //Console.Write("\u2514" + new string('\u2500', 5) + "\u2534" + new string('\u2500', 60) + "\u2518");
            //connection.Close();
            //Console.ReadKey(true);
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
