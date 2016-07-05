using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Resources;

namespace QuizWeb.Misc
{
    public class LanguageService
    {
        private static LanguageService _instance = new LanguageService();
        private List<ResourceManager> _resourceManagers = new List<ResourceManager>();

        private LanguageService()
        {
        }

        public static LanguageService Instance { get { return _instance; } }

        public void Add(ResourceManager mgr)
        {
            _resourceManagers.Add(mgr);
        }

        public string Translate(string key)
        {
            foreach (var item in _resourceManagers)
            {
                var value = item.GetString(key);
                if (value != null)
                    return value;
            }

            return null;
        }
    }
}