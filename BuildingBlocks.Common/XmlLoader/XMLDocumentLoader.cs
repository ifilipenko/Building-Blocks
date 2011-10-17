using System.Collections.Generic;
using System.Xml;

namespace BuildingBlocks.Common.XmlLoader
{
    /// <summary>
    /// Pattern: Wrapper 
    /// </summary>
    /// <typeparam name="ResultClass"></typeparam>
    public class XMLDocumentLoader<ResultClass>// : XMLItemLoader
        where ResultClass : new()
    {
        readonly ResultClass[] _resultAsArray;

        public XMLDocumentLoader(XmlNode node)
            //: base(typeof(ResultClass), node)
        {
            ClassMember classMember = new ClassMember(typeof(ResultClass));
            if (classMember.ClassNodeName == node.Name)
            {
                _resultAsArray = new ResultClass[] {(ResultClass) classMember.LoadValueFromNode(node)};
            }
            else
            {
                List<ResultClass> results = new List<ResultClass>(node.ChildNodes.Count);
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (classMember.ClassNodeName == childNode.Name)
                    {
                        results.Add((ResultClass) classMember.LoadValueFromNode(childNode));
                    }
                }
                _resultAsArray = results.ToArray();
            }
        }

        //protected override void PerformLoad()
        //{
        //    _result = new List<ResultClass>();
        //    base.PerformLoad();
        //    _resultAsArray = _result.ToArray();
        //}

        //protected override void AddToResult(object value)
        //{
        //    _result.Add((ResultClass)value);
        //}

        public ResultClass[] Result
        {
            get { return _resultAsArray; }
        }
    }
}