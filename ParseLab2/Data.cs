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
using System.Runtime.Serialization.Formatters.Binary;

namespace ParseLab2
{
    //[Serializable]
    public class Data
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public string Source { get; set; }
        public string ObjImpact { get; set; }
        public string Confidentiality { get; set; }
        public string Integrity { get; set; }
        public string Availability { get; set; }
        public Data(string id,string name,string discription, string source, string objImpact, string confidentiality,string integrity, string availability)
        {
            ID = id;
            Name = name;
            Discription = discription;
            Source = source;
            ObjImpact = objImpact;
            Confidentiality = confidentiality;
            Integrity = integrity;
            Availability = availability;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Data d = obj as Data;
            if (d as Data == null) return false;
            return d.ID == this.ID && d.Name == this.Name && d.Discription == this.Discription && d.Source == this.Source && d.ObjImpact == this.ObjImpact && d.Confidentiality == this.Confidentiality && d.Integrity == this.Integrity && d.Availability == this.Availability;
                         
        }
        //Функция записывает строку из таблицы Excel в новый экземпляр класса и помещает его в список
        public static List<Data> ExcelParce(string link)
        {
            List<Data> tempList = new List<Data>();
            FileInfo fl = new FileInfo(link);
            using (ExcelPackage excelPackage = new ExcelPackage(fl))
            {

                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1]; //получить лист из книги
                int colCount = worksheet.Dimension.End.Column;  //количество столбцов
                int rowCount = worksheet.Dimension.End.Row;     //количество строк
                for (int row = 3; row <= rowCount; row++)
                {
                    string id;
                    string name;
                    string discription;
                    string source;
                    string objImpact;
                    string confidentiality;
                    string integrity;
                    string availability;
                    string temp = worksheet.Cells[row, 1].Value?.ToString().Trim();
                    id = "УБИ." + temp;
                    name = worksheet.Cells[row, 2].Value?.ToString().Trim();
                    discription = worksheet.Cells[row, 3].Value?.ToString().Trim();
                    source = worksheet.Cells[row, 4].Value?.ToString().Trim();
                    objImpact = worksheet.Cells[row, 5].Value?.ToString().Trim();
                    string con = worksheet.Cells[row, 6].Value?.ToString().Trim();
                    int zeroOrOne = int.Parse(con);
                    if (zeroOrOne == 1)
                    {
                        confidentiality = "Да";
                    }
                    else
                    {
                        confidentiality = "Нет";
                    }

                    string ig = worksheet.Cells[row, 7].Value?.ToString().Trim();
                    zeroOrOne = int.Parse(ig);

                    if (zeroOrOne == 1)
                    {
                        integrity = "Да";
                    }
                    else
                    {
                        integrity = "Нет";
                    }

                    string av = worksheet.Cells[row, 8].Value?.ToString().Trim();
                    zeroOrOne = int.Parse(av);

                    if (zeroOrOne == 1)
                    {
                        availability = "Да";
                    }
                    else
                    {
                        availability = "Нет";
                    }
                    
                    tempList.Add(new Data(id, name, discription, source, objImpact, confidentiality, integrity, availability));
                }
            }
            return tempList;
        }
        public static void SaveDbLoacl(List<Data> list,string link,string fileName)
        {
            //Create a new ExcelPackage
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet");
                FileInfo fi = new FileInfo(link);
                excelPackage.SaveAs(fi);
            }
            var path = System.IO.Path.GetFullPath(fileName);
            FileInfo file = new FileInfo(path);
            //create a new Excel package from the file
            using (ExcelPackage excelPackage = new ExcelPackage(file))
            {
                //create an instance of the the first sheet in the loaded file
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];
                //Добавление данных
                int num = 0; //Индекс записи в листе
                for (int i = 1; i < list.Count + 3; i++)//строки = количество элементов в листе
                {
                    if(i== 1 || i == 2)
                    {
                        for (int j = 1; j <= 8; j++)
                        {
                            worksheet.Cells[i, j].Value = "***";
                        }
                    }
                    else
                    {
                        string temp = list[num].ID;
                        string[] tempArr = temp.Split('.');
                        worksheet.Cells[i, 1].Value = tempArr[1];
                        worksheet.Cells[i, 2].Value = list[num].Name;
                        worksheet.Cells[i, 3].Value = list[num].Discription;
                        worksheet.Cells[i, 4].Value = list[num].Source;
                        worksheet.Cells[i, 5].Value = list[num].ObjImpact;
                        if (list[num].Confidentiality == "Да")
                        {
                            worksheet.Cells[i, 6].Value = 1;
                        }
                        else
                        {
                            worksheet.Cells[i, 6].Value = 0;
                        }
                        if (list[num].Integrity == "Да")
                        {
                            worksheet.Cells[i, 7].Value = 1;
                        }
                        else
                        {
                            worksheet.Cells[i, 7].Value = 0;
                        }
                        if (list[num].Availability == "Да")
                        {
                            worksheet.Cells[i, 8].Value = 1;
                        }
                        else
                        {
                            worksheet.Cells[i, 8].Value = 0;
                        }
                        num++;
                    }
                                
                }

                //save the changes
                excelPackage.Save();
            }
        }
        public static List<Data> Pagination(List<Data> allList,List<Data> onePage, bool leftOrRight)
        {
            List<Data> clear = new List<Data>();
            if (onePage.Count == 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    try
                    {
                        onePage.Add(allList[i]);
                    }
                    catch
                    {
                        break;
                    }
                }
                return onePage;
            }
            else if (onePage[0].ID == allList[0].ID && leftOrRight == false)
            {
                int check = 0;
                onePage = clear;
                for (int i = allList.Count - 1; i >= 0; i--)
                {
                    if (check > (allList.Count % 15)) { break; }
                    onePage.Add(allList[i]);
                    check++;
                }
                onePage.Reverse();
                return onePage;

            }
            else if (onePage[onePage.Count - 1].ID == allList[allList.Count - 1].ID && leftOrRight)
            {
                onePage = clear;
                for (int i = 0; i < 15; i++)
                {
                    try
                    {
                        onePage.Add(allList[i]);
                    }
                    catch
                    {
                        break;
                    }
                }
                return onePage;
            }
            else
            {
                if (leftOrRight)
                {
                    string last = onePage[onePage.Count - 1].ID;
                    string[] temp = last.Split('.');
                    int numLast = int.Parse(temp[1]);
                    onePage = clear;
                    for (int i = 0; i < 15; i++)
                    {
                        try
                        {
                            onePage.Add(allList[numLast]);
                            numLast++;
                        }
                        catch
                        {

                            break;
                        }

                    }
                }
                else
                {
                    string first = onePage[0].ID;
                    string[] temp = first.Split('.');
                    int numFirst = int.Parse(temp[1]) - 2;
                    onePage = clear;
                    for (int i = 0; i < 15; i++)
                    {
                        try
                        {
                            onePage.Add(allList[numFirst]);
                            numFirst--;
                        }
                        catch
                        {

                            break;
                        }
                    }
                    onePage.Reverse();
                }
                return onePage;
            }

        }
        public static List<Data> Update()
        {
            List<Data> newVersion = new List<Data>();
            List<Data> lastVersion = new List<Data>();
            WebClient webClient = new WebClient();
            string link = @"https://bdu.fstec.ru/files/documents/thrlist.xlsx";
            string save_path = "..\\Debug\\";

            try
            {
                webClient.DownloadFile(link, save_path + "DB.xlsx");
            }
            catch (WebException)
            {
                MessageBox.Show("Ошибка обновления ! Проверьте подключение к интернету");
                return null;
            }
            newVersion = Data.ExcelParce(@"..\Debug\DB.xlsx");//Запись данных в лист
            Data.SaveDbLoacl(newVersion, @"..\Debug\tempDB.xlsx", "tempDB.xlsx");//сохраняем БД локально
            var delPath = System.IO.Path.GetFullPath("DB.xlsx");//Находим путь скаченого файла
            File.Delete(System.IO.Path.Combine(delPath));//Удаляем скаченный файл

            //Все что выше нужно, что бы скаченная БД и локальная были сохранены одинаково
            newVersion = Data.ExcelParce(@"..\Debug\tempDB.xlsx");
            lastVersion = Data.ExcelParce(@"..\Debug\localDB.xlsx");

            List<Data> log = new List<Data>();

            if(newVersion.Count == lastVersion.Count)
            {
                for (int i = 0; i < newVersion.Count; i++)
                {
                    if (newVersion[i].Equals(lastVersion[i]) == false)
                    {
                        log.Add(lastVersion[i]);
                    }
                }
            }
            else if(newVersion.Count > lastVersion.Count)
            {
                for (int i = lastVersion.Count; i < newVersion.Count; i++)
                {
                        log.Add(newVersion[i]);
                }
            }
            else
            {
                for (int i = lastVersion.Count; i < newVersion.Count; i++)
                {
                    log.Add(newVersion[i]);
                }
            }
            delPath = System.IO.Path.GetFullPath("tempDB.xlsx");//Находим путь скаченого файла
            File.Delete(System.IO.Path.Combine(delPath));//Удаляем скаченный файл
            return log;
        }
    }
}
