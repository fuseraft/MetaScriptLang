namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void SetSubstring(string arg1, string arg2, string beforeBracket)
        {
            if (!engine.IsStringVariable(beforeBracket))
            {
                ErrorLogger.Error(ErrorLogger.NULL_STRING, beforeBracket, false);
            }

            System.Collections.Generic.List<string> listRange = StringHelper.GetBracketRange(arg2);
            string variableString = engine.GetVariableString(beforeBracket);

            if (listRange.Count < 1 || listRange.Count > 2)
            {
                ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, arg2, false);
            }
            else if (listRange.Count == 1)
            {
                string rangeBegin = listRange[0];

                if (rangeBegin.Length <= 0 || !StringHelper.IsNumeric(rangeBegin))
                {
                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + "..", false);
                    return;
                }

                if (variableString.Length - 1 >= StringHelper.StoI(rangeBegin) && StringHelper.StoI(rangeBegin) >= 0)
                {
                    string tempString = string.Empty + variableString[StringHelper.StoI(rangeBegin)];

                    if (engine.VariableExists(arg1))
                        engine.SetVariableString(arg1, tempString);
                    else
                        engine.CreateStringVariable(arg1, tempString);
                }
            }
            else if (listRange.Count == 2)
            {
                System.Text.StringBuilder tempString = new();
                string rangeBegin = listRange[0], rangeEnd = listRange[1];

                if ((!(rangeBegin.Length != 0 && rangeEnd.Length != 0))
                    || !(StringHelper.IsNumeric(rangeBegin) && StringHelper.IsNumeric(rangeEnd)))
                {
                    ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                    return;
                }

                if (StringHelper.StoI(rangeBegin) < StringHelper.StoI(rangeEnd))
                {
                    if (!(variableString.Length - 1 >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0))
                    {
                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                        return;
                    }

                    for (int i = StringHelper.StoI(rangeBegin); i <= StringHelper.StoI(rangeEnd); i++)
                        tempString.Append(variableString[i]);

                    if (engine.VariableExists(arg1))
                        engine.SetVariableString(arg1, tempString.ToString());
                    else
                        engine.CreateStringVariable(arg1, tempString.ToString());
                }
                else if (StringHelper.StoI(rangeBegin) > StringHelper.StoI(rangeEnd))
                {
                    if (!(variableString.Length >= StringHelper.StoI(rangeEnd) && StringHelper.StoI(rangeBegin) >= 0))
                    {
                        ErrorLogger.Error(ErrorLogger.OUT_OF_BOUNDS, rangeBegin + ".." + rangeEnd, false);
                        return;
                    }

                    for (int i = StringHelper.StoI(rangeBegin); i >= StringHelper.StoI(rangeEnd); i--)
                        tempString.Append(variableString[i]);

                    if (engine.VariableExists(arg1))
                        engine.SetVariableString(arg1, tempString.ToString());
                    else
                        engine.CreateStringVariable(arg1, tempString.ToString());
                }
            }
        }
    }
}
