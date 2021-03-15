using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MangaScraper
{
    class Manhua
    {
        private string Title;
        private byte VolumeNumber;
        private ushort ChapterNumber;
        private string BackgroundImg;
        private List<String> OtherTitles = new List<String>();
        private string ManhuaStatus;
        private string TranslateStatus;
        private string Description;
        private List<String> Genres = new List<String>();
        private string Category;
        private List<String> Screenweiters = new List<String>();
        private List<String> Painters = new List<String>();
        private ushort ReleaseYear;
        private List<String> Other = new List<String>();
        private List<String> Publishings = new List<String>();
        private List<String> Tranlators = new List<String>();
        private List<String> Chapters = new List<String>();
        public void getManhuaContent(IWebDriver drv, Manhua mna)
        {
            /* |переработать считывание манг и манхв и маньхуа| */
            mna.Title = drv.FindElement(By.XPath(@"//span[@class='name']")).Text; //название манги
            mna.BackgroundImg = drv.FindElements(By.XPath(@"//img[@class='fotorama__img']"))[0].GetAttribute("src"); //получение задней картины
            try
            {
                mna.ChapterNumber = ushort.Parse(drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.Substring(drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.LastIndexOf(" ") + 1, drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.Length - drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.LastIndexOf(" ") - 1)); //количество глав
            }
            catch (Exception e) { }

            try
            {
                mna.VolumeNumber = byte.Parse(Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[1]")).Text, @"Томов: ", "")); //получение кол-ва глав
            }
            catch (Exception e) { }

            try
            {
                mna.TranslateStatus = Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[2]")).Text, @"Перевод: ", ""); //получение статуса перевода
            }
            catch (Exception e) { }

            try
            {
                mna.ReleaseYear = ushort.Parse(drv.FindElement(By.XPath(@"//span[@class='elem_year ']/a")).Text); //получение года выпуска
            }
            catch (Exception e) { }

            ICollection<IWebElement> genres = drv.FindElements(By.XPath(@"//span[@class='elem_genre ']/a")); //подготовка данных к заненсению в список Genres
            ICollection<IWebElement> hidden_genres = drv.FindElements(By.XPath(@"//p[@class='elementList']/span[@class='elem_genre hide']")); //подготовка скрытых данных к заненсению в список Genres

            foreach (var gnrs in genres) //запись данных в список Genres
            {
                mna.Genres.Add(gnrs.Text);
            }
            foreach (var hdn_genres in hidden_genres) //запись данных в список Genres
            {
                mna.Genres.Add(hdn_genres.Text);
            }
            try
            {
                //mnh.Painter = drv.FindElement(By.XPath(@"//span[@class='elem_author ']/a")).Text; //получение автора
            }
            catch (Exception e) { }

            try
            {
                mna.Description = drv.FindElement(By.XPath(@"//div[@class='manga-description']")).Text; //получение описания
            }
            catch (Exception e) { }

            ICollection<IWebElement> chapter = drv.FindElements(By.XPath(@"//a[@class='cp-l']")); //получение ссылок на главы
            foreach (var ch in chapter)
            {
                mna.Chapters.Add(ch.GetAttribute("href") + "|" + ch.Text);
            }

        }
    }
}
