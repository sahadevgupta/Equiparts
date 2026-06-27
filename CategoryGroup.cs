using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Equiparts
{
    public class CategoryGroup : ObservableCollection<SubCategoryItem>
    {
        public string CategoryName { get; set; }

        public CategoryGroup(string categoryName, IEnumerable<SubCategoryItem> items)
            : base(items)
        {
            CategoryName = categoryName;
        }
    }
}
