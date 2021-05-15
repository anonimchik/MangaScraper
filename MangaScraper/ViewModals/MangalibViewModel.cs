using System;
using System.Collections.Generic;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Net;
using System.IO;
using OpenQA.Selenium.Firefox;
using System.Text;

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
            //var opt = new OperaOptions();
            /* 
             opt.BinaryLocation = @"C:\Users\edik2\AppData\Local\Programs\Opera\launcher.exe";
             IWebDriver dr = new OperaDriver(opt);
            */
            /// FirefoxProfile profile = new FirefoxProfile(@"C:\Users\edik2\AppData\Local\Mozilla\Firefox\Profiles\gjh9gjq9.axixa");
            // FirefoxOptions option = new FirefoxOptions();
            // option.Profile = profile;
            /*
            IWebDriver dr = new EdgeDriver(options);
            for (int i=1; i <= 3; i++)
            {
                dr.Navigate().GoToUrl("https://www.radiorecord.ru/podcast/15761");
            }
            */
            using (IWebDriver driver = new EdgeDriver(options)) //основная работа парсера
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                BaseModel bm = new BaseModel();
                /*   |Получение ссылок на страницы|   */
                for (int i = 1; i < 50; i++)
                {
                    driver.Navigate().GoToUrl(Url+"?page="+i.ToString()); //переход на сайт 
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("const newProto = navigator.__proto__ delete newProto.webdriver navigator.__proto__ = newProto");
                    if (driver.FindElements(By.XPath(@"//div[@class=' page__wrapper page__wrapper_left  paper manga-search']")).Count > 0) pages.Add(Url + "?page=" + i.ToString());
                    Thread.Sleep(5000);
                }
                var mg = new List<String>();
                
                getTitleUrl(driver, pages, mg); //получение ссылок на тайтлы
                bm.OtherTitles = new List<String>();
                bm.Painters = new List<String>();
                bm.Publishings = new List<String>();
                bm.Genres = new List<String>();
                bm.Translators = new List<String>();
                getCurrentTitleInformation(driver, mg, bm); //получение информации о тайтле
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
        /// <param name="driver">Объект класса IWebDriver</param>
        /// <param name="mg">Список ссылок на тайтлы</param>
        /// <param name="bm">Объект класса BaseModel</param>
        public void getCurrentTitleInformation(IWebDriver driver, List<String> mg, BaseModel bm)
        {
            List<BaseModel> Titles = new List<BaseModel>();
            for (int i = 0; i < mg.Count; i++)
            {
                driver.Navigate().GoToUrl(mg[i]); //переход на страницу конкретного тайтла
                var img=driver.FindElement(By.XPath(@"//div[@class='media-sidebar__cover paper']/img"));
                bm.BackgroundImg = img.GetAttribute("src"); //запись ссылки на обложку тайтла
                bm.Title=driver.FindElement(By.XPath(@"//div[@class='media-name__main']")).Text; //запись названия тайтла
                bm.EngTitle=driver.FindElement(By.XPath(@"//div[@class='media-name__alt']")).Text; //запись английского названия тайтла
                bm.Category = driver.FindElement(By.XPath(@"//a[contains(@href,'types')]")).FindElement(By.XPath(@"//div[@class='media-info-list__value']")).Text; //запись категории
                bm.ChapterNumber = ushort.Parse(driver.FindElement(By.XPath(@"//div[text()='Загружено глав']/parent::div/div[@class='media-info-list__value text-capitalize']")).Text); //запись кол-ва глав
                bm.ReleaseYear = ushort.Parse(driver.FindElement(By.XPath(@"//a[contains(@href,'year')]/div[@class='media-info-list__value']")).Text); //запись года выпуска
                bm.TranslateStatus = driver.FindElement(By.XPath(@"//a[contains(@href,'status')]/div[@class='media-info-list__value text-capitalize']")).Text; //запись статуса перевода
                bm.TitleStatus = driver.FindElement(By.XPath(@"//a[contains(@href,'manga_status')]/div[@class='media-info-list__value']")).Text; //запись статуса тайтла
                bm.Author = driver.FindElement(By.XPath(@"//div[text()='Автор']/parent::div/div[@class='media-info-list__value']")).Text; //запись автора
                
                ICollection<IWebElement> painters = driver.FindElements(By.XPath(@"//div[text()='Художник']/parent::div/div[@class='media-info-list__value']/a"));
                foreach (var _painter in painters) //запись художников
                {
                    bm.Painters.Add(_painter.Text);
                }
                ICollection<IWebElement> publishings = driver.FindElements(By.XPath(@"//div[text()='Издательство']/parent::div/div[@class='media-info-list__value']/a"));
                foreach (var _publishing in publishings) //запись издательств
                {
                    bm.Publishings.Add(_publishing.Text);
                }
                bm.Description = driver.FindElement(By.XPath(@"//div[@class='media-description__text']")).Text; //запись описания
                if (driver.FindElements(By.XPath(@"//div[@class='media-tag-item media-tag-item_more']/i[@class='fa fa-ellipsis-h']")).Count > 0) //Проверка наличия блока "Показать все жанры" 
                {
                    driver.FindElement(By.XPath(@"//div[@class='media-tag-item media-tag-item_more']/i[@class='fa fa-ellipsis-h']")).Click(); //имитация клика кнопки показать все жанры
                }
                if (driver.FindElements(By.XPath(@"//div[@class='media-info-list__expand']")).Count > 0) //Проверка наличия блока "Показать все альтернативные названия"
                {
                    driver.FindElement(By.XPath(@"//div[@class='media-info-list__expand']")).Click(); //имитация клика на кнопку для раскрытия полного списка других названий
                }
                ICollection<IWebElement> otherTitles = driver.FindElements(By.XPath(@"//div[@class='media-info-list__expand']/parent::div/div[@class='media-info-list__value']/div"));
                foreach (var _otherTitles in otherTitles) //запись других названий
                {
                    bm.OtherTitles.Add(_otherTitles.Text);
                }
                ICollection<IWebElement> genres = driver.FindElements(By.XPath(@"//div[@class='media-tags']/a"));
                foreach (var _genre in genres) //запись жанров
                {
                    bm.Genres.Add(_genre.Text);
                }
                ICollection<IWebElement> translators = driver.FindElements(By.XPath(@"//a[@class='team-list-item team-list-item_xs']/div[@class='team-list-item__name']"));
                foreach (var _translator in translators) //запись переводчиков
                {
                    bm.Translators.Add(_translator.Text);
                }

                getChapterUrl(driver, mg, bm, i); //запись ссылок на главы в bm.Chapters  
                getChapterImages(driver, bm); //запись ссылок на изображения конкретного тайтла
                Titles.Add(bm); //запись полученного объекта в список Titles
                downloadImagesToDirectory(Titles);

                //добавить запись информации в бд

                bm = new BaseModel(); //обнуление объекта
                break;
            }

        }

        /// <summary>
        /// Запись в bm.Chapters ссылок на главы тайтла
        /// </summary>
        /// <param name="driver">Объект класса IWebDriver</param>
        /// <param name="mg">Список ссылок на тайтлы</param>
        /// <param name="bm">Объект класса BaseModel</param>
        /// <param name="i">Счетчик</param>
        public void getChapterUrl(IWebDriver driver, List<String> mg, BaseModel bm, int i)
        {
            driver.Navigate().GoToUrl(mg[i] + "?section=chapters"); //переход на страницу с главами
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver; //создание объекта класса IJavaScriptExecutor для выполнения JS сценариев

            /*
                |"var header=document.getElementsByClassName('tabs paper')[0];" +
                |"var footer = document.getElementsByClassName('footer__inner paper')[0];" +
                |"var pointY ={" +
                |"firstPosition : header.offsetTop," +
                |"secondPosition: footer.offsetTop" +
                |"};" +
                |"return pointY.firstPosition + '|' + pointY.secondPosition; ").ToString();
             */

            /*   |Получение Y координаты футера и первого блока с главой|   */
            string positions = js.ExecuteScript("var header=document.getElementsByClassName('tabs paper')[0];" +
                             "var footer = document.getElementsByClassName('footer__inner paper')[0];" +
                             "var pointY ={" +
                             "firstPosition : header.offsetTop," +
                             "secondPosition: footer.offsetTop" +
                             "};" +
                             "return pointY.firstPosition + '|' + pointY.secondPosition; ").ToString();
            var position = positions.Split("|"); //разбиение строки на массив строк (1 - У координата блока с первой главой, 2 - У координата футера)
            int firstPosition = int.Parse(position[0]); // У координата блока с первой главой
            int secondPosition = int.Parse(position[1]); //У координата футера
            for (int j = firstPosition; j < secondPosition; j = j + 58) //Скролл страницы на высоту одного блока с главой (58px)
            {
                /* 
                    |var header=document.getElementsByClassName('tabs paper')[0];         |   
                    |var footer=document.getElementsByClassName('footer__inner paper')[0];|
                    |var firstBlock={                                                     |
                    |   x : header.offsetLeft,                                            |
                    |   y : header.offsetTop                                              |
                    |};                                                                   |
                    |var secondBlock={                                                    |
                    |   x : footer.offsetLeft,                                            |
                    |   y : footer.offsetTop                                              |
                    |};                                                                   |
                    |window.scroll(firstPosition);|                                       |
               */

                /*   |JS сценарий для скролла страницы на 58px|   */
                var debugstr = "var header=document.getElementsByClassName('tabs paper')[0];" +
                            "var footer = document.getElementsByClassName('footer__inner paper')[0];" +
                            "var firstBlock ={" +
                                            "x : header.offsetLeft," +
                                            "y: header.offsetTop" +
                                            "};" +
                            "var secondBlock ={" +
                                            "x : footer.offsetLeft," +
                                            "y: footer.offsetTop" +
                                            "};" +
                            "window.scroll(header.offsetTop, " + j.ToString() + ");";
                js.ExecuteScript(debugstr); //Скролл страницы на 58px
                ICollection<IWebElement> chapters = driver.FindElements(By.XPath(@"//div[@class='media-chapter__name text-truncate']/a")); //получение коллекции глав
                Thread.Sleep(500); //задержка
                foreach (var _chapter in chapters) //запись ссылок на главы в bm.Chapters
                {
                    if (!bm.Chapters.Contains(_chapter.GetAttribute("href")))
                        bm.Chapters.Add(_chapter.GetAttribute("href"));
                }
            }
        }
        
        /// <summary>
        /// Запись изображений с конкретной главы
        /// </summary>
        /// <param name="driver">Объект класса IWebDriver</param>
        /// <param name="bm">Объект класса BaseModel</param>
        public void getChapterImages(IWebDriver driver, BaseModel bm)
        {
            bm.Chapters.Reverse();
            for (int i = 0; i < bm.Chapters.Count; i++)
            {
                driver.Navigate().GoToUrl(bm.Chapters[i]); //переход на конкретную страницу
                int pageNumber=int.Parse(driver.FindElement(By.XPath(@"//label[@class='button reader-pages__label reader-footer__btn']/span")).Text.Substring(driver.FindElement(By.XPath(@"//label[@class='button reader-pages__label reader-footer__btn']/span")).Text.LastIndexOf(" "), driver.FindElement(By.XPath(@"//label[@class='button reader-pages__label reader-footer__btn']/span")).Text.Length- driver.FindElement(By.XPath(@"//label[@class='button reader-pages__label reader-footer__btn']/span")).Text.LastIndexOf(" "))); //получение кол-ва страниц
                List<String> subImages = new List<String>(); //создание вспомогательного списка для изображений
                for (int j = 1; j <= pageNumber; j++) //запись изображений конкретной главы
                {
                    driver.Navigate().GoToUrl(bm.Chapters[i] + "?page=" + j); //переход по страницам конкретного тайтла
                    var chapter = bm.Chapters[i].Split("/");
                    subImages.Add(driver.FindElement(By.XPath(@"//div[@class='reader-view__wrap']/img")).GetAttribute("src")+"|"+chapter[3]+"/"+chapter[4]+"/"+chapter[5]); //запись ссылок на изображения в вспомогателный список
                    break;                    
                }
                bm.Images.Add(new List<String>(subImages)); //запись ссылок на изображения в список списков
                break;
            }
        }

        /// <summary>
        /// Скачивание изображений конкретного тайтла
        /// </summary>
        /// <param name="Titles">Список объектов BaseModel</param>
        public void downloadImagesToDirectory(List<BaseModel> Titles)
        {
            string rootDir = @"C:\Test";
            for (int i = 0; i < Titles.Count; i++)
            {
                for (int j = 0; j <Titles[i].Images.Count; j++)
                {
                    for (int k = 0; k < Titles[i].Images[j].Count; k++)
                    {
                        DirectoryInfo rootDirInfo = new DirectoryInfo(rootDir);
                        if (!rootDirInfo.Exists)
                        {
                            rootDirInfo.Create();
                        }
                        var Substrings = Titles[i].Images[j][k].Split("|"); //разбиение на массив строки 
                        string Volume=Substrings[1].Split("/")[1]; //запись тома
                        string Title=Substrings[1].Split("/")[0]; //запись названия тайтла
                        int Chapter=j+1; //запись главы
                        string Category = Substrings[0].Split("/")[4]; //запись категории
                        string ImgTitle=k+1.ToString()+Substrings[0].Split("/")[8].Substring(Substrings[0].Split("/")[8].LastIndexOf("."), Substrings[0].Split("/")[8].Length-Substrings[0].Split("/")[8].LastIndexOf(".")); //запись названия картинки
                        string Path = Category +"/"+ Title + "/" + Volume + "/ch" + Chapter.ToString()+ "/" + ImgTitle;
                        DirectoryInfo categoryDir = new DirectoryInfo(rootDir + @"\" + Category);
                        if (!categoryDir.Exists) rootDirInfo.Create(); //создание директории C:\Test\Category
                        DirectoryInfo titleDir = new DirectoryInfo(rootDir + @"\" + Category + @"\" + Title);
                        if (!titleDir.Exists) titleDir.Create(); //создание директории C:\Test\Category\Title
                        DirectoryInfo volumeDir = new DirectoryInfo(rootDir + @"\" + Category + @"\" + Title + @"\" + Volume);
                        if (!volumeDir.Exists) categoryDir.Create(); //создание категории C:\Test\Category\Title\Volume
                        DirectoryInfo chDir = new DirectoryInfo(rootDir + @"\" + Category + @"\"+Title+@"\" + Volume + @"\ch" + Chapter);
                        if (!chDir.Exists) chDir.Create(); //создание категории C:\Test\Category\Title\Volume\Chapter
                        using (WebClient webclient = new WebClient())
                        {
                            webclient.DownloadFile(Titles[i].Images[j][k].Split("|")[0], rootDir+@"\"+Path); //закачка изображений
                        }

                        //добавить запись пути к картинке в бд

                        break;
                    }
                }
            }
        }

        
    }
}
