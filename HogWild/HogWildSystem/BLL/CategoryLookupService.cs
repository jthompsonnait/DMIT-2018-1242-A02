using HogWildSystem.DAL;
using HogWildSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.BLL
{
    public class CategoryLookupService
    {
        private readonly HogWildContext _hogWildContext;

        //  Constructor for the CategoryLookupService class.
        internal CategoryLookupService(HogWildContext hogWildContext)
        {
            _hogWildContext = hogWildContext;
        }

        public List<LookupView> GetLookups(string categoryName)
        {
            return _hogWildContext.Lookups
                .Where(x => x.Category.CategoryName == categoryName
                            && !x.RemoveFromViewFlag)
                .OrderBy(x => x.Name)
                .Select(x => new LookupView
                {
                    LookupID = x.LookupID,
                    CategoryID = x.CategoryID,
                    Name = x.Name,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).ToList();
        }
    }
}
