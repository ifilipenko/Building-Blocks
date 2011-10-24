using System.Collections.Generic;

namespace BuildingBlocks.Testing.Persistence.Model
{
    public class PreParent
    {
        public PreParent()
        {
            Childs = new List<Parent>(0);
        }

        public virtual long ID { get; set; }
        public virtual IList<Parent> Childs { get; set; } 
    }

    public class Parent
    {
        public Parent()
        {
            Childs = new List<Child>(0);
        }

        public virtual long ID { get; set; }
        public virtual PreParent PreParent { get; set; }
        public virtual IList<Child> Childs { get; set; } 
    }

    public class Child
    {
        public virtual long ID { get; set; }
        public virtual Parent Parent { get; set; } 
    }
}