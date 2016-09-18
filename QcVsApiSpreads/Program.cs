// Copyright (C) SquidEyes, LLC. - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and Confidential
// Written by Louis S. Berman <louis@squideyes.com>, 9/18/2016

using System;
using System.Collections.Generic;
using System.IO;

namespace QcVsApiSpreads
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetBufferSize(80, 1000);

            var dates = GetDates();

            using (var writer = new StreamWriter("AvgSpreads.csv"))
            {
                writer.WriteLine("Symbol,Hour,AvgQcSpread,AvgApiSpread,Variance");

                foreach (Symbol symbol in Enum.GetValues(typeof(Symbol)))
                {
                    foreach (var date in dates)
                    {
                        var qc = GetAvgSpreads(
                            Symbol.EURUSD, GetQcTicks(symbol, date));

                        var api = GetAvgSpreads(
                            Symbol.EURUSD, GetApiTicks(symbol, date));

                        for (int hour = 1; hour <= 13; hour++)
                        {
                            var varience = (qc[hour] / api[hour]);

                            writer.WriteLine(
                                $"{symbol},{date.AddHours(hour)},{qc[hour].ToPips(symbol):N1},{api[hour].ToPips(symbol):N1},{varience:N2}");

                            Console.WriteLine(
                                $"{symbol} {date.AddHours(hour):MM/dd/yyyy hh:00:00 tt}, QC PIPs: {qc[hour].ToPips(symbol):N1}, API PIPs: {api[hour].ToPips(symbol):N1}, Varience: {varience:P0}");
                        }

                        Console.WriteLine();
                    }
                }
            }
            Console.WriteLine();
            Console.Write("Press any key to terminate...");

            Console.ReadKey(true);
        }

        private static void ParseCsv(string csv, Action<string[]> onLine)
        {
            using (var reader = new StringReader(csv))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                    onLine(line.Split(','));
            }
        }

        private static List<Tick> GetApiTicks(Symbol symbol, DateTime date)
        {
            var fileName = Path.Combine(Properties.Settings.Default.BasePath,
                "FromGetFxcmHistory", symbol.ToString(),
                $"FXCM_FCAPI_{symbol}_{date:yyyy_MM_dd}_00_24H_EST.csv");

            string csv;

            using (var reader = new StreamReader(fileName))
                csv = reader.ReadToEnd();

            var ticks = new List<Tick>();

            ParseCsv(csv,
                fields =>
                {
                    var tickOnEst = DateTime.Parse(fields[1]);

                    if (tickOnEst.Hour < 1 || tickOnEst.Hour > 13)
                        return;

                    ticks.Add(new Tick()
                    {
                        Symbol = symbol,
                        TickOnEst = tickOnEst,
                        BidRate = double.Parse(fields[2]).ToRoundedRate(symbol),
                        AskRate = double.Parse(fields[3]).ToRoundedRate(symbol)
                    });
                });

            return ticks;
        }

        private static string GetQcCsv(Symbol symbol, DateTime date)
        {
            var fileName = Path.Combine(Properties.Settings.Default.BasePath,
                "FromQuantConnect", symbol.ToString(),
                $"{date:yyyyMMdd}_{symbol}_tick_quote.csv");

            using (var reader = new StreamReader(fileName))
                return reader.ReadToEnd();
        }

        private static DateTime UtcDate(DateTime date) =>
            new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);

        private static List<Tick> GetQcTicks(Symbol symbol, DateTime date)
        {
            var ticks = new List<Tick>();

            ParseCsv(GetQcCsv(symbol, date.AddDays(-1)) + GetQcCsv(symbol, date),
                fields =>
                {
                    var tickOnEst = UtcDate(date).AddMilliseconds(
                        int.Parse(fields[0])).ToEstFromUtc();

                    if (tickOnEst.Hour < 1 || tickOnEst.Hour > 13)
                        return;

                    ticks.Add(new Tick()
                    {
                        Symbol = symbol,
                        TickOnEst = tickOnEst,
                        BidRate = double.Parse(fields[1]).ToRoundedRate(symbol),
                        AskRate = double.Parse(fields[2]).ToRoundedRate(symbol)
                    });
                });

            return ticks;
        }

        private static Dictionary<int, double> GetAvgSpreads(Symbol symbol, List<Tick> ticks)
        {
            var spreadInfos = new Dictionary<int, SpreadInfo>();

            for (int hour = 1; hour <= 13; hour++)
                spreadInfos[hour] = new SpreadInfo(symbol);

            foreach (var tick in ticks)
            {
                var spreadInfo = spreadInfos[tick.TickOnEst.Hour];

                spreadInfo.Count++;
                spreadInfo.Total += tick.Spread;
            }

            var avgSpreads = new Dictionary<int, double>();

            foreach (var hour in spreadInfos.Keys)
                avgSpreads.Add(hour, spreadInfos[hour].AvgSpread);

            return avgSpreads;
        }

        private static List<DateTime> GetDates()
        {
            var dates = new List<DateTime>();

            dates.Add(new DateTime(2015, 01, 15));
            dates.Add(new DateTime(2015, 02, 03));
            dates.Add(new DateTime(2015, 03, 18));
            dates.Add(new DateTime(2015, 04, 30));
            dates.Add(new DateTime(2015, 05, 06));
            dates.Add(new DateTime(2015, 06, 29));
            dates.Add(new DateTime(2015, 07, 07));
            dates.Add(new DateTime(2015, 08, 24));
            dates.Add(new DateTime(2015, 09, 01));
            dates.Add(new DateTime(2015, 10, 22));
            dates.Add(new DateTime(2015, 11, 12));
            dates.Add(new DateTime(2015, 12, 03));

            return dates;
        }
    }
}
