namespace MetaScriptLang.Processing
{
    using MetaScriptLang.Helpers;

    public partial class Parser
    {

        bool notStandardZeroSpace(string arg)
        {
            string standardZeroSpaceWords =
                "};break;caught;clear_all!;clear_constants!clear_lists!;clear_methods!;"
                + "clear_objects!;clear_variables!;else;end;exit;failif;leave!;"
                + "no_methods?;no_objects?;no_variables?;parser;pass;private;public;try";

            return !StringHelper.ContainsString(standardZeroSpaceWords, arg);
        }

        bool notStandardOneSpace(string arg)
        {
            string standardOneSpaceWords =
                "!;?;__begin__;call_method;cd;chdir;collect?;"
                + "decrypt;delay;directory?;dpush;dpop;"
                + "encrypt;err;error;file?;for;forget;fpush;fpop;"
                + "garbage?;globalize;goto;if;init_dir;intial_directory;"
                + "directory?;file?;list?;lowercase?;method?;"
                + "number?;object?;string?;uppercase?;variable?;"
                + "list;list?;load;lock;loop;lose;"
                + "method;[method];object;out;"
                + "print;println;prompt;remember;remove;return;"
                + "save;say;see;see_string;see_number;stdout;switch;"
                + "template;unlock;";

            return !StringHelper.ContainsString(standardOneSpaceWords, arg);
        }

        bool notStandardTwoSpace(string arg)
        {
            return !StringHelper.ContainsString("=;+=;-=;*=;%=;/=;**=;+;-;*;**;/;%;++=;--=;?;!", arg);
        }
    }
}
