using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.DI
{
    public class ShoppingCart
    {
        IValueCalculator calculator;
        IDiscountHelper discount;

        //构造函数，参数为实现了IValueCalculator接口的类的实例
        //我们所需要的是，在一个类内部，不通过创建对象的实例而能够获得某个实现了公开接口的对象的引用。这种“需要”，就称为DI（依赖注入，Dependency Injection），和所谓的IoC（控制反转，Inversion of Control ）是一个意思。
        public ShoppingCart(IValueCalculator calcParam, IDiscountHelper discountParam)
        {
            calculator = calcParam;
            discount = discountParam;
        }
        public decimal CalculateStockValue()
        {
            Product[] products = {
            new Product {Name = "西瓜", Category = "水果", Price = 2.3M},
            new Product {Name = "苹果", Category = "水果", Price = 4.9M},
            new Product {Name = "空心菜", Category = "蔬菜", Price = 2.2M},
            new Product {Name = "地瓜", Category = "蔬菜", Price = 1.9M}
            };
            IValueCalculator calculator = new LinqValueCalculator(discount);
            //计算商品总价钱 
            decimal totalValue = calculator.ValueProduct(products);
            return totalValue;
        }
    }
}
