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

        (int count, int collums, int constraints, int inserts, float mark) Tables;
        (int clusteredIndexes, int NotClasteredIndexes, float mark) indexes;
        (bool database, bool file, bool filegroup, bool log, bool differential, float mark) backup;
        (int count, int paramsCount, int operations, bool isExecuted, float mark) Procedures;
        (int insert, int delete, int update, float mark) Triggers;


        bool isRandomProcedure = false;
        int rules;
        int types;
        int views;

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
            int.TryParse(mainWindow.TableCollums.Text, out Tables.collums);
        }

        public void SaveIndexes()
        {
            int.TryParse(mainWindow.TablesClastedIndexses.Text, out indexes.clusteredIndexes);
            int.TryParse(mainWindow.TablesNonclastedIndexes.Text, out indexes.NotClasteredIndexes);
        }

        public void SaveTypes()
        {
            int.TryParse(mainWindow.TablesTypes.Text, out types);
            int.TryParse(mainWindow.TablesRules.Text, out rules);
        }
        
        public void SaveBackups()
        {
            backup.database = (bool)mainWindow.CheckBoxBase.IsChecked;
            backup.log = (bool)mainWindow.CheckBoxLog.IsChecked;
            backup.file = (bool)mainWindow.CheckBoxFile.IsChecked;
            backup.filegroup = (bool)mainWindow.CheckBoxFileGroup.IsChecked;
            backup.differential = (bool)mainWindow.CheckBoxDefferential.IsChecked;
        }
        public void SaveProcedures()
        {
            int.TryParse(mainWindow.ProcedureCount.Text, out Procedures.count);
            int.TryParse(mainWindow.ProcedureParams.Text, out Procedures.paramsCount);
            Procedures.isExecuted = (bool)mainWindow.CheckBoxProcedure.IsChecked;
        }

        public void SaveTriggers()
        {
            int.TryParse(mainWindow.TriggersInsert.Text, out Triggers.insert);
            int.TryParse(mainWindow.TriggersDelete.Text, out Triggers.delete);
            int.TryParse(mainWindow.TriggersUpdate.Text, out Triggers.update);
        }

        public void SaveViews()
        {
            int.TryParse(mainWindow.ViewsCount.Text, out views);
        }
        public void UpdateValues()
        {
            mainWindow.TablesCount.Text = Tables.count.ToString();
            mainWindow.TablesInserts.Text = Tables.inserts.ToString();
            mainWindow.TablesConstraints.Text = Tables.constraints.ToString();
            mainWindow.TableCollums.Text = Tables.collums.ToString();

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

            mainWindow.ViewsCount.Text = views.ToString();         
        }

        public override string ToString()
        {
            return Tables.ToString();
        }
    }
}
