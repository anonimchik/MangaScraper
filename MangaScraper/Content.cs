using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MangaScraper
{
    class Content:BaseModel
    {
        #region getting image urls
        /// <summary>
        /// Парсинг изображений
        /// </summary>
        /// <param name="drv"></param>
        /// <param name="mng"></param>
        public void parseMangaImageUrls(IWebDriver drv, Content mng)
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
        #endregion

        #region download images from chapters
        /// <summary>
        /// Скачивание картинок
        /// </summary>
        /// <param name="MangaList"></param>
        public void downloadImages(List<Content> MangaList)
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

        public void getMangaContent(IWebDriver drv, Content mng)
        {
            Scraper srp = new Scraper();
            srp.setFailCounter();
            mng.Category = "Манга";
            try
            {  
                /*   Получение названия манги   */
                mng.Title = drv.FindElement(By.XPath(@"//span[@class='name']")).Text; 
            }
            catch (Exception e) 
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message+e.StackTrace);
            }

            try
            {
                /*   Получение английского названия   */
                mng.OtherTitles.Add(drv.FindElement(By.XPath(@"//span[@class='eng-name']")).Text); 
            }
            catch (Exception e) 
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
            }

            try
            {
                /*   Получение оригинального названия   */
                mng.OtherTitles.Add(drv.FindElement(By.XPath(@"//span[@class='original-name']")).Text); 
            }
            catch (Exception e) 
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
            }

            try
            {
                /*   получение задней картины   */
                mng.BackgroundImg = drv.FindElements(By.XPath(@"//img[@class='fotorama__img']"))[0].GetAttribute("src"); 
            }
            catch (Exception e) 
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
            }

            try
            {
                /*   Получение кол-ва томов   */
                var str = Regex.Replace(drv.FindElement(By.XPath(@"//tbody/tr[1]/td[1]/a")).Text, @"[^\d|^-]", "");
                mng.VolumeNumber = byte.Parse(str.Substring(0, str.IndexOf("-")));
            }
            catch (Exception e) 
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
            }

            try
            {
                /*   получение статуса перевода   */
                mng.TranslateStatus = Regex.Replace(drv.FindElement(By.XPath(@"//div[@class='subject-meta col-sm-7']/p[2]")).Text, @"Перевод: ", ""); 
            }
            catch (Exception e) 
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
            }

            try
            {
                /*   получение года выпуска   */
                mng.ReleaseYear = ushort.Parse(drv.FindElement(By.XPath(@"//span[@class='elem_year ']/a")).Text); 
            }
            catch (Exception e) 
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
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
            catch(Exception e)
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
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
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
            }

            try
            {
                /*   Получение художника   */
                mng.Author = drv.FindElement(By.XPath(@"//span[@class='elem_author ']/a")).Text;
            }
            catch (Exception e)
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
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
            catch(Exception e) 
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
            }

            try
            {
                /*   Получение описания   */
                mng.Description = drv.FindElement(By.XPath(@"//div[@class='manga-description']")).Text; 
            }
            catch (Exception e) 
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
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
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
            }

            try
            {
                /*   Получение количества глав   */
                mng.ChapterNumber = Convert.ToUInt16(mng.Chapters.Count); 
            }

            catch (Exception e)
            {
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
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
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
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
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
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
                srp.setFailCounter(+1);
                srp.setErrorMessage(e.Message + e.StackTrace);
            }

        }

        #endregion
    }
}
