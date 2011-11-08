using System.Web;

namespace BuildingBlocks.Persistence.Scope
{
    class HttpContextSessionLocatorContext : ISessionLocatorContext
    {
        public static bool HasContext
        {
            get
            {
                try
                {
                    return HttpContext.Current != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        private readonly HttpContext _context;
        private readonly string _key;

        public HttpContextSessionLocatorContext(string key)
        {
            _key = key;
            _context = HttpContext.Current;
        }

        public SessionLocatorItem Item
        {
            get
            {
                var item = _context.Items[_key];
                return item as SessionLocatorItem;
            }
            set
            {
                if (Equals(value, null))
                {
                    RemoveItem();
                }
                else
                {
                    _context.Items[_key] = value;
                }
            }
        }

        public void RemoveItem()
        {
            _context.Items.Remove(_key);
        }

    }
}