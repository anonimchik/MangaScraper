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
using OpenQA.Selenium;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium.Support.UI;

namespace MangaScraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Scraper sp = new Scraper(); //создание объекта класса
            sp.SetMainUrl("https://readmanga.live/list"); //ссылка для получения полного списка манги
            var options = new EdgeOptions(); //задание опций для Edge
            options.UseChromium = true; //включение хромиума
            
            using (IWebDriver driver = new EdgeDriver(options)) //основная работа парсера
            {
                
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                driver.Navigate().GoToUrl(sp.GetMainUrl()); //переход на сайт 
                Manga mg = new Manga(); //создание объекта mg класса Manga
                sp.ParseFirstPage(driver, sp); //парсинг первой страницы с каталогом манг
                var i = 0; //счетчик
                //while(driver.FindElements(By.XPath(@"//a[@class='nextLink']")).Count > 0) //парсинг всех страниц //откоментировать когда будет готов парсер
                while(i<1) //закоментировать когда будет готов парсер
                {
                    driver.Navigate().GoToUrl(sp.getNextPageUrl()); //переход на следующую страницу
                    sp.ParseAllPages(driver); 
                    i++;
                }
                List<Manga> MangaList = new List<Manga>(); //создание списка MangaList
                List<Manhua> ManhuaList = new List<Manhua>();
                List<Manhwa> ManhwaList = new List<Manhwa>();
                //for (int j = 0; j < sp.GetMangaUrl().Count; j++) //парсинг манги
                for (int j = 0; j < 2; j++)
                {
                    Manga mng = new Manga(); //создание объекта mng класса Manga
                    Manhwa mnh = new Manhwa(); //создание объекта mnh класса Manhwa
                    Manhua mna = new Manhua(); //создание объекта mna класса Manhua
                    driver.Navigate().GoToUrl(sp.GetMangaUrl()[j]); //переход по страницам
                    if (driver.FindElements(By.XPath(@"//p[@class='elementList']/span[@class='js-link']")).Count > 0) //раскрытие скрытых жанров
                    {
                        driver.FindElement(By.XPath(@"//p[@class='elementList']/span[@class='js-link']")).Click();
                    }
                    if (driver.FindElements(By.XPath(@"//span[@class='elem_category ']/a")).Count == 0) //парсинг манги
                    {
                        mng.getMangaContent(driver, mng); //получение данных
                        mng.parseMangaImageUrls(driver, mng);
                        MangaList.Add(mng); //запись объе в список
                       
                    }
                    else
                    {
                        if (driver.FindElements(By.XPath(@"//span[@class='elem_category ']/a"))[0].Text == "Манхва") //парсинг манхвы
                        {
                            mnh.getManhwaContent(driver, mnh); //получение данных
                            ManhwaList.Add(mnh); //запись объе в список
                            
                        }
                        if (driver.FindElements(By.XPath(@"//span[@class='elem_category ']/a"))[0].Text == "Маньхуа") //парсинг маньхуа
                        {
                            mna.getManhuaContent(driver, mna); //получение данных
                            ManhuaList.Add(mna); //запись объе в список
                            
                            
                        }
                    }

                }
                //mg.downloadImages(MangaList);
                for (int l = 0; l < MangaList.Count; l++)
                {
                    var item = new TreeViewItem();

                    /*
                    item.Header = "Манга";
                    item.Items.Add(MangaList[l].Title);
                    
                    foreach (var otherTitles in MangaList[l].OtherTitles)
                    {
                        item.Items.Add(otherTitles);
                    }
                    item.Items.Add(item.Header = "1");
                    
                    TreeView.Items.Add(item);
                    */
                    TreeView.ItemsSource = MangaList[l].Chapters;

                }
            }
            

        }
    }
}
