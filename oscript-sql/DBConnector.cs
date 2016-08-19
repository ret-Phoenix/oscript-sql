﻿using ScriptEngine.Machine;
using ScriptEngine.HostedScript.Library;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using ScriptEngine.Machine.Contexts;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Data.SQLite;

namespace OScriptSql
{
    /// <summary>
    /// Соединение с БД. Используется для указания источника данных объекта Запрос.
    /// </summary>
    [ContextClass("Соединение", "Connection")]
    class DBConnector : AutoContext<DBConnector>
    {
        private int _dbType;
        private int _port;
        private string _server;
        private string _dbName;
        private string _login;
        private string _password;
        private DbConnection _connection;
        private string _connectionString;

        public DBConnector()
        {
            _dbType = 0;
            _port = 0;
            _server = "";
            _dbName = "";
            _login = "";
            _password = "";
            _connectionString = "";
            
        }

        public override string ToString()
        {
            return "Соединение";
        }


        /// <summary>
        /// Типы поддерживаемых СУБД
        /// </summary>
        /// <value>ТипСУБД</value>
        [ContextProperty("ТипыСУБД", "DBTypes")]
        public IValue DbTypes
        {
            get
            {
                var dtype = new EnumDBType();
                return dtype;
            }
        }


        /// <summary>
        /// Тип подключенной СУБД
        /// </summary>
        /// <value>ТипСУБД</value>
        [ContextProperty("ТипСУБД", "DBType")]
        public int DbType
        {
            get
            {
                return _dbType;
            }

            set
            {
                _dbType = value;
            }
        }

        /// <summary>
        /// Порт подключения
        /// </summary>
        /// <value>Число</value>
        [ContextProperty("Порт", "Port")]
        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
            }
        }

        /// <summary>
        /// Имя или IP сервера
        /// </summary>
        /// <value>Строка</value>
        [ContextProperty("Сервер", "Server")]
        public string Server
        {
            get
            {
                return _server;
            }

            set
            {
                _server = value;
            }
        }

        /// <summary>
        /// Имя базы, в случае с SQLITE - путь к базе
        /// </summary>
        /// <value>Строка</value>
        [ContextProperty("ИмяБазы", "DbName")]
        public string DbName
        {
            get
            {
                return _dbName;
            }

            set
            {
                _dbName = value;
            }
        }

        /// <summary>
        /// Пользователь под которым происходит подключение.
        /// Если СУБД MS SQL и пользователь не указан - используется Windows авторизация.
        /// </summary>
        /// <value>Строка</value>
        [ContextProperty("ИмяПользователя", "Login")]
        public string Login
        {
            get
            {
                return _login;
            }

            set
            {
                _login = value;
            }
        }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        /// <value>Строка</value>
        [ContextProperty("Пароль", "Password")]
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        /// <summary>
        /// Статус соединения с БД
        /// </summary>
        /// <value>ConnectionState</value>
        [ContextProperty("Открыто", "IsOpen")]
        public string IsOpen
        {
            get
            {
                return Connection.State.ToString();
            }
        }

        public DbConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>
        /// Подготовленная строка соединения. В случае sqlite аналог ИмяБазы
        /// </summary>
        /// <value>Строка</value>
        [ContextProperty("СтрокаСоединения", "ConnectionString")]
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }

            set
            {
                _connectionString = value;
            }
        }

        [ScriptConstructor]
        public static IRuntimeContextInstance Constructor()
        {
            return new DBConnector();
        }

        /// <summary>
        /// Открыть соединение с БД
        /// </summary>
        /// <returns>Булево</returns>
        [ContextMethod("Открыть", "Open")]
        public bool Open()
        {
            if (DbType == (new EnumDBType()).sqlite)
            {
                string filePath = DbName;
                if (filePath == string.Empty )
                {
                    filePath = ConnectionString;
                }
                _connection = new SQLiteConnection(string.Format("Data Source={0};", filePath));
                if (System.IO.File.Exists(DbName))
                {
                    try
                    {
                        _connection.Open();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }

                }
                else
                {
                    Console.WriteLine("Create db: " + DbName);
                    _connection.Open();
                    return true;
                }
            }
            else if (DbType == (new EnumDBType()).MSSQLServer)
            {
                _connection = new SqlConnection();

                if (ConnectionString != String.Empty)
                {
                    _connection.ConnectionString = ConnectionString;
                }
                else
                {
                    string connStr = @"Data Source=" + Server;
                    if (Port != 0)
                    {
                        connStr += "," + Port.ToString();
                    }
                    connStr += "; Initial Catalog= " + DbName + ";";

                    if (Login != String.Empty)
                    {
                        connStr += "User ID = " + Login +";";
                        if (Password != String.Empty)
                        {
                            connStr += "Password = " + Password + ";";
                        }
                    }
                    else
                    {
                        connStr += "Integrated Security=True";
                    }
                    
                    _connection.ConnectionString = connStr;
                }

                _connection.Open();
            }
            return false;
        }

        /// <summary>
        /// Закрыть соединение с БД
        /// </summary>
        [ContextMethod("Закрыть", "Close")]
        public void Close()
        {
            _connection.Close();
            _connection.ConnectionString = "";
            _connection.Dispose();
            _connection = null;
        }

    }
}
