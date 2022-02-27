using System;

public static class CaptureCodeUtils
{
	/// <summary>
	/// The canonical order that the characters go in, with A as no punches and / as all 6 punches
	/// </summary>
	public static readonly char[] hashCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToCharArray();

	/// <summary>
	/// Turn a captcha code into its hashed number version
	/// </summary>
	public static long HashCaptureCode(string captchaCode)
	{
		if (captchaCode.Length != 8)
			throw new ArgumentException("Captcha code must have 8 characters");

		long hash = 0;
		for (int i = 0; i < 8; i++)
		{
			if (Array.IndexOf(hashCharacters, captchaCode[i]) == -1)
				throw new ArgumentException("Captcha code contains illegal characters");

			hash |= (1L << i * 6) * Array.IndexOf(hashCharacters, captchaCode[i]);
		}
		return hash;
	}

	/// <summary>
	/// Turn a hashed captcha code into its unhashed string version
	/// </summary>
	public static string UnhashCaptcha(long captchaHash)
	{
		if (captchaHash == -1)
			return null;

		if ((captchaHash & ~((1L << 48) - 1L)) != 0)
			throw new ArgumentException("Captcha hash is too big of a value");

		string code = "";
		for (int i = 0; i < 8; i++)
			code += GetCaptchaChar(captchaHash, i);
		return code;
	}

	/// <summary>
	/// Return a single digit of a captcha code
	/// </summary>
	public static int GetCaptchaDigit(long captchaHash, int i)
	{
		return (int) ((captchaHash >> 6 * i) & ((1L << 6) - 1));
	}

	/// <summary>
	/// Return a single digit of a captcha code as a fraction between 0 ("0") and 1 ("!")
	/// </summary>
	public static float GetCaptchaPercent(long captchaHash, int i)
	{
		return GetCaptchaDigit(captchaHash, i) / 63f;
	}

	/// <summary>
	/// Return a single digit of a captcha code as a character
	/// </summary>
	public static char GetCaptchaChar(long captchaHash, int i)
	{
		return hashCharacters[GetCaptchaDigit(captchaHash, i)];
	}

	/// <summary>
	/// Return a single bit of a captcha code
	/// </summary>
	public static bool GetCaptchaBit(long captchaHash, int i)
	{
		return (captchaHash & (1L << i)) != 0;
	}
}