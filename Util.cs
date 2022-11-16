using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace MindPlus
{
    public static class Util
    {
        public static Double IntTruncate(double value, int digit = 0)
        {
            Double Temp = Math.Pow(10.0, digit);
            return Math.Truncate(value * Temp) / Temp;
        }
        public static Double IntRound(double value, int digit = 0)
        {
            Double Temp = Math.Pow(10.0, digit);
            return Math.Round(value * Temp) / Temp;
        }

        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = new CultureInfo("en-US");
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt) =>
            dt.FirstDayOfWeek().AddDays(6);

        public static DateTime FirstDayOfMonth(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, 1);

        public static DateTime LastDayOfMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

        public static DateTime FirstDayOfNextMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1);


        public static UIView[] MergeTwoArray(UIView[] firstArray, UIView[] secondArray)
        {
            List<UIView> result = new List<UIView>();
            foreach (var item in firstArray)
            {
                result.Add(item);
            }
            foreach (var item in secondArray)
            {
                result.Add(item);
            }
            return result.ToArray();
        }
        public static Sprite ConvertBytes(byte[] bytes)
        {
            Texture2D result = new Texture2D(2, 2);
            result.LoadImage(bytes);
            Rect rect = new Rect(0, 0, result.width, result.height);
            Sprite sprite = Sprite.Create(result, rect, new Vector2(0.5f, 0.5f));
            return sprite;
        }

        public static void GetFlexibleTextWidth(MonoBehaviour caller, Text text, Action<float> callback)
        {
            caller.StartCoroutine(DoGetFlexibleTextWidth(text, callback));
        }
        private static IEnumerator DoGetFlexibleTextWidth(Text text, Action<float> callback)
        {
            var rt = text.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1000, rt.sizeDelta.y);
            yield return new WaitForEndOfFrame();
            TextGenerator textGenerator = text.cachedTextGenerator;
            float width = 0;
            foreach (var item in textGenerator.characters)
            {
                width += item.charWidth;
            }
            callback.Invoke(width);
        }


        public static void SetFlexibleTextWidth(MonoBehaviour caller, Text text, float offset = 0)
        {
            caller.StartCoroutine(DoSetFlexibleTextWidth(text, offset));
        }
        private static IEnumerator DoSetFlexibleTextWidth(Text text, float offset = 0)
        {
            yield return new WaitForEndOfFrame();
            TextGenerator textGenerator;
            do
            {
                textGenerator = text.cachedTextGenerator;
                float width = 0;
                foreach (var item in textGenerator.characters)
                {
                    width += item.charWidth;
                }
                var rt = text.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(width + offset, rt.sizeDelta.y);
                yield return null;
            } while (textGenerator.lineCount > 1);
        }
        public static void ShortenText(MonoBehaviour caller, string name, Text text)
        {
            caller.StartCoroutine(DoShortenText(name, text));
        }

        private static IEnumerator DoShortenText(string name, Text text)
        {
            text.text = name;

            yield return null;

            TextGenerator textGenerator = text.cachedTextGenerator;
            if (textGenerator.lineCount > 2)
            {
                List<UILineInfo> lines = textGenerator.lines as List<UILineInfo>;
                int startIndex = lines[2].startCharIdx;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < startIndex; i++)
                {
                    sb.Append(text.text[i]);
                }
                text.text = sb.ToString();
            }

            if (textGenerator.lineCount > 1)
            {
                List<UILineInfo> lines = textGenerator.lines as List<UILineInfo>;
                List<UICharInfo> characters = textGenerator.characters as List<UICharInfo>;


                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                int lineFirstIndex = lines[1].startCharIdx;

                for (int i = 0; i < lineFirstIndex; i++)
                {
                    sb.Append(text.text[i]);
                }

                bool isRemain = false;
                for (int i = lineFirstIndex; i < characters.Count; i++)
                {
                    if (characters[i].cursorPos.x <= -121)
                    {
                        sb.Append(text.text[i]);
                    }
                    else
                    {
                        isRemain = true;
                    }
                }
                if (isRemain)
                {
                    sb.Append("...");
                    text.text = sb.ToString();
                }
            }
        }

        public static bool NameCheck(string text)
        {
            return Regex.IsMatch(text, @"[^]0-9a-zA-Z-!@#$%^&*()_+={[}|;:;''<,>./?]");
        }

        public static bool LengthCheck(int min, int max, string text)
        {
            if (text.Length < min || text.Length > max)
                return true;
            else
                return false;
        }
    }
}
