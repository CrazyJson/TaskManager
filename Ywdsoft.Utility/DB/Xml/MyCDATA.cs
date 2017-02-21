using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Ywdsoft.Utility.Extensions.Xml
{
    /// <summary>
    /// 支持CDATA序列化的包装类
    /// </summary>
    [Serializable]
	public class MyCDATA : IXmlSerializable
	{
		private string _value;

		/// <summary>
		/// 构造函数
		/// </summary>
		public MyCDATA() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="value">初始值</param>
		public MyCDATA(string value)
		{
			this._value = value;
		}

		/// <summary>
		/// 原始值。
		/// </summary>
		public string Value
		{
			get { return _value; }
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			this._value = reader.ReadElementContentAsString();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteCData(this._value);
		}

		/// <summary>
		/// ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this._value;
		}

		/// <summary>
		/// 重载操作符，支持隐式类型转换。
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static implicit operator MyCDATA(string text)
		{
			return new MyCDATA(text);
		}

		/// <summary>
		/// 重载操作符，支持隐式类型转换。
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static implicit operator string(MyCDATA text)
		{
			return text.ToString();
		}
	}
}
