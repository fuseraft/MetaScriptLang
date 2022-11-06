namespace MetaScriptLang.Logging
{
    public class ErrorLogger
    {
        public const int IS_NULL = 0;
        public const int BAD_LOAD = 1;
        public const int CONV_ERR = 2;
        public const int INFINITE_LOOP = 3;
        public const int INVALID_OP = 4;
        public const int DIR_EXISTS = 5;
        public const int DIR_NOT_FOUND = 6;
        public const int FILE_EXISTS = 7;
        public const int FILE_NOT_FOUND = 8;
        public const int OUT_OF_BOUNDS = 9;
        public const int INVALID_RANGE_SEP = 10;
        public const int INVALID_SEQ = 11;
        public const int INVALID_SEQ_SEP = 12;
        public const int INVALID_VAR_DECL = 13;
        public const int LIST_UNDEFINED = 14;
        public const int METHOD_DEFINED = 15;
        public const int METHOD_UNDEFINED = 16;
        public const int NULL_NUMBER = 17;
        public const int NULL_STRING = 18;
        public const int OBJ_METHOD_UNDEFINED = 19;
        public const int OBJ_UNDEFINED = 20;
        public const int OBJ_VAR_UNDEFINED = 21;
        public const int VAR_DEFINED = 22;
        public const int VAR_UNDEFINED = 23;
        public const int TARGET_UNDEFINED = 24;
        public const int CONST_UNDEFINED = 25;
        public const int INVALID_OPERATOR = 26;
        public const int IS_EMPTY = 27;
        public const int READ_FAIL = 28;
        public const int DIVIDED_BY_ZERO = 29;
        public const int UNDEFINED = 30;
        public const int UNDEFINED_OS = 31;

        public static string getErrorString(int errorType)
        {
            System.Text.StringBuilder errorString = new();

            switch (errorType)
            {
                case IS_NULL:
                    errorString.Append("is null");
                    break;
                case BAD_LOAD:
                    errorString.Append("bad load");
                    break;
                case CONV_ERR:
                    errorString.Append("conversion error");
                    break;
                case INFINITE_LOOP:
                    errorString.Append("infinite loop");
                    break;
                case INVALID_OP:
                    errorString.Append("invalid operation");
                    break;
                case DIR_EXISTS:
                    errorString.Append("directory already exists");
                    break;
                case DIR_NOT_FOUND:
                    errorString.Append("directory does not exist");
                    break;
                case FILE_EXISTS:
                    errorString.Append("file already exists");
                    break;
                case FILE_NOT_FOUND:
                    errorString.Append("file does not exist");
                    break;
                case OUT_OF_BOUNDS:
                    errorString.Append("index out of bounds");
                    break;
                case INVALID_RANGE_SEP:
                    errorString.Append("invalid range separator");
                    break;
                case INVALID_SEQ:
                    errorString.Append("invalid sequence");
                    break;
                case INVALID_SEQ_SEP:
                    errorString.Append("invalid sequence separator");
                    break;
                case INVALID_VAR_DECL:
                    errorString.Append("invalid variable declaration");
                    break;
                case LIST_UNDEFINED:
                    errorString.Append("list undefined");
                    break;
                case METHOD_DEFINED:
                    errorString.Append("method defined");
                    break;
                case METHOD_UNDEFINED:
                    errorString.Append("method undefined");
                    break;
                case NULL_NUMBER:
                    errorString.Append("null number");
                    break;
                case NULL_STRING:
                    errorString.Append("null string");
                    break;
                case OBJ_METHOD_UNDEFINED:
                    errorString.Append("object method undefined");
                    break;
                case OBJ_UNDEFINED:
                    errorString.Append("object undefined");
                    break;
                case OBJ_VAR_UNDEFINED:
                    errorString.Append("object variable undefined");
                    break;
                case VAR_DEFINED:
                    errorString.Append("variable defined");
                    break;
                case VAR_UNDEFINED:
                    errorString.Append("variable undefined");
                    break;
                case TARGET_UNDEFINED:
                    errorString.Append("target undefined");
                    break;
                case CONST_UNDEFINED:
                    errorString.Append("constant defined");
                    break;
                case INVALID_OPERATOR:
                    errorString.Append("invalid operator");
                    break;
                case IS_EMPTY:
                    errorString.Append("is empty");
                    break;
                case READ_FAIL:
                    errorString.Append("read failure");
                    break;
                case DIVIDED_BY_ZERO:
                    errorString.Append("cannot divide by zero");
                    break;
                case UNDEFINED:
                    errorString.Append("undefined");
                    break;
                case UNDEFINED_OS:
                    errorString.Append("undefined_os");
                    break;
            }

            return errorString.ToString();
        }

    }
}
