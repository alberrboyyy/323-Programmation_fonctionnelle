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
        cols[1], cols[2], int.Parse(cols[4]), int.Parse(cols[5]),
        int.Parse(cols[6]), int.Parse(cols[7]), int.Parse(cols[8]), bool.Parse(cols[9])
    );

    static void ExportCs2(string player, IEnumerable<Cs2Match> matches, string path)
    {
        var header = "date,player,map,start_side,kills,deaths,assists,mvps,won";
        var lines = matches.Select((m, i) =>
            $"2024-01-{i + 1:D2},{m.Player},{m.Map},{m.StartSide},{m.Kills},{m.Deaths},{m.Assists},{m.Mvps},{m.Won.ToString().ToLower()}"
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
                    var series = DataSeries<Cs2Match>.From(MatchGenerator.GenerateCs2(player, 20));

                    var validMatches = series.Values.Where(isValid);

                    ExportCs2(player, validMatches, $"{player.ToLower()}_generated.csv");
                    Console.WriteLine($"{player} : {validMatches.Count()} données valides générées et exportées.");
                }
                return;
            }
        }

        try
        {
            var valorant = DataSeries<ValorantMatch>.FromCsv("data/valorant.csv", ParseValorant);
            var cs2 = DataSeries<Cs2Match>.FromCsv("data/cs2.csv", ParseCs2);
            var lol = DataSeries<LolMatch>.FromCsv("data/lol.csv", ParseLol);

            Console.WriteLine($"Valorant : {valorant.Count} matchs chargés.");
            Console.WriteLine($"CS2      : {cs2.Count} matchs chargés.");
            Console.WriteLine($"LoL      : {lol.Count} matchs chargés.");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Dossier 'data/' introuvable. Assure-toi d'avoir les fichiers au bon endroit !");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Fichier introuvable : {ex.FileName}");
        }
    }
}