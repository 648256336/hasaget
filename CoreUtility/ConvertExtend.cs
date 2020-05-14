using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

public static class ConvertExtend
{
    public static bool In(this int value, params int[] arr)
    {
        if (arr == null) return false;
        return arr.Any(p => p == value);
    }
    public static int ToInt(this object value, int defaultValue = 0)
    {
        if (value == null)
            return defaultValue;
        try
        {
            var rst = Convert.ToInt32(value);
            return rst;
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }
    public static long ToLong(this object value, long defaultValue = 0)
    {
        if (value == null)
            return defaultValue;
        try
        {
            var rst = Convert.ToInt64(value);
            return rst;
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// 转换字符串
    /// </summary>
    /// <param name="value">需要转换的对象</param>
    /// <param name="length">字符串结果的最大长度，如果无限制则为-1</param>
    /// <param name="replacetxt">代替省略的字符串的字符，默认为...</param>
    /// <returns></returns>
    public static string SubStringReplaceOver(this object value, int length = -1, string replacetxt = "...")
    {
        try
        {
            if (value == null)
            {
                return "";
            }
            else
            {
                if (length == -1)
                {
                    return value.ToString();
                }
                int strLength = 0;
                StringBuilder strb = new StringBuilder();
                char[] temp = value.ToString().ToCharArray();
                for (int i = 0; i != temp.Length; i++)
                {
                    if (length == -1 || strLength >= length) //
                    {
                        strb.Append(replacetxt);
                        break;
                    }
                    else
                    {
                        if (((int)temp[i]) < 255) //大于255的都是汉字或者特殊字符
                        {
                            strLength++;
                        }
                        else
                        {
                            strLength = strLength + 2;
                        }
                        strb.Append(temp[i]);
                    }
                }
                return strb.ToString();
            }
        }
        catch (Exception)
        {
            return "";
        }
    }


    /// <summary>
    /// 将对象转换为字符串
    /// </summary>
    /// <param name="value">需要转换的对象</param>
    /// <param name="defaultValue">如果对象为空，则返回该值</param>
    /// <returns></returns>
    public static string ToNullableString(this object value, string defaultValue = "")
    {
        if (value == null)
        {
            return defaultValue;
        }
        else
        {
            return value.ToString();
        }
    }

    public static decimal ToDecimal(this object value, decimal defaultValue = 0)
    {
        if (value == null)
            return defaultValue;
        try
        {
            return Convert.ToDecimal(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public static double ToDouble(this object value, double defaultValue = 0)
    {
        if (value == null)
            return defaultValue;
        try
        {
            return Convert.ToDouble(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public static float ToFloat(this object value, float defaultValue = 0)
    {

        if (value == null) return defaultValue;
        try
        {
            return Convert.ToSingle(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public static bool ToBool(this object value, bool defaultValue = false)
    {
        if (value == null) return defaultValue;
        try
        {
            return Convert.ToBoolean(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public static DateTime ToDateTime(this object value, DateTime defaultValue)
    {
        try
        {
            return Convert.ToDateTime(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }


    private static DateTime _dtStart = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
    public static DateTime ToDateTime(this long timeStamp)
    {
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return _dtStart.Add(toNow);

        //new DateTime().AddMilliseconds(621355968000000000 / 10000).AddMilliseconds(timeStamp);
        //return _dtStart.AddSeconds(timeStamp);
    }

    /// <summary>
    /// 固定字符替换指定部分的值
    /// </summary>
    /// <param name="str"></param>
    /// <param name="starNum">保留前几位正常显示</param>
    /// <param name="endNum">保留后几位正常显示</param>
    /// <param name="chr">加密符号</param>
    /// <returns></returns>
    public static string ReplaceChar(this string str, int starNum, int endNum, char chr = '*')
    {
        string rst = string.Empty;
        if (str == null) return "";
        int chrNum = str.Length - starNum - endNum > 0 ? str.Length - starNum - endNum : 0;
        if (str.Length > starNum + endNum)
        {
            rst = $"{str.Substring(0, starNum)}{"".PadLeft(chrNum, chr)}{str.Substring(str.Length - endNum)}";
        }
        else
        {
            rst = str;
        }
        return rst;
    }


    public static string ReplaceCharName(this string str, int starNum, int endNum, char chr = '*')
    {
        string rst = string.Empty;
        if (str == null) return "";
        if (str.Length >= starNum + endNum)
        {
            rst = $"{str.Substring(0, starNum)}{chr}{str.Substring(str.Length - endNum)}";
        }
        else
        {
            rst = str;
        }
        return rst;
    }

    /// <summary>
    /// 对象转换为json格式的string类型
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToJson(this object obj)
    {
        if (obj == null)
        {
            return "";
        }
        //var dtConverter = new IsoDateTimeConverter();
        //dtConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //var settings = new JsonSerializerSettings
        //{
        //    DefaultValueHandling = DefaultValueHandling.Ignore,
        //    NullValueHandling = NullValueHandling.Ignore,
        //    MissingMemberHandling = MissingMemberHandling.Ignore,
        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //    PreserveReferencesHandling = PreserveReferencesHandling.None,
        //    Converters = new List<JsonConverter>
        //    {
        //        new IsoDateTimeConverter()
        //    }
        //};
        return JsonSerializer.Serialize(obj);
    }

    /// <summary>
    /// 将string类型反序列化为指定对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T DeserializeForJson<T>(this string str, T defaultValue = null) where T : class
    {
        try
        {
            if (str == null) return default(T);
            return JsonSerializer.Deserialize<T>(str.ToNullableString());
        }
        catch
        {
            return defaultValue;
        }
    }

    public static DateTime MinDate
    {
        get
        {
            return new DateTime(2000, 1, 1);
        }
    }
    public static DateTime MaxDate
    {
        get
        {
            return new DateTime(2079, 1, 1);
        }
    }
    /// <summary>
    /// 是否不存在有效值
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool HasValue(this object obj)
    {
        if (obj == null) return false;
        if (obj is string) return (obj as string) != string.Empty;
        if (obj is int) return (int)obj != int.MinValue;
        if (obj is DateTime)
        {
            var time = Convert.ToDateTime(obj);
            if (time > MinDate && time < MaxDate) return true;
            else return false;
        }
        if (obj is float) return (float)obj != float.MinValue;
        if (obj is decimal) return (decimal)obj != decimal.MinValue;
        if (obj is long) return (long)obj != long.MinValue;

        return !obj.IsDefaultValue();
    }

    /// <summary>
    /// 是否为默认值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsDefaultValue<T>(this T obj)
    {
        if (obj == null) return true;
        var propertyType = obj.GetType();
        var defaultValue = (propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null);
        return obj.Equals(defaultValue);
    }

    /// <summary>
    /// 比对模式
    /// </summary>
    public enum BetweenEqualStyle
    {
        /// <summary>
        /// 大于最小值，小于最大值
        /// </summary>
        NoEqual = 0,
        /// <summary>
        /// 大于等于最小值，小于最大值
        /// </summary>
        LeftEqual = 1,
        /// <summary>
        /// 大于最小值，小于等于最大值
        /// </summary>
        RightEqual = 2,
        /// <summary>
        /// 大于等于最小值，小于等于最大值
        /// </summary>
        TowEqual = 3,
    }

    /// <summary>
    /// 该时间是否在两指定时间之间
    /// </summary>
    /// <param name="datetime">时间</param>
    /// <param name="min">最小时间</param>
    /// <param name="max">最大时间</param>
    /// <param name="style">比对模式:是否可以和边界值相等</param>
    /// <returns></returns>
    public static bool Between(this DateTime datetime, DateTime min, DateTime max, BetweenEqualStyle style = BetweenEqualStyle.NoEqual)
    {
        if (datetime == null) return false;
        switch (style)
        {
            case BetweenEqualStyle.LeftEqual:
                if (datetime == min) return true;
                break;
            case BetweenEqualStyle.RightEqual:
                if (datetime == max) return true;
                break;
            case BetweenEqualStyle.TowEqual:
                if (datetime == min || datetime == max) return true;
                break;
            default:
                break;
        }
        return datetime > min && datetime < max;
    }

    /// <summary>
    /// 该值是否在两指定时间之间
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="style">比对模式:是否可以和边界值相等</param>
    /// <returns></returns>
    public static bool Between(this int? value, int min, int max, BetweenEqualStyle style = BetweenEqualStyle.NoEqual)
    {
        if (value == null) return false;
        switch (style)
        {
            case BetweenEqualStyle.LeftEqual:
                if (value == min) return true;
                break;
            case BetweenEqualStyle.RightEqual:
                if (value == max) return true;
                break;
            case BetweenEqualStyle.TowEqual:
                if (value == min || value == max) return true;
                break;
            default:
                break;
        }
        return value >= min && value <= max;
    }


    public static bool Between(this int value, int min, int max, BetweenEqualStyle style = BetweenEqualStyle.NoEqual)
    {
        switch (style)
        {
            case BetweenEqualStyle.LeftEqual:
                if (value == min) return true;
                break;
            case BetweenEqualStyle.RightEqual:
                if (value == max) return true;
                break;
            case BetweenEqualStyle.TowEqual:
                if (value == min || value == max) return true;
                break;
            default:
                break;
        }
        return value >= min && value <= max;
    }

    /// <summary>
    /// 将属性值拷贝转换到指定的类型对应的属性值。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T CopyProperties<T>(this object source, T target = null, bool overwrite = false) where T : class, new()
    {
        if (source != null)
        {
            if (target == null)
            {
                target = new T();
            }
            Type sourceType = source.GetType();
            Type targetType = typeof(T);
            PropertyInfo[] sourceProperties = targetType.GetProperties();
            PropertyInfo sourceProp = null;
            foreach (PropertyInfo targetProp in sourceProperties)
            {
                sourceProp = sourceType.GetProperty(targetProp.Name);
                var temp2 = targetType.GetProperty(targetProp.Name);

                if (sourceProp != null && temp2 != null)
                {
                    if (sourceProp.PropertyType.Equals(temp2.PropertyType))
                    {
                        var value = sourceProp.GetValue(source, null);
                        if (!value.IsDefaultValue())
                        {
                            if (overwrite)
                                targetProp.SetValue(target, value, null);
                            else
                            {
                                var targetValue = targetProp.GetValue(target, null);
                                if (targetValue.IsDefaultValue())
                                {
                                    targetProp.SetValue(target, value, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        var value = sourceProp.GetValue(source, null);
                        if (!value.IsDefaultValue())
                        {
                            var sourceIsNullAble = (sourceProp.PropertyType.IsGenericType && sourceProp.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
                            var targetIsNullAble = (targetProp.PropertyType.IsGenericType && targetProp.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
                            Type sourceOrgType = sourceIsNullAble ? sourceProp.PropertyType.GetGenericArguments()[0] : sourceProp.PropertyType;
                            Type targetOrgType = targetIsNullAble ? targetProp.PropertyType.GetGenericArguments()[0] : targetProp.PropertyType;
                            if (sourceOrgType == targetOrgType)
                            {
                                //剥开NullAble后的最终类型相等时，可赋值
                                if (overwrite)
                                {
                                    if (value != null)
                                    {
                                        targetProp.SetValue(target, value, null);
                                    }
                                }
                                else
                                {
                                    var targetValue = targetProp.GetValue(target, null);
                                    if (targetValue.IsDefaultValue())
                                    {
                                        if (value != null)
                                        {
                                            targetProp.SetValue(target, value, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        return target;
    }



    /// <summary>
    /// 将对象转换为byte数组
    /// </summary>
    /// <param name="obj">被转换对象</param>
    /// <returns>转换后byte数组</returns>
    public static byte[] Object2Bytes(object obj)
    {
        byte[] buff;
        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter iFormatter = new BinaryFormatter();
            iFormatter.Serialize(ms, obj);
            buff = ms.GetBuffer();
        }
        return buff;
    }

    /// <summary>
    /// 将byte数组转换成对象
    /// </summary>
    /// <param name="buff">被转换byte数组</param>
    /// <returns>转换完成后的对象</returns>
    public static object Bytes2Object(byte[] buff)
    {
        object obj;
        using (MemoryStream ms = new MemoryStream(buff))
        {
            IFormatter iFormatter = new BinaryFormatter();
            obj = iFormatter.Deserialize(ms);
        }
        return obj;
    }

    /// <summary>
    /// 是否由字母，数字，下划线组成
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static bool IsW(this string account)
    {
        if (account.HasValue())
        {
            Regex rx = new Regex(@"^\w+$");
            return rx.IsMatch(account);
        }
        else return false;
    }
    /// <summary>
    /// 是否由字母，数字线组成
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static bool IsLetterOrNumber(this string account)
    {
        if (account.HasValue())
        {
            Regex rx = new Regex(@"^[A-Za-z0-9]+$");
            return rx.IsMatch(account);
        }
        else return false;
    }
    public static bool IsCellphone(this string cellphone)
    {
        if (cellphone.HasValue())
        {
            Regex rx = new Regex(@"^1+\d{10}$");
            return rx.IsMatch(cellphone);
        }
        else return false;
    }
    public static bool IsEMail(this string email)
    {
        if (email.HasValue())
        {
            Regex rx = new Regex(@"^([a-zA-Z]|[0-9])(\w|\-)+@[a-zA-Z0-9]+\.([a-zA-Z]{2,4})$");
            return rx.IsMatch(email);
        }
        else return false;
    }

    /// <summary>  
    /// 时间戳转为C#格式时间  
    /// </summary>  
    /// <param name="timeStamp">Unix时间戳格式</param>  
    /// <returns>C#格式时间</returns>  
    public static DateTime GetTime(string timeStamp)
    {
        //TimeZone.CurrentTimeZone.ToLocalTime(); 过时
        DateTime dtStart = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);
        if (timeStamp.Length == 10)
        {
            timeStamp += "0000000";
        }
        long lTime = long.Parse(timeStamp + "0000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }


    /// <summary>  
    /// DateTime时间格式转换为Unix时间戳格式  
    /// </summary>  
    /// <param name="time"> DateTime时间格式</param>  
    /// <returns>Unix时间戳格式</returns>  
    public static int ConvertDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);
        return (int)(time - startTime).TotalSeconds;
    }

    public static long ToUtcLong(this DateTime time)
    {
        return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }

    public static decimal CutDecimalWithN(decimal d, int n)
    {
        string strDecimal = d.ToString();
        int index = strDecimal.IndexOf(".");
        if (index == -1 || strDecimal.Length < index + n + 1)
        {
            strDecimal = string.Format("{0:F" + n + "}", d);
        }
        else
        {
            int length = index;
            if (n != 0)
            {
                length = index + n + 1;
            }
            strDecimal = strDecimal.Substring(0, length);
        }
        return Decimal.Parse(strDecimal);
    }

    private static JsonSerializerOptions _jsonSerializerOptions;
    public static JsonSerializerOptions JsonOptions
    {
        get
        {
            if (_jsonSerializerOptions == null)
            {
                _jsonSerializerOptions = new JsonSerializerOptions()
                {
                    //Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                };
                _jsonSerializerOptions.Converters.Add(new DateTimeConverterLong10());
            }
            return _jsonSerializerOptions;
        }
    }

    #region BinarySearch
    /// <summary>
    /// 二分算法搜索集合数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">必须是按照搜索条件排好序的集合</param>
    /// <param name="filter">条件参数</param>
    /// <param name="func">条件参数</param>
    /// <returns></returns>
    public static T BinarySearchSingle<T>(this List<T> obj, T filter, Func<T, T, int> func) where T : new()
    {
        int idx = obj.BinarySearch(filter, new ComparerSearch<T>((x, y) => func(x, y)));
        if (idx >= 0)
        {
            return obj[idx];
        }
        return default(T);
    }

    public static List<T> BinarySearchList<T>(this List<T> obj, T filter, Func<T, T, int> func) where T : new()
    {
        List<T> rst = new List<T>();
        int idx = obj.BinarySearch(filter, new ComparerSearch<T>((x, y) => func(x, y)));

        int pidx = idx;
        int didx = idx - 1;
        while (pidx >= 0 && obj.Count > pidx)
        {
            var model = obj[pidx++];
            if (func(model, filter) == 0)
                rst.Add(model);
            else break;
        }
        while (didx >= 0 && obj.Count > didx)
        {
            var model = obj[didx--];
            if (func(model, filter) == 0)
                rst.Add(model);
            else break;
        }
        return rst;
    }

    public class ComparerSearch<T> : IComparer<T>
    {
        public delegate int CompareFunc(T t1, T t2);
        public CompareFunc Func;
        public ComparerSearch(CompareFunc func)
        {
            Func = func;
        }

        public int Compare(T x, T y)
        {
            if (Func != null) return Func(x, y);
            return 0;
        }
    }

    #endregion
}

public class DateTimeConverterLong10 : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUtcLong());
    }
}
public class DateTimeConverterLong13 : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((value.ToUniversalTime().Ticks - 621355968000000000) / 10000);
    }
}
