using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace MangaScraper.ViewModals
{
    class Manga24ViewModel : BaseModel
    {
        public void Manga24Main()
        {
            SourceUrl = "https://manga24.ru/onepiece";
            var option = new EdgeOptions();
            option.UseChromium = true;
            option.PageLoadStrategy = PageLoadStrategy.Eager;
            using (IWebDriver drv = new EdgeDriver(option))
            {
                List<string> catalog_pages = new List<string>();
                List<string> title_page = new List<string>();
                BaseModel bm = new BaseModel();
                drv.Navigate().GoToUrl(SourceUrl);
                get_title_info(drv, bm);
                get_chapter_info(drv, bm);
                get_images_from_chapters(drv, bm);
            }
        }

        /// <summary>
        /// Получение инфорации о тайтле
        /// </summary>
        /// <param name="drv">Объект класса EdgeDriver</param>
        /// <param name="bm">Объект класса BaseModel</param>
        public void get_title_info(IWebDriver drv, BaseModel bm)
        {
            bm.Title = drv.FindElement(By.XPath("//h1[@class='manga-data__title']")).Text;
            bm.EngTitle = drv.FindElement(By.XPath("//h2[@class='manga-data__subtitle']")).Text;
            ICollection<IWebElement> background_images = drv.FindElements(By.XPath("//div[@class='owl-stage']/div/img"));
            foreach (var _background_image in background_images)
            {
                if (!bm.BackgroundImages.Contains(_background_image.GetAttribute("src"))) bm.BackgroundImages.Add(_background_image.GetAttribute("src"));
            }
            ICollection<IWebElement> genres = drv.FindElements(By.XPath("//div[@class='manga-data__genre']/a"));
            foreach (var _genre in genres)
            {
                bm.Genres.Add(_genre.Text);
            }
            bm.Category = drv.FindElement(By.XPath("//p[@class='manga-data__type']/span")).Text;
            ICollection<IWebElement> authors = drv.FindElements(By.XPath("//p[@class='manga-data__author']/span"));
            foreach (var _author in authors)
            {
                bm.Authors.Add(_author.Text);
            }
            string[] translators = drv.FindElement(By.XPath("//p[@class='manga-data__translate']/span")).Text.Split(",");
            foreach (var _translator in translators)
            {
                bm.Translators.Add(_translator);
            }
            bm.TranslateStatus = drv.FindElement(By.XPath("//p[@class='manga-data__translate-status']/span")).Text;
            drv.FindElement(By.XPath("//span[@class='read-more']")).Click();
            bm.Description = Regex.Replace(drv.FindElement(By.XPath("//p[@class='read-more-block']")).Text, @"(\s{2,}|скрыть)", "");
        }

        /// <summary>
        /// Получение ссылки на главу
        /// </summary>
        /// <param name="drv"></param>
        /// <param name="bm"></param>
        public void get_chapter_info(IWebDriver drv, BaseModel bm)
        {
            IWebElement[] chapter_number = new IWebElement[drv.FindElements(By.XPath("//span[@class='chapter-num']")).Count];
            drv.FindElements(By.XPath("//span[@class='chapter-num']")).CopyTo(chapter_number, 0);
            IWebElement[] chapter_name = new IWebElement[drv.FindElements(By.XPath("//span[@class='chapter-name']")).Count];
            drv.FindElements(By.XPath("//span[@class='chapter-name']")).CopyTo(chapter_name, 0);
            IWebElement[] translator = new IWebElement[drv.FindElements(By.XPath("//td[@class='translator']")).Count];
            drv.FindElements(By.XPath("//td[@class='translator']")).CopyTo(translator, 0);
            IWebElement[] link = new IWebElement[drv.FindElements(By.XPath("//a[@class='chapter-link']")).Count];
            drv.FindElements(By.XPath("//a[@class='chapter-link']")).CopyTo(link, 0);
            for(int i = 0; i < chapter_number.Length; i++) //доделать скролинг таблицы с главами
            {
                bm.Chapters.Add(link[i].GetAttribute("href") + "|" + chapter_number[i].Text + "|" + chapter_name[i].Text + "|" + translator[i].Text);
            }

        }

        public void get_images_from_chapters(IWebDriver drv, BaseModel bm)
        {
            List<string> sub_images = new List<string>();
            for (int i = 0; i < bm.Chapters.Count; i++) //проход по главам
            {
                drv.Navigate().GoToUrl(bm.Chapters[i].Split("|")[0]);
                sub_images = new List<string>();
                IWebElement[] sub_imgs = new IWebElement[drv.FindElements(By.XPath("//div[@id='preload']/img")).Count];
                if(!sub_images.Contains(sub_imgs[0].GetAttribute("src")))sub_images.Add(sub_imgs[0].GetAttribute("src"));
            }
            bm.Images.Add(new List<string>(sub_images));
        }

        public void download_images_to_local_storage()
        {

        }
    }
}
