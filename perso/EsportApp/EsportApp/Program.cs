using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataSeries;

namespace EsportApp;

class Program
{
    static ValorantMatch ParseValorant(string[] cols) => new ValorantMatch(
        cols[1], cols[2], int.Parse(cols[3]), int.Parse(cols[4]),
        int.Parse(cols[5]), int.Parse(cols[6]), int.Parse(cols[7]), bool.Parse(cols[8])
    );

    static Cs2Match ParseCs2(string[] cols) => new Cs2Match(
        cols[1], cols[2], cols[3], int.Parse(cols[4]),
        int.Parse(cols[5]), int.Parse(cols[6]), int.Parse(cols[7]), bool.Parse(cols[8])
    );

    static LolMatch ParseLol(string[] cols) => new LolMatch(
        cols[1], cols[2], cols[3], int.Parse(cols[4]), int.Parse(cols[5]),
        int.Parse(cols[6]), int.Parse(cols[7]), int.Parse(cols[8]), bool.Parse(cols[9])
    );

    static void ExportCs2(DataSeries<Cs2Match> matches, string path)
    {
        var header = "date,player,map,start_side,kills,deaths,assists,mvps,won";
        var lines = matches.DataPoints.Select(dp =>
            $"{dp.Timestamp:yyyy-MM-dd},{dp.Value.Player},{dp.Value.Map},{dp.Value.StartSide}," +
            $"{dp.Value.Kills},{dp.Value.Deaths},{dp.Value.Assists},{dp.Value.Mvps}," +
            $"{dp.Value.Won.ToString().ToLower()}"
        );
        File.WriteAllLines(path, lines.Prepend(header));
    }

    static void ExportValorant(DataSeries<ValorantMatch> matches, string path)
    {
        var header = "date,player,agent,kills,deaths,assists,headshots,rounds_won,won";
        var lines = matches.DataPoints.Select(dp =>
            $"{dp.Timestamp:yyyy-MM-dd},{dp.Value.Player},{dp.Value.Agent},{dp.Value.Kills}," +
            $"{dp.Value.Deaths},{dp.Value.Assists},{dp.Value.Headshots},{dp.Value.RoundsWon}," +
            $"{dp.Value.Won.ToString().ToLower()}"
        );
        File.WriteAllLines(path, lines.Prepend(header));
    }

    static void Main(string[] args)
    {
        Func<Cs2Match, bool> isValid = m => m.Kills + m.Assists <= 50 && m.Deaths >= 1;

        if (args.Contains("--generate"))
        {
            var targetIndex = Array.IndexOf(args, "--generate") + 1;
            if (targetIndex < args.Length)
            {
                var target = args[targetIndex];
                var players = target == "all"
                    ? new[] { "Raphaël", "Kiara", "Dylan", "Noé" }
                    : new[] { target };

                foreach (var player in players)
                {
                    var series = MatchGenerator.GenerateCs2(player, 20);

                    var validMatches = series.Filter(isValid);

                    ExportCs2(validMatches, $"data/gen/{player.ToLower()}_generated.csv");
                    Console.WriteLine($"{player} : {validMatches.Count} données valides générées et exportées.");
                }
                return;
            }
        }

        string? playerFilter = args.Contains("--player")
            ? args[Array.IndexOf(args, "--player") + 1]
            : null;

        string filterMode = args.Contains("--filter")
            ? args[Array.IndexOf(args, "--filter") + 1]
            : "all";

        string errorMode = args.Contains("--error")
            ? args[Array.IndexOf(args, "--error") + 1]
            : "soft";

        var filters = new Dictionary<string, Func<ValorantMatch, bool>>
        {
            ["wins"]   = m => m.Won,
            ["losses"] = m => !m.Won,
            ["all"]    = m => true,
        };



        try
        {
            var valorant = DataSeries<ValorantMatch>.FromCsv("data/valorant.csv", ParseValorant);
            var cs2 = DataSeries<Cs2Match>.FromCsv("data/cs2.csv", ParseCs2);
            var lol = DataSeries<LolMatch>.FromCsv("data/lol.csv", ParseLol);

            Func<ValorantMatch, bool> isOutlier = m =>
                m.Kills < 0 || m.Kills > 50 ||
                m.Deaths < 0 || m.Deaths > 30 ||
                m.Assists < 0;

            if (errorMode == "strict")
            {
                var bad = valorant.Outliers(isOutlier);
                    Console.WriteLine(string.Join("\n", bad.DataPoints.Select(dp =>
                        $"{dp.Timestamp:yyyy-MM-dd} — {dp.Value.Player} : {dp.Value.Kills} kills"
                    )));
                return;
            }

            var propre = valorant.Sanitize(isOutlier);

            if (errorMode == "hard")
                ExportValorant(propre, "data/valorant_clean.csv");

            var result = propre
                .Filter(m => playerFilter == null || m.Player == playerFilter)
                .Filter(filters[filterMode]);

            Console.WriteLine($"{result.Count} matchs.");
        }

        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Dossier 'data/' introuvable.");
        }

        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Fichier introuvable : {ex.FileName}");
        }
    }
}
