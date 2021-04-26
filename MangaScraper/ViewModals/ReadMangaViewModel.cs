using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Edge.SeleniumTools;
using System.Net;

namespace MangaScraper.ViewModals
{
    class ReadMangaViewModel:ReadMangaModel
    {

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
                NextPageUrl = drv.FindElement(By.XPath(@"//a[@class='nextLink']")).GetAttribute("href"); //запись ссылки на следующую страницу в поле NextPageUrl класса Scraper
            }
        }

        /// <summary>
        /// Функция для парсинга первой станицы списка манг (https://readmanga.live/list)
        /// </summary>
        /// <param name="drv">Объект driver интерфейса IWebDriver</param>
        /// <param name="url">Результат выполнения метода GetMainUrl класса Parser</param>
        /// <param name="sp">Объект sp класса Scraper</param>
        public void ParseFirstPage(IWebDriver drv)
        {
            /*  |Парсинг первой страницы отдельно необходим для получение ссылки на следующую страницу|  */
            ICollection<IWebElement> mangaUrl = drv.FindElements(By.XPath(@"//div[@class='desc']/h3/a")); //получение ссылок на манги
            foreach (var mangaurl in mangaUrl) //перебор коллекции ссылок
            {
                MangaUrl.Add(mangaurl.GetAttribute("href")); //запиcь ссылок в список
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
                MangaUrl.Add(mangaurl.GetAttribute("href")); //запись ссылок в список
            }
        }

        /// <summary>
        /// Парсинг информации
        /// </summary>
        /// <param name="bm"></param>
        public void parseInfo(ObservableCollection<BaseModel> bm)
        {
            Stopwatch stopwatch = new Stopwatch(); //создание объекта класса Stopwatch
            stopwatch.Start(); //таймер
            BaseModel mg = new BaseModel(); //создание объекта класса
            Url = "https://readmanga.live/list"; //ссылка для получения полного списка манги
            var options = new EdgeOptions(); //задание опций для Edge
            options.UseChromium = true; //включение хромиума
            MangaUrl = new List<String>();
            using (IWebDriver driver = new EdgeDriver(options)) //основная работа парсера
            {

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                driver.Navigate().GoToUrl(Url); //переход на сайт 
                TitleUrl = new List<String>();
                ParseFirstPage(driver); //парсинг первой страницы с каталогом манг

                #region rotating to every page and get page url

                var l = 0; //счетчик
                           //while(driver.FindElements(By.XPath(@"//a[@class='nextLink']")).Count > 0) //парсинг всех страниц //откоментировать когда будет готов парсер
                while (l < 1) //закоментировать когда будет готов парсер
                {
                    driver.Navigate().GoToUrl(NextPageUrl); //переход на следующую страницу
                    ParseAllPages(driver);
                    l++;
                }

                #endregion


                #region navigate to current content page and parse data from it

                /*   Парсинг конкректной страницы   */
                //for (int j = 0; j < sp.GetMangaUrl().Count; j++) //парсинг манги
                for (int j = 0; j < 3; j++)
                {
                    BaseModel mng = new BaseModel(); //создание объекта mng класса Manga
                    driver.Navigate().GoToUrl(MangaUrl[j]); //переход по страницам

                    /*   Раскрытие скрытых жанров   */
                    if (driver.FindElements(By.XPath(@"//p[@class='elementList']/span[@class='js-link']")).Count > 0)
                    {
                        driver.FindElement(By.XPath(@"//p[@class='elementList']/span[@class='js-link']")).Click(); //имитация клика
                    }

                    /*   Парсинг манги   */

                    getMangaContent(driver, mng); //получение данных
                    // mng.parseMangaImageUrls(driver, mng);
                    bm.Add(mng); //запись объе в список  
                }
            }

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

        #region getting image urls
        /// <summary>
        /// Парсинг изображений
        /// </summary>
        public void parseMangaImageUrls(IWebDriver drv, BaseModel mng)
        {
            List<String> subImages = new List<String>();
            for (int i = 0; i < 1; i++)
            {
                drv.Navigate().GoToUrl(mng.Chapters[i].Substring(0, mng.Chapters[i].IndexOf("|")));
                int LastPage = int.Parse(drv.FindElement(By.XPath(@"//span[@class='pages-count']")).Text) - 1;
                for (int j = 0; j <= LastPage; j++)
                {
                    drv.Navigate().GoToUrl(mng.Chapters[i].Substring(0, mng.Chapters[i].IndexOf("|")) + "#page=" + j);
                    drv.Navigate().Refresh();
                    drv.FindElement(By.XPath(@"//img[@id='mangaPicture']")).GetAttribute("src");
                    subImages.Add(drv.FindElement(By.XPath(@"//img[@id='mangaPicture']")).GetAttribute("src"));
                }
                mng.Images.Add(new List<string>(subImages));
            }

        }
        #endregion

        #region download images from chapters
        /// <summary>
        /// Скачивание картинок
        /// </summary>
        /// <param name="MangaList"></param>
        public void downloadImages(List<BaseModel> MangaList)
        {
            for (int i = 0; i < MangaList.Count; i++)
            {
                for (int j = 0; j < MangaList[i].Images.Count; j++)
                {
                    for (int k = 0; k < MangaList[i].Images[j].Count; k++)
                    {
                        using (WebClient webclient = new WebClient())
                        {
                            webclient.DownloadFile(MangaList[i].Images[j][k].ToString(), k + ".png");
                        }
                    }
                }

            }
        }
        #endregion

        #region getting manga info
        /// <summary>
        /// Парсинг информации о манге
        /// </summary>
        /// <param name="drv"></param>
        public void getMangaContent(IWebDriver drv, BaseModel mng)
        {
            ReadMangaModel srp = new ReadMangaModel();
            mng.Category = drv.FindElement(By.XPath(@"//h1[@class='names']")).Text.Substring(0, drv.FindElement(By.XPath(@"//h1[@class='names']")).Text.IndexOf(" "));
            try
            {
                /*   Получение названия манги   */
                mng.Title = drv.FindElement(By.XPath(@"//span[@class='name']")).Text;
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage=e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение английского названия   */
                mng.OtherTitles.Add(drv.FindElement(By.XPath(@"//span[@class='eng-name']")).Text);
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение оригинального названия   */
                mng.OtherTitles.Add(drv.FindElement(By.XPath(@"//span[@class='original-name']")).Text);
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   получение задней картины   */
                mng.BackgroundImg = drv.FindElements(By.XPath(@"//img[@class='fotorama__img']"))[0].GetAttribute("src");
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение кол-ва томов   */
                var str = Regex.Replace(drv.FindElement(By.XPath(@"//tbody/tr[1]/td[1]/a")).Text, @"[^\d|^-]", "");
                mng.VolumeNumber = byte.Parse(str.Substring(0, str.IndexOf("-")));
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   получение статуса перевода   */
                mng.TranslateStatus = Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[2]")).Text, @"Перевод: ", "");
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   получение года выпуска   */
                mng.ReleaseYear = ushort.Parse(drv.FindElement(By.XPath(@"//span[@class='elem_year ']/a")).Text);
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение списка жанров   */
                ICollection<IWebElement> genres = drv.FindElements(By.XPath(@"//span[@class='elem_genre ']/a")); //подготовка данных к заненсению в список Genres
                ICollection<IWebElement> hidden_genres = drv.FindElements(By.XPath(@"//p[@class='elementList']/span[@class='elem_genre hide']")); //подготовка скрытых данных к занесению в список Genres
                foreach (var gnrs in genres) //запись данных в список Genres
                {
                    mng.Genres.Add(gnrs.Text);
                }
                foreach (var hdn_genres in hidden_genres) //запись данных в список Genres
                {
                    mng.Genres.Add(Regex.Replace(hdn_genres.Text, ", ", ""));
                }
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение художника   */
                ICollection<IWebElement> painters = drv.FindElements(By.XPath(@"//span[@class='elem_illustrator ']/a"));
                foreach (var _painter in painters)
                {
                    mng.Painters.Add(_painter.Text);
                }

            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение художника   */
                mng.Author = drv.FindElement(By.XPath(@"//span[@class='elem_author ']/a")).Text;
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение сценариста   */
                ICollection<IWebElement> screnwriters = drv.FindElements(By.XPath(@"//span[@class='elem_screenwriter ']/a"));
                foreach (var _screenwriter in screnwriters)
                {
                    mng.Screenwriters.Add(_screenwriter.Text);
                }

            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение описания   */
                mng.Description = drv.FindElement(By.XPath(@"//div[@class='manga-description']")).Text;
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение ссылок на главы   */
                ICollection<IWebElement> chapter = drv.FindElements(By.XPath(@"//a[@class='chapter-link cp-l']"));
                foreach (var ch in chapter)
                {
                    mng.Chapters.Add(ch.GetAttribute("href") + "|" + ch.Text);
                }
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение количества глав   */
                mng.ChapterNumber = Convert.ToUInt16(mng.Chapters.Count);
            }

            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }


            try
            {
                /*   Получение списка журналов   */
                ICollection<IWebElement> magazines = drv.FindElements(By.XPath(@"//span[@class='elem_magazine ']/a"));
                foreach (var _magazine in magazines)
                {
                    mng.Magazines.Add(_magazine.Text);
                }
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение списка переводчиков   */
                ICollection<IWebElement> translators = drv.FindElements(By.XPath(@"//span[@class='elem_translator ']/a"));
                foreach (var _translator in translators)
                {
                    mng.Translators.Add(_translator.Text);
                }
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

            try
            {
                /*   Получение списка издательств   */
                ICollection<IWebElement> publishings = drv.FindElements(By.XPath(@"//span[@class='elem_publisher ']"));
                foreach (var _publishing in publishings)
                {
                    mng.Publishings.Add(Regex.Replace(_publishing.Text, ", | ", ""));

                }
            }
            catch (Exception e)
            {
                FailCounter++;
                ErrorMessage = e.Message + e.StackTrace;
            }

        }

        #endregion

        #region writing data to file
        public void writeToFile(ObservableCollection<BaseModel> bm)
        {
            string path = @"C:\Users\Админ\Desktop";
            using (FileStream fs = new FileStream(path + @"\dataFile.txt", FileMode.Create))
            {
                string record;
                byte[] array; ;
                for (int i = 0; i < bm.Count; i++)
                {
                    record = $"Название: " + bm[i].Title + "\r\nДругие названия: ";
                    bool l = false;
                    foreach (var othertitle in bm[i].OtherTitles)
                    {
                        record += othertitle + ",";
                        l = true;
                    }
                    if (l) record = record.Substring(0, record.LastIndexOf(","));
                    record += "\r\nКатегория: " + bm[i].Category +
                        "\r\nАвтор: " + bm[i].Author + "\r\nСценарист: ";
                    l = false;
                    for (int j = 0; j < bm[i].Screenwriters.Count; j++)
                    {
                        record += bm[i].Screenwriters[j] + ",";
                        l = true;
                    }
                    if (l) record = record.Substring(0, record.LastIndexOf(","));
                    record += "\r\nОписание: " + bm[i].Description + "\r\nКоличество томов: " + bm[i].VolumeNumber +
                        "\r\nКоличество глав: " + bm[i].ChapterNumber + "\r\nСтатус перевода: " + bm[i].TranslateStatus + "\r\nХудожник: ";
                    l = false;
                    foreach (var painter in bm[i].Painters)
                    {
                        record += painter + ",";
                    }
                    if (l) record = record.Substring(0, record.LastIndexOf(","));
                    record += "\r\nЖанры: ";
                    l = false;
                    foreach (var genre in bm[i].Genres)
                    {
                        record += genre + ",";
                        l = true;
                    }
                    if (l) record = record.Substring(0, record.LastIndexOf(","));
                    record += "\r\nГод выпуска: " + bm[i].ReleaseYear + "\r\n\r\n";
                    array = System.Text.Encoding.Default.GetBytes(record);
                    fs.Write(array, 0, array.Length);
                }
            }
        }
        #endregion
    }
}
