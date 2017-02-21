using System;
using Ywdsoft.Model.Entity;
using Ywdsoft.Utility.DB;

namespace YwdSoft.Service
{
    public class DapperDemoService
    {
        public static void Test()
        {
            //查询
            Console.WriteLine("查询所有数据");
            var list = DapperHelper.GetList<ExpressCompany>();
            foreach (var item in list)
            {
                Console.WriteLine(item.CompanyCode + "||" + item.CompanyName);
            }

            //条件查询
            Console.WriteLine("查询快递公司名称为“顺丰快递”的数据");
            foreach (var item in DapperHelper.GetList<ExpressCompany>(new { CompanyName = "顺丰快递" }))
            {
                Console.WriteLine(item.CompanyCode + "||" + item.CompanyName);
            }

            //条件查询
            Console.WriteLine("查询快递公司名称中包含为“快递”的数据");
            string likename = "%快递%";
            foreach (var item in DapperHelper.GetList<ExpressCompany>("where CompanyName like @CompanyName", new { CompanyName = likename }))
            {
                Console.WriteLine(item.CompanyCode + "||" + item.CompanyName);
            }

            //分页查询
            Console.WriteLine("查询快递公司名称中包含为“快递”的第一页前10条数据");
            likename = "%快递%";
            foreach (var item in DapperHelper.GetListPaged<ExpressCompany>(1, 10, "where CompanyName like @CompanyName", "CompanyName desc", new { CompanyName = likename }))
            {
                Console.WriteLine(item.CompanyCode + "||" + item.CompanyName);
            }

            //新增
            Console.WriteLine("插入一条名称为“测试快递公司”的数据");
            ExpressCompany itemNew = new ExpressCompany
            {
                CompanyName = "测试快递公司",
                CompanyCode = "TestCode"
            };
            var strCompanyGUID = DapperHelper.Insert<Guid>(itemNew);
            Console.WriteLine("插入的数据主键为:" + strCompanyGUID);
            //更新
            Console.WriteLine("更新刚插入的数据名称为“测试快递公司New”");
            itemNew.CompanyGUID = strCompanyGUID;
            itemNew.CompanyName = "测试快递公司New";
            DapperHelper.Update(itemNew);
            Console.WriteLine("查询刚更新的数据名称：" + DapperHelper.Get<ExpressCompany>(strCompanyGUID).CompanyName);

            //删除
            Console.WriteLine("删除刚插入的数据");
            DapperHelper.Delete<ExpressCompany>(strCompanyGUID);
        }
    }
}
