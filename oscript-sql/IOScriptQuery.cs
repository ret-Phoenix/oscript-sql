using ScriptEngine.Machine;
using ScriptEngine.HostedScript.Library;

namespace OScriptSql
{
    interface IOScriptQuery : IValue
    {
        // props
        StructureImpl Parameters { get; }
        string Text { get; set; }

        // methods
        IValue Execute();
        void SetParameter(string ParametrName, IValue ParametrValue);

        // my methods
        void SetConnection(DBConnector connector);


    }
}
