using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MangaScraper
{
    class Manga:BaseContent
    {
        public void parseMangaImageUrls(IWebDriver drv, Manga mng)
        {
            List<String> subImages = new List<String>();
            for (int i = 0; i < 1; i++)
            {
                drv.Navigate().GoToUrl(Chapters[i].Substring(0, Chapters[i].IndexOf("|")));
                int LastPage = int.Parse(drv.FindElement(By.XPath(@"//span[@class='pages-count']")).Text) - 1;
                for (int j = 0; j <= LastPage; j++)
                {
                    drv.Navigate().GoToUrl(Chapters[i].Substring(0, Chapters[i].IndexOf("|")) + "#page=" + j);
                    drv.Navigate().Refresh();
                    drv.FindElement(By.XPath(@"//img[@id='mangaPicture']")).GetAttribute("src") ;
                    subImages.Add(drv.FindElement(By.XPath(@"//img[@id='mangaPicture']")).GetAttribute("src"));                   
                }
                mng.Images.Add(new List<string>(subImages));
            }

        }

        /// <summary>
        /// Скачивание картинок
        /// </summary>
        /// <param name="MangaList"></param>
        public void downloadImages(List<Manga> MangaList)
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

        /// <summary>
        /// Парсинг информации о манге
        /// </summary>
        /// <param name="drv"></param>
        public void getMangaContent(IWebDriver drv, Manga mng)
        {
            /* |переработать считывание манг и манхв и маньхуа| */
            mng.Title = drv.FindElement(By.XPath(@"//span[@class='name']")).Text; //название манги
            mng.OtherTitles.Add(drv.FindElement(By.XPath(@"//span[@class='eng-name']")).Text); //Английское название 
            mng.OtherTitles.Add(drv.FindElement(By.XPath(@"//span[@class='original-name']")).Text); //Оригинальное название
            mng.BackgroundImg = drv.FindElements(By.XPath(@"//img[@class='fotorama__img']"))[0].GetAttribute("src"); //получение задней картины
            try
            {
                mng.ChapterNumber = ushort.Parse(drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.Substring(drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.LastIndexOf(" ") + 1, drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.Length - drv.FindElement(By.XPath(@"//div[@class='flex-row']/div[2]/h4/a")).Text.LastIndexOf(" ") - 1)); //количество глав
            }
            
            catch (Exception e) { }

            try
            {
                mng.VolumeNumber = byte.Parse(Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[1]")).Text, @"Томов: ", "")); //получение кол-ва глав
            }
            catch (Exception e) { }

            try
            {
                mng.TranslateStatus = Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[2]")).Text, @"Перевод: ", ""); //получение статуса перевода
            }
            catch (Exception e) { }

            try
            {
                mng.ReleaseYear = ushort.Parse(drv.FindElement(By.XPath(@"//span[@class='elem_year ']/a")).Text); //получение года выпуска
            }
            catch (Exception e) { }

            ICollection<IWebElement> genres = drv.FindElements(By.XPath(@"//span[@class='elem_genre ']/a")); //подготовка данных к заненсению в список Genres
            ICollection<IWebElement> hidden_genres =  drv.FindElements(By.XPath(@"//p[@class='elementList']/span[@class='elem_genre hide']")); //подготовка скрытых данных к занесению в список Genres
            foreach (var gnrs in genres) //запись данных в список Genres
            {
                mng.Genres.Add(gnrs.Text);
            }
            foreach(var hdn_genres in hidden_genres) //запись данных в список Genres
            {
                mng.Genres.Add(Regex.Replace(hdn_genres.Text, ", ", ""));
            }
            try
            {
                mng.Painter = drv.FindElement(By.XPath(@"//span[@class='elem_author ']/a")).Text; //получение автора
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
