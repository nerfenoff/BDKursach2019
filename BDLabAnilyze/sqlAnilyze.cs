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

        public string[] Commands = {"CREATE", "INSERT", "DROP", "UPDATE", "DELETE", "EXECUTE", "EXEC", "GO", "ALTER", "USE", "BACKUP", "RESTORE", "BEGIN", "COMMIT", "END", "SET", "SELECT" };
        public Conditions conditions;
        SqlConnection connection;
        CommandData CD;
        public SQLAnilyze(SqlConnection connection, ref Conditions conditions)
        {
            this.connection = connection;
            connection.Close();
            CD = new CommandData(Commands);
            this.conditions = conditions;
        }

        //Парсеры команд
        string GetSelectOperation(string text, ref int index)
        {
            string result = "SELECT ";
            bool isEnd = true;
            int end = index;
            while (end < text.Length)
            {
                end = GetIndexOfCommand(text, end);
                isEnd = true;
                for (int i = 0; end - i > 0; ++i)
                {
                    char temp = text[end - i];
                    if (!char.IsWhiteSpace(temp))
                    {
                        if (temp == '(')
                        {
                            end += 2;
                            Console.WriteLine(text[end]);
                            isEnd = false;
                            
                        }
                        break;
                    }
                }
                if (isEnd)
                    break;

            }

            while(index < end)
            {
                result += text[index++];
            }

            return result;
        }
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
                        isFind = true;
                        break;
                    case "PROC":
                    case "PROCEDURE":
                        result = "CREATE " + word + " ";
                        end = GetEqualBeginEndIndex(text, index);
                        while (index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
                        break;
                    case "TRIGGER":
                        result += "CREATE " + word + " ";
                        end = GetEqualBeginEndIndex(text, index);
                        while (index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
                        break;
                    case "VIEW":
                        result += "CREATE " + word + " ";
                        end = GetIndexOfGO(text, index);
                        while (index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
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
                        break;
                    case "UNIQUE CLUSTERED INDEX":
                    case "CLUSTERED INDEX":
                    case "UNIQUE NONCLUSTERED INDEX":
                    case "NONCLUSTERED INDEX":
                        end = GetIndexOfCommand(text, index);
                        result = "CREATE " + word + " ";
                        while(index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
                        break;
                    case "FUNCTION":
                        end = GetIndexOfGO(text, index);
                        result = "CREATE " + word + " ";
                        while (index < end)
                        {
                            result += text[index++];
                        }
                        isFind = true;
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
                            result += target + " " + GetFirsWord(text, ref index);
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
                    break;
                case "TRIGGER":
                    result += target + " ";
                    end = GetEqualBeginEndIndex(text, index);
                    while (index < end)
                    {
                        result += text[index++];
                    }
                    break;
                case "VIEW":
                    result += target + " ";
                    end = GetIndexOfGO(text, index);
                    while (index < end)
                    {
                        result += text[index++];
                    }
                    break;
                case "FUNCTION":
                    end = GetIndexOfGO(text, index);
                    result = target + " ";
                    while (index < end)
                    {
                        result += text[index++];
                    }
                    break;
                case "DATABASE":
                    result = "Use master;\nALTER ";
                    end = GetIndexOfCommand(text, index);
                    result += target + " ";
                    while (index < end)
                    {
                        result += text[index++];
                    }
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
                if (result == "/*")
                {
                    Regex regex = new Regex(@"\*\/");
                    int i = regex.Match(text, index).Index+1;
                    if (i != 0)
                    {
                        index = i;
                        result = "";
                        continue;
                    }
                    else
                    {
                        index = text.Length;
                        return "";
                    }
                }
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
            string command = "";

            int beginTrans = 0;
            int CommitTrans = 0;

            int beginTry = 0;
            bool isImplict = false;

            (string, string, int) tryCath = ("", "", 0);

            List<string> commands = new List<string>();
            string commandToExecute = "";
            
            for (int i = 0; i < text.Length; ++i)
            {
                word = GetFirsWord(text, ref i);
                if (!Array.Exists(Commands, (x) => x == word.ToUpper()))
                    continue;

                switch (word.ToUpper())
                {
                    case "SELECT":
                        command = GetSelectOperation(text, ref i);
                        break;
                    case "CREATE":
                        command = GetCreateOperation(text, ref i);
                        break;
                    case "INSERT":
                        command = GetInsertOperation(text, ref i);
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
                        break;
                    case "EXECUTE": command = "EXECUTE " + GetExecuteOperation(text, ref i);
                        break;
                    case "BACKUP": command = GetBackupOperation(text, ref i);
                        break;
                    case "RESTORE": command = GetRestoreOperation(text, ref i);
                        break;
                    case "END":
                        word = GetFirsWord(text, ref i).ToUpper();
                        if (word == "TRY")
                        {
                            i = tryCath.Item3;
                            tryCath.Item1 = "";
                            tryCath.Item2 = "";

                        }
                        break;
                    case "COMMIT":
                        int length = word.Length + 1;
                        word = GetFirsWord(text, ref i).ToUpper();
                        if (word == "TRAN" || word == "TRANSACTION")
                        {
                            ++CommitTrans;
                            
                        }
                        break;
                    case "SET":
                        word = GetFirsWord(text, ref i).ToUpper();
                        if (word != "IMPLICIT_TRANSACTIONS")
                            continue;
                        string status = GetFirsWord(text, ref i).ToUpper();
                        if (status == "ON")
                        {
                            beginTrans++;
                            isImplict = true;
                        }
                        else
                        {
                            isImplict = false;
                            if (beginTrans > CommitTrans)
                                ++CommitTrans;
                        }
                        command = "SET " + word + " " + status;
                        CD.trans.implicit_transactions = true;
                        continue;
                    case "BEGIN":
                        word = GetFirsWord(text, ref i).ToUpper();

                        if (word == "TRAN" || word == "TRANSACTION")
                        {
                            ++beginTrans;
                            CD.trans.tran = true;
                            if (beginTrans > 1)
                                CD.trans.innerTran = true;
                        }
                            
                        else if (word == "TRY")
                        {
                            tryCath.Item1 = "BEGIN TRY\n";
                            ++beginTry;
                            int start = i;
                            int end = i;
                            while (i < text.Length - 1)
                            {
                                word = GetFirsWord(text, ref i).ToUpper();
                                if (word == "END")
                                {
                                    word = GetFirsWord(text, ref i);
                                    if (word == "TRY")
                                    {
                                        break;
                                    }
                                }
                            }

                            tryCath.Item2 = "END TRY\n";
                            int beginRead = i;
                            while (i < text.Length - 1)
                            {
                                word = GetFirsWord(text, ref i).ToUpper();
                                if (word == "END")
                                {
                                    word = GetFirsWord(text, ref i);
                                    if (word == "CATCH")
                                    {
                                        end = i-1;
                                        break;
                                    }
                                }
                            }

                            while (beginRead < end+1)
                            {
                                tryCath.Item2 += text[beginRead++];
                                tryCath.Item3 = beginRead;
                            }
                            i = start;

                        }
                        else if (word == "CATCH")
                        {
                            i = tryCath.Item3;
                            continue;
                        }

                        continue;
                    case "USE":
                        Regex regex = new Regex(@"Catalog=(\w+)?");
                        Match match = regex.Match(connection.ConnectionString);
                        word = GetFirsWord(text, ref i);
                        connection.ConnectionString = regex.Replace(connection.ConnectionString, $"Catalog={word}");
                        continue;
                        
                }

                if(beginTrans!= 0)
                {
                    if (beginTrans == CommitTrans)
                    {
                        beginTrans = 0;
                        CommitTrans = 0;
                        if(!isImplict)
                            commandToExecute = $"BEGIN TRAN\n{commandToExecute}\nCOMMIT TRAN\n";
                        else
                            commandToExecute = $"\n{commandToExecute}\nCOMMIT TRAN\n";
                        command = commandToExecute;
                        commandToExecute = "";
                        if (isImplict)
                            beginTrans++;
                    }
                    else
                    {
                        commands.Add(command);
                        commandToExecute += command + "\n";
                    }
                }

                if (command != "" && commandToExecute == "" )
                {
                    if(tryCath.Item2 != "")
                    {
                        command = tryCath.Item1 + command + tryCath.Item2;
                        if (isImplict)
                            command = "SET IMPLICIT_TRANSACTIONS ON\n" + command;
                    }
                    bool isFail = false;
                    try
                    {
                        connection.Open();

                        Console.WriteLine(command + "\n");
                        SqlCommand sqlcommand = new SqlCommand(command, connection);                  
                        sqlcommand.ExecuteNonQuery();
                        connection.Close();

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message + "\n\n");
                        isFail = true;
                        if (tryCath.Item1 != "")
                        {
                            tryCath.Item1 = "";
                            tryCath.Item2 = "";
                            i = tryCath.Item3;
                        }
                        commands.Clear();
                    }
                    finally
                    {
                        Console.WriteLine("\n\n\n\n");
                        connection.Close();                        
                    }

                    if (!isFail)
                    {
                        if(commands.Count == 0)
                        {
                            CD.GetCommand(command);
                            command = "";
                            continue;
                        }
                        for(int j = 0; j < commands.Count; ++j)
                        {
                            CD.GetCommand(commands[j]);
                        }
                        command = "";
                        commands.Clear();
                    }
                        
                   
                }

            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            //Console.WriteLine(CD);
            conditions.GetMark(CD);
            return CD.ToString();
        }
    }
}
