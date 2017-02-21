using System;
using System.Xml.Serialization;

namespace Ywdsoft.Utility.Extensions.Xml
{
    /// <summary>
    /// 表示*.config文件中的一个XmlCommand配置项。
    /// </summary>
    [XmlType("XmlCommand")]
    [Serializable]
    public class XmlCommandItem
    {
        /// <summary>
        /// 命令的名字，这个名字将在XmlCommand.From时被使用。
        /// </summary>
        [XmlAttribute("Name")]
        public string CommandName;

        /// <summary>
        /// 命令的文本。是一段可运行的SQL脚本或存储过程名称。
        /// </summary>
        [XmlElement]
        public MyCDATA CommandText;
    }
}
