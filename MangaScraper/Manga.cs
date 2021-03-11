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

        public List<String> Image = new List<string>();
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
        public void ParseMainInfo(string url, IWebDriver drv, Scraper sp)
        {
            drv.Navigate().GoToUrl(url); //переход на страницу с конкретной мангой
            if (Regex.IsMatch(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']")).Text, "Информация о манге")) //поиск только манг
            {
                Title = drv.FindElement(By.XPath(@"//span[@class='name']")).Text; //название манги
                BackgroundImg = drv.FindElements(By.XPath(@"//img[@class='fotorama__img']"))[0].GetAttribute("src"); //получение задней картины
                try
                {
                    NumberChapters = int.Parse(drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.Substring(drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.LastIndexOf(" ") + 1, drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.Length - drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.LastIndexOf(" ") - 1)); //количество глав
                }
                catch (Exception e) { }

                try
                {
                    NumberVolumes = int.Parse(Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[1]")).Text, @"Томов: ", "")); //получение кол-ва глав
                }
                catch (Exception e) { }

                try
                {
                    TranslateStatus = Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[2]")).Text, @"Перевод: ", ""); //получение статуса перевода
                }
                catch (Exception e) { }

                try
                {
                    ReleaseYear = int.Parse(drv.FindElement(By.XPath(@"//span[@class='elem_year ']/a")).Text); //получение года выпуска
                }
                catch (Exception e) { }

                ICollection<IWebElement> genres = drv.FindElements(By.XPath(@"//span[@class='elem_genre ']/a")); //подготовка данных к заненсению в список Genres
                foreach (var gnrs in genres) //запись данных в список Genres
                {
                    Genres.Add(gnrs.Text);
                }
                try
                {
                    Author = drv.FindElement(By.XPath(@"//span[@class='elem_author ']/a")).Text; //получение автора
                }
                catch (Exception e) { }

                try
                {
                    Description = drv.FindElement(By.XPath(@"//div[@class='manga-description']")).Text; //получение описания
                }
                catch (Exception e) { }

                ICollection<IWebElement> chapter = drv.FindElements(By.XPath(@"//a[@class='cp-l']")); //получение ссылок на главы
                foreach (var ch in chapter)
                {
                    Chapters.Add(ch.GetAttribute("href") + "|" + ch.Text);
                }

            }

        }

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
        /// Функция для парсинга всего списка манга на странице https://readmanga.live/list
        /// </summary>
        /// <param name="drv">Объект driver интерфейса IWebDriver</param>
        /// <param name="url">Результат выполнения метода GetMainUrl класса Parser</param>
        /// <
        public void ParseFullListManga(IWebDriver drv, string url)
        {
            drv.Navigate().GoToUrl(url); //переход на сайт 

            /*  |обработка при наличии кнопки далее|  */
            if (drv.FindElements(By.XPath(@"//a[@class='nextLink']")).Count > 0)
            {
                ICollection<IWebElement> pages = drv.FindElements(By.XPath(@"//span[@class='step']"));
            }

            /*  |обработка при наличии кнопки назад|  */
            if (drv.FindElements(By.XPath(@"//a[@class='prevLink']")).Count > 0)
            {

            }

            /*  |обработка при наличии кнопки далее и назад|  */
            if (drv.FindElements(By.XPath(@"//a[@class='nextLink']")).Count > 0 && drv.FindElements(By.XPath(@"//a[@class='prevLink']")).Count > 0)
            {

            }
        }
    }
}
