using MVC.DI;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建Ninject内核实例
            IKernel ninjectKernel = new StandardKernel();
            // 绑定接口到实现了该接口的类
            ninjectKernel.Bind<IValueCalculator>().To<LinqValueCalculator>();
            ninjectKernel.Bind<IDiscountHelper>().To<DefaultDiscountHelper>().WithPropertyValue("DiscountSize", 5M);

            // 获得实现接口的对象实例
            IValueCalculator calcImpl = ninjectKernel.Get<IValueCalculator>();
            IDiscountHelper dis = ninjectKernel.Get<IDiscountHelper>();
            // 创建ShoppingCart实例并注入依赖
            ShoppingCart cart = new ShoppingCart(calcImpl,dis);
            // 计算商品总价钱并输出结果
            Console.WriteLine("Total: {0:c}", cart.CalculateStockValue());
            Console.ReadKey();
        }
    }
}
