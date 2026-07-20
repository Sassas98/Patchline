
using System;
using System.Collections.Generic;
using System.Linq;
public class CodePaletteApplier
{
	private string Keyword, Simboli, Variabili, Numeri;

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
				list.Add(MarkText(line, selectColor));
			else if (string.IsNullOrWhiteSpace(line))
				list.Add(line);
			else
			{
				var list2 = new List<string>();
				foreach (var word in line.Split(" "))
				{
					if (string.IsNullOrEmpty(word))
						list2.Add(word);
					else if(word.All(c => char.IsNumber(c)))
						list2.Add(MarkText(word, Numeri));
					else if(IsCMD(word))
						list2.Add(MarkText(word, Keyword));
					else if(IsSymbol(word))
						list2.Add(MarkText(word, Simboli));
					else list2.Add(MarkText(word, Variabili));
				}
				list.Add(string.Join(" ", list2));
			}
			counter++;
		}
		return string.Join("\n", list);
	}

	private bool IsSymbol(string word)
	{
		return word.ToLower() == "or" 
			|| word.ToLower() == "and" 
			|| !word.All(c => char.IsLetter(c));
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