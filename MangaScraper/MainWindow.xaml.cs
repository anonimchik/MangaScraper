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
using System.Diagnostics;

namespace MangaScraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Stopwatch stopwatch = new Stopwatch(); //создание объекта класса Stopwatch
            stopwatch.Start(); //таймер
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

                #region rotating to every page and get page url

                var l = 0; //счетчик
                //while(driver.FindElements(By.XPath(@"//a[@class='nextLink']")).Count > 0) //парсинг всех страниц //откоментировать когда будет готов парсер
                while (l < 1) //закоментировать когда будет готов парсер
                {
                    driver.Navigate().GoToUrl(sp.getNextPageUrl()); //переход на следующую страницу
                    sp.ParseAllPages(driver);
                    l++;
                }

                #endregion

                List<Manga> MangaList = new List<Manga>(); //создание списка MangaList
                List<Manhua> ManhuaList = new List<Manhua>();
                List<Manhwa> ManhwaList = new List<Manhwa>();

                #region navigate to current content page and parse data from it

                /*   Парсинг конкректной страницы   */
                //for (int j = 0; j < sp.GetMangaUrl().Count; j++) //парсинг манги
                for (int j = 0; j < 6; j++)
                {
                    Manga mng = new Manga(); //создание объекта mng класса Manga
                    Manhwa mnh = new Manhwa(); //создание объекта mnh класса Manhwa
                    Manhua mna = new Manhua(); //создание объекта mna класса Manhua
                    driver.Navigate().GoToUrl(sp.GetMangaUrl()[j]); //переход по страницам

                    /*   Раскрытие скрытых жанров   */
                    if (driver.FindElements(By.XPath(@"//p[@class='elementList']/span[@class='js-link']")).Count > 0) 
                    {
                        driver.FindElement(By.XPath(@"//p[@class='elementList']/span[@class='js-link']")).Click(); //имитация клика
                    }

                    /*   Парсинг манги   */
                    if (driver.FindElements(By.XPath(@"//span[@class='elem_category ']/a")).Count == 0)
                    {
                        #region parse manga

                        mng.getMangaContent(driver, mng); //получение данных
                        mng.parseMangaImageUrls(driver, mng);
                        MangaList.Add(mng); //запись объе в список

                        #endregion
                    }
                    else
                    {
                        #region parse manhwa

                        /*   Парсинг манхвы   */
                        if (driver.FindElements(By.XPath(@"//span[@class='elem_category ']/a"))[0].Text == "Манхва")
                        {
                            mnh.getManhwaContent(driver, mnh); //получение данных
                            ManhwaList.Add(mnh); //запись объе в список
                        }

                        #endregion

                        #region parse manhua
                        /*   Парсинг маньхуа   */
                        if (driver.FindElements(By.XPath(@"//span[@class='elem_category ']/a"))[0].Text == "Маньхуа") 
                        {
                            mna.getManhuaContent(driver, mna); //получение данных
                            ManhuaList.Add(mna); //запись объе в список
                        }
                        #endregion

                    }

                }

                #endregion

                #region tree creating

                /* Формирование дерерва объектов   */
                for (int i = 0; i < MangaList.Count; i++)
                {

                    var mainTree = new TreeViewItem();
                    mainTree.Header = MangaList[i].Category + " { " + i + " } ";

                    /*   Формирование названия   */
                    var title = new TreeViewItem();
                    title.Header = "Название";
                    title.Items.Add(MangaList[i].Title);
                    mainTree.Items.Add(title);

                    if (MangaList[i].OtherTitles.Count > 0) 
                    {
                        /*   Формирование других названий   */
                        var otherTitles = new TreeViewItem();
                        otherTitles.Header = "Другие названия";
                        foreach (var _otherTitles in MangaList[i].OtherTitles)
                        {
                            otherTitles.Items.Add(_otherTitles);
                        }
                        mainTree.Items.Add(otherTitles);
                    }
                    
                    /*  Формирование описания   */
                    var description = new TreeViewItem();
                    description.Header = "Описание";
                    description.Items.Add(MangaList[i].Description);
                    mainTree.Items.Add(description);

                    /*   Формирование кол-ва томов   */
                    var volumeNumber = new TreeViewItem();
                    volumeNumber.Header = "Кол-во томов";
                    volumeNumber.Items.Add(MangaList[i].VolumeNumber);
                    mainTree.Items.Add(volumeNumber);

                    /*   Формировнаие кол-ва глав   */
                    var chapterNumber = new TreeViewItem();
                    chapterNumber.Header = "Кол-во глав";
                    chapterNumber.Items.Add(MangaList[i].ChapterNumber);
                    mainTree.Items.Add(chapterNumber);

                    /*   Формирование года выпуска   */
                    var releaseYear = new TreeViewItem();
                    releaseYear.Header = "Год выпуска";
                    releaseYear.Items.Add(MangaList[i].ReleaseYear);
                    mainTree.Items.Add(releaseYear);

                    /*   Формирование статуса перевода   */
                    var translateStatus = new TreeViewItem();
                    translateStatus.Header = "Статус перевода";
                    translateStatus.Items.Add(MangaList[i].TranslateStatus);
                    mainTree.Items.Add(translateStatus);

                    /*   Формирование списка переводчиков   */
                    var translators = new TreeViewItem();
                    translators.Header = "Переводчики";
                    foreach (var _translator in MangaList[i].Tranlators)
                    {
                        translators.Items.Add(_translator);
                    }
                    mainTree.Items.Add(translators);

                    /*   Формирование списка глав   */
                    var chapters = new TreeViewItem();
                    chapters.Header = "Главы";
                    foreach (var _chapters in MangaList[i].Chapters)
                    {
                        chapters.Items.Add(_chapters);
                    }
                    mainTree.Items.Add(chapters);

                    /*   Формирование списка художников   */
                    if (MangaList[i].Painter != null)
                    {
                        var painter = new TreeViewItem();
                        painter.Header = "Художник";
                        painter.Items.Add(MangaList[i].Painter);
                        mainTree.Items.Add(painter);
                    }

                    /*   Формирование списка авторов*/
                    if (MangaList[i].Author != null)
                    {
                        var author = new TreeViewItem();
                        author.Header = "Автор";
                        author.Items.Add(MangaList[i].Author);
                        mainTree.Items.Add(author);
                    }

                    /*   Формирование списка сценаристов   */
                    if (MangaList[i].Screenwriter != null)
                    {
                        var screenwriter = new TreeViewItem();
                        screenwriter.Header = "Сценарист";
                        screenwriter.Items.Add(MangaList[i].Screenwriter);
                        mainTree.Items.Add(screenwriter);
                    }

                    TreeView.Items.Add(mainTree);

                }

                #endregion
                stopwatch.Stop(); //отключение таймера
                var time = stopwatch.Elapsed;
                /*   Формирование времени   */
                Timer.Content += String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            time.Hours, time.Minutes, time.Seconds,
                                            time.Milliseconds / 10);
                Fail.Content += sp.getErrorMessage();
                FailCounter.Content += sp.getFailCounter().ToString();
            }
        }
    }
}
