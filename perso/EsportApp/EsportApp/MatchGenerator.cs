using System;
using System.Collections.Generic;
using System.Linq;
using DataSeries;

namespace EsportApp;

public static class MatchGenerator
{
    public static DataSeries<DataPoint<Cs2Match>> GenerateCs2(string player, int count, int seed = 42)
    {
        var rng = new Random(seed);
        var maps = new[] { "Dust2", "Mirage", "Inferno", "Nuke", "Ancient" };
        var sides = new[] { "CT", "T" };
        var start = new DateTime(2023, 9, 1);

        return DataSeries<DataPoint<Cs2Match>>.From(
            Enumerable.Range(1, count)
                .Select(i => new DataPoint<Cs2Match>(
                    start.AddDays(i),
                    new Cs2Match(
                        player,                         // player
                        maps[rng.Next(maps.Length)],    // map
                        sides[rng.Next(2)],             // start_side
                        rng.Next(10, 28),               // kills
                        rng.Next(6, 18),                // deaths
                        rng.Next(0, 8),                 // assists
                        rng.Next(0, 5),                 // mvps
                        rng.Next(2) == 0                // won
                    )
                ))
        );
    }

    public static DataSeries<DataPoint<ValorantMatch>> GenerateValorant(string player, int count, int seed = 42)
    {
        var rng = new Random(seed);
        var agents = new[] { "Jett", "Reyna", "Omen", "Sova", "Sage", "Killjoy", "Raze", "Phoenix", "Cypher", "Viper" };
        var start = new DateTime(2023, 9, 1);

        return DataSeries<DataPoint<ValorantMatch>>.From(
            Enumerable.Range(1, count)
                .Select(i => new DataPoint<ValorantMatch>(
                    start.AddDays(i),
                    new ValorantMatch(
                        player,                             // player
                        agents[rng.Next(agents.Length)],    // agent
                        rng.Next(10, 28),                   // kills
                        rng.Next(6, 18),                    // deaths
                        rng.Next(0, 8),                     // assists
                        rng.Next(0, 13),                    // headshots
                        rng.Next(0, 14),                    // rounds_won
                        rng.Next(2) == 0                    // won
                    )
                ))
        );
    }

    public static DataSeries<DataPoint<LolMatch>> GenerateLol(string player, int count, int seed = 42)
    {
        var rng = new Random(seed);
        var champions = new[]
        {
            ("Thresh", "Support"), ("Nautilus", "Support"), ("Lulu", "Support"),
            ("Ahri", "Mid"),       ("Syndra", "Mid"),       ("Zed", "Mid"),
            ("Jinx", "ADC"),       ("Caitlyn", "ADC"),      ("Ezreal", "ADC"),
            ("Lee Sin", "Jungle"), ("Vi", "Jungle"),
            ("Darius", "Top"),     ("Ornn", "Top")
        };
        var start = new DateTime(2023, 9, 1);

        return DataSeries<DataPoint<LolMatch>>.From(
            Enumerable.Range(1, count)
                .Select(i =>
                {
                    var (champion, role) = champions[rng.Next(champions.Length)];

                    return new DataPoint<LolMatch>(
                        start.AddDays(i),
                        new LolMatch(
                            player,             // player
                            champion,           // champion
                            role,               // role
                            rng.Next(0, 16),    // kills
                            rng.Next(1, 11),    // deaths
                            rng.Next(2, 22),    // assists
                            rng.Next(30, 300),  // cs
                            rng.Next(15, 90),   // vision_score
                            rng.Next(2) == 0    // won
                        )
                    );
                })
        );
    }
}
