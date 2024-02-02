﻿using HtmlAgilityPack;
using Model.Entities;
using Utils;

namespace BLL
{
    public class DividendBLL
    {
        const string filePath = @"C:\Projetos\Flopinguim\WebScraper_CSharp\Data.txt";

        public static HtmlDocument requestSite(string dividend)
        {
            string url = $"https://www.fundsexplorer.com.br/funds/{dividend.ToLower()}";
            var httpClient = new HttpClient();
            var html = httpClient.GetStringAsync(url).Result;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        public static List<decimal> scrapperValuesToDecimalList(string dividend)
        {
            HtmlDocument htmlDocument = requestSite(dividend);

            // Remove as tags <small> do HTML usando LINQ
            htmlDocument.DocumentNode.Descendants("small").ToList().ForEach(smallTag => smallTag.Remove());

            // Extrai os valores das tags <b

            List<decimal> objValues = new List<decimal>();
            foreach (var div in htmlDocument.DocumentNode.SelectNodes("//div[@class='indicators__box']"))
            {
                // Obtendo o texto das tags 'p' dentro da div
                string value = div.SelectSingleNode("p[2]/b").InnerText.Trim().filterChar();
                objValues.Add(decimal.Parse(value));
            }

            foreach (var div in htmlDocument.DocumentNode.SelectNodes("//div[@class='basicInformation__grid__box']"))
            {
                // Obtendo o texto das tags 'p' dentro da div
                string value = div.SelectSingleNode("p[2]/b").InnerText.Trim().filterChar();
                objValues.Add(value.toDecimal());
            }
            return objValues;
        }

        public static List<string> scrapperLabelsToStringList(string dividend)
        {
            HtmlDocument htmlDocument = requestSite(dividend);

            // Remove as tags <small> do HTML usando LINQ
            htmlDocument.DocumentNode.Descendants("small").ToList().ForEach(smallTag => smallTag.Remove());

            // Extrai os valores das tags <b

            List<string> labels = new List<string>();
            foreach (var div in htmlDocument.DocumentNode.SelectNodes("//div[@class='indicators__box']"))
            {
                // Obtendo o texto das tags 'p' dentro da div
                string label = div.SelectSingleNode("p[1]").InnerText.Trim();
                labels.Add(label);
            }

            foreach (var div in htmlDocument.DocumentNode.SelectNodes("//div[@class='basicInformation__grid__box']"))
            {
                // Obtendo o texto das tags 'p' dentro da div
                string label = div.SelectSingleNode("p[1]").InnerText.Trim();
                labels.Add(label);
            }
            return labels;
        }

        public static Dividend createDividendFromList(string dividend)
        {
            List<string> labels = scrapperLabelsToStringList(dividend);
            List<decimal> values = scrapperValuesToDecimalList(dividend);

            Dividend fundInfo = new Dividend
            {
                CompanyName = labels[0],
                Name = labels[1],
                Segment = labels[2],
                CNPJ = labels[3],
                AverageDailytrading = values[0],
                LastYield = values[1],
                DividendYield = values[2],
                NetWorth = values[3],
                AssetValue = values[4],
                RentabilityPerMonth = values[5],
                PVP = values[6]
            };

            return fundInfo;
        }
    }
}
