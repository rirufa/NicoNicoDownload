using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace NicoNico.Net.Tools
{
    /// <summary>
    /// 列挙型のフィールドにラベル文字列を付加するカスタム属性です。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class LabeledEnumAttribute : Attribute
    {
        /// <summary>
        /// ラベル文字列。
        /// </summary>
        public string label;

        /// <summary>
        /// LabeledEnumAttribute クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="label">ラベル文字列</param>
        public LabeledEnumAttribute(string label)
        {
            this.label = label;
        }
    }

    public static class LabeledExt
    {
        /// <summary>
        /// 属性で指定されたラベル文字列を取得する。
        /// </summary>
        /// <param name="value">ラベル付きフィールド</param>
        /// <returns>ラベル文字列</returns>
        public static string GetLabel(this Enum value)
        {
            return GetLabel(value, 0);
        }

        public static string GetLabel(this Enum value,int index)
        {
            //EnumのTypeを取得する
            //  type = {Name = "フォーマット形式共通" FullName = "共通.フォーマット形式共通"}
            var type = value.GetType();
            //
            //Enumのフィールドを取得する
            //  name = "Default"
            var name = Enum.GetName(type, value);
            //
            //クラスの配列を取得する
            //  myClass = {共通.LabeledEnumAttribute[1]}
            var myClass =
                        (LabeledEnumAttribute[])type.GetRuntimeField(name)
                        .GetCustomAttributes(typeof(LabeledEnumAttribute), false);

            return myClass[index].label;
        }
    }
}
