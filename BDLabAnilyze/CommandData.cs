using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BDLabAnilyze
{
    public class CommandData
    {
        string[] commands;
        public Dictionary<string, (int collums, int constraints, int inserts, int clusteredIndexes, int NotClasteredIndexes)> Tables = new Dictionary<string, (int, int, int, int, int)>();
        public Dictionary<string, bool> Rules = new Dictionary<string, bool>();
        public Dictionary<string, (int paramsCount, int operations, bool isExecuted)> Procedures = new Dictionary<string, (int paramsCount, int operations, bool isExecuted)>();
        public Dictionary<string, (int operations, string type)> Triggers = new Dictionary<string, (int operations, string type)>();
        public Dictionary<string, bool> Views = new Dictionary<string, bool>();

        public List<string> Types = new List<string>();
        public List<string> Functions = new List<string>();

        public (bool database, bool file, bool filegroup, bool log, bool differential) backup;
        public (bool database, bool file, bool filegroup, bool log, bool differential) restore;
        public (bool tran, bool innerTran, bool implicit_transactions) trans;


        public bool isRandomProcedure = false;
        public CommandData() { }
        public CommandData(string[] commands)
        {
            this.commands = commands;

        }

        
        public void GetCommand(string commandText)
        {
            int i = 0;
            string word = SQLAnilyze.GetFirsWord(commandText, ref i);

            switch (word.ToUpper())
            {
                case "CREATE":
                    Creates(commandText, ref i);
                    break;
                case "INSERT":
                    IsertAnalyze(commandText);
                    break;
                case "UPDATE":

                    break;
                case "DELETE":

                    break;
                case "ALTER":
                    Alters(commandText,ref i);
                    break;
                case "EXEC":
                case "EXECUTE":
                    ExecuteAnalyze(commandText);
                    break;
                case "BACKUP":
                    BackupAnalyze(commandText);
                    break;
                case "RESTORE":
                    RestoreAnalyze(commandText);
                    break;
            }
        }

        void Creates(string text, ref int index)
        {
            string word = SQLAnilyze.GetFirsWord(text, ref index).ToUpper();
            for (int i = 0; i < 3; ++i)
            {
                switch (word)
                {

                    case "TABLE":
                        TableAnalyze(text);
                        return;
                    case "PROC":
                    case "PROCEDURE":
                        ProcedureAnalyze(text);
                        return;
                    case "TRIGGER":
                        TriggerAnalyze(text);
                        return;
                    case "VIEW":
                        ViewAnalyze(text);
                        return;
                    case "TYPE":
                    case "RULE":
                        TypesAnalyze(text);
                        return;
                    case "UNIQUE CLUSTERED INDEX":
                    case "CLUSTERED INDEX":
                    case "UNIQUE NONCLUSTERED INDEX":
                    case "NONCLUSTERED INDEX":
                        IndexAnalyze(text);
                        return;
                    case "FUNCTION":
                        FunctionAnalyze(text);
                        return;
                }
                word += " " + SQLAnilyze.GetFirsWord(text, ref index).ToUpper();
            }
        }
        void Alters(string text, ref int index)
        {
            string target = SQLAnilyze.GetFirsWord(text, ref index);
            switch (target.ToUpper())
            {
                case "TABLE":
                    SQLAnilyze.GetFirsWord(text, ref index);
                    target = SQLAnilyze.GetFirsWord(text, ref index).ToUpper() + " " + SQLAnilyze.GetFirsWord(text, ref index).ToUpper();
                    switch (target)
                    {
                        case "DROP CONSTRAINT":
                        case "ADD CONSTRAINT":                          
                            ConstraintAnalyze(text);
                            return;
                    }
                    return;
                case "PROCEDURE":                   
                    ProcedureAnalyze(text);
                    return;
                case "TRIGGER":                    
                    TriggerAnalyze(text);
                    return;
                case "VIEW":                   
                    ViewAnalyze(text);
                    return;
                case "FUNCTION":                 
                    FunctionAnalyze(text);
                    return;
            }
        }

        //Разбор команд
        public void ConstraintAnalyze(string CommandText)
        {
            int i = 0;
            (int, int, int, int, int) temp = (0, 0, 0, 0, 0);
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            SQLAnilyze.GetFirsWord(CommandText, ref i);

            string name = SQLAnilyze.GetFirsWord(CommandText, ref i);
            if (!Tables.ContainsKey(name))
            {
                return;
            }
            temp = Tables[name];
            string command = SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper() + " " + SQLAnilyze.GetFirsWord(CommandText, ref i);
            switch (command)
            {
                case "DROP CONSTRAINT": --temp.Item2;
                    break;
                case "ADD CONSTRAINT": ++temp.Item2;
                    break;
            }
            Tables[name] = temp;
        }
        public void TableAnalyze(string CommandText)
        {
            int i = 0;
            (int, int, int, int, int) temp = (0, 0, 0, 0, 0);
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            SQLAnilyze.GetFirsWord(CommandText, ref i);

            string name = SQLAnilyze.GetFirsWord(CommandText, ref i);
           
            if (!Tables.ContainsKey(name))
            {
                Tables.Add(name, temp);
            }

            Regex constraint = new Regex("CONSTRAINT", RegexOptions.IgnoreCase);
            MatchCollection matchCollection = constraint.Matches(CommandText);
            int end = CommandText.Length;
            if (matchCollection.Count != 0)
                end = matchCollection[0].Index;
            temp.Item1 = GetParamsCount(CommandText,end); // считать до первого констраинта          
            temp.Item2 = matchCollection.Count;

            Tables[name] = temp;
        }
        public void IsertAnalyze(string CommandText)
        {
            int i = 0;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            string name = SQLAnilyze.GetFirsWord(CommandText, ref i);
            if (!Tables.ContainsKey(name))
            {
                Tables.Add(name, (0,0,0,0,0));
            }
                
            (int, int, int, int, int) temp = Tables[name];
            ++temp.Item3;
            Tables[name] = temp;
        }
        public void IndexAnalyze(string CommandText)
        {
            int i = 0;
            
            Regex reg = new Regex(@"ON\s+\w+", RegexOptions.IgnoreCase);
            string name = reg.Match(CommandText).Value;
            if (name == null)
                return;
            name = name.Remove(0, 2);
            name = name.Trim();

            if (!Tables.ContainsKey(name))
                return;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            string index = SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper();
            if(index == "UNIQUE")
                index = SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper();
            (int, int, int, int, int) temp = Tables[name];
            switch (index)
            {
                case "CLUSTERED": temp.Item4++;
                    break;
                case "NONCLUSTERED": temp.Item5++;
                    break;
            }
            Tables[name] = temp;

        }
        public void TypesAnalyze(string CommandText)
        {
            int i = 0;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            string operation = SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper();
            string name = SQLAnilyze.GetFirsWord(CommandText, ref i);
            if (operation == "TYPE")
            {
               
                if (!Types.Contains(name))
                {
                    Types.Add(name);
                }
            }
            else
            {
                if (!Rules.ContainsKey(name))
                {
                    Rules.Add(name, false);
                }
            }
        }
        public void ExecuteAnalyze(string CommandText)
        {
            int i = 0;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            string command = SQLAnilyze.GetFirsWord(CommandText, ref i);
            if (command.ToLower() == "sp_bindrule")
            {
                string temp = SQLAnilyze.GetFirsWord(CommandText, ref i);
                temp = temp.Remove(0, 1).Remove(temp.Length - 2, 1);
                if (!Rules.ContainsKey(temp))
                    return;

                Rules[temp] = true;
            }
            else
            if (Procedures.ContainsKey(command))
            {
                (int, int, bool) temp = Procedures[command];
                temp.Item3 = true;
                Procedures[command] = temp;
            }
            else isRandomProcedure = true;
            
        }
        public void ProcedureAnalyze(string CommandText)
        {
            FindTransactions(CommandText);
            int i = 0;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            (int paramsCount, int operations, bool isExecuted) temp = (0, 0, false);
            string name = SQLAnilyze.GetFirsWord(CommandText, ref i);
            if (!Procedures.ContainsKey(name))
            {
                Procedures.Add(name, temp);
            }
            else
                temp.isExecuted = Procedures[name].isExecuted;

            bool isStart = true;
            while (i < CommandText.Length)
            {
                string word = SQLAnilyze.GetFirsWord(CommandText, ref i);
                if (isStart && word[0] == '@')
                    temp.paramsCount++;
                if (commands.Contains(word.ToUpper()))
                    temp.operations++;
                if (word.ToUpper() == "AS")
                    isStart = false;
            }

            Procedures[name] = temp;
        }
        public void TriggerAnalyze(string CommandText)
        {
            FindTransactions(CommandText);
            int i = 0;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            SQLAnilyze.GetFirsWord(CommandText, ref i);

            string name = SQLAnilyze.GetFirsWord(CommandText, ref i);


            SQLAnilyze.GetFirsWord(CommandText, ref i);
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            string type = SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper();
            (int operations, string type) temp = (0, type);

            while (i < CommandText.Length)
            {
                string word = SQLAnilyze.GetFirsWord(CommandText, ref i);

                if (commands.Contains(word.ToUpper()))
                    temp.operations++;
            }

            if (!Triggers.ContainsKey(name))
                Triggers.Add(name, temp);
            else
                Triggers[name] = temp;
        }
        public void ViewAnalyze(string CommandText)
        {
            int i = 0;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            SQLAnilyze.GetFirsWord(CommandText, ref i);

            string name = SQLAnilyze.GetFirsWord(CommandText, ref i);

            if(!Views.ContainsKey(name))
            {
                Views.Add(name, false);
            }

            if(!Views[name])
            {
                Regex regex = new Regex(@"\sJOIN\s");
                MatchCollection matches = regex.Matches(CommandText);
                if (matches.Count > 1)
                {
                    Views[name] = true;
                }
            }
        }
        public void BackupAnalyze(string CommandText)
        {
            int i = 0;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            if(SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper() == "LOG")
            {
                backup.log = true;
                return;
            }
            SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper();
            string temp = SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper();
            if (temp == "FILE")
            {
                backup.file = true;
                return;
            }
            if (temp == "FILEGROUP")
            {
                backup.filegroup = true;
                return;
            }
            temp = SQLAnilyze.GetFirsWord(CommandText, ref i);
            while (temp[temp.Length-1] != '\'')
            {
                temp = SQLAnilyze.GetFirsWord(CommandText, ref i);
            }
            if (i >= CommandText.Length)
            {
                backup.database = true;
            }
            else
                backup.differential = true;
        }
        public void RestoreAnalyze(string CommandText)
        {
            int i = 0;
            restore.database = true;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            if (SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper() == "LOG")
            {
                restore.log = true;
            }
            SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper();
            string temp = SQLAnilyze.GetFirsWord(CommandText, ref i).ToUpper();
            if (temp == "FILE")
            {
                restore.file = true;
                return;
            }
            if (temp == "FILEGROUP")
            {
                restore.filegroup = true;
                return;
            }
            temp = SQLAnilyze.GetFirsWord(CommandText, ref i);
            while (temp[temp.Length - 1] != '\'')
            {
                temp = SQLAnilyze.GetFirsWord(CommandText, ref i);
            }
            if (i < CommandText.Length)
            {
                restore.differential = true;
            }
                
        }
        public void FunctionAnalyze(string CommandText)
        {
            FindTransactions(CommandText);
            int i = 0;
            SQLAnilyze.GetFirsWord(CommandText, ref i);
            SQLAnilyze.GetFirsWord(CommandText, ref i);

            string name = SQLAnilyze.GetFirsWord(CommandText, ref i);

            if (!Functions.Contains(name))
            {
                Functions.Add(name);
            }
        }
        //вспомогательные
        int GetParamsCount(string text, int end)
        {
            int index = 0;
            int left = 0;
            int right = 0;
            int count = 0;
            bool isFind = false;

            while ((!isFind || left != right) && index < end)
            {
                if(text[index] == ',' && left == right+1)
                {
                    ++count;
                }
                else if (text[index] == '(')
                {
                    ++left;
                    if (!isFind)
                        isFind = true;
                }
                else if (text[index] == ')')
                    ++right;

                index++;
            }
            if (text.Length != end)
            {
                int i = 1;
                for (; text[end - i] == '\n' || text[end - i] == '\t' || text[end - i] == '\r' || text[end - i] == ' '; ++i) ;
                if (text[end - i] != ',')
                    count++;
            }
            else
                count++;
            return count;
        }
        void FindTransactions(string text)
        {
            Regex regex = new Regex(@"BEGIN (TRAN|TRANSACTION)\s+", RegexOptions.IgnoreCase);
            int count = regex.Matches(text).Count;
            if(count > 0)
            {
                trans.tran = true;
                if (count > 1)
                    trans.innerTran = true;
            }
            regex = new Regex(@"SET\s+IMPLICIT_TRANSACTIONS\s+ON\s+", RegexOptions.IgnoreCase);
            if(regex.Match(text).Success)
            {
                trans.implicit_transactions = true;
            }
        }

        //вывод
        public override string ToString()
        {
            string result = string.Empty;
            int i = 0;
            result += $"Tables - {Tables.Keys.Count}\n";
            foreach(string key in Tables.Keys)
            {
                result += key + ": Params - ";
                result += Tables[key].collums + ", CONSTRAINTS - " + Tables[key].constraints;
                result += ", Inserts - " + Tables[key].inserts;
                result += ", Clustered Indexes - " + Tables[key].clusteredIndexes;
                result += ", NONCLUSTERED Indexes - " + Tables[key].NotClasteredIndexes;
                result += "\n";
                ++i;
            }
            result += "\n";
            result += "Types - " + Types.Count;
            result += "\n\n";
            result += $"Rules: \nCount - {Rules.Keys.Count}\n"; 
            foreach (string key in Rules.Keys)
            {
                result += key + ", is Binded - " + Rules[key] + "\n";
            }
            result += "\n";

            result += $"Procedures - {Procedures.Keys.Count}\n";
            foreach(string key in Procedures.Keys)
            {
                result += $"{key}: params - {Procedures[key].paramsCount}, operations - {Procedures[key].operations}, isExecuted - {Procedures[key].isExecuted}\n";
            }
            result += $"\nUse Standart Procedure: {isRandomProcedure} \n\n";

            result += $"Triggers - {Triggers.Keys.Count}\n";
            foreach(string key in Triggers.Keys)
            {
                result += $"{key}, type - {Triggers[key].type}, operations - {Triggers[key].operations}\n";
            }

            result += $"\nViews: {Views.Count}\n";
            foreach(string key in Views.Keys)
            {
                result += $"{key}, is multi table - {Views[key]}\n";
            }

            result += "\nBackup: \n";
            result += $"database - {backup.database}, differential - {backup.differential}, log - {backup.log}, file - {backup.file}, filegroup - {backup.filegroup}\n\n";
            result += "Restore: \n";
            result += $"database - {restore.database}, differential - {restore.differential}, log - {restore.log}, file - {restore.file}, filegroup - {restore.filegroup}\n\n";

            result += $"Transactions: defult - {trans.tran}, inner transaction - {trans.innerTran}, implicit_transactions - {trans.implicit_transactions}\n\n";
            
            result += $"Functions - {Functions.Count}";
            return result;
        }
    }
}
