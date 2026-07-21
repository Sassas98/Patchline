
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
public class CodePaletteApplier
{
	private string Keyword,  Simboli, Variabili, Numeri;

	public void SetPalette(string Keyword, string Simboli, string Variabili, string Numeri)
	{
		this.Keyword = Keyword;
		this.Simboli = Simboli;
		this.Variabili = Variabili;
		this.Numeri = Numeri;
	}

	public string MarkText(string text, int selectLine, string selectColor)
	{
		var lines = text.Replace("\r", "").Split('\n');
		var list = new List<string>();
		int counter = 0;
		foreach (var line in lines)
		{
			if (counter == selectLine)
				list.Add(GenerateSpace(line.Length - line.TrimStart().Length, selectColor) + MarkText(line, selectColor));
			else if (string.IsNullOrWhiteSpace(line))
				list.Add(GenerateSpace(line.Length, selectColor));
			else
			{
				int spaces = 0;
				var list2 = new List<string>();
				foreach (var word in line.Split(" "))
				{
					if (string.IsNullOrEmpty(word))
						spaces++;
					else if (word.All(c => char.IsNumber(c)))
						list2.Add(MarkText(word, Numeri));
					else if (IsCMD(word))
						list2.Add(MarkText(word, Keyword));
					else if (IsSymbol(word))
						list2.Add(MarkText(word, Simboli));
					else list2.Add(MarkText(word, Variabili));
				}
                list.Add(GenerateSpace(spaces, selectColor) + string.Join(" ", list2));
			}
			counter++;
		}
		return string.Join("\n", list);
	}

	private string GenerateSpace(int n, string color)
	{
        return n <= 0 ? "" : 
			MarkText(string.Join("", 
				Enumerable.Range(0, n).Select(_ => "_"))
			, color);
    }

	private bool IsSymbol(string word)
	{
		return word.ToLower() == "or" 
			|| word.ToLower() == "and" 
			|| !word.Any(c => char.IsLetter(c));
	}
	private bool IsCMD(string word)
	{
		return Enum.GetNames(typeof(CMD))
			.Select(z => z.ToUpper())
			.Contains(word.Trim().ToUpper());
	}

	private string MarkText(string text, string color)
	{
		return $"<color={color}>{text}</color>";
	}
}