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


        bool isRandomProcedure = false;
        int rules;
        int types;
        int typesMark = 0;

        int MarksCount = 0;

        int mark = 0;

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

            if (types != 0 || types != null)
                typesMark = 1;
            else if (rules != 0 || rules != null)
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
        public void UpdateValues()
        {
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
        }

        public void GetMark(CommandData CD)
        {
            GetMarkPerCondition();
            float result = 0;
            result += GetMarkForTable(ref CD);
            result += GetMarkForindexes(ref CD);
            result += GetMarkForTypes(ref CD);
            result += GetMarkForBackups(ref CD);

            mainWindow.Mark.Text = "Оценка: " + (result * 10).ToString();
        }

        float GetMarkForTable(ref CommandData CD)
        {
            if (Tables.mark == 0)
                return 0;

            float mark = Tables.mark / 3f;
            float result = 0;


            if (CD.Tables.Keys.Count >= Tables.count)
                result += mark;
            else
                result += mark * ((float)CD.Tables.Keys.Count / (float)Tables.count);

            float tempMark = mark / (float)Tables.count;
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

            float mark = Procedures.mark / 2f;
            float result = 0;


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
        }

        public override string ToString()
        {
            return Tables.ToString();
        }
    }
}
