using MetaScriptLang.Data;
using MetaScriptLang.Logging;
using System.Linq;

namespace MetaScriptLang.Processing
{
    public partial class Parser
    {
        void forLoop(Method m)
        {
            __DefaultLoopSymbol = "$";

            if (m.isListLoop())
            {
                int i = 0, stop = m.getList().size();

                while (i < stop)
                {
                    for (int z = 0; z < m.size(); z++)
                    {
                        string cleaned = string.Empty, builder = string.Empty;
                        int len = m.at(z).Length;
                        bool buildSymbol = false, almostBuild = false, ended = false;

                        for (int a = 0; a < len; a++)
                        {
                            if (almostBuild)
                            {
                                if (m.at(z)[a] == '{')
                                    buildSymbol = true;
                            }

                            if (buildSymbol)
                            {
                                if (m.at(z)[a] == '}')
                                {
                                    almostBuild = false;
                                    buildSymbol = false;
                                    ended = true;

                                    builder = subtractString(builder, "{");

                                    if (builder == m.getSymbolString())
                                    {
                                        cleaned += (m.getList().at(i));
                                    }

                                    builder = string.Empty;
                                }
                                else
                                {
                                    builder += (m.at(z)[a]);
                                }
                            }

                            if (m.at(z)[a] == '$')
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
                                    cleaned += (m.at(z)[a]);
                                }
                            }
                        }

                        parse(cleaned);
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
                if (m.isInfinite())
                {
                    if (__Negligence)
                    {
                        for (; ; )
                        {
                            for (int z = 0; z < m.size(); z++)
                                parse(m.at(z));

                            if (__Breaking == true)
                            {
                                __Breaking = false;
                                break;
                            }
                        }
                    }
                    else
                        error(ErrorLogger.INFINITE_LOOP, "", true);
                }
                else if (m.start() < m.stop())
                {
                    int start = m.start(), stop = m.stop();

                    while (start <= stop)
                    {
                        for (int z = 0; z < m.size(); z++)
                        {
                            string cleanString = (""), builder = (""), tmp = (m.at(z));
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

                                        builder = subtractString(builder, "{");

                                        if (builder == m.getSymbolString())
                                            cleanString += (itos(start));

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

                            parse(cleanString);
                        }

                        start++;

                        if (__Breaking == true)
                        {
                            __Breaking = false;
                            break;
                        }
                    }
                }
                else if (m.start() > m.stop())
                {
                    int start = m.start(), stop = m.stop();

                    while (start >= stop)
                    {
                        for (int z = 0; z < m.size(); z++)
                        {
                            string cleaned = string.Empty, builder = string.Empty, tmp = (m.at(z));
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

                                        builder = subtractString(builder, "{");

                                        if (builder == m.getSymbolString())
                                            cleaned += (itos(start));

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

                            parse(cleaned);
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
