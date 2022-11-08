namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;

    public partial class Parser
    {

        //	modes:
        //		0 = createVariable
        //		1 = setVariable
        //		2 = setLastValue
        //      3 = print
        void InternalGetEnv(string arg0, string after, int mode)
        {
            Crypt c = new();
            string defaultValue = c.e(DateTime.Now.ToString());
            string sValue = (defaultValue);
            double dValue = 0;

            if (after == "cwd")
            {
                sValue = Environment.CurrentDirectory;
            }
            else if (after == "noctis")
            {
                sValue = __Noctis;
            }
            else if (after == "os?")
            {
                sValue = "windows";
            }
            else if (after == "user")
            {
                sValue = Environment.UserName;
            }
            else if (after == "machine")
            {
                sValue = Environment.MachineName;
            }
            else if (after == "init_dir" || after == "initial_directory")
            {
                sValue = __InitialDirectory;
            }
            else if (after == "this_second")
            {
                dValue = (double)DateTime.Now.Second;
            }
            else if (after == "this_minute")
            {
                dValue = (double)DateTime.Now.Minute;
            }
            else if (after == "this_hour")
            {
                dValue = (double)DateTime.Now.Hour;
            }
            else if (after == "this_month")
            {
                dValue = (double)DateTime.Now.Month;
            }
            else if (after == "this_year")
            {
                dValue = (double)DateTime.Now.Year;
            }
            else if (after == "day_of_this_month")
            {
                dValue = Convert.ToDouble(DateTime.Now.ToString("dd"));
            }
            else if (after == "day_of_this_year")
            {
                sValue = "obsolete";
            }
            else if (after == "day_of_this_week")
            {
                sValue = DateTime.Now.ToString("dddd");
            }
            else if (after == "month_of_this_year")
            {
                sValue = DateTime.Now.ToString("MMMM");
            }
            else if (after == "am_or_pm")
            {
                sValue = DateTime.Now.ToString("tt");
            }
            else if (after == "now")
            {
                sValue = DateTime.Now.ToString("O");
            }
            else if (after == "last_error")
            {
                sValue = __LastError;
            }
            else if (after == "last_value")
            {
                sValue = __LastValue;
            }
            else if (after == "empty_string")
            {
                sValue = string.Empty;
            }
            else if (after == "empty_number")
            {
                dValue = 0;
            }
            else
            {
                sValue = System.Environment.GetEnvironmentVariable(after) ?? string.Empty;
            }

            switch (mode)
            {
                case 0:
                    if (sValue != defaultValue)
                    {
                        engine.CreateVariableString(arg0, sValue);
                    }
                    else
                    {
                        engine.CreateVariableNumber(arg0, dValue);
                    }
                    break;
                case 1:
                    if (sValue != defaultValue)
                    {
                        engine.SetVariableString(arg0, sValue);
                    }
                    else
                    {
                        engine.SetVariableNumber(arg0, dValue);
                    }
                    break;
                case 2:
                    if (sValue != defaultValue)
                    {
                        SetLastValue(sValue);
                    }
                    else
                    {
                        SetLastValue(StringHelper.DtoS(dValue));
                    }
                    break;
                case 3:
                    if (sValue != defaultValue)
                    {
                        writeline(sValue);
                    }
                    else
                    {
                        writeline(StringHelper.DtoS(dValue));
                    }
                    break;
            }
        }
    }
}
