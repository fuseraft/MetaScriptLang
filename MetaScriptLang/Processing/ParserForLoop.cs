namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Data;
    using MetaScriptLang.Helpers;
    using MetaScriptLang.Logging;

    public partial class Parser
    {
        void forLoop(Method m)
        {
            __DefaultLoopSymbol = "$";

            if (m.IsListLoop())
            {
                int i = 0, stop = m.GetList().GetSize();

                while (i < stop)
                {
                    for (int z = 0; z < m.GetMethodSize(); z++)
                    {
                        string cleaned = string.Empty, builder = string.Empty;
                        int len = m.GetLine(z).Length;
                        bool buildSymbol = false, almostBuild = false, ended = false;

                        for (int a = 0; a < len; a++)
                        {
                            if (almostBuild)
                            {
                                if (m.GetLine(z)[a] == '{')
                                    buildSymbol = true;
                            }

                            if (buildSymbol)
                            {
                                if (m.GetLine(z)[a] == '}')
                                {
                                    almostBuild = false;
                                    buildSymbol = false;
                                    ended = true;

                                    builder = StringHelper.SubtractString(builder, "{");

                                    if (builder == m.GetSymbol())
                                    {
                                        cleaned += (m.GetList().GetItemAt(i));
                                    }

                                    builder = string.Empty;
                                }
                                else
                                {
                                    builder += (m.GetLine(z)[a]);
                                }
                            }

                            if (m.GetLine(z)[a] == '$')
                            {
                                almostBuild = true;
                            }

                            if (!almostBuild && !buildSymbol)
                            {
                                if (ended)
                                {
                                    ended = false;
                                }
                                else
                                {
                                    cleaned += (m.GetLine(z)[a]);
                                }
                            }
                        }

                        ParseString(cleaned);
                    }

                    i++;

                    if (__Breaking == true)
                    {
                        __Breaking = false;
                        break;
                    }
                }
            }
            else
            {
                if (m.IsInfinite())
                {
                    if (__Negligence)
                    {
                        for (; ; )
                        {
                            for (int z = 0; z < m.GetMethodSize(); z++)
                                ParseString(m.GetLine(z));

                            if (__Breaking == true)
                            {
                                __Breaking = false;
                                break;
                            }
                        }
                    }
                    else
                        ErrorLogger.Error(ErrorLogger.INFINITE_LOOP, "", true);
                }
                else if (m.Start() < m.Stop())
                {
                    int start = m.Start(), stop = m.Stop();

                    while (start <= stop)
                    {
                        for (int z = 0; z < m.GetMethodSize(); z++)
                        {
                            string cleanString = (""), builder = (""), tmp = (m.GetLine(z));
                            int l = (tmp.Length);
                            bool buildSymbol = false, almostBuild = false, ended = false;

                            for (int a = 0; a < l; a++)
                            {
                                if (almostBuild)
                                {
                                    if (tmp[a] == '{')
                                        buildSymbol = true;
                                }

                                if (buildSymbol)
                                {
                                    if (tmp[a] == '}')
                                    {
                                        almostBuild = false;
                                        buildSymbol = false;
                                        ended = true;

                                        builder = StringHelper.SubtractString(builder, "{");

                                        if (builder == m.GetSymbol())
                                            cleanString += (StringHelper.ItoS(start));

                                        builder = string.Empty;
                                    }
                                    else
                                        builder += (tmp[a]);
                                }

                                if (tmp[a] == '$')
                                    almostBuild = true;

                                if (!almostBuild && !buildSymbol)
                                {
                                    if (ended)
                                        ended = false;
                                    else
                                        cleanString += (tmp[a]);
                                }
                            }

                            ParseString(cleanString);
                        }

                        start++;

                        if (__Breaking == true)
                        {
                            __Breaking = false;
                            break;
                        }
                    }
                }
                else if (m.Start() > m.Stop())
                {
                    int start = m.Start(), stop = m.Stop();

                    while (start >= stop)
                    {
                        for (int z = 0; z < m.GetMethodSize(); z++)
                        {
                            string cleaned = string.Empty, builder = string.Empty, tmp = (m.GetLine(z));
                            int l = (tmp.Length);
                            bool buildSymbol = false, almostBuild = false, ended = false;

                            for (int a = 0; a < l; a++)
                            {
                                if (almostBuild)
                                {
                                    if (tmp[a] == '{')
                                        buildSymbol = true;
                                }

                                if (buildSymbol)
                                {
                                    if (tmp[a] == '}')
                                    {
                                        almostBuild = false;
                                        buildSymbol = false;
                                        ended = true;

                                        builder = StringHelper.SubtractString(builder, "{");

                                        if (builder == m.GetSymbol())
                                            cleaned += (StringHelper.ItoS(start));

                                        builder = string.Empty;
                                    }
                                    else
                                        builder += (tmp[a]);
                                }

                                if (tmp[a] == '$')
                                    almostBuild = true;

                                if (!almostBuild && !buildSymbol)
                                {
                                    if (ended)
                                        ended = false;
                                    else
                                        cleaned += (tmp[a]);
                                }
                            }

                            ParseString(cleaned);
                        }

                        start--;

                        if (__Breaking == true)
                        {
                            __Breaking = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}
