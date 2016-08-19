using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OScriptSql
{
    interface IOScriptQuery : IValue
    {
        // props
        StructureImpl Parameters();
        string Text { get; set; }

        // methods
        IValue Execute();
        void SetParameter(string ParametrName, IValue ParametrValue);

        // my methods
        void SetConnection(DBConnector connector);

    }
}
