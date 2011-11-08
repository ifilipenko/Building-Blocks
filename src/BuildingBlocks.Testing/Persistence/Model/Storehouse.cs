using System.Collections.Generic;

namespace BuildingBlocks.Testing.Persistence.Model
{
    public class ProductItem
    {
        public virtual long ID { get; set; }
        public string ProductName { get; set; }
        public Storehouse Storehouse { get; set; }
    }

    public class Employee
    {
        public virtual long ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual Department Department { get; set; }

        public virtual long EmployeeID
        {
            get { return ID; }
            set { ID = value; }
        }
    }

    public class Department
    {
        public virtual string Name { get; set; }
        public virtual long ID { get; set; }
    }

    public class Storehouse : Department
    {
        private readonly List<ProductItem> _products;
        private readonly List<Employee> _employeers;

        public Storehouse()
        {
            _products = new List<ProductItem>();
            _employeers = new List<Employee>();
        }

        public virtual long ID { get; set; }
        public virtual string Name { get; set; }

        public IEnumerable<ProductItem> Products
        {
            get { return _products; }
        }

        public IEnumerable<Employee> Employeers
        {
            get { return _employeers; }
        }

        public void AddItem(string product)
        {
            var item = new ProductItem {ProductName = product};
            _products.Add(item);
            item.Storehouse = this;
        }

        public void RemoveItem(ProductItem productItem)
        {
            _products.Remove(productItem);
            productItem.Storehouse = null;
        }

        public void AssignEmployee(Employee employee)
        {
            _employeers.Add(employee);
            employee.Department = this;
        }

        public void ReleaseEmployee(Employee employee)
        {
            _employeers.Remove(employee);
            employee.Department = null;
        }
    }
}