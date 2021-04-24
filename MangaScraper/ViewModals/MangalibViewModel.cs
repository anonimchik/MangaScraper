using System;
using System.Collections.Generic;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MangaScraper.ViewModals
{
    class MangalibViewModel:ReadMangaModel
    {
        public void mm()
        {
            var options = new EdgeOptions(); //задание опций для Edge
            options.UseChromium = true; //включение хромиума
            Url = "https://mangalib.me/manga-list";
            var pages = new List<String>();
            using (IWebDriver driver = new EdgeDriver(options)) //основная работа парсера
            {

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                BaseModel bm = new BaseModel();
                /*   |Получение ссылок на страницы|   */
                for (int i = 1; i < 2; i++)
                {
                    driver.Navigate().GoToUrl(Url+"?page="+i.ToString()); //переход на сайт 
                    if (driver.FindElements(By.XPath(@"//div[@class=' page__wrapper page__wrapper_left  paper manga-search']")).Count > 0) pages.Add(Url + "?page=" + i.ToString());
                }
                var mg = new List<String>();
                
                getTitleUrl(driver, pages, mg); //получение ссылок на тайтлы
                bm.OtherTitles = new List<String>();
                bm.Painters = new List<String>();
                bm.Publishings = new List<String>();
                bm.Genres = new List<String>();
                bm.Translators = new List<String>();
                getCurrentTitleInformation(driver, mg, bm);
            }
        }

        /// <summary>
        /// Получение ссылок на тайтлы
        /// </summary>
        /// <param name="driver">Объект класса IWebdriver</param>
        /// <param name="pages">Список страниц</param>
        /// <param name="mg">Получаемый список ссылок на тайтлы</param>
        public void getTitleUrl(IWebDriver driver, List<String> pages, List<String> mg)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                driver.Navigate().GoToUrl(pages[i]); //переход по страницам (https://mangalib.me/manga-list?page=2...)
                ICollection<IWebElement> titles = driver.FindElements(By.XPath(@"//div[@class='media-card-wrap']/a"));
                foreach (var _title in titles) //запись ссылок на тайтлы в список
                {
                    mg.Add(_title.GetAttribute("href"));
                }
            }
            
        }
        /// <summary>
        /// Получение данных по конкретному тайтлу
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="mg"></param>
        /// <param name="bm"></param>
        public void getCurrentTitleInformation(IWebDriver driver, List<String> mg, BaseModel bm)
        {
            for (int i = 0; i < mg.Count; i++)
            {
                driver.Navigate().GoToUrl(mg[i]);
                var img=driver.FindElement(By.XPath(@"//div[@class='media-sidebar__cover paper']/img"));
                bm.BackgroundImg = img.GetAttribute("src");
                bm.Title=driver.FindElement(By.XPath(@"//div[@class='media-name__main']")).Text;
                bm.EngTitle=driver.FindElement(By.XPath(@"//div[@class='media-name__alt']")).Text;
                bm.Category = driver.FindElement(By.XPath(@"//a[contains(@href,'types')]")).FindElement(By.XPath(@"//div[@class='media-info-list__value']")).Text;
                bm.ChapterNumber = ushort.Parse(driver.FindElement(By.XPath(@"//div[text()='Загружено глав']/parent::div/div[@class='media-info-list__value text-capitalize']")).Text);
                bm.ReleaseYear = ushort.Parse(driver.FindElement(By.XPath(@"//a[contains(@href,'year')]/div[@class='media-info-list__value']")).Text);
                bm.TranslateStatus = driver.FindElement(By.XPath(@"//a[contains(@href,'status')]/div[@class='media-info-list__value text-capitalize']")).Text;
                bm.TitleStatus = driver.FindElement(By.XPath(@"//a[contains(@href,'manga_status')]/div[@class='media-info-list__value']")).Text;
                bm.Author = driver.FindElement(By.XPath(@"//div[text()='Автор']/parent::div/div[@class='media-info-list__value']")).Text;
                
                ICollection<IWebElement> painters = driver.FindElements(By.XPath(@"//div[text()='Художник']/parent::div/div[@class='media-info-list__value']/a"));
                foreach (var _painter in painters)
                {
                    bm.Painters.Add(_painter.Text);
                }
                ICollection<IWebElement> publishings = driver.FindElements(By.XPath(@"//div[text()='Издательство']/parent::div/div[@class='media-info-list__value']/a"));
                foreach (var _publishing in publishings)
                {
                    bm.Publishings.Add(_publishing.Text);
                }
                bm.Description = driver.FindElement(By.XPath(@"//div[@class='media-description__text']")).Text;
                if (driver.FindElements(By.XPath(@"//div[@class='media-tag-item media-tag-item_more']/i[@class='fa fa-ellipsis-h']")).Count > 0) //Проверка наличия блока "Показать все жанры" 
                {
                    driver.FindElement(By.XPath(@"//div[@class='media-tag-item media-tag-item_more']/i[@class='fa fa-ellipsis-h']")).Click();
                }
                if (driver.FindElements(By.XPath(@"//div[@class='media-info-list__expand']")).Count > 0) //Проверка наличия блока "Показать все альтернативные названия"
                {
                    driver.FindElement(By.XPath(@"//div[@class='media-info-list__expand']")).Click();
                }
                ICollection<IWebElement> otherTitles = driver.FindElements(By.XPath(@"//div[@class='media-info-list__expand']/parent::div/div[@class='media-info-list__value']/div"));
                foreach (var _otherTitles in otherTitles)
                {
                    bm.OtherTitles.Add(_otherTitles.Text);
                }
                ICollection<IWebElement> genres = driver.FindElements(By.XPath(@"//div[@class='media-tags']/a"));
                foreach (var _genre in genres)
                {
                    bm.Genres.Add(_genre.Text);
                }
                ICollection<IWebElement> translators = driver.FindElements(By.XPath(@"//a[@class='team-list-item team-list-item_xs']/div[@class='team-list-item__name']"));
                foreach (var _translator in translators)
                {
                    bm.Translators.Add(_translator.Text);
                }

                driver.Navigate().GoToUrl(mg[i] + "?section=chapters");
                driver.FindElements(By.XPath(@"//div[@class='media-chapter__name text-truncate']/a and"));
                
            }
        }
        
    }
}
