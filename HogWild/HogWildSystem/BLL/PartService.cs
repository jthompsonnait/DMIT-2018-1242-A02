using HogWildSystem.DAL;
using HogWildSystem.Entities;
using HogWildSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.BLL
{
    public class PartService
    {

        #region Fields

        /// <summary>
        /// The hog wild context
        /// </summary>
        private readonly HogWildContext _hogWildContext;

        #endregion

        // Constructor for the PartService class.
        internal PartService(HogWildContext hogWildContext)
        {
            // Initialize the _hogWildContext field with the provided HogWildContext instance.
            _hogWildContext = hogWildContext;
        }

        //	Get all parts
        public List<PartView> GetParts(int partCategoryID, string description, List<int> existingPartIDs)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	both part id must be valid and/or description  cannot be empty
            //		rule: 	part Ids in existingPartIDs will be ignore 
            //		rule: 	RemoveFromViewFlag must be false

            if (partCategoryID == 0 && string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException("Please provide either a category and/or description");
            }

            //  need to update parameters so we are not searching on an empty value.
            //	this will return all records
            Guid tempGuild = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(description))
            {
                description = tempGuild.ToString();
            }

            //	ignore any parts that are in the "existing part ID" list
            return _hogWildContext.Parts.Where(x => !existingPartIDs.Contains(x.PartID) &&
            (description.Length > 0 && description != tempGuild.ToString() && partCategoryID > 0
                                    ? (x.Description.Contains(description) && x.PartCategoryID == partCategoryID)
                                    : (x.Description.Contains(description) || x.PartCategoryID == partCategoryID)
                                                                              && !x.RemoveFromViewFlag))

                        .Select(x => new PartView
                        {
                            PartID = x.PartID,
                            PartCategoryID = x.PartCategoryID,
                            CategoryName = x.PartCategory.Name,
                            Description = x.Description,
                            Cost = x.Cost,
                            Price = x.Price,
                            ROL = x.ROL,
                            QOH = x.QOH,
                            Taxable = (bool)x.Taxable,
                            RemoveFromViewFlag = x.RemoveFromViewFlag
                        })
                        .OrderBy(x => x.Description)
                        .ToList();
        }

        //	Get part
        public PartView GetPart(int partID)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	partID must be valid 
            if (partID == 0)
            {
                throw new ArgumentNullException("Please provide a part");
            }

            return _hogWildContext.Parts
                .Where(x => (x.PartID == partID
                             && !x.RemoveFromViewFlag))
                .Select(x => new PartView
                {
                    PartID = x.PartID,
                    PartCategoryID = x.PartCategoryID,
                    CategoryName = _hogWildContext.Lookups //  No navigational property was created in database
                                    .Where(l => l.LookupID == x.PartCategoryID)
                                    .Select(l => l.Name).FirstOrDefault(),
                    Description = x.Description,
                    Cost = x.Cost,
                    Price = x.Price,
                    ROL = x.ROL,
                    QOH = x.QOH,
                    Taxable = (bool)x.Taxable,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).FirstOrDefault();
        }
    }
}
