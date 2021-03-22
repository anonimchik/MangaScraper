using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium.Support.UI;
using System.Windows.Controls;

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
        
    }
}
