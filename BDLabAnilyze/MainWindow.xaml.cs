using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BDLabAnilyze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string connectionString;// = @"Data Source=" + materialSingleLineTextField1.Text + ";Initial Catalog=HOTEL;Integrated Security=True";
        string filePath = @"C:\Users\Kochmarik\Documents\SQL Server Management Studio\Кочмарик_ЛР5.sql";
        string text;
        SQLAnilyze sqlAnilyze;
        Conditions conditions;

        bool checkErrors = true;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            conditions = new Conditions(this);
                       
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                filePath = files[0];
                fileName.Text = files[0];
            }
            else
                return;
            //---------------
            

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                text = sr.ReadToEnd();
            }
                //Console.WriteLine(ConnectionDataBase.Text);
            string connectionString = @"Data Source=" + ConnectionDataBase.Text
                    + ";Initial Catalog=master;Integrated Security=True";
           
            sqlAnilyze = new SQLAnilyze(connectionString, ref conditions);

            ResultView.Content = sqlAnilyze.AnalyzeCode(text,checkErrors);
        }

        private void ButtonClouse_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Text documents (*.txt)|*.txt|Sql files (*.sql)|*.sql|All files (*.*)|*.*";
            dialog.FilterIndex = 2;

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                fileName.Text = dialog.FileName;
                filePath = dialog.FileName;
            }
        }

        private void SaveConditions(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();

            saveFileDialog1.Filter = "All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(saveFileDialog1.OpenFile(), conditions);
            }
        }

        private void LoadConditions(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                fileName.Text = dialog.FileName;
                filePath = dialog.FileName;

                for (int i = 0; i < 5; ++i)
                {
                    // десериализация
                    using (FileStream fs = new FileStream(dialog.FileName, FileMode.OpenOrCreate))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        conditions = (Conditions)formatter.Deserialize(fs);
                        conditions.mainWindow = this;
                        conditions.UpdateValues();
                    }
                }
            }
        }

        private void TablesCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void ComboBox_KeyDown(object sender, TextChangedEventArgs e)
        {
            conditions.SaveTables();
        }
        private void TablesNonclastedIndexes_TextChanged(object sender, TextChangedEventArgs e)
        {
            conditions.SaveIndexes();
        }

        private void TablesTypes_KeyDown(object sender, TextChangedEventArgs e)
        {
            conditions.SaveTypes();
        }

        private void CheckBoxBase_Click(object sender, RoutedEventArgs e)
        {
            conditions.SaveBackups();
        }

        private void ProcedureUpdate(object sender, TextChangedEventArgs e)
        {
            conditions.SaveProcedures();
        }
        private void ProcedureUpdate(object sender, RoutedEventArgs e)
        {
            conditions.SaveProcedures();
        }

        private void TriggersIUpdate(object sender, TextChangedEventArgs e)
        {
            conditions.SaveTriggers();
        }

        private void ViewsUpdate(object sender, TextChangedEventArgs e)
        {
            conditions.SaveViews();
        }

        private void ViewsIsJoins_Click(object sender, RoutedEventArgs e)
        {
            conditions.SaveViews();
        }

        private void TransactiosUpdate(object sender, RoutedEventArgs e)
        {
            conditions.SaveTransactions();
        }

        private void FunctionsUpdate(object sender, TextChangedEventArgs e)
        {
            conditions.SaveFunctions();
        }

        private void SelectUpdate(object sender, TextChangedEventArgs e)
        {
            conditions.SaveSelects();
        }

        private void CheckBoxErrors_Click(object sender, RoutedEventArgs e)
        {
            checkErrors = (bool)((CheckBox)sender).IsChecked;
        }
    }
}
