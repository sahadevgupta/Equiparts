using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Equiparts
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<SubCategoryItem> _allItems;

        public ObservableCollection<CategoryGroup> GroupedItems { get; set; }
            = new();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value)
                    return;

                _searchText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public MainViewModel()
        {
            _allItems = new List<SubCategoryItem>
            {
                // Backhoe Loader Parts
                new SubCategoryItem { Category = "Backhoe Loader Parts", Name = "Genuine" },
                new SubCategoryItem { Category = "Backhoe Loader Parts", Name = "Fabrication" },
                new SubCategoryItem { Category = "Backhoe Loader Parts", Name = "Pin & Bushes" },
                new SubCategoryItem { Category = "Backhoe Loader Parts", Name = "Coming Soon (Backhoe Loader All Range)" },

                // Excavator Parts
                new SubCategoryItem { Category = "Excavator Parts", Name = "Buckets" },
                new SubCategoryItem { Category = "Excavator Parts", Name = "Fabrication" },
                new SubCategoryItem { Category = "Excavator Parts", Name = "Pin & Bushes" },
                new SubCategoryItem { Category = "Excavator Parts", Name = "Stickend" },
                new SubCategoryItem { Category = "Excavator Parts", Name = "Spacers" },
                new SubCategoryItem { Category = "Excavator Parts", Name = "Lock Bush" },
                new SubCategoryItem { Category = "Excavator Parts", Name = "Complete Range of Undercarriage & GET Parts" },

                // ✅ Rock Breaker Parts
                new SubCategoryItem { Category = "Rock Breaker Parts", Name = "Chisels" },
            };

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allItems
                : _allItems.Where(x =>
                        x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        x.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            var grouped = filtered
                .GroupBy(x => x.Category)
                .Where(g => g.Any())   
                .Select(g => new CategoryGroup(g.Key, g.ToList()))
                .ToList();

            GroupedItems.Clear();

            foreach (var group in grouped)
            {
                GroupedItems.Add(group);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
