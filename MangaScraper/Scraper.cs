using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium.Support.UI;

namespace MangaScraper
{
    class Scraper
    {
        private string MainUrl;
        private List<String> MangaUrl = new List<String>();
        private ICollection<IWebElement> PageUrl;
        private string Query;
        public void SetMainUrl(string mainUrl)
        {
            MainUrl = mainUrl;
        }
        public string GetMainUrl()
        {
            return MainUrl;
        }
        public void SetMangaUrl(string mangaUrl)
        {
            MangaUrl.Add(mangaUrl);
        }
        public List<String> GetMangaUrl()
        {
            return MangaUrl;
        }
        public void SetQuery(string query)
        {
            Query = query;
        }
        public string GetQuery()
        {
            return Query;
        }
        public void setManga()
        {

        }
    }
}
