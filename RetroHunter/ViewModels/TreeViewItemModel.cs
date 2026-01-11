using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using RetroHunter.Utils;
using System;
using System.Collections.ObjectModel;

namespace RetroHunter.ViewModels
{
    public partial class TreeViewItemModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = string.Empty;

        public virtual bool IsSelectable { get; } = true;

        public virtual string IconSrc { get; } = "";

        public Bitmap? Icon => HasIcon ? ImageHelper.LoadFromResource(new Uri(IconSrc)) : null;

        public bool HasIcon => IconSrc != string.Empty;

        [ObservableProperty]
        private bool _isChecked = true;


        [ObservableProperty]
        private ObservableCollection<TreeViewItemModel> _children = [];

        partial void OnIsCheckedChanged(bool value)
        {
            foreach (var item in Children)
            {
                item.IsChecked = value;
                item.OnItemChecked(value);
            }
        }

        protected virtual void OnItemChecked(bool value)
        {
        }

        protected virtual bool CanShowDetails => false;
        protected virtual bool CanCompress => false;

    }
}
