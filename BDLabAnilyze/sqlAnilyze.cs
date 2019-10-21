using System;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BDLabAnilyze
{
    class SQLAnilyze
    {

        public string[] Commands = {"CREATE", "INSERT", "DROP", "UPDATE", "DELETE", "EXECUTE", "EXEC", "GO", "ALTER", "USE", "BACKUP", "RESTORE" };
        public Conditions conditions = null;
        SqlConnection connection;
        CommandData CD;

        public SQLAnilyze(SqlConnection connection)
        {
            this.connection = connection;
            CD = new CommandData(Commands);
        }

        //Парсеры команд
        string GetCreateOperation(string text, ref int index)
        {
            string result = "";
            string word = GetFirsWord(text, ref index).ToUpper();
            bool isFind = false;
            int end = 0;
            for (int i = 0; i < 3 && !isFind; ++i)
            {
                switch (word)
                {

                    case "TABLE":
                        result = "CREATE " + word + GetEqualScobe(text, ref index);
                        CD.TableAnalyze(result);
                        isFind = true;
                        break;
                    case "PROCEDURE":
                        result = "CREATE " + word + " ";
                        end = GetEqualBeginEndIndex(text, index);
                        while (index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
                        CD.ProcedureAnalyze(result);
                        break;
                    case "TRIGGER":
                        result += "CREATE " + word + " ";
                        end = GetEqualBeginEndIndex(text, index);
                        while (index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
                        CD.TriggerAnalyze(result);
                        break;
                    case "VIEW":
                        result += "CREATE " + word + " ";
                        end = GetIndexOfCommand(text, index);
                        while (index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
                        CD.ViewAnalyze(result);
                        break;
                    case "TYPE":
                    case "RULE":
                        end = GetIndexOfCommand(text, index);
                        result = "CREATE " + word + " ";
                        while (index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
                        CD.TypesAnalyze(result);
                        break;
                    case "UNIQUE CLUSTERED INDEX":
                    case "CLUSTERED INDEX": 
                    case "NONCLUSTERED INDEX":
                        end = GetIndexOfCommand(text, index);
                        result = "CREATE " + word + " ";
                        while(index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
                        CD.IndexAnalyze(result);
                        break;
                    case "FUNCTION":
                        end = GetIndexOfGO(text, index);
                        result = "CREATE " + word + " ";
                        while (index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
                        CD.FunctionAnalyze(result);
                        break;
                }
                if(!isFind)
                    word += " " + GetFirsWord(text, ref index).ToUpper();
            }
            return result;

        }
        string GetAlterOperation(string text, ref int index)
        {
            string result = "ALTER ";
            string target = GetFirsWord(text, ref index);
            int end;
            switch (target.ToUpper())
            {
                case "TABLE":
                    result += "TABLE " + GetFirsWord(text, ref index) + " ";
                    target = GetFirsWord(text, ref index).ToUpper() + " " + GetFirsWord(text, ref index).ToUpper();
                    switch (target)
                    {
                        case "DROP CONSTRAINT":
                            result += target + GetFirsWord(text, ref index);
                            CD.ConstraintAnalyze(result);
                            break;
                        case "ADD CONSTRAINT":
                            result += target;
                            bool isEnd = false;
                            while (!isEnd)
                            {
                                end = GetIndexOfCommand(text, index);
                                while (index < end)
                                {
                                    result += text[index++];
                                }
                                isEnd = true;
                                string temp = result[result.Length - 2] + "" + result[result.Length - 1];
                                if (temp.ToUpper() == "ON")
                                {
                                    result += " ";
                                    result += GetFirsWord(text, ref index);
                                    isEnd = false;
                                }
                                else
                                    isEnd = true;
                            }
                            CD.ConstraintAnalyze(result);
                            break;
                    }
                    break;
                case "PROCEDURE":
                    result += target + " ";
                    end = GetEqualBeginEndIndex(text, index);
                    while (index < end)
                    {
                        result += text[index++];
                    }
                    CD.ProcedureAnalyze(result);
                    break;
                case "TRIGGER":
                    result += target + " ";
                    end = GetEqualBeginEndIndex(text, index);
                    while (index < end)
                    {
                        result += text[index++];
                    }
                    CD.TriggerAnalyze(result);
                    break;
                case "VIEW":
                    result += target + " ";
                    end = GetIndexOfCommand(text, index);
                    while (index < end)
                    {
                        result += text[index++];
                    }
                    CD.ViewAnalyze(result);
                    break;
                case "FUNCTION":
                    end = GetIndexOfGO(text, index);
                    result = target + " ";
                    while (index < end)
                    {
                        result += text[index++];
                    }
                    CD.FunctionAnalyze(result);
                    break;
            }
            return result;
        }
        string GetInsertOperation(string text, ref int index)
        {
            return "INSERT " + GetEqualScobeWithText(text, "VALUES", ref index);
        }
        string GetUpdateOperation(string text, ref int index)
        {
            string result = "UPDATE ";
            int end = GetIndexOfCommand(text, index);
            while (index < end)
            {
                result += text[index++];
            }

            return result;
        }
        string GetDeleteOperation(string text, ref int index)
        {
            string result = "DELETE ";
            int end = GetIndexOfCommand(text, index);
            while (index < end)
            {
                result += text[index++];
            }

            return result;
        }
        string GetExecuteOperation(string text, ref int index)
        {
            string result = "";
            int end = GetIndexOfCommand(text, index);
            while (index < end)
            {
                result += text[index++];
            }
            return result;
        }
        string GetBackupOperation(string text, ref int index)
        {
            string result = "BACKUP ";
            int end = GetIndexOfCommand(text, index);
            while (index < end)
            {
                result += text[index++];
            }
            CD.BackupAnalyze(result);
            return result;
        }
        string GetRestoreOperation(string text, ref int index)
        {
            string result = "RESTORE ";
            int end = GetIndexOfCommand(text, index);
            while (index < end)
            {
                result += text[index++];
            }
            CD.RestoreAnalyze(result);
            return result;
        }
        //***************

        //Вспомогательные
        string GetEqualScobeWithText(string text, string word, ref int index)
        {
            string result = "";
            int left = 0;
            int right = 0;
            bool isFind = false;
            string words = "";
            while (words.ToUpper() != word)
            {
                words = GetFirsWord(text, ref index);
                result += words + " ";
            }
            while (!isFind || left != right)
            {
                result += text[index];
                if (text[index] == '(')
                {
                    ++left;
                    if (!isFind)
                        isFind = true;
                }
                if (text[index] == ')')
                    ++right;
                index++;
            }
            return result;
        }
        public static string GetFirsWord(string text, ref int index)
        {
            string result = "";
            while (text.Length > index)
            {
                if (result == "\n" || result == "\r" || result == " " || result == "\t")
                    result = "";
                if (result == ",")
                    break;
                if (result != "" && (text[index] == ' ' || text[index] == '(' || text[index] == '\r' || text[index] == '\n' || text[index] == '\t' || text[index] == ','))
                {
                    break;
                }
                else
                    result += text[index];
                index++;
            }
            return result;
        }
        int GetIndexOfCommand(string text, int index)
        {
            string target = "";
            while (text.Length < index || !Array.Exists(Commands, (x) => x == target.ToUpper()))
            {
                target = GetFirsWord(text, ref index);
                if (text.Length == index || target[0] == '-' && target[1] == '-')
                    break;
            }
            index -= (target.Length + 1);
            while (true)
            {
                if (text[index] == '\n' || text[index] == '\t' || text[index] == '\r' || text[index] == ' ')
                    --index;
                else
                    break;
            }
            if (text[index] != ';')
                ++index;
            return index;
        }
        int GetIndexOfGO(string text, int index)
        {
            string target = "";
            while (text.Length < index || "GO" != target.ToUpper())
            {
                target = GetFirsWord(text, ref index);
                if (target[0] == '-' && target[1] == '-')
                    break;
            }
            index -= (target.Length + 1);
            while (true)
            {
                if (text[index] == '\n' || text[index] == '\t' || text[index] == '\r' || text[index] == ' ')
                    --index;
                else
                    break;
            }
            if (text[index] != ';')
                ++index;
            return index;
        }
        string GetEqualScobe(string text, ref int index)
        {
            string result = "";
            int left = 0;
            int right = 0;
            bool isFind = false;
            while (!isFind || left != right)
            {
                result += text[index];
                if (text[index] == '(')
                {
                    ++left;
                    if (!isFind)
                        isFind = true;
                }
                if (text[index] == ')')
                    ++right;
                index++;
            }
            return result;
        }
        int GetEqualBeginEndIndex(string text, int index)
        {
            string word = "";
            int left = 0;
            int right = 0;
            bool isFind = false;
            bool isTran = false;
            while (!isFind || left != right)
            {
                word = GetFirsWord(text, ref index).ToUpper();
                if (word == "BEGIN")
                {
                    ++left;
                    if (!isFind)
                        isFind = true;
                }
                if (word == "END" || word == "COMMIT")
                    ++right;
            }
            return index;
        }

        //***************
        public string AnalyzeCode(string text)
        {
            string word;
            for(int i = 0; i < text.Length; ++i)
            {
                word = GetFirsWord(text, ref i);
                if (!Array.Exists(Commands, (x) => x == word.ToUpper()))
                    continue;

                string command = "";
                switch (word.ToUpper())
                {
                    case "CREATE":
                        command = GetCreateOperation(text, ref i);
                        break;
                    case "INSERT":
                        command = GetInsertOperation(text, ref i);
                        CD.IsertAnalyze(command);
                        break;
                    case "UPDATE":
                        command = GetUpdateOperation(text, ref i);
                        break;
                    case "DELETE":
                        command = GetDeleteOperation(text, ref i);
                        break;
                    case "ALTER":
                        command = GetAlterOperation(text, ref i);
                        break;
                    case "EXEC": command = "EXEC " + GetExecuteOperation(text, ref i);
                        CD.ExecuteAnalyze(command);
                        break;
                    case "EXECUTE": command = "EXECUTE " + GetExecuteOperation(text, ref i);
                        CD.ExecuteAnalyze(command);
                        break;
                    case "BACKUP": command = GetBackupOperation(text, ref i);
                        break;
                    case "RESTORE": command = GetRestoreOperation(text, ref i);
                        break;
                        
                }
                if(command != "")
                    Console.WriteLine(command + "\n");

            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(CD);
            return CD.ToString();
        }
    }
}
