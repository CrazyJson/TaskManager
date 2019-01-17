using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Ywdsoft.Utility.Mef
{
    public class MefConfig
    {
        private static CompositionContainer _container;
        public static void Init()
        {
            //1.Mef接管
            DirectoryCatalog catalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "Ywdsoft.*.dll");
            _container = new CompositionContainer(catalog);
            _container.ComposeParts();
        }

        /// <summary>
        /// 获取解析类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T TryResolve<T>()
        {
            return _container.GetExportedValueOrDefault<T>();
        }

        /// <summary>
        /// 获取解析类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ResolveMany<T>()
        {
            return _container.GetExportedValues<T>();
        }

        public static T Resolve<T>()
        {
            return _container.GetExportedValue<T>();
        }
    }
}
