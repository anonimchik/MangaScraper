using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Collections.Specialized;

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
                /// Дописать алгоритм получения каталога всех тайтлов
                drv.Navigate().GoToUrl(SourceUrl);
                ///
                get_title_info(drv, bm);
                get_chapter_info(drv, bm);
                get_images_from_chapters(drv, bm);
                //download_images_to_local_storage(bm);
                create_directories_on_server(bm);
                upload_images_to_server(bm);
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
        /// Получение ссылки на главу c информацией о номере главы, названии главы и переводчиках
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
            IJavaScriptExecutor js = (IJavaScriptExecutor)drv;
            js.ExecuteScript("document.getElementsByClassName('table-wrap ps')[0].style.maxHeight='inherit'");
            for (int i = 0; i < chapter_number.Length; i++) //доделать скролинг таблицы с главами
            {
                bm.Chapters.Add(link[i].GetAttribute("href") + "|" + chapter_number[i].Text + "|" + chapter_name[i].Text + "|" + translator[i].Text);
                bm.Directories.Add(bm.EngTitle+"/"+ link[i].GetAttribute("href").Split("/")[4]);
                break;
            }

        }

        /// <summary>
        /// Получение списка с картинками к каждой главе
        /// </summary>
        /// <param name="drv">Объект класса IWebDriver</param>
        /// <param name="bm">Объект класса BaseModel</param>
        public void get_images_from_chapters(IWebDriver drv, BaseModel bm)
        {
            //обрезание строки и получение названия картинки https://manga24.ru/Content/pages/onepiece/1025/op_1024_000a.jpg -> 000а.jpg
            for (int i = 0; i < bm.Chapters.Count; i++) //проход по главам
            {
                List<String> sub_images = new List<String>();
                List<String> sub_path_image = new List<String>();
                drv.Navigate().GoToUrl(bm.Chapters[i].Split("|")[0]);
                int page_count = drv.FindElements(By.XPath("//select[@id='page']/option")).Count;
                string[] _path_to_file = drv.FindElements(By.XPath("//div[@id='preload']/img"))[0].GetAttribute("src").Substring((drv.FindElements(By.XPath("//div[@id='preload']/img"))[0].GetAttribute("src").LastIndexOf("/") + 1), drv.FindElements(By.XPath("//div[@id='preload']/img"))[0].GetAttribute("src").Length - (drv.FindElements(By.XPath("//div[@id='preload']/img"))[0].GetAttribute("src").LastIndexOf("/") + 1)).Split(@"_");
                string path_to_file = "";

                /// Получение пути к файлу (картинке)
                if (_path_to_file.Length > 0) 
                {
                    int l = 0;
                    while (l<_path_to_file.Length-1)
                    {
                        path_to_file += _path_to_file[l] + "_";
                        l++;
                    }
                }
                string path_to_dir = drv.FindElements(By.XPath("//div[@id='preload']/img"))[0].GetAttribute("src").Substring(0, (drv.FindElements(By.XPath("//div[@id='preload']/img"))[0].GetAttribute("src").LastIndexOf("/") + 1));
                string[] file_name;
                if (_path_to_file.Length > 0)
                {
                    file_name = _path_to_file[_path_to_file.Length - 1].Split(".");
                }
                else
                {
                    file_name = path_to_file.Split(".");
                }
                bool is_png_image = false,
                     is_jpg_image = false;

                /// Формирование ссылки до картинки каждой главы путем получение первой ссылки на первую картинку.
                /// После получения ссылки на первую картинку отпарвляются запросы к manga24 для получения расширения картинки
                /// Далее формируется список с ссылками к картинкам и список с путем к картинкам
                for (int j = 0; j < page_count-1; j++)
                {
                    if (_path_to_file[_path_to_file.Length - 1].IndexOf("a") > -1)
                    {
                        drv.Navigate().GoToUrl(path_to_dir + path_to_file + file_name[0] + "." + file_name[1]);
                        is_jpg_image = (drv.FindElements(By.XPath("//img[contains(@src,'https://manga24.ru/')]")).Count > 0) ? true : false;
                        drv.Navigate().GoToUrl(path_to_dir + path_to_file + file_name[0] + ".png");
                        is_png_image = (drv.FindElements(By.XPath("//img[contains(@src,'https://manga24.ru/')]")).Count > 0) ? true : false;
                    }
                    if (!sub_images.Contains(path_to_dir + path_to_file + file_name[0] + "." + file_name[1]) && is_jpg_image) sub_images.Add(path_to_dir + path_to_file + file_name[0] + "." + file_name[1]);
                    if (!sub_images.Contains(path_to_dir + path_to_file + file_name[0] + ".png") && is_png_image) sub_images.Add(path_to_dir + path_to_file + file_name[0] + ".png");
                    if (file_name[0].IndexOf("0") > -1)
                    {
                        if (j / 10.0 < 1.0)
                        {
                            file_name[0] = "00" + (j+1).ToString();
                        }
                        else
                        {
                            file_name[0] = "0" + (j+1).ToString();
                        }
                    }
                    drv.Navigate().GoToUrl(path_to_dir + path_to_file + file_name[0] + "." + file_name[1]);
                    is_jpg_image = (drv.FindElements(By.XPath("//img[contains(@src,'https://manga24.ru/')]")).Count > 0) ? true : false;
                    drv.Navigate().GoToUrl(path_to_dir + path_to_file + file_name[0] + ".png");
                    is_png_image = (drv.FindElements(By.XPath("//img[contains(@src,'https://manga24.ru/')]")).Count > 0) ? true : false;
                    if (!sub_images.Contains(path_to_dir + path_to_file + file_name[0] + "." + file_name[1]) && is_jpg_image) sub_images.Add(path_to_dir + path_to_file + file_name[0] + "." + file_name[1]);
                    if (!sub_images.Contains(path_to_dir + path_to_file + file_name[0] + ".png") && is_png_image) sub_images.Add(path_to_dir + path_to_file + file_name[0] + ".png");
                }
                int k = 0;
                while (k < sub_images.Count)
                {
                    sub_path_image.Add(bm.EngTitle + path_to_dir.Substring(path_to_dir.LastIndexOf("/", path_to_dir.LastIndexOf("/") - 1)) + sub_images[k].Split("/")[sub_images[k].Split("/").Length - 1].Split("_")[sub_images[k].Split("/")[sub_images[k].Split("/").Length - 1].Split("_").Length-1] );
                    k++;
                }
                bm.PathImages.Add(new List<String>(sub_path_image));
                bm.Images.Add(new List<string>(sub_images));
                //break;
            }

        }

        public void create_directories_on_server(BaseModel bm)
        {
            string json = "",
                response = "";
            string url = "http://95.54.44.39:6001/MyWeb/SpaceManga/Scraper/scraper.php"; //исполняемый файл;
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            json = JsonSerializer.Serialize(bm.Directories, options); //формирование json строки для отправки на сервер
            using (var webClient = new WebClient())
            {
                var pars = new NameValueCollection();
                pars.Add("dir", json); //post данные 
                response = Regex.Replace(Encoding.UTF8.GetString(webClient.UploadValues(url, pars)), @"\W", ""); //удаление лишнего в ответе сервера
            }
        }
        public void upload_images_to_server(BaseModel bm)
        {
            for (int i = 0; i < bm.Images.Count; i++)
            {
                for (int j = 0; j < bm.Images[i].Count; j++)
                {
                    var expansion = bm.Images[i][j].IndexOf(".jpeg") == 1 ? ".jpeg" : ".png";
                    FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://95.54.44.39/SpaceManga/Titles/" + bm.PathImages[i][j] + "/" + (j + 1).ToString() + expansion);
                    req.Credentials = new NetworkCredential("admin", "64785839");
                    req.Method = WebRequestMethods.Ftp.UploadFile;
                    try
                    {
                        using (Stream fileStream = File.OpenRead(@"A:MangaScraper/" + bm.PathImages[i][j].Split("/")[0] + "/" + bm.PathImages[i][j].Split("/")[1] + "/" + (j + 1).ToString() + expansion))
                        using (Stream ftpStream = req.GetRequestStream())
                        {
                            fileStream.CopyTo(ftpStream);
                        }
                    }
                    catch (Exception ex) { }
                }
                break;
            }

        }

        public void download_images_to_local_storage(BaseModel bm)
        {
            WebClient client = new WebClient();
            for (int i = 0; i < bm.Images.Count; i++)
            {
                string jpg = "",
                       png = "",
                       exp = "";
                List<String> expansion = new List<String>();
                for (int j = 0; j < bm.Images[i].Count; j++)
                {
                    exp = (bm.Images[i][j].IndexOf(".jpg")) > -1 ? ".jpg" : ".png";
                    DirectoryInfo dir = new DirectoryInfo("D:MangaScraper/" + bm.PathImages[i][j].Split("/")[0] + "/" + bm.PathImages[i][j].Split("/")[1]);
                    if (!dir.Exists) dir.Create();
                    if (bm.Images[i][j].IndexOf(".jpg") > -1)
                    {
                        jpg += (j + 1) + ",";
                    }
                    if (bm.Images[i][j].IndexOf(".png") > -1)
                    {
                        png += (j + 1) + ",";
                    }
                    client.DownloadFile(bm.Images[i][j], "D:MangaScraper/" + bm.PathImages[i][j].Split("/")[0] + "/" + bm.PathImages[i][j].Split("/")[1] + "/" + (j + 1).ToString() + exp);
                    //break;
                }
                expansion.Add(Regex.Replace(jpg, @"\,$", "") + "|" + Regex.Replace(png, @"\,$", ""));
                bm.Expansion.Add(new List<String>(expansion));
                break;
            }
        }
    }
}
