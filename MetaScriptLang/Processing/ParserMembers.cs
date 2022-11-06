using MetaScriptLang.Data;

namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        #region Private Members
        private string __InitialDirectory = string.Empty;
        private bool __CaptureParse = false;
        private string __ParsedOutput = string.Empty;

        private string cout 
        {
            set
            {
                Console.Write(value);
            }
        }

        private string cerr
        {
            set
            {
                Console.Error.Write(value);
            }
        }

        System.Collections.Generic.List<Method> methods = new();
        System.Collections.Generic.List<MetaScriptLang.Data.Object> objects = new();
        System.Collections.Generic.List<Variable> variables = new();
        System.Collections.Generic.List<MetaScriptLang.Data.List> lists = new();
        System.Collections.Generic.List<Constant> constants = new();
        System.Collections.Generic.List<Method> ifStatements = new();
        System.Collections.Generic.List<Method> forLoops = new();
        System.Collections.Generic.List<Method> whileLoops = new();
        System.Collections.Generic.List<string> args = new();
        System.Collections.Generic.List<Module> modules = new();
        System.Collections.Generic.List<Script> scripts = new();

        Switch mainSwitch = new();

        string __CurrentLine = string.Empty;
        string __CurrentMethodObject = string.Empty;
        string __CurrentModule = string.Empty;
        string __CurrentObject = string.Empty;
        string __CurrentScript = string.Empty;
        string __CurrentScriptName = string.Empty;
        string __GoTo = string.Empty;
        string __LastValue = string.Empty;
        string __LogFile = string.Empty;
        string __Noctis = string.Empty;
        string __PreviousScript = string.Empty;
        string __PromptStyle = string.Empty;
        string __SavedVarsPath = string.Empty;
        string __SavedVars = string.Empty;
        string __SwitchVarName = string.Empty;
        string __DefaultLoopSymbol = string.Empty;
        string __Null = string.Empty;
        double __NullNum = -Double.MaxValue;

        int __ArgumentCount = 0;
        int __BadMethodCount = 0;
        int __BadObjectCount = 0;
        int __BadVarCount = 0;
        int __CurrentLineNumber = 0;
        int __IfStatementCount = 0;
        int __ForLoopCount = 0;
        int __ParamVarCount = 0;
        int __WhileLoopCount = 0;

        bool __IsCommented = false;
        bool __MultilineComment = false;

        bool __Breaking = false;
        bool __DefiningIfStatement = false;
        bool __DefiningForLoop = false;
        bool __DefiningLocalForLoop = false;
        bool __DefiningLocalSwitchBlock = false;
        bool __DefiningLocalWhileLoop = false;
        bool __DefiningMethod = false;
        bool __DefiningModule = false;
        bool __DefiningNest = false;
        bool __DefiningObject = false;
        bool __DefiningObjectMethod = false;
        bool __DefiningParameterizedMethod = false;
        bool __DefiningPrivateCode = false;
        bool __DefiningPublicCode = false;
        bool __DefiningScript = false;
        bool __DefiningSwitchBlock = false;
        bool __DefiningWhileLoop = false;
        bool __DontCollectMethodVars = false;
        bool __ExecutedIfStatement = false;
        bool __ExecutedMethod = false;
        bool __ExecutedTemplate = false;
        bool __FailedIfStatement = false;
        bool __FailedNest = false;
        bool __GoToLabel = false;
        bool __InDefaultCase = false;
        bool __Returning = false;
        bool __SkipDefaultBlock = false;
        bool __UseCustomPrompt = false;

        string __ErrorVarName = string.Empty;
        string __LastError = string.Empty;
        bool __ExecutedTryBlock = false;
        bool __RaiseCatchBlock = false;
        bool __Negligence = false;
        bool __SkipCatchBlock = false;

        bool suc_stat = false;
        string __GuessedOS = string.Empty;
        #endregion
    }
}
