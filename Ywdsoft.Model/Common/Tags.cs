using System;

namespace Ywdsoft.Model.Common
{
    ///<summary>
    ///标签定义表
    ///</summary>
    public class Tags
    {
        ///<summary>
        ///标签ID 
        ///</summary> 
        public string TagGUID { get; set; }

        ///<summary>
        ///标签类型 
        ///</summary> 
        public string TagType { get; set; }

        ///<summary>
        ///关联表主键 
        ///</summary> 
        public string SourceId { get; set; }

        ///<summary>
        ///标签名称 层级标签通过/进行分割(名称不能含有特殊字符) 
        ///</summary> 
        public string TagName { get; set; }

        ///<summary>
        ///标签热度 
        ///</summary> 
        public int? TagHeat { get; set; }

        ///<summary>
        ///创建日期 
        ///</summary> 
        public DateTime? CreatedTime { get; set; }

        ///<summary>
        ///创建人(用户代码) 
        ///</summary> 
        public string Creator { get; set; }

        ///<summary>
        ///修改日期 
        ///</summary> 
        public DateTime? ModifiedTime { get; set; }

        ///<summary>
        ///修改人(用户代码) 
        ///</summary> 
        public string Modifier { get; set; }
    }
}

