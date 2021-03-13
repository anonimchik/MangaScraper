using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MangaScraper
{
    class Manga
    {
        public string Title { set; get; } //название 
        public string BackgroundImg { set; get; } //фоновое изображение 
        public string Description { set; get; } //описание 
        public int NumberVolumes { set; get; } //количество томов
        public int NumberChapters { set; get; }
        public string TranslateStatus { set; get; }
        public string Author { set; get; }
        public List<String> Chapters = new List<String>();
        public int ReleaseYear { set; get; }

        public List<String> Genres = new List<String>();
        public int PagesCount { set; get; }
        public List<String> Illustrator = new List<String>();
        public List<String> Other = new List<String>();
        public List<String> Image = new List<String>();
        public List<String> Pages = new List<String>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="sp"></param>
        public void ParserInit(IWebDriver driver, Scraper sp)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            driver.Navigate().GoToUrl(sp.GetMainUrl() + "search");
            driver.FindElement(By.Name("q")).SendKeys(sp.GetQuery() + Keys.Enter);
            wait.Until(WebDriver => WebDriver.FindElement(By.Id("mangaResults")).Displayed);
            ICollection<IWebElement> Mangaurl = driver.FindElements(By.XPath("//a[@class='non-hover']"));
            foreach (var mgurl in Mangaurl)
            {
                sp.SetMangaUrl(mgurl.GetAttribute("href"));
            }

        }
        public int GetUrlVolume(Scraper sp)
        {
            return (int)sp.GetMangaUrl().Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="drv"></param>
        public void parseImages(string chapter, IWebDriver drv)
        {
            drv.Navigate().GoToUrl(chapter.Substring(0, chapter.IndexOf("|")));
            int LastPage = int.Parse(drv.FindElement(By.XPath(@"//span[@class='pages-count']")).Text) - 1;
            for (int i = 0; i <= LastPage; i++)
            {
                drv.Navigate().GoToUrl(chapter.Substring(0, chapter.IndexOf("|")) + "#page=" + i);
                drv.Navigate().Refresh();
                Image.Add(drv.FindElement(By.XPath(@"//img[@id='mangaPicture']")).GetAttribute("src"));
            }

        }

        /// <summary>
        /// Функция для парсинга первой станицы списка манг (https://readmanga.live/list)
        /// </summary>
        /// <param name="drv">Объект driver интерфейса IWebDriver</param>
        /// <param name="url">Результат выполнения метода GetMainUrl класса Parser</param>
        /// <param name="sp">Объект sp класса Scraper</param>
        public void ParseFirstListManga(IWebDriver drv, Scraper sp)
        {
            /*  |Парсинг первой страницы отдельно необходим для получение ссылки на следующую страницу|  */
            ICollection<IWebElement> mangaUrl = drv.FindElements(By.XPath(@"//div[@class='desc']/h3/a")); //получение ссылок на манги
            foreach (var mangaurl in mangaUrl) //перебор коллекции ссылок
            {
                sp.SetMangaUrl(mangaurl.GetAttribute("href")); //запиcь ссылок в список
            }
            ParsePageListOnCurrentPage(drv, sp); //парсинг списка манг
        }

        /// <summary>
        /// Получение ссылок на все манги в каталоге
        /// </summary>
        /// <param name="drv">Объект driver интерфейса IWebDriver</param>
        /// <param name="sp">Объект sp класса Scraper</param>
        public void ParseAllMangaPage(IWebDriver drv, Scraper sp)
        {
            /*  |Парсинг ссылок на конкректную мангу|  */
            ParsePageListOnCurrentPage(drv, sp); //парсинг списка манг
            ICollection<IWebElement> mangaUrl = drv.FindElements(By.XPath(@"//div[@class='desc']/h3/a")); //получение ссылок на манги
            foreach (var mangaurl in mangaUrl) //перебор коллекции ссылок
            {
                sp.SetMangaUrl(mangaurl.GetAttribute("href")); //запись ссылок в список
            }
        }

        /// <summary>
        /// Получение ссылки на следующую страницу
        /// </summary>
        /// <param name="drv">Объект driver интерфейса IWebDriver</param>
        /// <param name="sp">Объект sp класса Scraper</param>
        public void ParsePageListOnCurrentPage(IWebDriver drv, Scraper sp)
        {
            /*  |Парсинг ссылки на следующую страницу с катологом манг|  */
            if(drv.FindElements(By.XPath(@"//a[@class='nextLink']")).Count > 0) //получение сслылки на следующую страницу
            {
                sp.setNextPageUrl(drv.FindElement(By.XPath(@"//a[@class='nextLink']")).GetAttribute("href")); //запись ссылки на следующую страницу в поле NextPageUrl класса Scraper
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drv"></param>
        public void getMangaContent(IWebDriver drv, Manga mng)
        {
            /* |переработать считывание манг и манхв и маньхуа| */
            if (Regex.IsMatch(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']")).Text, "Информация о манге")) //поиск только манг
            {
                mng.Title = drv.FindElement(By.XPath(@"//span[@class='name']")).Text; //название манги
                mng.BackgroundImg = drv.FindElements(By.XPath(@"//img[@class='fotorama__img']"))[0].GetAttribute("src"); //получение задней картины
                try
                {
                    ICollection<IWebElement> illustrator = drv.FindElements(By.XPath(@"//span[@class='elem_illustrator ']/a")); //получение художника
                    foreach (var _illustrator in illustrator) //перебор коллекции
                    {
                        Illustrator.Add(_illustrator.Text); //добавление в список художников
                    }
                }
                catch(Exception e) { }

                try
                {
                    ICollection<IWebElement> other = drv.FindElements(By.XPath(@"span[@class='elem_tag ']"));
                    foreach (var _other in other)
                    {
                        Other.Add(_other.Text);
                    }
                }
                catch(Exception e) { }

                try
                {
                   mng.NumberChapters = int.Parse(drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.Substring(drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.LastIndexOf(" ") + 1, drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.Length - drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.LastIndexOf(" ") - 1)); //количество глав
                }
                catch (Exception e) { }

                try
                {
                    mng.NumberVolumes = int.Parse(Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[1]")).Text, @"Томов: ", "")); //получение кол-ва глав
                }
                catch (Exception e) { }

                try
                {
                    mng.TranslateStatus = Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[2]")).Text, @"Перевод: ", ""); //получение статуса перевода
                }
                catch (Exception e) { }

                try
                {
                    mng.ReleaseYear = int.Parse(drv.FindElement(By.XPath(@"//span[@class='elem_year ']/a")).Text); //получение года выпуска
                }
                catch (Exception e) { }

                ICollection<IWebElement> genres = drv.FindElements(By.XPath(@"//span[@class='elem_genre ']/a")); //подготовка данных к заненсению в список Genres
                foreach (var gnrs in genres) //запись данных в список Genres
                {
                    mng.Genres.Add(gnrs.Text);
                }
                try
                {
                    mng.Author = drv.FindElement(By.XPath(@"//span[@class='elem_author ']/a")).Text; //получение автора
                }
                catch (Exception e) { }

                try
                {
                    mng.Description = drv.FindElement(By.XPath(@"//div[@class='manga-description']")).Text; //получение описания
                }
                catch (Exception e) { }

                ICollection<IWebElement> chapter = drv.FindElements(By.XPath(@"//a[@class='cp-l']")); //получение ссылок на главы
                foreach (var ch in chapter)
                {
                    mng.Chapters.Add(ch.GetAttribute("href") + "|" + ch.Text);
                }

            }
        }
    }
}
