using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DataSeries;

public class DataSeries<T>
{
    private readonly IEnumerable<DataPoint<T>> _data;

    private DataSeries(IEnumerable<DataPoint<T>> data) => _data = data;

    public static DataSeries<T> From(IEnumerable<DataPoint<T>> source)
        => new DataSeries<T>(source);

    public static DataSeries<T> FromCsv(string path, Func<string[], T> parser)
    {
        var lines = File.ReadAllLines(path).Skip(1);
        return new DataSeries<T>(lines.Select(line =>
        {
            var cols = line.Split(',');
            return new DataPoint<T>(
                DateTime.Parse(cols[0], CultureInfo.InvariantCulture),
                parser(cols)
            );
        }));
    }

    public int Count => _data.Count();

    public IEnumerable<T> Values => _data.Select(dp => dp.Value);

    public IEnumerable<DataPoint<T>> DataPoints => _data;

    public DataSeries<T> Filter(Func<T, bool> predicate)
        => new DataSeries<T>(_data.Where(dp => predicate(dp.Value)));

    public DataSeries<T> Outliers(Func<T, bool> isOutlier)
        => new DataSeries<T>(_data.Where(dp => isOutlier(dp.Value)));

    public DataSeries<T> Sanitize(Func<T, bool> isOutlier)
        => Filter(x => !isOutlier(x));
}
