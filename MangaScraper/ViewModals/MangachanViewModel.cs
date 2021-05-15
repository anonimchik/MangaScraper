using System.Collections.Generic;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace MangaScraper.ViewModals
{
    class MangachanViewModel:BaseModel
    {
        public void MangachanMain()
        {
            BaseModel bm = new BaseModel();
            bm.SourceUrl = "https://manga-chan.me/";
            var options = new EdgeOptions(); //задание опций для Edge
            options.UseChromium = true;
            using(IWebDriver drv=new EdgeDriver(options))
            {
                getPagesList(drv, bm);
                getTitlePageUrl(drv, bm);
                getTitleInfo(drv, bm);
                getChapterImages(drv, bm);
            }
        }
        /// <summary>
        /// Получение списка страниц
        /// </summary>
        /// <param name="drv">EdgeDriver</param>
        /// <param name="bm">BaseModel</param>
        public void getPagesList(IWebDriver drv, BaseModel bm)
        {
            drv.Navigate().GoToUrl(bm.SourceUrl+"catalog");
            ICollection<IWebElement> pages = drv.FindElements(By.XPath("//a[contains(@href,'?offset=')]"));
            foreach (var _pages in pages)
            {
                if(!bm.CatalogPages.Contains(_pages.GetAttribute("href")))
                    bm.CatalogPages.Add(_pages.GetAttribute("href"));
                break;
            }
        }
        /// <summary>
        /// Получение списка на страницы тайтлов
        /// </summary>
        /// <param name="drv"></param>
        /// <param name="bm"></param>
        public void getTitlePageUrl(IWebDriver drv, BaseModel bm)
        {
            for(int i=0; i<bm.CatalogPages.Count; i++)
            {
                drv.Navigate().GoToUrl(bm.CatalogPages[i]);
                ICollection<IWebElement> titlePages = drv.FindElements(By.XPath("//a[@class='title_link']"));
                foreach (var _titlePage in titlePages)
                {
                    bm.TitlePages.Add(_titlePage.GetAttribute("href"));
                }
                break;
            }
        }
        /// <summary>
        /// Получение информации о тайтле
        /// </summary>
        public void getTitleInfo(IWebDriver drv, BaseModel bm)
        {
            for(int i=0; i<bm.TitlePages.Count; i++)
            {
                drv.Navigate().GoToUrl(bm.TitlePages[i]);
                bm.Category = drv.FindElement(By.XPath("//a[contains(@href,'/type')]")).Text;
                bm.Title = drv.FindElement(By.XPath("//a[@class='title_top_a']")).Text;
                bm.TitleStatus = drv.FindElement(By.XPath("//td[contains(text(), 'Статус (Томов)')]/parent::tr/td[2]")).Text.Substring(drv.FindElement(By.XPath("//td[contains(text(), 'Статус (Томов)')]/parent::tr/td[2]")).Text.IndexOf(", ") + 2, drv.FindElement(By.XPath("//td[contains(text(), 'Статус (Томов)')]/parent::tr/td[2]")).Text.Length - drv.FindElement(By.XPath("//td[contains(text(), 'Статус (Томов)')]/parent::tr/td[2]")).Text.IndexOf(", ") - 2);
                bm.VolumeNumber = byte.Parse(drv.FindElement(By.XPath("//td[contains(text(), 'Статус (Томов)')]/parent::tr/td[2]")).Text.Substring(0, drv.FindElement(By.XPath("//td[contains(text(), 'Статус (Томов)')]/parent::tr/td[2]")).Text.IndexOf(" том")));
                bm.TranslateStatus = drv.FindElement(By.XPath("//td[contains(text(), 'Загружено')]/parent::tr/td[2]")).Text.Substring(drv.FindElement(By.XPath("//td[contains(text(), 'Загружено')]/parent::tr/td[2]")).Text.IndexOf(", ") + 2, drv.FindElement(By.XPath("//td[contains(text(), 'Загружено')]/parent::tr/td[2]")).Text.Length - drv.FindElement(By.XPath("//td[contains(text(), 'Загружено')]/parent::tr/td[2]")).Text.IndexOf(", ") - 2);
                bm.ChapterNumber = ushort.Parse(drv.FindElement(By.XPath("//td[contains(text(), 'Загружено')]/parent::tr/td[2]")).Text.Substring(0, drv.FindElement(By.XPath("//td[contains(text(), 'Загружено')]/parent::tr/td[2]")).Text.IndexOf(" глав")));
                ICollection<IWebElement> genres = drv.FindElements(By.XPath("//a[contains(@href, 'https://manga-chan.me/tags')]"));
                foreach (var _genre in genres)
                {
                    bm.Genres.Add(_genre.Text);
                }
                ICollection<IWebElement> translators = drv.FindElements(By.XPath("//a[contains(@href, '/translation/')]"));
                foreach (var _translator in translators)
                {
                    if(_translator.Text!="")
                        bm.Translators.Add(_translator.Text);
                }
                string[] titles = Regex.Split(drv.FindElement(By.XPath("//td[contains(text(), 'Другие названия')]/parent::tr/td[2]/h2")).Text, @" / |;");
                foreach (var _title in titles)
                {
                    bm.OtherTitles.Add(_title);
                }
                ICollection<IWebElement> authors = drv.FindElements(By.XPath("//a[contains(@href, '/mangaka/')]"));
                foreach (var _author in authors)
                {
                    bm.Authors.Add(_author.Text);
                }
                bm.Description = drv.FindElement(By.XPath("//div[@id='description']")).Text;
                ICollection<IWebElement> chapters = drv.FindElements(By.XPath("//div[@class='manga2']/a"));
                foreach (var _chapter in chapters)
                {
                    bm.Chapters.Add(_chapter.GetAttribute("href"));
                }
                break;
            }
            
        }

        public void getChapterImages(IWebDriver drv, BaseModel bm)
        {
            for(int i=0; i<bm.Chapters.Count; i++)
            {
                drv.Navigate().GoToUrl(bm.Chapters[i]);
                IList<IWebElement> page = drv.FindElements(By.XPath("//select[@class='drop']/option"));
                int count = int.Parse(page[page.Count-1].Text.Substring(page[page.Count-1].Text.IndexOf(". ") + 1, page[page.Count-1].Text.Length - page[page.Count-1].Text.IndexOf(". ") - 1)); //кол-во страниц конкретной главы
                for(int j=0; j<count; j++)
                {
                    drv.Navigate().GoToUrl(bm.Chapters[i] + "?page=" + j);
                    drv.FindElement(By.XPath("//a[contains(@href, '#page=')]/img"));
                }
                break;
            }
        }
    }
}
