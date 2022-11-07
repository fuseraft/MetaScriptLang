namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void __true()
        {
            setLastValue("true");
        }

        void __false()
        {
            setLastValue("false");
        }

        bool success()
        {
            return (suc_stat);
        }

        void failedFor()
        {
            Method forMethod = new("[for#" + itos(__ForLoopCount) + "]");
            forMethod.SetIsForLoop(false);
            __DefiningForLoop = true;
            forLoops.Add(forMethod);
            __DefaultLoopSymbol = "$";
            suc_stat = false;
        }

        void failedWhile()
        {
            Method whileMethod = new("[while#" + itos(__WhileLoopCount) + "]");
            whileMethod.SetIsWhileLoop(false);
            __DefiningWhileLoop = true;
            whileLoops.Add(whileMethod);
        }

        void successfullWhile(string v1, string op, string v2)
        {
            Method whileMethod = new("[while#" + itos(__WhileLoopCount) + "]");
            whileMethod.SetIsWhileLoop(true);
            whileMethod.SetWhileLoopValues(v1, op, v2);
            __DefiningWhileLoop = true;
            whileLoops.Add(whileMethod);
            __WhileLoopCount++;
        }

        void successfulFor(List list)
        {
            Method forMethod = new("[for#" + itos(__ForLoopCount) + "]");
            forMethod.SetIsForLoop(true);
            forMethod.SetForListLoop(list);
            forMethod.SetListLoop();
            forMethod.SetSymbol(__DefaultLoopSymbol);
            __DefiningForLoop = true;
            forLoops.Add(forMethod);
            __ForLoopCount++;
            suc_stat = true;
        }

        void successfulFor(double a, double b, string op)
        {
            Method forMethod = new("[for#" + itos(__ForLoopCount) + "]");
            forMethod.SetIsForLoop(true);
            forMethod.SetSymbol(__DefaultLoopSymbol);

            if (op == "<=")
                forMethod.SetForLoopValues((int)a, (int)b);
            else if (op == ">=")
                forMethod.SetForLoopValues((int)a, (int)b);
            else if (op == "<")
                forMethod.SetForLoopValues((int)a, (int)b - 1);
            else if (op == ">")
                forMethod.SetForLoopValues((int)a, (int)b + 1);

            __DefiningForLoop = true;
            forLoops.Add(forMethod);
            __ForLoopCount++;
            suc_stat = true;
        }

        void successfulFor()
        {
            Method forMethod = new("[for#" + itos(__ForLoopCount) + "]");
            forMethod.SetIsForLoop(true);
            forMethod.SetInfinite();
            __DefiningForLoop = true;
            forLoops.Add(forMethod);
            __ForLoopCount++;
            suc_stat = true;
        }

        void setFalseIf()
        {
            __LastValue = "false";

            if (!__DefiningNest)
            {
                Method ifMethod = new("[failif]");
                ifMethod.SetIsIfStatement(false);
                __DefiningIfStatement = true;
                ifStatements.Add(ifMethod);
                __FailedIfStatement = true;
                __FailedNest = true;
            }
            else
                __FailedNest = true;
        }

        void setTrueIf()
        {
            __LastValue = "true";

            if (__DefiningNest)
            {
                ifStatements[(int)ifStatements.Count - 1].BuildNest();
                __FailedNest = false;
            }
            else
            {
                Method ifMethod = new("[if#" + itos(__IfStatementCount) + "]");
                ifMethod.SetIsIfStatement(true);
                __DefiningIfStatement = true;
                ifStatements.Add(ifMethod);
                __IfStatementCount++;
                __FailedIfStatement = false;
                __FailedNest = false;
            }
        }

        void error(int errorType, string errorInfo, bool quit)
        {
            System.Text.StringBuilder completeError = new();
            completeError.Append("##\n# error:\t");
            completeError.Append(ErrorLogger.getErrorString(errorType));
            completeError.Append(":\t");
            completeError.Append(errorInfo);
            completeError.Append("\n# line ");
            completeError.Append(itos(__CurrentLineNumber));
            completeError.Append(":\t");
            completeError.Append(__CurrentLine);
            completeError.Append("\n##\n");

            if (__ExecutedTryBlock)
            {
                __RaiseCatchBlock = true;
                __LastError = completeError.ToString();
            }
            else
            {
                if (__CaptureParse)
                    __ParsedOutput += completeError;
                else
                    cerr = completeError.ToString();
            }

            if (!__Negligence)
            {
                if (quit)
                {
                    clearAll();
                    System.Environment.Exit(0);
                }
            }
        }
    }
}
