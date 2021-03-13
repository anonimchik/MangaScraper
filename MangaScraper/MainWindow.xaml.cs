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
                
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl(sp.GetMainUrl()); //переход на сайт 
                Manga mg = new Manga(); //создание объекта mg класса Manga
                mg.ParseFirstListManga(driver, sp); //парсинг первой страницы с каталогом манг
                var i = 0; //счетсчик
                //while(driver.FindElements(By.XPath(@"//a[@class='nextLink']")).Count > 0) //парсинг всех страниц //откоментировать когда будет готов парсер
                while(i<1) //закоментировать когда будет готов парсер
                {
                    driver.Navigate().GoToUrl(sp.getNextPageUrl()); //переход на следующую страницу
                    mg.ParseAllMangaPage(driver, sp); 
                    i++;
                }
                List<Manga> MangaList = new List<Manga>(); //создание списка MangaList
                for (int j = 0; j < sp.GetMangaUrl().Count; j++) //парсинг манги
                {
                    Manga mng = new Manga(); //создание объекта mng класса Manga
                    driver.Navigate().GoToUrl(sp.GetMangaUrl()[j]); //переход по страницам
                    mng.getMangaContent(driver, mng); //получение
                    MangaList.Add(mng); //запись объе в список
                }
            }

        }
    }
}
