using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using OfficeOpenXml;



namespace ParseLab2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Data> allData = new List<Data>();//список всех записей
        public List<Data> onePage = new List<Data>();//список записей на 1 странице


        public MainWindow()
        {
            InitializeComponent();
            var path = @"..\Debug\localDB.xlsx";
            if (File.Exists(path) == false)
            {
                MessageBox.Show("Локальной базы не существует, нажмите кнопку загрузить для первичной загрузки\nУ автора лапки, поэтому дизайна неть(");
            }
            else
            {
                allData = Data.ExcelParce(@"..\Debug\localDB.xlsx");//Запись данных в лист
                onePage = Data.Pagination(allData, onePage, true);
                AllData.ItemsSource = onePage;
                
                
            }

        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            var path = @"..\Debug\localDB.xlsx";
            if (File.Exists(path))
            {
                MessageBox.Show("Локальная база данных уже существует !");
                /*allData = Data.ExcelParce(@"..\Debug\DB.xlsx");//Запись данных в лист              
                AllData.ItemsSource = allData;*/
            }
            else
            {
                
                WebClient webClient = new WebClient();
                string link = @"https://bdu.fstec.ru/files/documents/thrlist.xlsx";
                string save_path = "..\\Debug\\";
                try
                {
                    webClient.DownloadFile(link, save_path + "DB.xlsx");
                    allData = Data.ExcelParce(@"..\Debug\DB.xlsx");//Запись данных в лист
                    Data.SaveDbLoacl(allData, @"..\Debug\localDB.xlsx", "localDB.xlsx");//сохраняем БД локально
                    var delPath = System.IO.Path.GetFullPath("DB.xlsx");//Находим путь скаченого файла
                    File.Delete(System.IO.Path.Combine(delPath));//Удаляем скаченный файл
                    onePage = Data.Pagination(allData, onePage, true);
                    AllData.ItemsSource = onePage;
                }
                catch(WebException)
                {
                    MessageBox.Show("Ошибка, проверьте подключение к интернету");
                }
                
            }
        }


        private void Next_Page_Click(object sender, RoutedEventArgs e)
        {
            onePage = Data.Pagination(allData, onePage, true);
            AllData.ItemsSource = onePage;
            AllData.Items.Refresh();
        }

        private void Last_Page_Click(object sender, RoutedEventArgs e)
        {
            onePage = Data.Pagination(allData, onePage, false);
            AllData.ItemsSource = onePage;
            AllData.Items.Refresh();
        }

        private void Update_Data_Click(object sender, RoutedEventArgs e)
        {

            var path = @"..\Debug\localDB.xlsx";
            if (File.Exists(path))
            {

                List<Data> temp = Data.Update();
                Information.ItemsSource = temp;
                WebClient webClient = new WebClient();
                string link = @"https://bdu.fstec.ru/files/documents/thrlist.xlsx";
                string save_path = "..\\Debug\\";
                try
                {
                    webClient.DownloadFile(link, save_path + "DB.xlsx");
                    allData = Data.ExcelParce(@"..\Debug\DB.xlsx");//Запись данных в лист
                    Data.SaveDbLoacl(allData, @"..\Debug\localDB.xlsx", "localDB.xlsx");//сохраняем БД локально
                    onePage = Data.Pagination(allData, onePage, true);
                    AllData.ItemsSource = onePage;
                    if (temp.Count == 0)
                    {
                        MessageBox.Show($"Успешно!\nНо изменений не было(");
                    }
                    else
                    {
                        MessageBox.Show($"Успешно!\nИзменено {temp.Count} строк");
                    }
                    
                }
                catch(WebException)
                {
                    MessageBox.Show("Нажмите ОК что бы продолжить");
                }
                finally
                {
                    var delPath = System.IO.Path.GetFullPath("DB.xlsx");//Находим путь скаченого файла
                    File.Delete(System.IO.Path.Combine(delPath));//Удаляем скаченный файл
                }
            }
            else
            {
                MessageBox.Show("Локальной базы не существует ! Выполните первичную загрузку");
            }
        }

        private void Information_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AllData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Загрузить - автоматически создаеть локальную базу данных\nОбновить- обновть лакальную базу(в окне 'информация об обновлении' отображены старые строки, которые были обновлены)\nНажатие на строку - просмотр полной информации");
        }
    }
}
