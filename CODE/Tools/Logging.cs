using Godot;
using System;
using System.Collections.Generic;

public static class Logging
{

	public static String Category_File_Management = "File Management";
	public static String Category_Data_Management = "Data Management";

	private static Dictionary<String, float> Timers = new Dictionary<string, float>();
	
	public static void PrintTemp(String category, String message)
	{
		GD.PrintRich($"[color=#d5ff00][font_size=12]{category, -20}[/font_size][/color] {message}");
	}
	
	public static void PrintInfo(String category, String message, String timerKey = null)
	{
		try
		{
			if (timerKey != null)
			{
				if (Timers.ContainsKey(timerKey))
				{
					message = $"{message,-20} {(Time.GetTicksMsec() - Timers[timerKey]) / 1000f}";
					Timers.Remove(timerKey);
				}
				else
				{
					Timers[timerKey] = Time.GetTicksMsec();
				}
			}
		}
		catch (Exception e)
		{
			GD.PrintErr(e.ToString());
		}

		GD.PrintRich($"[color=#4285f4][font_size=12]{category, -20}[/font_size][/color] {message}");
	}
	
	public static void PrintWarning(String category, String message)
	{
		GD.PrintRich($"[color=#d88e00][size=150]{category,-20}[/size][/color] {message}");

	}
	
	public static void PrintError(String category, String message)
	{
		GD.PrintRich($"[color=#da2c38][size=150]{category,20}[/color][/size=] {message}");
	}
}
