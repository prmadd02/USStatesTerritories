using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace USStatesTerritories
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: --summary");
                Console.WriteLine("Return: Mean, Median, and Standard Deviation of US population");
                Console.WriteLine("");
                Console.WriteLine("Usage: --state X");
                Console.WriteLine("Return: Finds state/territory and returns population of state/territory");
                return;
            }

            List<USStatesTerritoriesCollection> USCensusData = new List<USStatesTerritoriesCollection>();

            parseWikiTables(USCensusData);

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("--summary"))
                {
                    Console.WriteLine(PopulationSummary(USCensusData));
                }

                if (args[i].Contains("--state"))
                {
                    Console.WriteLine(PopulationSearch(USCensusData, args[i + 1]));
                    i++;
                }

            }
        }

        public static void parseWikiTables(List<USStatesTerritoriesCollection> uSCensusData)
        {
            WebClient wc = new WebClient();
            string url = "https://en.wikipedia.org/wiki/List_of_U.S._states_and_territories_by_population";
            string html;

            html = wc.DownloadString(url);

            // Create an HtmlDocument
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tables = doc.DocumentNode.SelectNodes(".//table");
            //Console.WriteLine(tables.Count);

            foreach (var table in tables)
            {
                var rows = table.SelectNodes(".//tr");
                if (rows != null && rows[0].InnerText.Contains("Rank"))
                {
                    // The first two rows are headers, just skip them
                    // Last 4 rows aren;t needed as well
                    for (int x = 2; x < 58; x++)
                    {
                        var row = rows[x];
                        var cells = row.SelectNodes(".//td");

                        var ustc = new USStatesTerritoriesCollection();

                        ustc.state = cells[2].InnerText;
                        ustc.census20 = cells[3].InnerText;

                        uSCensusData.Add(ustc);
                    }
                }
                                
            }

            //Console.WriteLine(uSCensusData.Count);
            //Console.WriteLine(uSCensusData[uSCensusData.Count-1].census20);
        }

        public class USStatesTerritoriesCollection
        {
            public string state { get; set; }
            public string census20 { get; set; }

        }

        public static string PopulationSummary(List<USStatesTerritoriesCollection> uSCensusData)
        {
            double result = 0;

            uSCensusData.OrderBy(x => x.census20);
            int count = uSCensusData.Count - 1;
            int location = count / 2;
            string median = uSCensusData[location].census20;
            double average = uSCensusData.Average(x => double.Parse(x.census20));
            double sum = uSCensusData.Sum(x => Math.Pow(double.Parse(x.census20) - average, 2));

            result = Math.Sqrt((sum) / (count));

            string Summary = $"Mean: {average}, Median: {median}Standard Deviation: {result}";

            return Summary;
                       
        }

        public static string PopulationSearch(List<USStatesTerritoriesCollection> uSCensusData, string search)
        {
            if (uSCensusData.Exists(x => x.state.Contains(search)))
            {
                int index = uSCensusData.FindIndex(0, uSCensusData.Count - 1, x => x.state.Contains(search));

                return uSCensusData[index].census20;
            }
            else
            {
                return "Data Not Found";
            }
        }

    }
}

