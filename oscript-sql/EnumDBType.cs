using ScriptEngine.Machine.Contexts;

namespace OScriptSql
{
    /// <summary>
    /// Тип поддерживаемой СУБД
    /// </summary>
    [ContextClass("ТипСУБД", "DBType")]
    public class EnumDBType : AutoContext<EnumDBType>
    {
        /// <summary>
        /// Тип базы данных SQLite
        /// </summary>
        [ContextProperty("sqlite", "sqlite")]
        public int Sqlite => 0;

        /// <summary>
        /// Тип базы данных MSSQLServer
        /// </summary>
        [ContextProperty("MSSQLServer", "MSSQLServer")]
        public int MSSQLServer => 1;

        /// <summary>
        /// Тип базы данных MySQL
        /// </summary>
        [ContextProperty("MySQL", "MySQL")]
        public int MySQL => 2;

        /// <summary>
        /// Тип базы данных PostgreSQL
        /// </summary>
        [ContextProperty("PostgreSQL", "PostgreSQL")]
        public int PostgreSQL => 3;

    }
}
