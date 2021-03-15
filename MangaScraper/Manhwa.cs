using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MangaScraper
{
    class Manhwa
    {
        private string Title;
        private byte VolumeNumber;
        private ushort ChapterNumber;
        private string BackgroundImg;
        private List<String> OtherTitles = new List<String>();
        private string TranslateStatus;
        private List<String> Genres = new List<String>();
        private string Category;
        private List<String> Screenweiters = new List<String>();
        private List<String> Painters = new List<String>();
        private ushort ReleaseYear;
        private string Description;
        private List<String> Other = new List<String>();
        private List<String> Magazines = new List<String>();
        private List<String> Tranlators = new List<String>();
        private List<String> Publishing = new List<String>();
        private List<String> Chapters = new List<String>();
        public void getManhwaContent(IWebDriver drv, Manhwa mnh)
        {
            /* |переработать считывание манг и манхв и маньхуа| */
            mnh.Title = drv.FindElement(By.XPath(@"//span[@class='name']")).Text; //название манги
            mnh.BackgroundImg = drv.FindElements(By.XPath(@"//img[@class='fotorama__img']"))[0].GetAttribute("src"); //получение задней картины
            try
            { 
                mnh.ChapterNumber =ushort.Parse(Regex.Replace(Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-actions col-sm-7']/h4/a")).Text, @"Читать \d+ - ", ""), @" новое",""));

            }
            catch (Exception e) { }

            try
            {
                mnh.VolumeNumber=byte.Parse(Regex.Replace(Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[1]")).Text, @"Томов: ", ""), @", выпуск продолжается", "")); //получение кол-ва глав
            }
            catch (Exception e) { }

            try
            {
                mnh.TranslateStatus = Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[2]")).Text, @"Перевод: ", ""); //получение статуса перевода
            }
            catch (Exception e) { }

            try
            {
                mnh.ReleaseYear = ushort.Parse(drv.FindElement(By.XPath(@"//span[@class='elem_year ']/a")).Text); //получение года выпуска
            }
            catch (Exception e) { }

            ICollection<IWebElement> genres = drv.FindElements(By.XPath(@"//span[@class='elem_genre ']/a")); //подготовка данных к заненсению в список Genres
            ICollection<IWebElement> hidden_genres = drv.FindElements(By.XPath(@"//p[@class='elementList']/span[@class='elem_genre hide']")); //подготовка скрытых данных к заненсению в список Genres
            foreach (var gnrs in genres) //запись данных в список Genres
            {
                mnh.Genres.Add(gnrs.Text);
            }
            foreach (var hdn_genres in hidden_genres) //запись данных в список Genres
            {
                mnh.Genres.Add(hdn_genres.Text);
            }
            {
                //mnh.Painter = drv.FindElement(By.XPath(@"//span[@class='elem_author ']/a")).Text; //получение автора
            }

            try
            {
                mnh.Category = drv.FindElement(By.XPath(@"//span[@class='elem_category ']")).Text;
            }
            catch(Exception e) { }
            try
            {
                mnh.Description = drv.FindElement(By.XPath(@"//div[@class='manga-description']")).Text; //получение описания
            }
            catch (Exception e) { }

            try
            {
                ICollection<IWebElement> magazines = drv.FindElements(By.XPath(@"//span[@class='elem_magazine ']/a"));
                foreach (var _magines in magazines)
                {
                    mnh.Magazines.Add(_magines.Text);
                }
            }
            catch(Exception e){ }

            ICollection<IWebElement> chapter = drv.FindElements(By.XPath(@"//a[@class='cp-l']")); //получение ссылок на главы
            foreach (var ch in chapter)
            {
                mnh.Chapters.Add(ch.GetAttribute("href") + "|" + ch.Text);
            }

            try
            {
                ICollection<IWebElement> other = drv.FindElements(By.XPath(@"//span[@class='elem_tag ']/a"));
                foreach (var _other in other)
                {
                    mnh.Other.Add(_other.Text);
                }
            }
            catch(Exception e) { }

            try
            {
                mnh.OtherTitles.Add(drv.FindElement(By.XPath(@"//span[@class='eng-name']")).Text);
                mnh.OtherTitles.Add(drv.FindElement(By.XPath(@"//span[@class='original-name']")).Text);
            }
            catch(Exception e) { }

            try
            {
                ICollection<IWebElement> screenwriters = drv.FindElements(By.XPath(@"//span[@class='elem_screenwriter ']"));
                foreach (var _screenwriters in screenwriters)
                {
                    mnh.Screenweiters.Add(Regex.Replace(_screenwriters.Text, ", ", "")); 
                }
            }
            catch(Exception e) { }

            try
            {
                ICollection<IWebElement> painters = drv.FindElements(By.XPath(@"//span[@class='elem_illustrator ']"));
                foreach (var _painters in painters)
                {
                    mnh.Painters.Add(Regex.Replace(_painters.Text, ", ", ""));
                }
            }
            catch(Exception e) { }

            try
            {
                ICollection<IWebElement> translators = drv.FindElements(By.XPath(@"//span[@class='elem_translator ']"));
                foreach (var _translators in translators)
                {
                    mnh.Tranlators.Add(Regex.Replace(_translators.Text, ", ", ""));
                }
            }
            catch(Exception e) { }

            try
            {
                ICollection<IWebElement> publishing = drv.FindElements(By.XPath(@"//span[@class='elem_publisher ']"));
                foreach (var _publishing in publishing)
                {
                    mnh.Publishing.Add(Regex.Replace(_publishing.Text, ", ", ""));
                }
            }
            catch(Exception e) { }
        }
    }

}
