// See https://aka.ms/new-console-template for more information

/*
 * field            f00
 * river   vertical rv0, horizontal rh0, top-right  rtr, top-left rtl, left-bottom rbl, right bottom rbr
 * path    vertical pv0, horizontal ph0, top-right  ptr, top-left ptl, left-bottom pbl, right bottom pbr
 * path start/finish     top sft, bottom sfb, left sfl, right sfr
 * bridge vertical  bv0, horizontal bh0
 *
 */

//15x11

using System.Collections.Frozen;
using System.Text;

string mapRaw =
    @"
f00 f00 f00 f00 f00 f00 f00 f00 f00 f00 f00 f00 f00 rv0 f00 
f00 pbr ph0 ph0 ph0 ph0 ph0 ph0 ph0 ph0 ph0 pbl f00 rv0 f00 
f00 sft f00 f00 f00 f00 f00 f00 f00 f00 f00 pv0 f00 rv0 f00 
f00 f00 f00 pbr ph0 ph0 ph0 pbl f00 f00 f00 pv0 f00 rv0 f00 
f00 f00 f00 pv0 f00 f00 f00 pv0 f00 rbr rh0 bv0 rh0 rtl f00 
f00 f00 f00 pv0 f00 rbr rh0 bv0 rh0 rtl f00 pv0 f00 f00 f00 
f00 rbr rh0 bv0 rh0 rtl f00 pv0 f00 f00 f00 pv0 f00 f00 f00 
f00 rv0 f00 pv0 f00 f00 f00 ptr ph0 ph0 ph0 ptl f00 f00 f00 
f00 rv0 f00 pv0 f00 f00 f00 f00 f00 f00 f00 f00 f00 sfb f00 
f00 rv0 f00 ptr ph0 ph0 ph0 ph0 ph0 ph0 ph0 ph0 ph0 ptl f00 
f00 rv0 f00 f00 f00 f00 f00 f00 f00 f00 f00 f00 f00 f00 f00 
";

var cellMap = CreateCellsMap();

StringBuilder mapFinalBuilder = new StringBuilder();
StringBuilder turretsTerrainBuilder = new StringBuilder();

var rows = mapRaw.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

foreach (var row in rows)
{
    var columns = row.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    foreach (var column in columns)
    {
        var aliases = cellMap[column];
        var aliasIdx = Random.Shared.Next(0, aliases.Length);

        var alias = aliases[aliasIdx];

        mapFinalBuilder.Append($"{alias} ");

        var cellState = GetCellTurretState(column);
        turretsTerrainBuilder.Append($"{cellState} ");
    }

    mapFinalBuilder.AppendLine();
    turretsTerrainBuilder.AppendLine();
}

Console.Clear();
Console.WriteLine("Final map:");
Console.WriteLine(mapFinalBuilder.ToString().Trim());

Console.WriteLine();
Console.WriteLine("Build grid");
Console.WriteLine(turretsTerrainBuilder.ToString().Trim());

return;


static FrozenDictionary<string, string[]> CreateCellsMap()
{
    Dictionary<string, string[]> mapCells = [];

//fields
    mapCells.Add("f00", ["00", "10", "20", "30"]);

    //PATHS-----------------------------------------------
//path horizontal
    mapCells.Add("ph0", ["01", "11"]);

//path vertical
    mapCells.Add("pv0", ["21", "31"]);

//path bottom-left
    mapCells.Add("pbl", ["02", "03"]);

//path top-left
    mapCells.Add("ptl", ["12", "13"]);

//path top-right
    mapCells.Add("ptr", ["22", "23"]);

//path bottom-right
    mapCells.Add("pbr", ["32", "33"]);

    //paths start finish-----------------------------------------------
    mapCells.Add("sfl", ["09"]);

    mapCells.Add("sft", ["19"]);

    mapCells.Add("sfr", ["29"]);

    mapCells.Add("sfb", ["39"]);


    //RIVERS-----------------------------------------------
//river horizontal
    mapCells.Add("rh0", ["04", "05", "14", "15"]);

//river vertical
    mapCells.Add("rv0", ["24", "25", "34", "35"]);

//river bottom-left
    mapCells.Add("rbl", ["06", "07"]);

//river top-left
    mapCells.Add("rtl", ["16", "17"]);

//river top-right
    mapCells.Add("rtr", ["26", "27"]);

//river bottom-right
    mapCells.Add("rbr", ["36", "37"]);

    //bridges-----------------------------------------------
//bridges horizontal
    mapCells.Add("bh0", ["08", "18"]);

//bridges vertical
    mapCells.Add("bv0", ["28", "38"]);

    return mapCells.ToFrozenDictionary();
}

string GetCellTurretState(string cell)
{
    return cell is "f00"
        ? "01"
        : "00";
}