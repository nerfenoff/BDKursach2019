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
        (int count, int paramsCount, int operations, bool isExecuted) Procedures;
        (int operations, string type) Triggers;

        List<string> Views = new List<string>();
        List<string> Functions = new List<string>();

        (bool database, bool file, bool filegroup, bool log, bool differential) backup;
        (bool database, bool file, bool filegroup, bool log, bool differential) restore;
        (int count, int collums, int constraints, int inserts, int clusteredIndexes, int NotClasteredIndexes) Tables = (0, 0, 0, 0, 0, 0);


        bool isRandomProcedure = false;
        int rules;
        int types;

        public Conditions() { }
        public Conditions(MainWindow MW)
        {
            mainWindow = MW;
        }
        public void SaveTables(params int[] values)
        {
            Tables.count = values[0];
            Tables.inserts = values[1];
            Tables.constraints = values[2];
            Tables.collums = values[3];           
        }

        public void SaveIndexes()
        {
            int.TryParse(mainWindow.TablesClastedIndexses.Text, out Tables.clusteredIndexes);
            int.TryParse(mainWindow.TablesNonclastedIndexes.Text, out Tables.NotClasteredIndexes);
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
        public void UpdateValues()
        {
            mainWindow.TablesCount.Text = Tables.count.ToString();
            mainWindow.TablesInserts.Text = Tables.inserts.ToString();
            mainWindow.TablesConstraints.Text = Tables.constraints.ToString();
            mainWindow.TableCollums.Text = Tables.collums.ToString();

            mainWindow.TablesClastedIndexses.Text = Tables.clusteredIndexes.ToString();
            mainWindow.TablesNonclastedIndexes.Text = Tables.NotClasteredIndexes.ToString();

            mainWindow.TablesTypes.Text = types.ToString();
            mainWindow.TablesRules.Text = rules.ToString();


        }

        public override string ToString()
        {
            return Tables.ToString();
        }
    }
}
