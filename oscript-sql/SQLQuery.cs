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
using MySql.Data.MySqlClient;
using Npgsql;

namespace OScriptSql
{
    /// <summary>
    /// Предназначен для выполнения запросов к базе данных.
    /// </summary>
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


        [ContextProperty("Параметры", "Parameters")]
        public StructureImpl Parameters
        {
            get { return _parameters; }
        }



        /// <summary>
        /// Содержит исходный текст выполняемого запроса.
        /// </summary>
        /// <value>Строка</value>
        [ContextProperty("Текст", "Text")]
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
            }
        }

        /// <summary>
        /// Выполняет запрос к базе данных. 
        /// </summary>
        /// <returns>РезультатЗапроса</returns>
        [ContextMethod("Выполнить", "Execute")]
        public IValue Execute()
        {
            var result = new QueryResult();
            DbDataReader reader = null;

            _command.Parameters.Clear();
            _command.CommandText = _text;
            _command.Prepare();

            if (_connector.DbType == (new EnumDBType()).sqlite)
            {
                foreach (IValue prm in _parameters)
                {
                    ((SQLiteCommand)_command).Parameters.AddWithValue("@" + ((KeyAndValueImpl)prm).Key.ToString(), ((KeyAndValueImpl)prm).Value);
                }
                reader = _command.ExecuteReader();
            }
            else if (_connector.DbType == (new EnumDBType()).MSSQLServer)
            {
                foreach (IValue prm in _parameters)
                {
                    var paramVal = ((KeyAndValueImpl)prm).Value;
                    var paremKey = ((KeyAndValueImpl)prm).Key.AsString();

                    if (paramVal.DataType == DataType.String)
                    {
                        ((SqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsString());
                    }
                    else if (paramVal.DataType == DataType.Number)
                    {
                        ((SqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsNumber());
                    }
                    else if (paramVal.DataType == DataType.Date)
                    {
                        ((SqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsDate());
                    }
                    else if (paramVal.DataType == DataType.Boolean)
                    {
                        ((SqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsBoolean());
                    }
                }
                reader = ((SqlCommand)_command).ExecuteReader();
            }
            else if (_connector.DbType == (new EnumDBType()).MySQL)
            {
                foreach (IValue prm in _parameters)
                {
                    var paramVal = ((KeyAndValueImpl)prm).Value;
                    var paremKey = ((KeyAndValueImpl)prm).Key.AsString();

                    if (paramVal.DataType == DataType.String)
                    {
                        ((MySqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsString());
                    }
                    else if (paramVal.DataType == DataType.Number)
                    {
                        ((MySqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsNumber());
                    }
                    else if (paramVal.DataType == DataType.Date)
                    {
                        ((MySqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsDate());
                    }
                    else if (paramVal.DataType == DataType.Boolean)
                    {
                        ((MySqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsBoolean());
                    }
                }
                reader = ((MySqlCommand)_command).ExecuteReader();
            }
            else if (_connector.DbType == (new EnumDBType()).PostgreSQL)
            {
                foreach (IValue prm in _parameters)
                {
                    var paramVal = ((KeyAndValueImpl)prm).Value;
                    var paremKey = ((KeyAndValueImpl)prm).Key.AsString();

                    if (paramVal.DataType == DataType.String)
                    {
                        ((NpgsqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsString());
                    }
                    else if (paramVal.DataType == DataType.Number)
                    {
                        ((NpgsqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsNumber());
                    }
                    else if (paramVal.DataType == DataType.Date)
                    {
                        ((NpgsqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsDate());
                    }
                    else if (paramVal.DataType == DataType.Boolean)
                    {
                        ((NpgsqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsBoolean());
                    }
                }
                reader = ((NpgsqlCommand)_command).ExecuteReader();
                
            }
            result = new QueryResult(reader);
            return result;
        }

        /// <summary>
        /// Выполняет запрос на модификацию к базе данных. 
        /// </summary>
        /// <returns>Число - Число обработанных строк.</returns>
        [ContextMethod("ВыполнитьКоманду", "ExecuteCommand")]
        public int ExecuteCommand()
        {
            var sec = new SystemEnvironmentContext();
            string versionOnescript = sec.Version;

            string[] verInfo = versionOnescript.Split('.');

            if (Convert.ToInt64(verInfo[2]) >= 15)
            {
                Console.WriteLine("> 15");
            }

            var result = new QueryResult();

            _command.Parameters.Clear();
            _command.CommandText = _text;
            //_command.Prepare();

            if (_connector.DbType == (new EnumDBType()).sqlite)
            {
                foreach (IValue prm in _parameters)
                {
                    ((SQLiteCommand)_command).Parameters.AddWithValue("@" + ((KeyAndValueImpl)prm).Key.ToString(), ((KeyAndValueImpl)prm).Value);
                }
                return _command.ExecuteNonQuery();

            }
            else if (_connector.DbType == (new EnumDBType()).MSSQLServer)
            {
                foreach (IValue prm in _parameters)
                {
                    var paramVal = ((KeyAndValueImpl)prm).Value;
                    var paremKey = ((KeyAndValueImpl)prm).Key.AsString();

                    if (paramVal.DataType == DataType.String)
                    {
                        ((SqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsString());
                    }
                    else if (paramVal.DataType == DataType.Number)
                    {
                        ((SqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsNumber());
                    }
                    else if (paramVal.DataType == DataType.Date)
                    {
                        ((SqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsDate());
                    }
                    else if (paramVal.DataType == DataType.Boolean)
                    {
                        ((SqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsBoolean());
                    }
                    //else if (paramVal.DataType == DataType.Object)
                    //{
                    //    cmd1.Parameters.AddWithValue(paremKey, paramVal.AsObject());
                    //}
                }
                return _command.ExecuteNonQuery();
            }
            else if (_connector.DbType == (new EnumDBType()).MySQL)
            {
                foreach (IValue prm in _parameters)
                {
                    //var vl = ((KeyAndValueImpl)prm).Value;
                    //((MySqlCommand)_command).Parameters.AddWithValue("@" + ((KeyAndValueImpl)prm).Key.ToString(), vl);
                    var paramVal = ((KeyAndValueImpl)prm).Value;
                    var paremKey = ((KeyAndValueImpl)prm).Key.AsString();

                    if (paramVal.DataType == DataType.String)
                    {
                        ((MySqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsString());
                    }
                    else if (paramVal.DataType == DataType.Number)
                    {
                        ((MySqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsNumber());
                    }
                    else if (paramVal.DataType == DataType.Date)
                    {
                        ((MySqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsDate());
                    }
                    else if (paramVal.DataType == DataType.Boolean)
                    {
                        ((MySqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsBoolean());
                    }

                }
                return _command.ExecuteNonQuery();
            }
            else if (_connector.DbType == (new EnumDBType()).PostgreSQL)
            {
                foreach (IValue prm in _parameters)
                {
                    var paramVal = ((KeyAndValueImpl)prm).Value;
                    var paremKey = ((KeyAndValueImpl)prm).Key.AsString();

                    if (paramVal.DataType == DataType.String)
                    {
                        ((NpgsqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsString());
                    }
                    else if (paramVal.DataType == DataType.Number)
                    {
                        ((NpgsqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsNumber());
                    }
                    else if (paramVal.DataType == DataType.Date)
                    {
                        ((NpgsqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsDate());
                    }
                    else if (paramVal.DataType == DataType.Boolean)
                    {
                        ((NpgsqlCommand)_command).Parameters.AddWithValue(paremKey, paramVal.AsBoolean());
                    }
                    //else if (paramVal.DataType == DataType.Object)
                    //{
                    //    cmd1.Parameters.AddWithValue(paremKey, paramVal.AsObject());
                    //}

                }
                return _command.ExecuteNonQuery();
            }
            return 0;
        }

        /// <summary>
        /// Устанавливает параметр запроса. Параметры доступны для обращения в тексте запроса. 
        /// С помощью этого метода можно передавать переменные в запрос, например, для использования в условиях запроса.
        /// ВАЖНО: В запросе имя параметра указывается с использованием '@'.
        /// </summary>
        /// <example>
        /// Запрос.Текст = "select * from mytable where category_id = @category_id";
        /// Запрос.УстановитьПараметр("category_id", 1);
        /// </example>
        /// <param name="ParametrName">Строка - Имя параметра</param>
        /// <param name="ParametrValue">Произвольный - Значение параметра</param>
        [ContextMethod("УстановитьПараметр", "SetParameter")]
        public void SetParameter(string ParametrName, IValue ParametrValue)
        {
            _parameters.Insert(ParametrName, ParametrValue);
        }

        /// <summary>
        /// Установка соединения с БД.
        /// </summary>
        /// <param name="connector">Соединение - объект соединение с БД</param>
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
            else if (_connector.DbType == (new EnumDBType()).MySQL)
            {
                _command = new MySqlCommand();
                _command.Connection = (MySqlConnection)connector.Connection;
            }
            else if (_connector.DbType == (new EnumDBType()).PostgreSQL)
            {
                _command = new NpgsqlCommand();
                _command.Connection = (NpgsqlConnection)connector.Connection;
            }

        }


    }
}
