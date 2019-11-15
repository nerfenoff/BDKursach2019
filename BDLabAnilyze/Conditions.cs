using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDLabAnilyze
{
    [Serializable]
    public class Conditions
    {
        [NonSerialized]
        public MainWindow mainWindow;
        
        (int count, int collums, int constraints, int inserts, float mark) Tables = (0,0,0,0,0);
        (int clusteredIndexes, int NotClasteredIndexes, float mark) indexes;
        (bool database, bool file, bool filegroup, bool log, bool differential, float mark) backup;
        (int count, int paramsCount, int operations, bool isExecuted, float mark) Procedures;
        (int insert, int delete, int update, float mark) Triggers;
        (int views, bool isJoins, float mark) views;
        (bool tran, bool inner, bool implicit_transactions, float mark) transactions;
        (int count, float mark) Functions;
        (int count, int Joins, int Where, int Having, int GroupBy, float mark) selects;


        bool isRandomProcedure = false;
        int rules;
        int types;
        int typesMark = 0;

        int MarksCount = 0;

        public Conditions() { }
        public Conditions(MainWindow MW)
        {
            mainWindow = MW;
        }
        public void SaveTables()
        {
            int.TryParse(mainWindow.TablesCount.Text, out Tables.count);
            int.TryParse(mainWindow.TablesInserts.Text, out Tables.inserts);
            int.TryParse(mainWindow.TablesConstraints.Text, out Tables.constraints);

            if (Tables.count != 0)
                Tables.mark = 1;
            else if (Tables.inserts != 0)
                Tables.mark = 1;
            else if (Tables.constraints != 0)
                Tables.mark = 1;
            else if (Tables.collums != 0)
                Tables.mark = 1;
            else
                Tables.mark = 0;

            //Console.WriteLine(Tables);
        }

        public void SaveIndexes()
        {
            int.TryParse(mainWindow.TablesClastedIndexses.Text, out indexes.clusteredIndexes);
            int.TryParse(mainWindow.TablesNonclastedIndexes.Text, out indexes.NotClasteredIndexes);

            if (indexes.clusteredIndexes != 0)
                indexes.mark = 1;
            else if (indexes.NotClasteredIndexes != 0)
                indexes.mark = 1;
            else
                indexes.mark = 0;
        }

        public void SaveTypes()
        {
            int.TryParse(mainWindow.TablesTypes.Text, out types);
            int.TryParse(mainWindow.TablesRules.Text, out rules);

            if (types != 0 || rules != 0)
                typesMark = 1;
            else
                typesMark = 0;
        }
        
        public void SaveBackups()
        {
            backup.database = (bool)mainWindow.CheckBoxBase.IsChecked;
            backup.log = (bool)mainWindow.CheckBoxLog.IsChecked;
            backup.file = (bool)mainWindow.CheckBoxFile.IsChecked;
            backup.filegroup = (bool)mainWindow.CheckBoxFileGroup.IsChecked;
            backup.differential = (bool)mainWindow.CheckBoxDefferential.IsChecked;

            if (backup.database)
                backup.mark = 1;
            else if (backup.log)
                backup.mark = 1;
            else if (backup.file)
                backup.mark = 1;
            else if (backup.filegroup)
                backup.mark = 1;
            else if (backup.differential)
                backup.mark = 1;
            else
                backup.mark = 0;
        }
        public void SaveProcedures()
        {
            int.TryParse(mainWindow.ProcedureCount.Text, out Procedures.count);
            int.TryParse(mainWindow.ProcedureParams.Text, out Procedures.paramsCount);
            Procedures.isExecuted = (bool)mainWindow.CheckBoxProcedure.IsChecked;

            if (Procedures.count != 0)
                Procedures.mark = 1;
            else if (Procedures.paramsCount != 0)
                Procedures.mark = 1;
            else if (Procedures.isExecuted)
                Procedures.mark = 1;
            else
                Procedures.mark = 0;
        }

        public void SaveTriggers()
        {
            int.TryParse(mainWindow.TriggersInsert.Text, out Triggers.insert);
            int.TryParse(mainWindow.TriggersDelete.Text, out Triggers.delete);
            int.TryParse(mainWindow.TriggersUpdate.Text, out Triggers.update);

            if (Triggers.insert != 0)
                Triggers.mark = 1;
            else if (Triggers.delete != 0)
                Triggers.mark = 1;
            else if (Triggers.update != 0)
                Triggers.mark = 1;
            else
                Triggers.mark = 0;
        }

        public void SaveViews()
        {
            int.TryParse(mainWindow.ViewsCount.Text, out views.views);
            views.isJoins = (bool)mainWindow.ViewsIsJoins.IsChecked;

            if (views.views != 0)
                views.mark = 1;
            else if (views.isJoins)
                views.mark = 1;
            else
                views.mark = 0;
        }

        public void SaveTransactions()
        {
            transactions.tran = (bool)mainWindow.CheckBoxTran.IsChecked;
            transactions.inner = (bool)mainWindow.CheckBoxInnerTran.IsChecked;
            transactions.implicit_transactions = (bool)mainWindow.CheckBoxImplicit.IsChecked;

            if (transactions.tran || transactions.inner || transactions.implicit_transactions)
                transactions.mark = 1;
            else
                transactions.mark = 0;
        }

        public void SaveFunctions()
        {
            int.TryParse(mainWindow.FunctionsCount.Text, out Functions.count);

            if (Functions.count != 0)
                Functions.mark = 1;
            else
                Functions.mark = 0;
        }

        public void SaveSelects()
        {
            int.TryParse(mainWindow.SelectesCount.Text,out selects.count);
            int.TryParse(mainWindow.SelectesJoins.Text,out selects.Joins);
            int.TryParse(mainWindow.SelectesWhere.Text,out selects.Where);
            int.TryParse(mainWindow.SelectesGroupBy.Text,out selects.GroupBy);
            int.TryParse(mainWindow.SelectesHaving.Text,out selects.Having);


            if (selects.count != 0 || selects.Where != 0 || selects.Joins != 0 || selects.Having != 0 || selects.GroupBy != 0)
                selects.mark = 1;
            else
                selects.mark = 0;
        }
        public void UpdateValues()
        {
            mainWindow.SelectesCount.Text = selects.count.ToString();
            mainWindow.SelectesJoins.Text = selects.Joins.ToString();
            mainWindow.SelectesWhere.Text = selects.Where.ToString();
            mainWindow.SelectesGroupBy.Text = selects.GroupBy.ToString();
            mainWindow.SelectesHaving.Text = selects.Having.ToString();

            mainWindow.TablesCount.Text = Tables.count.ToString();
            mainWindow.TablesInserts.Text = Tables.inserts.ToString();
            mainWindow.TablesConstraints.Text = Tables.constraints.ToString();

            mainWindow.TablesClastedIndexses.Text = indexes.clusteredIndexes.ToString();
            mainWindow.TablesNonclastedIndexes.Text = indexes.NotClasteredIndexes.ToString();

            mainWindow.TablesTypes.Text = types.ToString();
            mainWindow.TablesRules.Text = rules.ToString();

            mainWindow.CheckBoxBase.IsChecked = backup.database;
            mainWindow.CheckBoxLog.IsChecked = backup.log;
            mainWindow.CheckBoxFile.IsChecked = backup.file;
            mainWindow.CheckBoxFileGroup.IsChecked = backup.filegroup;
            mainWindow.CheckBoxDefferential.IsChecked = backup.differential;

            mainWindow.ProcedureCount.Text = Procedures.count.ToString();
            mainWindow.ProcedureParams.Text = Procedures.paramsCount.ToString();
            mainWindow.CheckBoxProcedure.IsChecked = Procedures.isExecuted;

            mainWindow.TriggersInsert.Text = Triggers.insert.ToString();
            mainWindow.TriggersDelete.Text = Triggers.delete.ToString();
            mainWindow.TriggersUpdate.Text = Triggers.update.ToString();

            mainWindow.ViewsCount.Text = views.views.ToString();
            mainWindow.ViewsIsJoins.IsChecked = views.isJoins;

            mainWindow.CheckBoxTran.IsChecked = transactions.tran;
            mainWindow.CheckBoxInnerTran.IsChecked = transactions.inner;
            mainWindow.CheckBoxImplicit.IsChecked = transactions.implicit_transactions;

            mainWindow.FunctionsCount.Text = Functions.count.ToString();

        }

        public void GetMark(CommandData CD)
        {
            GetMarkPerCondition();
            float result = 0;
            result += GetMarkForTable(ref CD);
            result += GetMarkForindexes(ref CD);
            result += GetMarkForTypes(ref CD);
            result += GetMarkForBackups(ref CD);
            result += GetMarkForProcedures(ref CD);
            result += GetMarkForTriggers(ref CD);
            result += GetMarkForViews(ref CD);
            result += GetMarkForTransactions(ref CD);
            result += GetMarkforFunctions(ref CD);

            mainWindow.Mark.Text = "Оценка: " + (result * 10).ToString();
        }

        float getMarkForSelects(ref CommandData CD)
        {
            if (selects.mark == 0)
                return 0;

            float result = 0;


            return result;
        }
        float GetMarkForTable(ref CommandData CD)
        {
            if (Tables.mark == 0)
                return 0;
            if (Tables.count == 0)
                Tables.count = 1;

            float mark = Tables.mark / 3f;
            float result = 0;


            if (CD.Tables.Keys.Count >= Tables.count)
                result += mark;
            else
                result += mark * ((float)CD.Tables.Keys.Count / (float)Tables.count);

            float tempMark = mark;
            if (Tables.count == 0)
                tempMark /= (float)CD.Tables.Count;
            else
                tempMark /= (float)Tables.count;

            int constraints = 0;

            foreach (string key in CD.Tables.Keys)
            {
                if (CD.Tables[key].inserts >= Tables.inserts)
                    result += tempMark;
                else
                    result += tempMark * ((float)CD.Tables[key].inserts / (float)Tables.inserts);

                constraints += CD.Tables[key].constraints;
            }

            if (constraints >= Tables.constraints)
                result += mark;
            else
                result += mark * ((float)constraints / (float)Tables.constraints);

            return result;
        }
        float GetMarkForindexes(ref CommandData CD)
        {
            if (indexes.mark == 0)
                return 0;
            float mark = Tables.mark / 2f;
            float result = 0;

            int clusteredIndexes = 0;
            int nonclastered = 0;

            foreach(string key in CD.Tables.Keys)
            {
                clusteredIndexes += CD.Tables[key].clusteredIndexes;
                nonclastered += CD.Tables[key].NotClasteredIndexes;
            }

            if (clusteredIndexes >= indexes.clusteredIndexes)
                result += mark;
            else
                result += mark * ((float)clusteredIndexes / (float)indexes.clusteredIndexes);

            if (nonclastered >= indexes.NotClasteredIndexes)
                result += mark;
            else
                result += mark * ((float)nonclastered / (float)indexes.NotClasteredIndexes);

            return result;
        }
        float GetMarkForTypes(ref CommandData CD)
        {
            if (typesMark == 0)
                return 0;

            float mark = typesMark / 2f;
            float result = 0;

            result += GetMarkOperation(types, CD.Types.Count, mark);

            float RulesMark = mark / (float)types;
            
            foreach(string key in CD.Rules.Keys)
            {
                float temp = RulesMark;
                if (!CD.Rules[key])
                    temp /= 2f;
                result += temp;
            }

            return result;

        }
        float GetMarkForBackups(ref CommandData CD)
        {
            if (backup.mark == 0)
                return 0;

            float mark = backup.mark / 5f / 2f;
            float result = 0;


            if (CD.backup.database)
                result += mark;
            if (CD.restore.database)
                result += mark;

            if (CD.backup.log)
                result += mark;
            if (CD.restore.log)
                result += mark;

            if (CD.backup.file)
                result += mark;
            if (CD.restore.file)
                result += mark;

            if (CD.backup.filegroup)
                result += mark;
            if (CD.restore.filegroup)
                result += mark;

            if (CD.backup.differential)
                result += mark;
            if (CD.restore.differential)
                result += mark;

            return result;
        }
        float GetMarkForProcedures(ref CommandData CD)
        {
            if (Procedures.mark == 0)
                return 0;

            float mark = Procedures.mark / 3f;
            float result = 0;

            float tempMark = mark / (float)Procedures.count;
            float tempMark2 = mark / (float)Procedures.count;
            if (Procedures.isExecuted)
                tempMark /= 2f;

            float temp = 0;
            float temp2 = 0;

            foreach (string key in CD.Procedures.Keys)
            {
                temp += tempMark;
                if (Procedures.isExecuted && CD.Procedures[key].isExecuted)
                    temp += tempMark;

                temp2 += GetMarkOperation(Procedures.paramsCount, CD.Procedures[key].paramsCount, tempMark2);
            }

            if (temp > mark)
                result += mark;
            else
                result += temp;

            if (temp2 > mark)
                result += mark;
            else
                result += temp2;

            if (isRandomProcedure && CD.isRandomProcedure)
                    result += mark;            
            else
                result += mark;

            return result;
        }
        float GetMarkForTriggers(ref CommandData CD)
        {
            if (Triggers.mark == 0)
                return 0;

            float mark = Triggers.mark / 3f;
            float result = 0;

            int inserts = 0;
            int deletes = 0;
            int updates = 0;

            foreach(string key in CD.Triggers.Keys)
            {
                switch (CD.Triggers[key].type)
                {
                    case "INSERT":
                        inserts++;
                        break;
                    case "DELETE":
                        deletes++;
                        break;
                    case "UPDATE":
                        updates++;
                        break;
                }
            }

            result += GetMarkOperation(Triggers.insert, inserts, mark);
            result += GetMarkOperation(Triggers.delete, deletes, mark);
            result += GetMarkOperation(Triggers.update, updates, mark);

            return result;
        }
        float GetMarkForViews(ref CommandData CD)
        {
            if (views.mark == 0)
                return 0;

            float mark = views.mark;
            float result = 0;

            if (views.isJoins)
                mark /= 2f;

            mark /= views.views;

            foreach(string key in CD.Views.Keys)
            {
                result += mark;
                if (views.isJoins && CD.Views[key])
                    result += mark;
            }

            return result;
        }
        float GetMarkForTransactions(ref CommandData CD)
        {
            if (transactions.mark == 0)
                return 0;

            float mark = transactions.mark / 3f;
            float result = 0;
            if (transactions.tran)
            {
                if (CD.trans.tran)
                {
                    result += mark;
                }
            }
            else
                result += mark;

            if (transactions.inner)
            {
                if (CD.trans.innerTran)
                {
                    result += mark;
                }
            }
            else
                result += mark;

            if (transactions.implicit_transactions)
            {
                if (CD.trans.implicit_transactions)
                {
                    result += mark;
                }
            }
            else
                result += mark;

            return result;
        }
        float GetMarkforFunctions(ref CommandData CD)
        {
            if (Functions.mark == 0)
                return 0;

            float mark = Functions.mark;
            float result = GetMarkOperation(Functions.count,CD.Functions.Count,mark);

            return result;
        }

        float GetMarkOperation(float target, float current, float mark)
        {
            float result = 0;
            if (current >= target)
                result += mark;
            else
                result += mark * (current / target);

            return result;
        }
        void GetMarkPerCondition()
        {

            MarksCount = 0;
            if (Tables.mark != 0)
                ++MarksCount;
            if (Procedures.mark != 0)
                ++MarksCount;
            if (backup.mark != 0)
                ++MarksCount;
            if (indexes.mark != 0)
                ++MarksCount;
            if (Triggers.mark != 0)
                ++MarksCount;
            if (views.mark != 0)
                ++MarksCount;
            if (typesMark != 0)
                ++MarksCount;
            if (transactions.mark != 0)
                ++MarksCount;
            if (Functions.mark != 0)
                ++MarksCount;


            if (Tables.mark != 0)
                Tables.mark /= MarksCount;
            if (Procedures.mark != 0)
                Procedures.mark /= MarksCount;
            if (backup.mark != 0)
                backup.mark /= MarksCount;
            if (indexes.mark != 0)
                indexes.mark /= MarksCount;
            if (Triggers.mark != 0)
                Triggers.mark /= MarksCount;
            if (views.mark != 0)
                views.mark /= MarksCount;
            if (typesMark != 0)
                typesMark /= MarksCount;
            if (transactions.mark != 0)
                transactions.mark /= MarksCount;
            if (Functions.mark != 0)
                Functions.mark /= MarksCount;
        }

        public override string ToString()
        {
            return Tables.ToString();
        }
    }
}
