using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript.Library;
using System.Data.SQLite;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using ScriptEngine.HostedScript.Library.Binary;

namespace OScriptSql
{
    /// <summary>
    /// Предназначен для выполнения запросов к базе данных.
    /// </summary>
    [ContextClass("Запрос", "Query")]
    public class Query : AutoContext<Query>, IOScriptQuery
    {

        private string _text;
        private DbConnection _connection;
        private DbCommand _command;
        private StructureImpl _parameters;

        private DBConnector _connector;

        /// <summary>
        /// Создает новый экземпляр класса Запрос.
        /// </summary>
        public Query()
        {
            _parameters = new StructureImpl();
            _text = "";
        }

        /// <summary>
        /// Создает новый экземпляр класса Запрос.
        /// </summary>
        /// <returns>Запрос</returns>
        [ScriptConstructor]
        public static IRuntimeContextInstance Constructor()
        {
            return new Query();
        }

        /// <summary>
        /// Параметры запроса
        /// </summary>
        [ContextProperty("Параметры", "Parameters")]
        public StructureImpl Parameters => _parameters;


        /// <summary>
        /// Управление таймауотом
        /// </summary>
        /// <value>Число</value>
        [ContextProperty("Таймаут", "Timeout")]
        public int Timeout
        {
            get { return _command.CommandTimeout; }
            set
            {
                _command.CommandTimeout = value;
            }
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

        private void setDbCommandParameters()
        {
            DbParameter param = null;

            foreach (IValue prm in _parameters)
            {
                var paramVal = ((KeyAndValueImpl)prm).Value;
                var paramKey = ((KeyAndValueImpl)prm).Key.AsString();

                if (paramVal.DataType == DataType.String)
                {
                    param = _command.CreateParameter();
                    param.ParameterName = "@" + paramKey;
                    param.Value = paramVal.AsString();
                }
                else if (paramVal.DataType == DataType.Number)
                {
                    param = _command.CreateParameter();
                    param.ParameterName = "@" + paramKey;
                    param.Value = paramVal.AsNumber();
                }
                else if (paramVal.DataType == DataType.Date)
                {
                    param = _command.CreateParameter();
                    param.ParameterName = "@" + paramKey;
                    param.Value = paramVal.AsDate();
                }
                else if (paramVal.DataType == DataType.Boolean)
                {
                    param = _command.CreateParameter();
                    param.ParameterName = "@" + paramKey;
                    param.Value = paramVal.AsBoolean();
                }
                else if (paramVal.DataType == DataType.Object ^ paramVal.GetType() == typeof(BinaryDataContext))
                {
                    param = _command.CreateParameter();
                    param.ParameterName = "@" + paramKey;
                    param.Value = (paramVal as BinaryDataContext).Buffer;
                }

                _command.Parameters.Add(param);
            }

        }

        /// <summary>
        /// Выполняет запрос к базе данных. 
        /// </summary>
        /// <returns>РезультатЗапроса</returns>
        [ContextMethod("Выполнить", "Execute")]
        public IValue Execute()
        {
            _command.Parameters.Clear();
            _command.CommandText = _text;

            setDbCommandParameters();
            DbDataReader reader = _command.ExecuteReader();
            QueryResult result = new QueryResult(reader);
            return result;
        }

        /// <summary>
        /// Выполняет запрос к базе данных. Cиноним для Выполнить
        /// </summary>
        /// <returns>РезультатЗапроса</returns>
        [ContextMethod("ВыполнитьЗапрос", "ExecuteQuery")]
        public IValue ExecuteQuery()
        {
            return Execute();
        }

        /// <summary>
        /// Выполняет запрос на модификацию к базе данных. 
        /// </summary>
        /// <returns>Число - Число обработанных строк.</returns>
        [ContextMethod("ВыполнитьКоманду", "ExecuteCommand")]
        public int ExecuteCommand()
        {
            _command.Parameters.Clear();
            _command.CommandText = _text;
            setDbCommandParameters();
            return _command.ExecuteNonQuery();
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

            if (_connector.DbType == new EnumDBType().Sqlite)
            {
                _command = new SQLiteCommand((SQLiteConnection)connector.Connection);
            }
            else if (_connector.DbType == new EnumDBType().MSSQLServer)
            {
                _command = new SqlCommand
                {
                    Connection = (SqlConnection)connector.Connection
                };
            }
            else if (_connector.DbType == new EnumDBType().MySQL)
            {
                _command = new MySqlCommand
                {
                    Connection = (MySqlConnection)connector.Connection
                };
            }
            else if (_connector.DbType == new EnumDBType().PostgreSQL)
            {
                _command = new NpgsqlCommand
                {
                    Connection = (NpgsqlConnection)connector.Connection
                };
            }

        }

        /// <summary>
        /// Возвращает идентификатор последней добавленной записи.
        /// </summary>
        /// <returns>Число - идентификатор записи</returns>
        [ContextMethod("ИДПоследнейДобавленнойЗаписи", "LastInsertRowId")]
        public int LastInsertRowId()
        {
            if (_connector.DbType == new EnumDBType().Sqlite)
            {
                return (int)((SQLiteConnection)_connection).LastInsertRowId;
            }
            else if (_connector.DbType == new EnumDBType().MSSQLServer)
            {
                return -1;
            }
            else if (_connector.DbType == new EnumDBType().MySQL)
            {
                return (int)((MySqlCommand)_command).LastInsertedId;
            }
            else if (_connector.DbType == new EnumDBType().PostgreSQL)
            {
                return -1;
            }
            return -1;
        }

    }
}
