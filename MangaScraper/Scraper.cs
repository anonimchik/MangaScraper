using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium.Support.UI;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
namespace MangaScraper
{
    class Scraper
    {
        private string MainUrl;
        private List<String> TitleUrl = new List<String>();
        private string NextPageUrl;
        private ICollection<IWebElement> PageUrl;
        private string Query;
        private List<String> Pages = new List<String>();
        private ushort FailCounter;
        private string ErrorMessage;
        public void SetMainUrl(string mainUrl)
        {
            MainUrl = mainUrl;
        }
        public string GetMainUrl()
        {
            return MainUrl;
        }
        public void SetTitleUrl(string mangaUrl)
        {
            TitleUrl.Add(mangaUrl);
        }
        public List<String> GetMangaUrl()
        {
            return TitleUrl;
        }
        public void SetQuery(string query)
        {
            Query = query;
        }
        public string GetQuery()
        {
            return Query;
        }
        public string getNextPageUrl()
        {
            return NextPageUrl;
        }
        public void setNextPageUrl(string nextPageUrl)
        {
            NextPageUrl = nextPageUrl;
        }
        public void setFailCounter(ushort _counter = 0)
        {
            FailCounter = _counter;
        }
        public ushort getFailCounter()
        {
            return FailCounter;
        }
        public void setErrorMessage(string _errorMessage)
        {
            ErrorMessage = _errorMessage;
        }
        public string getErrorMessage()
        {
            return ErrorMessage;
        }

        /// <summary>
        /// Получение ссылки на следующую страницу
        /// </summary>
        /// <param name="drv">Объект driver интерфейса IWebDriver</param>
        /// <param name="sp">Объект sp класса Scraper</param>
        public void ParseNextPageUrl(IWebDriver drv)
        {
            /*  |Парсинг ссылки на следующую страницу с катологом манг|  */
            if (drv.FindElements(By.XPath(@"//a[@class='nextLink']")).Count > 0) //получение сслылки на следующую страницу
            {
                setNextPageUrl(drv.FindElement(By.XPath(@"//a[@class='nextLink']")).GetAttribute("href")); //запись ссылки на следующую страницу в поле NextPageUrl класса Scraper
            }
        }

        /// <summary>
        /// Функция для парсинга первой станицы списка манг (https://readmanga.live/list)
        /// </summary>
        /// <param name="drv">Объект driver интерфейса IWebDriver</param>
        /// <param name="url">Результат выполнения метода GetMainUrl класса Parser</param>
        /// <param name="sp">Объект sp класса Scraper</param>
        public void ParseFirstPage(IWebDriver drv, Scraper sp)
        {
            /*  |Парсинг первой страницы отдельно необходим для получение ссылки на следующую страницу|  */
            ICollection<IWebElement> mangaUrl = drv.FindElements(By.XPath(@"//div[@class='desc']/h3/a")); //получение ссылок на манги
            foreach (var mangaurl in mangaUrl) //перебор коллекции ссылок
            {
                SetTitleUrl(mangaurl.GetAttribute("href")); //запиcь ссылок в список
            }
            ParseNextPageUrl(drv); //парсинг списка манг
        }

        /// <summary>
        /// Получение ссылок на все манги в каталоге
        /// </summary>
        /// <param name="drv">Объект driver интерфейса IWebDriver</param>
        /// <param name="sp">Объект sp класса Scraper</param>
        public void ParseAllPages(IWebDriver drv)
        {
            /*  |Парсинг ссылок на конкректную мангу|  */
            ParseNextPageUrl(drv); //парсинг списка манг
            ICollection<IWebElement> mangaUrl = drv.FindElements(By.XPath(@"//div[@class='desc']/h3/a")); //получение ссылок на манги
            foreach (var mangaurl in mangaUrl) //перебор коллекции ссылок
            {
                SetTitleUrl(mangaurl.GetAttribute("href")); //запись ссылок в список
            }
        }


        public void Action(ObservableCollection<BaseModel> bm)
        {
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
                Content mg = new Content(); //создание объекта mg класса Manga
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


                #region navigate to current content page and parse data from it

                /*   Парсинг конкректной страницы   */
                //for (int j = 0; j < sp.GetMangaUrl().Count; j++) //парсинг манги
                for (int j = 0; j < 2; j++)
                {
                    Content mng = new Content(); //создание объекта mng класса Manga
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
                                                          // mng.parseMangaImageUrls(driver, mng);
                        bm.Add(mng); //запись объе в список

                        #endregion
                    }
                    else
                    {
                        #region parse manhwa

                        /*   Парсинг манхвы   */
                        if (driver.FindElements(By.XPath(@"//span[@class='elem_category ']/a"))[0].Text == "Манхва")
                        {

                        }

                        #endregion

                        #region parse manhua
                        /*   Парсинг маньхуа   */
                        if (driver.FindElements(By.XPath(@"//span[@class='elem_category ']/a"))[0].Text == "Маньхуа")
                        {

                        }
                        #endregion

                    }

                }

                #endregion

                #region tree creating

                /* Формирование дерерва объектов   */

                
                
                #endregion
                    stopwatch.Stop(); //отключение таймера
                var time = stopwatch.Elapsed;
                /*   Формирование времени   */
               /* 
                Timer.Text += String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            time.Hours, time.Minutes, time.Seconds,
                                            time.Milliseconds / 10);
                Fail.Text += sp.getErrorMessage();
                FailCounter.Text += sp.getFailCounter().ToString();
               */
            }
        }
        
    }
}
