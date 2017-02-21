using Dapper;
using System;

namespace Ywdsoft.Model.Entity
{
    ///<summary>
    ///
    ///</summary>
    [Table("p_ExpressCompany")]
    public class ExpressCompany
    {
        ///<summary>      
        /// CompanyGUID
        ///</summary> 
        [Key]
        [Column("CompanyGUID")]
        public Guid CompanyGUID
        {
            get;
            set;
        }
        ///<summary>      
        /// CompanyName
        ///</summary> 

        [Column("CompanyName")]
        public string CompanyName
        {
            get;
            set;
        }
        ///<summary>      
        /// CompanyCode
        ///</summary> 

        [Column("CompanyCode")]
        public string CompanyCode
        {
            get;
            set;
        }
        ///<summary>      
        /// CreatedOn
        ///</summary> 
        [ReadOnly(true)]
        [Column("CreatedOn")]
        public DateTime CreatedOn
        {
            get;
            set;
        }
    }
}

