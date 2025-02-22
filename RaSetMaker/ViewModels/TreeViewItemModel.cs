using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using RaSetMaker.Utils;
using System;
using System.Collections.Generic;

namespace RaSetMaker.ViewModels
{
    public partial class TreeViewItemModel : ViewModelBase
    {
        public virtual string Title { get; } = "";

        public virtual bool IsSelectable { get; } = true;

        public virtual string IconSrc { get; } = "";

        public Bitmap? Icon => HasIcon ? ImageHelper.LoadFromResource(new Uri(IconSrc)) : null;

        public bool HasIcon => IconSrc != string.Empty;

        public string StatusColor => "";

        [ObservableProperty]
        private bool _isChecked = true;


        [ObservableProperty]
        private List<TreeViewItemModel> _children = [];

        partial void OnIsCheckedChanged(bool value)
        {
            Children.ForEach(c => c.IsChecked = value);
            OnItemChecked(value);
        }

        protected virtual void OnItemChecked(bool value)
        {
        }

    }
}
