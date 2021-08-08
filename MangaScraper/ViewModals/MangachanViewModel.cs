using System.Collections.Generic;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace MangaScraper.ViewModals
{
    class MangachanViewModel : BaseModel
    {
        public void MangachanMain()
        {
            SourceUrl = "https://manga-chan.me/";
            var options = new EdgeOptions(); //задание опций для Edge
            options.UseChromium = true;
            using (IWebDriver drv = new EdgeDriver(options))
            {
                List<String> CatalogPages = new List<String>();
                List<String> TitlePages = new List<String>();
                //uploadImagesToServer();
                getPagesList(drv, CatalogPages, SourceUrl);
                getTitlePageUrl(drv, CatalogPages, TitlePages);
                BaseModel bm = new BaseModel();
                getTitleInfo(drv, TitlePages, bm);

            }
        }
        /// <summary>
        /// Получение списка страниц
        /// </summary>
        /// <param name="drv">EdgeDriver</param>
        /// <param name="bm">BaseModel</param>
        public void getPagesList(IWebDriver drv, List<String> CatalogPages, string SourceUrl)
        {
            drv.Navigate().GoToUrl(SourceUrl + "catalog");
            ICollection<IWebElement> pages = drv.FindElements(By.XPath("//a[contains(@href,'?offset=')]"));
            CatalogPages.Add("https://manga-chan.me/catalog?offset=0");
            foreach (var _pages in pages)
            {
                if (!CatalogPages.Contains(_pages.GetAttribute("href")))
                    CatalogPages.Add(_pages.GetAttribute("href"));
                break;
            }
        }
        /// <summary>
        /// Получение списка на страницы тайтлов
        /// </summary>
        /// <param name="drv"></param>
        /// <param name="bm"></param>
        public List<String> getTitlePageUrl(IWebDriver drv, List<String> CatalogPages, List<String> TitlePages)
        {
            for (int i = 0; i < CatalogPages.Count; i++)
            {
                drv.Navigate().GoToUrl(CatalogPages[i]);
                ICollection<IWebElement> titlePages = drv.FindElements(By.XPath("//a[@class='title_link']"));
                foreach (var _titlePage in titlePages)
                {
                   TitlePages.Add(_titlePage.GetAttribute("href"));
                }
                break;
            }
            return TitlePages;
        }
        /// <summary>
        /// Получение информации о тайтле
        /// </summary>
        public void getTitleInfo(IWebDriver drv, List<String> TitlePages, BaseModel bm)
        {
            for (int i = 0; i < TitlePages.Count; i++)
            {
                bm = new BaseModel();
                drv.Navigate().GoToUrl(TitlePages[i]);
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
                    if (_translator.Text != "") bm.Translators.Add(_translator.Text);
                }
                string[] titles = Regex.Split(drv.FindElement(By.XPath("//td[contains(text(), 'Другие названия')]/parent::tr/td[2]/h2")).Text, @" / |;");
                foreach (var _title in titles)
                {
                    bm.OtherTitles.Add(_title);
                }
                ICollection<IWebElement> authors = drv.FindElements(By.XPath("//a[contains(@href, '/mangaka/')]"));
                foreach (var _author in authors)
                {
                    if (_author.Text != "") bm.Authors.Add(_author.Text);
                }
                bm.BackgroundImg = drv.FindElement(By.XPath("//img[@id='cover']")).GetAttribute("src");
                bm.Description = drv.FindElement(By.XPath("//div[@id='description']")).Text;
                ICollection<IWebElement> chapters = drv.FindElements(By.XPath("//div[@class='manga2']/a"));
                foreach (var _chapter in chapters)
                {
                    bm.Chapters.Add(_chapter.GetAttribute("href"));
                }
                break;
            }
            getChapterImages(drv, bm);
            downloadImagesToLocalStorage(bm);
            sendDataToServer(bm);
        }
        /// <summary>
        /// Получение изображений каждой главsы
        /// </summary>
        /// <param name="drv"></param>
        /// <param name="bm"></param>
        public void getChapterImages(IWebDriver drv, BaseModel bm)
        {
            for (int i = 0; i < bm.Chapters.Count; i++)
            {
                drv.Navigate().GoToUrl(bm.Chapters[i]);
                int count = int.Parse(drv.FindElements(By.XPath("//div[@id='thumbs']/a")).Count.ToString());
                List<String> subImages = new List<String>();
                List<String> subPathImages = new List<String>();
                for (int j = 1; j <= count; j++)
                {
                    drv.Navigate().GoToUrl(bm.Chapters[i] + "?page=" + j);
                    subImages.Add(drv.FindElement(By.XPath("//div[@id='image']/a/img")).GetAttribute("src"));
                   
                }
                var volCh = drv.FindElement(By.XPath("//a[@class='a-series-volch volch']")).Text.Split(" ");
                subPathImages.Add(bm.Title + "/v" + volCh[1] + "/ch" + volCh[3]);
                bm.Images.Add(new List<String>(subImages));
                bm.PathImages.Add(new List<String>(subPathImages));
                break;
            }
        }

        /// <summary>
        /// Отправка данных на сервре для записи в базу данных
        /// </summary>
        /// <param name="bm"></param>
        public void sendDataToServer(BaseModel bm)
        {
            string json = "",
                response = "";
            string url = "http://95.54.44.39:60000/MyWeb/SpaceManga/Scraper/scraper.php"; //исполняемый файл;
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            json = JsonSerializer.Serialize(bm, options); //формирование json строки для отправки на сервер
            json = Regex.Replace(json, @"[^a-zA-Zа-яА-ЯёЁ0-9""\:\{\}\].\-\,\\_ //\[\]]", "");
            json = Regex.Replace(json, @"( ){2,}", "");
            json = Regex.Replace(json, @"(\\"")", "");
            json = Regex.Replace(json, @"\\r\\nПрислать описание", "");
            using (var webClient = new WebClient())
            {
                var pars = new NameValueCollection();
                pars.Add("obj", json); //post данные 
                response = Encoding.UTF8.GetString(webClient.UploadValues(url, pars)); //получение ответа от сервера
                response = Regex.Replace(response, @"\W", ""); //удаление лишнего в ответе сервера
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void uploadImagesToServer()
        {
            /*
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(@"ftp://95.54.44.39:21/SpaceManga/Titles/sd.jpg");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("admin", "64785839");
            request.UseBinary = true;
            request.KeepAlive = true;
            request.UsePassive = true;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            */

            var request = (FtpWebRequest)WebRequest.Create(@"ftp://95.54.44.39:21/SpaceManga/Titles/sd.jpg");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("admin", "64785839");
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = true;
            using(var fileStream = File.OpenRead("D:/sd.jpg"))
            {
                try
                {
                    using (var requestStream = request.GetRequestStream())
                    {
                        fileStream.CopyTo(requestStream);
                        requestStream.Close();
                        var response = (FtpWebResponse)request.GetResponse();
                        fileStream.Close();
                    }
                }
                catch (Exception ex) { }
            }

           
            /*
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential("admin", "64785839");
            client.UploadFile(
                "ftp://95.54.44.39/SpaceManga/Titles/sd.jpg", "D:/sd.jpg");
            */

        }

        public void downloadImagesToLocalStorage(BaseModel bm)
        {
            WebClient client = new WebClient();
            for (int i = 0; i < bm.Images.Count; i++)
            {
                for (int j = 0; j < bm.Images[i].Count; j++)
                {
                    DirectoryInfo dir = new DirectoryInfo("D:/" + bm.Title + "/" + bm.PathImages[i][j].Split("/")[1] + "/" + bm.PathImages[i][j].Split("/")[2]);
                    if (!dir.Exists) dir.Create();
                    client.DownloadFile(bm.Images[i][j], "D:/" + bm.Title + "/" + bm.PathImages[i][j].Split("/")[1] + "/" + bm.PathImages[i][j].Split("/")[2] + "/" + (j + 1) + bm.Images[i][j].Substring(bm.Images[i][j].LastIndexOf("."), 4));
                    break;
                }
            }
        }

        public void getExpansion(List<String> subImages)
        {
            int fpngindex, 
                lpngindex, 
                fjpgindex, 
                ljpgindex;
            for (int i = 0; i < subImages.Count; i++) 
            {
                
            }
        }

    }
}
