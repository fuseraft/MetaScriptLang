namespace MetaScriptLang.Engine
{
    using MetaScriptLang.Data;

    public partial class StateEngine
    {
        #region Private Members
        public string cout
        {
            set
            {
                Console.Write(value);
            }
        }

        public string cerr
        {
            set
            {
                Console.Error.Write(value);
            }
        }

        public System.Collections.Generic.Dictionary<string, Method> methods = new();
        public System.Collections.Generic.Dictionary<string, MetaScriptLang.Data.Object> objects = new();
        public System.Collections.Generic.Dictionary<string, Variable> variables = new();
        public System.Collections.Generic.Dictionary<string, MetaScriptLang.Data.List> lists = new();
        public System.Collections.Generic.Dictionary<string, Constant> constants = new();
        public System.Collections.Generic.List<Method> ifStatements = new();
        public System.Collections.Generic.List<Method> forLoops = new();
        public System.Collections.Generic.List<Method> whileLoops = new();
        public System.Collections.Generic.List<string> args = new();
        public System.Collections.Generic.Dictionary<string, Module> modules = new();
        public System.Collections.Generic.Dictionary<string, Script> scripts = new();

        public Switch mainSwitch = new();

        public string __InitialDirectory = string.Empty;
        public bool __CaptureParse = false;
        public string __ParsedOutput = string.Empty;
        public string __CurrentLine = string.Empty;
        public string __CurrentMethodObject = string.Empty;
        public string __CurrentModule = string.Empty;
        public string __CurrentObject = string.Empty;
        public string __CurrentScript = string.Empty;
        public string __CurrentScriptName = string.Empty;
        public string __GoTo = string.Empty;
        public string __LastValue = string.Empty;
        public string __LogFile = string.Empty;
        public string __Noctis = string.Empty;
        public string __PreviousScript = string.Empty;
        public string __PromptStyle = string.Empty;
        public string __SavedVarsPath = string.Empty;
        public string __SavedVars = string.Empty;
        public string __SwitchVarName = string.Empty;
        public string __DefaultLoopSymbol = string.Empty;
        public string __Null = string.Empty;
        public double __NullNum = -Double.MaxValue;

        public int __ArgumentCount = 0;
        public int __BadMethodCount = 0;
        public int __BadObjectCount = 0;
        public int __BadVarCount = 0;
        public int __CurrentLineNumber = 0;
        public int __IfStatementCount = 0;
        public int __ForLoopCount = 0;
        public int __ParamVarCount = 0;
        public int __WhileLoopCount = 0;

        public bool __IsCommented = false;
        public bool __MultilineComment = false;

        public bool __Breaking = false;
        public bool __DefiningIfStatement = false;
        public bool __DefiningForLoop = false;
        public bool __DefiningLocalForLoop = false;
        public bool __DefiningLocalSwitchBlock = false;
        public bool __DefiningLocalWhileLoop = false;
        public bool __DefiningMethod = false;
        public bool __DefiningModule = false;
        public bool __DefiningNest = false;
        public bool __DefiningObject = false;
        public bool __DefiningObjectMethod = false;
        public bool __DefiningParameterizedMethod = false;
        public bool __DefiningPrivateCode = false;
        public bool __DefiningPublicCode = false;
        public bool __DefiningScript = false;
        public bool __DefiningSwitchBlock = false;
        public bool __DefiningWhileLoop = false;
        public bool __DontCollectMethodVars = false;
        public bool __ExecutedIfStatement = false;
        public bool __ExecutedMethod = false;
        public bool __ExecutedTemplate = false;
        public bool __FailedIfStatement = false;
        public bool __FailedNest = false;
        public bool __GoToLabel = false;
        public bool __InDefaultCase = false;
        public bool __Returning = false;
        public bool __SkipDefaultBlock = false;
        public bool __UseCustomPrompt = false;

        public string __ErrorVarName = string.Empty;
        public string __LastError = string.Empty;
        public bool __ExecutedTryBlock = false;
        public bool __RaiseCatchBlock = false;
        public bool __Negligence = false;
        public bool __SkipCatchBlock = false;

        public bool suc_stat = false;
        public string __GuessedOS = string.Empty;
        #endregion
    }
}
