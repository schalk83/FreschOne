using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FoFileParser
{
    public List<string> ReadFoFile(string tableName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", $"{tableName}.fo");

        if (!File.Exists(filePath))
            return null;  // Return null if file doesn't exist

        var lines = File.ReadAllLines(filePath);
        // Remove semicolon from the end of each line
        var cleanedLines = lines.Select(line => line.Trim().TrimEnd(';')).ToList();
        return cleanedLines;
    }

    public void ApplyLogicToRecord(ref Dictionary<string, object> record, List<string> logicLines)
    {
        foreach (var line in logicLines)
        {
            // Here you would parse and apply the logic
            if (line.Contains("=>"))
            {
                var parts = line.Split(new[] { "=>" }, StringSplitOptions.None);
                var condition = parts[0].Trim();
                var result = parts[1].Trim();

                if (IsConditionTrue(condition, record))
                {
                    ApplyResult(result, ref record);
                }
            }
        }
    }

    private bool IsConditionTrue(string condition, Dictionary<string, object> record)
    {
        // Simple checks for equality and comparisons. Extend as needed.
        var conditionParts = condition.Split(new[] { '=' }, StringSplitOptions.None);
        if (conditionParts.Length == 2)
        {
            var column = conditionParts[0].Trim();
            var value = conditionParts[1].Trim().Replace("\"", "");
            if (record.ContainsKey(column) && record[column]?.ToString() == value)
            {
                return true;
            }
        }

        return false;
    }

    private void ApplyResult(string result, ref Dictionary<string, object> record)
    {
        var resultParts = result.Split(new[] { '=' }, StringSplitOptions.None);
        if (resultParts.Length == 2)
        {
            var column = resultParts[0].Trim();
            var value = resultParts[1].Trim().Replace("\"", "");

            // For now, assume the value can be a string. Extend for other data types as necessary
            record[column] = value;
        }
    }
}
